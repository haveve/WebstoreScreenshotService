namespace Shared.Core.Validation;

public abstract class Validator<T>: IValidator<T>
{
    private readonly List<IRule<T>> _rules = [];

    public RuleBuilder<T, TProp> RuleFor<TProp>(
        string name,
        Func<T, TProp> selector)
    {
        var rule = new RuleBuilder<T, TProp>(name, selector);
        _rules.Add(rule);
        return rule;
    }

    public void Rule(Action<T, ValidationResult, string> action)
        => _rules.Add(new DelegateRule<T>(action));

    public void Include<TProp>(
        string name,
        Func<T, TProp?> selector,
        Validator<TProp> validator)
        where TProp : class
        => _rules.Add(new NestedRule<T, TProp>(name, selector, validator));

    public RuleBuilder<T, TCollection> ForEach<TItem, TCollection>(
        string name,
        Func<T, TCollection> selector,
        Validator<TItem> itemValidator) where TCollection: IEnumerable<TItem>
    {
        var rule = new CollectionRule<T, TItem, TCollection>(name, selector, itemValidator);
        _rules.Add(rule);
        return new RuleBuilder<T, TCollection>(name, selector);
    }

    public ValidationResult Validate(T instance)
    {
        var result = new ValidationResult();

        foreach (var rule in _rules)
            rule.Validate(instance, result, "");

        return result;
    }
}
