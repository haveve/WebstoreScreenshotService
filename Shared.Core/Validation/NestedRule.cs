namespace Shared.Core.Validation;

public class NestedRule<T, TProp> : IRule<T>
{
    private readonly string _name;
    private readonly Func<T, TProp?> _selector;
    private readonly Validator<TProp> _validator;

    public NestedRule(string name, Func<T, TProp?> selector, Validator<TProp> validator)
    {
        _name = name;
        _selector = selector;
        _validator = validator;
    }

    public void Validate(T instance, ValidationResult result, string path)
    {
        var value = _selector(instance);
        if (value == null) return;

        var currentPath = string.IsNullOrEmpty(path) ? _name : $"{path}.{_name}";
        var nested = _validator.Validate(value);

        result.Merge(currentPath, nested);
    }
}
