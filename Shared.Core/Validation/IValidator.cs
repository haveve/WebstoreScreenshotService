namespace Shared.Core.Validation;

public interface IValidator<T>
{
    public ValidationResult Validate(T instance);
}
