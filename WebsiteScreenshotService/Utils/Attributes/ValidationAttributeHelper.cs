namespace WebsiteScreenshotService.Utils.Attributes;

/// <summary>
/// Provides helper methods for validation attributes.
/// </summary>
public class ValidationAttributeHelper
{
    /// <summary>
    /// Determines whether the specified value is unset (null, default, or empty string).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is unset; otherwise, <c>false</c>.</returns>
    public static bool IsUnset(object? value)
    {
        if (value == null)
            return true;

        var type = value.GetType();

        if (value is string strValue && string.IsNullOrWhiteSpace(strValue))
            return false;

        if (!type.IsValueType)
            return value.Equals(null);

        var defaultValue = Activator.CreateInstance(type);
        return value.Equals(defaultValue);
    }
}

