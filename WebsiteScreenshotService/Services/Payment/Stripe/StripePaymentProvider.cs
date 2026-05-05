using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Services.Payment.Exceptions;
using WebsiteScreenshotService.Services.Payment.Models;

namespace WebsiteScreenshotService.Services.Payment.Stripe;

public class StripePaymentProvider(HttpClient http, IConfiguration config) : IPaymentProvider
{
    public string ProviderName => "Stripe";

    private readonly HttpClient _http = http;
    private readonly string _secretKey =
        config["Stripe:SecretKey"] ?? throw new Exception("Missing Stripe key");

    private const string BaseUrl = StripeConstants.BaseUrl;

    private const string ProviderCustomerId = StripeConstants.MetadataProviderCustomerId;

    private const string ClientSecret = StripeConstants.MetadataClientSecret;

    // PRICE RESOLUTION
    private static string PickPriceIdBasedOnSubscription(SubscriptionInfo subscriptionInfo)
        => subscriptionInfo.SubscriptionType switch
        {
            SubscriptionType.Pro =>
                subscriptionInfo.Duration == Duration.Monthly
                    ? "price_1TQaL1KGfL2wdOOTVsySR6Z9"
                    : "price_1TQaLeKGfL2wdOOT5tIwpKTD",

            SubscriptionType.Advanced =>
                subscriptionInfo.Duration == Duration.Monthly
                    ? "price_1TQaM2KGfL2wdOOTxA5MZ1mT"
                    : "price_1TQaNCKGfL2wdOOTNRmoYxxE",

            _ => throw new TransactionProcessingException($"Invalid subscription type: {subscriptionInfo.SubscriptionType}")
        };

    public async Task<StartPaymentResult> StartPayment(StartPaymentRequest request)
    {
        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{BaseUrl}{StripeConstants.PaymentIntents}"
        );

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
        httpRequest.Headers.Add("Idempotency-Key", $"order_{request.OrderId}");

        httpRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["amount"] = request.Amount.ToMinorUnits().ToString(),
            ["currency"] = request.Amount.Currency,
            ["metadata[order_id]"] = request.OrderId.ToString(),
            ["payment_method_types[]"] = "card"
        });

        var response = await _http.SendAsync(httpRequest);

        if (!response.IsSuccessStatusCode)
        {
            return new StartPaymentResult
            (
                PaymentId: string.Empty,
                Metadata: ReadOnlyDictionary<string, string>.Empty,
                Status: StartPaymentStatus.Failed
            );
        }

        var jsonStream = await response.Content.ReadAsStreamAsync();
        var dto = await JsonSerializer.DeserializeAsync<StripePaymentIntentResponse>(jsonStream);

        if (dto is null || !dto.IsValid())
        {
            return new StartPaymentResult
            (
                PaymentId: string.Empty,
                Metadata: ReadOnlyDictionary<string, string>.Empty,
                Status: StartPaymentStatus.Failed
            );
        }

        var metadata = new Dictionary<string, string>(1)
        {
            [ClientSecret] = dto.ClientSecret
        };

        return new StartPaymentResult
        (
            PaymentId: dto.Id,
            Metadata: new ReadOnlyDictionary<string, string>(metadata),
            Status: MapStartPaymentStatus(dto.Status)
        );
    }

    public async Task<CreateSubscriptionResult> CreateSubscription(CreateSubscriptionRequest request)
    {
        var priceId = PickPriceIdBasedOnSubscription(request.SubscriptionInfo);
        var customerId = await GetOrCreateCustomer(request);

        var existingSubscription = await GetActiveSubscription(customerId);

        var metadata = new Dictionary<string, string>(1)
        {
            [ProviderCustomerId] = customerId
        };

        if (existingSubscription is not null)
        {
            var mappedStatus = MapStatus(existingSubscription.Status);

            if (mappedStatus == CreateSubscriptionStatus.AlreadyActive)
            {
                return new CreateSubscriptionResult
                (
                    SubscriptionId: existingSubscription.Id,
                    Status: CreateSubscriptionStatus.AlreadyActive,
                    Metadata: new ReadOnlyDictionary<string, string>(metadata)
                );
            }

            if (mappedStatus == CreateSubscriptionStatus.AlreadyExistsIncomplete)
            {
                var clientSecretValue = existingSubscription.LatestInvoice?.PaymentIntent?.ClientSecret;

                if (string.IsNullOrEmpty(clientSecretValue))
                    throw new TransactionProcessingException("Incomplete subscription missing payment intent client secret");

                metadata.Add(ClientSecret, clientSecretValue);

                return new CreateSubscriptionResult
                (
                    SubscriptionId: existingSubscription.Id,
                    Status: CreateSubscriptionStatus.AlreadyExistsIncomplete,
                    Metadata: new ReadOnlyDictionary<string, string>(metadata)
                );
            }
        }

        // CREATE NEW SUBSCRIPTION
        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{BaseUrl}{StripeConstants.Subscriptions}"
        );

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _secretKey);

        httpRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        httpRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["customer"] = customerId,
            ["items[0][price]"] = priceId,
            ["payment_behavior"] = "default_incomplete",
            ["expand[]"] = "latest_invoice.payment_intent"
        });

        var response = await _http.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var dto = JsonSerializer.Deserialize<StripeSubscriptionResponse>(json)
                  ?? throw new TransactionProcessingException("Invalid subscription response");

        var clientSecret = dto.LatestInvoice?.PaymentIntent?.ClientSecret;

        if (string.IsNullOrEmpty(clientSecret))
            throw new TransactionProcessingException("Missing payment intent client secret");

        metadata.Add(ClientSecret, clientSecret);

        return new CreateSubscriptionResult
        (
            SubscriptionId: dto.Id,
            Status: CreateSubscriptionStatus.Created,
            Metadata: new ReadOnlyDictionary<string, string>(metadata)
        );
    }

    private static CreateSubscriptionStatus MapStatus(string stripeStatus)
    {
        return stripeStatus switch
        {
            "active" or "trialing" => CreateSubscriptionStatus.AlreadyActive,
            "incomplete" => CreateSubscriptionStatus.AlreadyExistsIncomplete,
            "past_due" => CreateSubscriptionStatus.AlreadyExistsIncomplete,
            "canceled" => CreateSubscriptionStatus.Created,
            _ => CreateSubscriptionStatus.Created
        };
    }

    private async Task<StripeSubscriptionResponse?> GetActiveSubscription(string customerId)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{BaseUrl}{StripeConstants.Subscriptions}?customer={customerId}&status=all&limit=10&order=desc"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _secretKey);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var list = JsonSerializer.Deserialize<StripeSubscriptionListResponse>(json);

        var subscriptions = list?.Data ?? [];

        var active = subscriptions
            .Where(s => s.Status is "active"
                or "trialing"
                or "past_due"
                or "incomplete")
            .ToList();

        if (active.Count > 1)
            throw new TransactionProcessingException("Stripe invariant violation: multiple active subscriptions");

        return active.FirstOrDefault();
    }

    private async Task<StripeSubscriptionResponse?> GetExistingSubscription(string subscriptionId)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{BaseUrl}{StripeConstants.Subscriptions}/{subscriptionId}"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _secretKey);

        var response = await _http.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var subscription = JsonSerializer.Deserialize<StripeSubscriptionResponse>(json);

        return subscription
            ?? throw new TransactionProcessingException($"Invalid Stripe response for subscription {subscriptionId}");
    }

    public async Task<ChangeSubscriptionPlanResult> ChangeSubscriptionPlan(ChangeSubscriptionPlanRequest request)
    {
        var newPriceId = PickPriceIdBasedOnSubscription(request.NewSubscriptionInfo);
        var isDowngrade = IsDowngrade(request);

        var subscription = await GetExistingSubscription(request.SubscriptionId)
            ?? throw new TransactionProcessingException(
                $"Subscription {request.SubscriptionId} not found"
            );

        ValidateSubscriptionState(subscription);

        var item = subscription.Items?.Data?.SingleOrDefault()
            ?? throw new TransactionProcessingException(
                $"Subscription {subscription.Id} must contain exactly 1 item"
            );

        var currentPriceId = item.Price?.Id;

        if (currentPriceId == newPriceId)
        {
            return new ChangeSubscriptionPlanResult
            {
                SubscriptionId = subscription.Id,
                Status = ChangeSubscriptionStatus.AlreadyOnSamePlan
            };
        }

        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{BaseUrl}{StripeConstants.Subscriptions}/{request.SubscriptionId}"
        );

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _secretKey);

        httpRequest.Headers.Add(
            "Idempotency-Key",
            $"sub-change-{subscription.Id}-{newPriceId}-{request.UpgradeTiming}"
        );

        var data = new Dictionary<string, string>
        {
            ["items[0][id]"] = item.Id,
            ["items[0][price]"] = newPriceId
        };

        ApplyProrationRules(data, request, isDowngrade);

        httpRequest.Content = new FormUrlEncodedContent(data);

        var response = await _http.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        return new ChangeSubscriptionPlanResult
        {
            SubscriptionId = subscription.Id,
            Status = MapChangeStatus(request.UpgradeTiming)
        };
    }

    private static readonly HashSet<string> ValidActiveStates = new()
        {
            "active",
            "trialing",
            "past_due"
        };

    private static void ApplyProrationRules(
    Dictionary<string, string> data,
    ChangeSubscriptionPlanRequest request,
    bool isDowngrade)
    {
        if (isDowngrade)
        {
            // business rule: downgrades never immediate
            data["proration_behavior"] = "none";
            return;
        }

        data["proration_behavior"] =
            request.UpgradeTiming == SubscriptionChangeTiming.Immediate
                ? "create_prorations"
                : "none";
    }

    private static ChangeSubscriptionStatus MapChangeStatus(SubscriptionChangeTiming timing)
    {
        return timing switch
        {
            SubscriptionChangeTiming.Immediate => ChangeSubscriptionStatus.UpgradeApplied,
            SubscriptionChangeTiming.EndOfPeriod => ChangeSubscriptionStatus.UpgradeScheduled,
            _ => throw new TransactionProcessingException($"Invalid timing type: {timing}")
        };
    }

    private static void ValidateSubscriptionState(StripeSubscriptionResponse subscription)
    {
        if (subscription == null)
            throw new TransactionProcessingException("Subscription is null");

        if (subscription.Status == "canceled")
            throw new TransactionProcessingException("Cannot modify a canceled subscription");

        if (!ValidActiveStates.Contains(subscription.Status))
            throw new TransactionProcessingException(
                $"Subscription is not in a modifiable state: {subscription.Status}"
            );
    }

    public async Task<CancelSubscriptionResult> CancelSubscription(CancelSubscriptionRequest request)
    {
        var url = $"{BaseUrl}{StripeConstants.Subscriptions}/{request.SubscriptionId}";

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _secretKey);

        httpRequest.Headers.Add(
            "Idempotency-Key",
            $"cancel-sub-{request.SubscriptionId}-{request.CancelAtPeriodEnd}"
        );

        var data = new Dictionary<string, string>();

        if (request.CancelAtPeriodEnd)
        {
            data["cancel_at_period_end"] = "true";
        }
        else
        {
            data["cancel_at_period_end"] = "false";
            data["proration_behavior"] = "none";
        }

        httpRequest.Content = new FormUrlEncodedContent(data);

        var response = await _http.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        return new CancelSubscriptionResult
        {
            SubscriptionId = request.SubscriptionId,
            Status = request.CancelAtPeriodEnd
                ? CancelSubscriptionStatus.Scheduled
                : CancelSubscriptionStatus.Canceled
        };
    }

    public async Task<RefundResult> Refund(RefundRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PaymentTransactionId))
            throw new TransactionProcessingException("PaymentIntentId is required for refund");

        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{BaseUrl}{StripeConstants.Refunds}"
        );

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _secretKey);

        httpRequest.Headers.Add(
            "Idempotency-Key",
            $"refund-full-{request.PaymentTransactionId}"
        );

        httpRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["payment_intent"] = request.PaymentTransactionId
        });

        var response = await _http.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var refund = JsonSerializer.Deserialize<StripeRefundResponse>(json)
            ?? throw new TransactionProcessingException("Invalid Stripe refund response");

        return new RefundResult
        {
            RefundId = refund.Id,
            Status = MapRefundStatus(refund.Status),
            OriginalPaymentTransactionId = request.PaymentTransactionId
        };
    }

    public async Task<PaymentCallbackResult> ProcessCallback(PaymentCallbackInput input)
    {
        if (!input.Metadata.TryGetValue(StripeConstants.MetadataType, out var type))
            throw new TransactionProcessingException("Missing Stripe metadata: type");

        var result = new PaymentCallbackResult
        {
            Provider = ProviderName,
            PaymentTransactionId = input.PaymentTransactionId,
            Status = MapCallbackStatus(type)
        };

        if (input.Metadata.TryGetValue(StripeConstants.MetadataSubscriptionId, out var subId)
            && !string.IsNullOrWhiteSpace(subId))
        {
            result.SubscriptionInfo = new Subscription
            {
                SubscriptionId = subId,
                PeriodEnd = TryParsePeriodEnd(input)
            };
        }

        return result;
    }

    private async Task<string> GetOrCreateCustomer(CreateSubscriptionRequest data)
    {
        if (data.Metadata.TryGetValue(ProviderCustomerId, out var existing) &&
            !string.IsNullOrWhiteSpace(existing))
        {
            return existing;
        }

        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{BaseUrl}{StripeConstants.Customers}"
        );

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _secretKey);

        // IMPORTANT: prevent duplicate customers on retry
        httpRequest.Headers.Add(
            "Idempotency-Key",
            $"customer-{data.UserId}"
        );

        httpRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["metadata[user_id]"] = data.UserId
            // optionally: ["email"] = data.Email
        });

        var response = await _http.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("id").GetString()!;
    }

    private static StartPaymentStatus MapStartPaymentStatus(string status) => status switch
    {
        // 🔹 All "needs user interaction" states → Pending
        "requires_payment_method" => StartPaymentStatus.Pending,
        "requires_confirmation" => StartPaymentStatus.Pending,
        "requires_action" => StartPaymentStatus.Pending,
        "processing" => StartPaymentStatus.Pending,

        // 🔹 Already completed (rare, but supported)
        "succeeded" => StartPaymentStatus.Succeeded,

        // 🔹 Explicit cancel
        "canceled" => StartPaymentStatus.Failed,

        // 🔹 Unknown → treat as failed (safe default)
        _ => StartPaymentStatus.Failed
    };

    private static RefundStatus MapRefundStatus(string status) => status switch
    {
        "succeeded" => RefundStatus.Succeeded,
        "pending" => RefundStatus.Pending,
        "failed" => RefundStatus.Failed,
        "canceled" => RefundStatus.Failed,
        _ => RefundStatus.Failed
    };

    private static PaymentCallbackStatus MapCallbackStatus(string type) => type switch
    {
        StripeConstants.Events.PaymentSucceeded => PaymentCallbackStatus.PaymentSucceeded,
        StripeConstants.Events.PaymentProcessing => PaymentCallbackStatus.PaymentProcessing,
        StripeConstants.Events.PaymentFailed => PaymentCallbackStatus.PaymentFailed,
        StripeConstants.Events.PaymentCanceled => PaymentCallbackStatus.PaymentCanceled,

        StripeConstants.Events.ChargeRefunded => PaymentCallbackStatus.RefundSucceeded,
        StripeConstants.Events.RefundCreated => PaymentCallbackStatus.RefundSucceeded,
        StripeConstants.Events.RefundFailed => PaymentCallbackStatus.RefundFailed,

        StripeConstants.Events.InvoiceSucceeded => PaymentCallbackStatus.SubscriptionPaymentSucceeded,
        StripeConstants.Events.InvoiceFailed => PaymentCallbackStatus.SubscriptionPaymentFailed,
        StripeConstants.Events.InvoiceActionRequired => PaymentCallbackStatus.SubscriptionPaymentActionRequired,

        StripeConstants.Events.SubscriptionCreated => PaymentCallbackStatus.SubscriptionCreated,
        StripeConstants.Events.SubscriptionUpdated => PaymentCallbackStatus.SubscriptionUpdated,
        StripeConstants.Events.SubscriptionDeleted => PaymentCallbackStatus.SubscriptionCanceled,
        StripeConstants.Events.SubscriptionPaused => PaymentCallbackStatus.SubscriptionPaused,
        StripeConstants.Events.SubscriptionResumed => PaymentCallbackStatus.SubscriptionResumed,

        _ => throw new TransactionProcessingException($"Unknown stripe callback type: {type}")
    };

    private static bool IsDowngrade(ChangeSubscriptionPlanRequest request)
        => request.NewSubscriptionInfo.SubscriptionType < request.SubscriptionInfo.SubscriptionType;

    private static DateTime? TryParsePeriodEnd(PaymentCallbackInput input)
    {
        if (input.Metadata.TryGetValue("period_end", out var raw) &&
            long.TryParse(raw, out var unix))
        {
            return DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;
        }

        return null;
    }
}

public class StripeSubscriptionCancelResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;

    [JsonPropertyName("cancel_at_period_end")]
    public bool CancelAtPeriodEnd { get; set; }
}

public class StripeSubscriptionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;

    [JsonPropertyName("latest_invoice")]
    public StripeInvoice? LatestInvoice { get; set; }

    [JsonPropertyName("items")]
    public StripeSubscriptionItemCollection Items { get; set; } = default!;
}

public class StripeSubscriptionItemCollection
{
    [JsonPropertyName("data")]
    public List<StripeSubscriptionItem> Data { get; set; } = [];
}

public class StripeSubscriptionItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("price")]
    public StripePrice Price { get; set; } = default!;
}

public class StripePrice
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;
}

public class StripeInvoice
{
    [JsonPropertyName("payment_intent")]
    public StripePaymentIntent? PaymentIntent { get; set; }
}

public class StripePaymentIntent
{
    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; set; }
}

public class StripeRefundResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    public bool IsValid()
        => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Status);
}

public class StripePaymentIntentResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    public bool IsValid()
        => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(ClientSecret) && !string.IsNullOrEmpty(Status);
}

public class StripeSubscriptionListResponse
{
    [JsonPropertyName("data")]
    public List<StripeSubscriptionResponse> Data { get; set; } = [];
}