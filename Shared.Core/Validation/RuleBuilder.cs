namespace Shared.Core.Validation;

public class RuleBuilder<T, TProp> : IRule<T>
{
    private readonly Func<T, TProp> _selector;
    private readonly string _name;
    private readonly List<Action<TProp, ValidationResult, string>> _validators = new();

    public RuleBuilder(string name, Func<T, TProp> selector)
    {
        _selector = selector;
        _name = name;
    }

    public RuleBuilder<T, TProp> Add(Action<TProp, ValidationResult, string> validator)
    {
        _validators.Add(validator);
        return this;
    }

    public void Validate(T instance, ValidationResult result, string path)
    {
        var value = _selector(instance);
        var currentPath = string.IsNullOrEmpty(path) ? _name : $"{path}.{_name}";

        foreach (var v in _validators)
            v(value, result, currentPath);
    }
}
