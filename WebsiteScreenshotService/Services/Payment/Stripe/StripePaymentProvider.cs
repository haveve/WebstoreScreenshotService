using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text.Json;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Services.Payment.Exceptions;
using WebsiteScreenshotService.Services.Payment.Models;

namespace WebsiteScreenshotService.Services.Payment.Stripe;

public class StripePaymentProvider(IHttpClientFactory httpClientFactory, IPaymentProviderConfigurationManager ConfigurationManager) : IPaymentProvider
{
    public string ProviderName => StripeConstants.ProviderName;
    private HttpClient Http => httpClientFactory.CreateClient();

    private const string ProviderCustomerId = StripeConstants.MetadataProviderCustomerId;
    private const string ClientSecret = StripeConstants.MetadataClientSecret;

    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string endpoint,
        string? idempotencyKey = null)
    {
        var ordinal = ConfigurationManager.GetConfigurations();
        var baseUrl = ordinal.Data[StripeConstants.Settings.Ordinal.Url];

        var request = new HttpRequestMessage(method, $"{baseUrl}{endpoint}");

        var sensitive = ConfigurationManager.GetSensitiveConfigurations();
        var secret = sensitive.Data[StripeConstants.Settings.Sensitive.Secret];

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secret);

        if (!string.IsNullOrWhiteSpace(idempotencyKey))
            request.Headers.Add("Idempotency-Key", idempotencyKey);

        return request;
    }

    private async Task<T> PostForm<T>(
        string endpoint,
        IDictionary<string, string> form,
        string? idempotencyKey = null)
    {
        var request = CreateRequest(HttpMethod.Post, endpoint, idempotencyKey);
        request.Content = new FormUrlEncodedContent(form);

        var response = await Http.SendAsync(request);
        await EnsureSuccessfulAsync<T>(response, endpoint);

        return await Deserialize<T>(response);
    }

    private async Task<T> Get<T>(string endpoint)
    {
        var request = CreateRequest(HttpMethod.Get, endpoint);

        var response = await Http.SendAsync(request);
        await EnsureSuccessfulAsync<T>(response, endpoint);

        return await Deserialize<T>(response);
    }

    private static async ValueTask EnsureSuccessfulAsync<T>(HttpResponseMessage response, string requestUrl)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();

            throw new TransactionProcessingException(
                $"Stripe request failed. " +
                $"Type={typeof(T).Name}, " +
                $"StatusCode={(int)response.StatusCode} ({response.StatusCode}), " +
                $"Endpoint={requestUrl}, " +
                $"ResponseBody={Truncate(body, 500)}"
            );
        }
    }

    private static string Truncate(string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return input.Length <= maxLength
            ? input
            : input[..maxLength] + "...";
    }

    private static async Task<T> Deserialize<T>(HttpResponseMessage response)
    {
        var dto = await response.Content.ReadFromJsonAsync<T>()
            ?? throw new TransactionProcessingException($"Invalid Stripe response: {typeof(T).Name}");

        return dto;
    }

    private static void Validate(StartPaymentRequest request)
    {
        if (request.OrderId == Guid.Empty)
            throw new TransactionProcessingException("OrderId is required");

        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new TransactionProcessingException("UserId is required");

        if (request.Amount.Amount <= 0)
            throw new TransactionProcessingException("Amount must be greater than 0");

        if (string.IsNullOrWhiteSpace(request.Amount.Currency))
            throw new TransactionProcessingException("Currency is required");
    }

    private static void Validate(CreateSubscriptionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new TransactionProcessingException("UserId is required");

        if (request.SubscriptionInfo.SubscriptionType == SubscriptionType.Regular)
            throw new TransactionProcessingException("Regular subscription cannot be purchased");

        if (request.SubscriptionInfo.Price.Amount <= 0)
            throw new TransactionProcessingException("Subscription price must be greater than 0");
    }

    private static void Validate(RefundRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PaymentTransactionId))
            throw new TransactionProcessingException("PaymentTransactionId is required");
    }

    public async Task<bool> TestAsync()
    {
        try
        {
            using var request = CreateRequest(
                HttpMethod.Get,
                "account");

            using var response = await Http.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<StartPaymentResult> StartPayment(StartPaymentRequest request)
    {
        Validate(request);

        var dto = await PostForm<StripePaymentIntentResponse>(
            StripeConstants.PaymentIntents,
            new Dictionary<string, string>
            {
                ["amount"] = request.Amount.ToMinorUnits().ToString(),
                ["currency"] = request.Amount.Currency,
                ["metadata[order_id]"] = request.OrderId.ToString(),
                ["payment_method_types[]"] = "card",
                ["metadata[payment_attempt_id]"] = request.PaymentAttemptId.ToString(),
                ["metadata[user_id]"] = request.UserId,
                ["metadata[payment_type]"] = "points"
            },
            $"order_{request.OrderId}");

        if (!dto.IsValid())
        {
            return new StartPaymentResult(
                string.Empty,
                StartPaymentStatus.Failed,
                ReadOnlyDictionary<string, string>.Empty
            );
        }

        return new StartPaymentResult(
            dto.Id,
            StartPaymentStatus.Pending,
            new Dictionary<string, string>
            {
                [ClientSecret] = dto.ClientSecret
            }.AsReadOnly());
    }

    public async Task<CreateSubscriptionResult> CreateSubscription(CreateSubscriptionRequest request)
    {
        Validate(request);

        var priceId = ResolvePriceId(request.SubscriptionInfo);

        var customerId = await GetOrCreateCustomer(request);

        var existing = await GetActiveSubscription(customerId);

        var metadata = new Dictionary<string, string>
        {
            [ProviderCustomerId] = customerId
        };

        if (existing is not null)
        {
            var status = MapStatus(existing.Status);

            if (status == CreateSubscriptionStatus.AlreadyActive)
            {
                return new CreateSubscriptionResult(
                    existing.Id,
                    status,
                    metadata.AsReadOnly());
            }

            if (status == CreateSubscriptionStatus.AlreadyExistsIncomplete)
            {
                var secret = existing.LatestInvoice?.PaymentIntent?.ClientSecret
                    ?? throw new TransactionProcessingException(
                        "Missing client secret");

                metadata[ClientSecret] = secret;

                return new CreateSubscriptionResult(
                    existing.Id,
                    status,
                    metadata.AsReadOnly());
            }
        }

        var dto = await PostForm<StripeSubscriptionResponse>(
            StripeConstants.Subscriptions,
            new Dictionary<string, string>
            {
                ["customer"] = customerId,
                ["items[0][price]"] = priceId,
                ["payment_behavior"] = "default_incomplete",
                ["expand[]"] = "latest_invoice.payment_intent",
                ["metadata[user_id]"] = request.UserId,
                ["metadata[subscription_type]"] = request.SubscriptionInfo.SubscriptionType.ToString(),
                ["metadata[subscription_period]"] = request.SubscriptionInfo.Duration.ToString(),
                ["metadata[internal_subscription_id]"] = Guid.CreateVersion7().ToString()
            });

        var clientSecret = dto.LatestInvoice?.PaymentIntent?.ClientSecret;

        if (string.IsNullOrWhiteSpace(clientSecret))
            throw new TransactionProcessingException(
                "Missing subscription client secret");

        metadata[ClientSecret] = clientSecret;

        return new CreateSubscriptionResult(
            dto.Id,
            CreateSubscriptionStatus.Created,
            metadata.AsReadOnly());
    }

    public async Task<CancelSubscriptionResult> CancelSubscription(CancelSubscriptionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SubscriptionId))
            throw new ValidationException("SubscriptionId is required");

        var dto = await PostForm<StripeSubscriptionCancelResponse>(
            $"{StripeConstants.Subscriptions}/{request.SubscriptionId}",
            new Dictionary<string, string>
            {
                ["cancel_at_period_end"] = "true",
                ["proration_behavior"] = "none"
            },
            $"cancel-{request.SubscriptionId}");

        return new CancelSubscriptionResult
        {
            SubscriptionId = request.SubscriptionId,
            Status = dto.CancelAtPeriodEnd
                ? CancelSubscriptionStatus.Canceled
                : CancelSubscriptionStatus.Failed
        };
    }

    public async Task<RefundResult> Refund(RefundRequest request)
    {
        Validate(request);

        await ValidateRefundable(request.PaymentTransactionId);

        var dto = await PostForm<StripeRefundResponse>(
            StripeConstants.Refunds,
            new Dictionary<string, string>
            {
                ["payment_intent"] = request.PaymentTransactionId
            },
            $"refund-{request.PaymentTransactionId}");

        if (!dto.IsValid())
            throw new TransactionProcessingException("Invalid refund response");

        return new RefundResult
        {
            RefundId = dto.Id,
            OriginalPaymentTransactionId = request.PaymentTransactionId,
            Status = MapRefundStatus(dto.Status)
        };
    }

    private async Task ValidateRefundable(string paymentIntentId)
    {
        var intent = await Get<StripePaymentIntentResponse>(
            $"{StripeConstants.PaymentIntents}/{paymentIntentId}");

        if (intent.Status != "succeeded")
            throw new TransactionProcessingException(
                "Only succeeded payments can be refunded");
    }

    public Task<PaymentCallbackResult> ProcessCallback(PaymentCallbackInput input)
    {
        if (string.IsNullOrWhiteSpace(input.PaymentTransactionId))
            throw new ValidationException("PaymentTransactionId is required");

        if (input.Metadata.Count == 0)
            throw new ValidationException("Metadata is missing");

        if (!input.Metadata.TryGetValue(StripeConstants.MetadataType, out var type))
            throw new TransactionProcessingException("Missing event type");

        var result = new PaymentCallbackResult
        {
            Provider = ProviderName,
            PaymentTransactionId = input.PaymentTransactionId,
            Status = MapCallbackStatus(type)
        };

        if (input.Metadata.TryGetValue(StripeConstants.MetadataSubscriptionId, out var subId))
        {
            result.SubscriptionInfo = new Subscription
            {
                ProviderSubscriptionId = subId,
                PeriodEnd = TryParsePeriodEnd(input)
            };
        }

        return Task.FromResult(result);
    }

    private async Task<string> GetOrCreateCustomer(CreateSubscriptionRequest request)
    {
        if (request.Metadata.TryGetValue(ProviderCustomerId, out var existing)
            && !string.IsNullOrWhiteSpace(existing))
        {
            return existing;
        }

        var dto = await PostForm<JsonElement>(
            StripeConstants.Customers,
            new Dictionary<string, string>
            {
                ["metadata[user_id]"] = request.UserId
            },
            $"customer-{request.UserId}");

        return dto.GetProperty("id").GetString()
            ?? throw new TransactionProcessingException("Customer id missing");
    }

    private async Task<StripeSubscriptionResponse?> GetActiveSubscription(string customerId)
    {
        var list = await Get<StripeSubscriptionListResponse>(
            $"{StripeConstants.Subscriptions}?customer={customerId}&status=all&limit=10");

        var active = list.Data
            .Where(x => x.Status is "active" or "trialing" or "past_due" or "incomplete")
            .ToList();

        if (active.Count > 1)
            throw new TransactionProcessingException("Multiple active subscriptions");

        return active.SingleOrDefault();
    }

    private static string ResolvePriceId(SubscriptionInfo info)
        => info.SubscriptionType switch
        {
            SubscriptionType.Pro => info.Duration == SubscriptionPeriod.Monthly
                    ? "price_1TQaL1KGfL2wdOOTVsySR6Z9"
                    : "price_1TQaLeKGfL2wdOOT5tIwpKTD",
            SubscriptionType.Advanced => info.Duration == SubscriptionPeriod.Monthly
                    ? "price_1TQaM2KGfL2wdOOTxA5MZ1mT"
                    : "price_1TQaNCKGfL2wdOOTNRmoYxxE",
            _ => throw new TransactionProcessingException($"Invalid subscription type: {info.SubscriptionType}")
        };

    private static CreateSubscriptionStatus MapStatus(string status)
        => status switch
        {
            "active" or "trialing" => CreateSubscriptionStatus.AlreadyActive,
            "incomplete" or "past_due" => CreateSubscriptionStatus.AlreadyExistsIncomplete,
            "canceled" => CreateSubscriptionStatus.Created,
            _ => CreateSubscriptionStatus.Created
        };

    private static RefundStatus MapRefundStatus(string status)
        => status switch
        {
            "succeeded" => RefundStatus.Succeeded,
            "pending" => RefundStatus.Pending,
            "failed" => RefundStatus.Failed,
            "canceled" => RefundStatus.Canceled,
            _ => RefundStatus.Unknown
        };

    private static PaymentCallbackStatus MapCallbackStatus(string type)
        => type switch
        {
            StripeConstants.Events.PaymentSucceeded => PaymentCallbackStatus.PaymentSucceeded,
            StripeConstants.Events.PaymentFailed => PaymentCallbackStatus.PaymentFailed,
            StripeConstants.Events.PaymentProcessing => PaymentCallbackStatus.PaymentProcessing,
            StripeConstants.Events.PaymentCanceled => PaymentCallbackStatus.PaymentCanceled,

            StripeConstants.Events.SubscriptionCreated => PaymentCallbackStatus.SubscriptionCreated,
            StripeConstants.Events.SubscriptionUpdated => PaymentCallbackStatus.SubscriptionUpdated,
            StripeConstants.Events.SubscriptionDeleted => PaymentCallbackStatus.SubscriptionCanceled,

            _ => throw new TransactionProcessingException($"Unknown event: {type}")
        };

    private static DateTime? TryParsePeriodEnd(PaymentCallbackInput input)
    {
        if (input.Metadata.TryGetValue("period_end", out var raw)
            && long.TryParse(raw, out var unix))
        {
            return DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;
        }

        return null;
    }

    public class StripeSubscriptionCancelResponse
    {
        public bool CancelAtPeriodEnd { get; set; }
    }

    public class StripeSubscriptionResponse
    {
        public string Id { get; set; } = default!;
        public string Status { get; set; } = default!;
        public StripeInvoice? LatestInvoice { get; set; }
    }

    public class StripeInvoice
    {
        public StripePaymentIntent? PaymentIntent { get; set; }
    }

    public class StripePaymentIntent
    {
        public string? ClientSecret { get; set; }
    }

    public class StripeRefundResponse
    {
        public string Id { get; set; } = default!;
        public string Status { get; set; } = default!;

        public bool IsValid() => !string.IsNullOrWhiteSpace(Id);
    }

    public class StripePaymentIntentResponse
    {
        public string Id { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public string Status { get; set; } = default!;

        public bool IsValid()
            => !string.IsNullOrWhiteSpace(Id)
            && !string.IsNullOrWhiteSpace(ClientSecret);
    }

    public class StripeSubscriptionListResponse
    {
        public List<StripeSubscriptionResponse> Data { get; set; } = [];
    }
}