using Shared.Core.Validation;

namespace WebsiteScreenshotService.Model.Validation;

public interface IValidatorRegistry
{
    bool TryGet(Type type, out Func<object, ValidationResult> validator);
}

public class ValidatorRegistry : IValidatorRegistry
{
    private readonly Dictionary<Type, Func<object, ValidationResult>> _map = [];

    public ValidatorRegistry Add<T>(params IValidator<T>[] validators)
    {
        _map[typeof(T)] = instance =>
        {
            var typed = (T)instance;
            var result = new ValidationResult();

            foreach (var validator in validators)
            {
                result.Merge(null, validator.Validate(typed));
            }

            return result;
        };

        return this;
    }

    public bool TryGet(Type type, out Func<object, ValidationResult> validator)
        => _map.TryGetValue(type, out validator!);
}