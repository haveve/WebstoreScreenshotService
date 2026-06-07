using System.Collections.ObjectModel;

namespace WebsiteScreenshotService.Entities;

public record Product(Guid Id, string Name, ProductType Type, ReadOnlyDictionary<string, string> AdditionalInfo);

public enum ProductType
{
    Points = 1,
    Subscription = 2,
}