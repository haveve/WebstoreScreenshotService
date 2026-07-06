namespace Shared.Core.Validation;

public record ValidationError(string Path, string Message);

public class ValidationResult
{
    private readonly List<ValidationError> _errors = [];

    public IReadOnlyList<ValidationError> Errors => _errors;

    public bool IsValid => _errors.Count == 0;

    public void Add(string path, string message)
    {
        _errors.Add(new ValidationError(NormalizePath(path), message));
    }

    public void Merge(string? prefix, ValidationResult other)
    {
        if (other == null || other._errors.Count == 0)
            return;

        foreach (var e in other.Errors)
        {
            var fullPath = CombinePath(prefix, e.Path);
            _errors.Add(new ValidationError(fullPath, e.Message));
        }
    }

    private static string NormalizePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        return path.Trim('.');
    }

    private static string CombinePath(string? prefix, string? path)
    {
        prefix = NormalizePath(prefix);
        path = NormalizePath(path);

        if (string.IsNullOrEmpty(prefix))
            return path ?? string.Empty;

        if (string.IsNullOrEmpty(path))
            return prefix;

        return $"{prefix}.{path}";
    }
}