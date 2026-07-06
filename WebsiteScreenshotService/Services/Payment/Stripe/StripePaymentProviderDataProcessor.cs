namespace WebsiteScreenshotService.Services.Payment.Stripe;

using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WebsiteScreenshotService.Services.Payment.Exceptions;
using WebsiteScreenshotService.Services.Payment.Models;
using WebsiteScreenshotService.Utils;

public class StripePaymentProviderDataProcessor(IPaymentProviderCallbackConfigurationManager ConfigurationManager) : IPaymentProviderDataProcessor
{
    public async Task<Result<PaymentCallbackInput>> ProcessCallbackRequestDataAsync(HttpRequest httpRequest)
    {
        try
        {
            httpRequest.EnableBuffering();
            httpRequest.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(httpRequest.Body);
            var body = await reader.ReadToEndAsync();

            var signatureHeader = httpRequest.Headers["Stripe-Signature"].ToString();

            VerifySignature(body, signatureHeader);

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

    private void VerifySignature(string payload, string header)
    {
        var secret = ConfigurationManager
            .GetSensitiveConfigurations()
            .Data[StripeConstants.Settings.Sensitive.Secret];

        if (string.IsNullOrWhiteSpace(header))
            throw new PaymentDataProcessingException("Missing Stripe signature header");

        string? timestamp = null;
        var signatures = new List<string>();

        foreach (var element in header.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = element.Split('=', 2, StringSplitOptions.TrimEntries);

            if (parts.Length != 2)
                continue;

            if (parts[0] == "t")
                timestamp = parts[1];

            if (parts[0] == "v1")
                signatures.Add(parts[1]);
        }

        if (timestamp is null || signatures.Count == 0)
            throw new PaymentDataProcessingException("Invalid Stripe signature header");

        if (!long.TryParse(timestamp, out var unixTime))
            throw new PaymentDataProcessingException("Invalid Stripe timestamp");

        var eventTime = DateTimeOffset.FromUnixTimeSeconds(unixTime);

        if (Math.Abs((DateTimeOffset.UtcNow - eventTime).TotalMinutes) > 5)
            throw new PaymentDataProcessingException("Stripe webhook expired");

        var signedPayload = $"{timestamp}.{payload}";

        var expectedSignature = ComputeHexHmac(secret, signedPayload);

        var valid = false;

        foreach (var sig in signatures)
        {
            if (CryptographicOperations.FixedTimeEquals(
                    HexToBytes(expectedSignature),
                    HexToBytes(sig)))
            {
                valid = true;
            }
        }

        if (!valid)
            throw new PaymentDataProcessingException("Invalid Stripe signature");
    }

    private static string ComputeHexHmac(string secret, string payload)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var msgBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(msgBytes);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static byte[] HexToBytes(string hex)
    {
        var bytes = new byte[hex.Length / 2];

        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

        return bytes;
    }
}