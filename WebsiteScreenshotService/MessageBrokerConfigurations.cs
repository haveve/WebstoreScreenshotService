using System.ComponentModel.DataAnnotations;
using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService;

public class MessageBrokerConfigurations
{
    [Required]
    public required ExchangeConfig Exchange { get; set; }

    [Required]
    public required QueueConfig Queue { get; set; }

    [Required]
    public required ConnectionConfig Connection { get; set; }
}

public class ExchangeConfig
{
    [Required]
    public required string Name { get; set; }
}

public class ConnectionConfig
{
    [Required]
    public required string HostName { get; set; }

    [Required]
    public required string UserName { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required int Port { get; set; }

    public string VirtualHost { get; set; } = "/";
}

public class QueueConfig : IValidatableObject
{
    [Required]
    public required Dictionary<SubscriptionType, string> QueuePerSubscription { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _)
        => ValidateContentInitializationStepsSettings();

    private IEnumerable<ValidationResult> ValidateContentInitializationStepsSettings()
    {
        foreach (var stepName in Enum.GetValues<SubscriptionType>())
        {
            if (!QueuePerSubscription.ContainsKey(stepName))
            {
                yield return new ValidationResult(
                    $"Missing required content initialization step: '{stepName}'.",
                    [nameof(QueuePerSubscription)]);
            }
        }

        foreach (var kvp in QueuePerSubscription)
        {
            var step = kvp.Value;
            var results = new List<ValidationResult>();
            var context = new ValidationContext(step);

            if (!Validator.TryValidateObject(step, context, results, validateAllProperties: true))
            {
                foreach (var validationResult in results)
                {
                    yield return new ValidationResult(
                        $"Step '{kvp.Key}' error: {validationResult.ErrorMessage}",
                        [$"{nameof(QueuePerSubscription)}[{kvp.Key}]"]);
                }
            }
        }
    }
}