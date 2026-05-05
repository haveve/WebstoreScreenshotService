namespace WebsiteScreenshotService.Services.Payment.Stripe;

using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WebsiteScreenshotService.Services.Payment.Exceptions;
using WebsiteScreenshotService.Services.Payment.Models;
using WebsiteScreenshotService.Utils;

public class StripePaymentProviderDataProcessor(IConfiguration config) : IPaymentProviderDataProcessor
{
    private readonly string secret = config["Stripe:WebhookSecret"]
                         ?? throw new InvalidDataException("Stripe webhook secret missing");

    public async Task<Result<PaymentCallbackInput>> ProcessCallbackRequestDataAsync(HttpRequest httpRequest)
    {
        try
        {
            httpRequest.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(httpRequest.Body);
            var body = await reader.ReadToEndAsync();

            var signatureHeader = httpRequest.Headers["Stripe-Signature"].ToString();

            VerifySignature(body, signatureHeader, secret);

            using var json = JsonDocument.Parse(body);
            var root = json.RootElement;

            if (!root.TryGetProperty("type", out var typeProp))
                return Result<PaymentCallbackInput>.Error("Missing event type");

            var eventType = typeProp.GetString();

            if (string.IsNullOrWhiteSpace(eventType))
                return Result<PaymentCallbackInput>.Error("Empty event type");

            if (!root.TryGetProperty("data", out var data) ||
                !data.TryGetProperty("object", out var obj))
            {
                return Result<PaymentCallbackInput>.Error("Missing event object");
            }

            var result = new PaymentCallbackInput
            {
                PaymentTransactionId = ExtractPaymentId(eventType, obj),
                Metadata = new Dictionary<string, string>
                {
                    ["type"] = eventType
                }
            };

            if (obj.TryGetProperty("subscription", out var subProp))
            {
                var subId = subProp.GetString();
                if (!string.IsNullOrWhiteSpace(subId))
                    result.Metadata["subscription_id"] = subId;
            }

            if (obj.TryGetProperty("payment_intent", out var piProp))
            {
                var paymentIntentId =
                    piProp.ValueKind == JsonValueKind.String
                        ? piProp.GetString()
                        : piProp.TryGetProperty("id", out var inner)
                            ? inner.GetString()
                            : null;

                if (!string.IsNullOrWhiteSpace(paymentIntentId))
                    result.Metadata["payment_intent"] = paymentIntentId;
            }

            if (obj.TryGetProperty("period_end", out var periodEnd))
            {
                if (periodEnd.ValueKind == JsonValueKind.Number &&
                    periodEnd.TryGetInt64(out var unix))
                {
                    result.Metadata["period_end"] = unix.ToString();
                }
            }

            // fallback id
            if (string.IsNullOrWhiteSpace(result.PaymentTransactionId) &&
                obj.TryGetProperty("id", out var idProp))
            {
                var id = idProp.GetString();
                if (!string.IsNullOrWhiteSpace(id))
                    result.PaymentTransactionId = id;
            }

            if (obj.TryGetProperty("metadata", out var metadata) &&
                metadata.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in metadata.EnumerateObject())
                {
                    var value = prop.Value.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        result.Metadata[prop.Name] = value;
                    }
                }
            }

            return Result<PaymentCallbackInput>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PaymentCallbackInput>.Error(ex.Message);
        }
    }

    private static string ExtractPaymentId(string eventType, JsonElement obj)
    {
        static string GetRequiredId(string eventType, JsonElement el)
        {
            if (el.TryGetProperty("id", out var idProp))
            {
                var id = idProp.GetString();
                if (!string.IsNullOrWhiteSpace(id))
                    return id;
            }

            throw new PaymentDataProcessingException(
                $"Stripe event '{eventType}' is missing required 'id' field."
            );
        }

        static string GetPaymentIntentId(string eventType, JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.String)
            {
                var id = el.GetString();
                if (!string.IsNullOrWhiteSpace(id))
                    return id;
            }

            return GetRequiredId(eventType, el);
        }

        return eventType switch
        {
            "payment_intent.succeeded" => GetRequiredId(eventType, obj),

            "payment_intent.payment_failed" => GetRequiredId(eventType, obj),

            "invoice.payment_succeeded" =>
                obj.TryGetProperty("payment_intent", out var pi)
                    ? GetPaymentIntentId(eventType, pi)
                    : GetRequiredId(eventType, obj),

            "invoice.payment_failed" => GetRequiredId(eventType, obj),

            "charge.refunded" =>
                obj.TryGetProperty("payment_intent", out var refundPi)
                    ? GetPaymentIntentId(eventType, refundPi)
                    : throw new InvalidOperationException(
                        $"Stripe event '{eventType}' missing payment_intent."
                    ),

            _ => GetRequiredId(eventType, obj)
        };
    }

    // Stripe signature verification
    private static void VerifySignature(string payload, string header, string secret)
    {
        var elements = header.Split(',');

        string? timestamp = null;
        string? signature = null;

        foreach (var element in elements)
        {
            var parts = element.Split('=');

            if (parts[0] == "t")
                timestamp = parts[1];

            if (parts[0] == "v1")
                signature = parts[1];
        }

        if (timestamp == null || signature == null)
            throw new PaymentDataProcessingException("Invalid Stripe signature header");

        var eventTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp));

        if (Math.Abs((DateTimeOffset.UtcNow - eventTime).TotalMinutes) > 5)
            throw new PaymentDataProcessingException("Stripe webhook expired");

        var signedPayload = $"{timestamp}.{payload}";

        var expectedBytes = ComputeHmac(secret, signedPayload);
        var actualBytes = Convert.FromHexString(signature);

        if (!CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes))
            throw new PaymentDataProcessingException("Invalid Stripe signature");
    }

    private static byte[] ComputeHmac(string secret, string payload)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var msgBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);

        return hmac.ComputeHash(msgBytes);
    }
}