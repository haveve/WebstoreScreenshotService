namespace Shared.Core.Validation;

public interface IRule<T>
{
    void Validate(T instance, ValidationResult result, string path);
}
