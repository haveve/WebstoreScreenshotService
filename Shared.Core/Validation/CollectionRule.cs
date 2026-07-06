namespace Shared.Core.Validation;

public class CollectionRule<T, TItem, TCollection> : IRule<T> where TCollection : IEnumerable<TItem>
{
    private readonly string _name;
    private readonly Func<T, TCollection> _selector;
    private readonly Validator<TItem> _validator;

    public CollectionRule(string name, Func<T, TCollection> selector, Validator<TItem> validator)
    {
        _name = name;
        _selector = selector;
        _validator = validator;
    }

    public void Validate(T instance, ValidationResult result, string path)
    {
        var items = _selector(instance);
        if (items == null) return;

        int i = 0;

        foreach (var item in items)
        {
            var currentPath = $"{_name}[{i}]";
            var nested = _validator.Validate(item);

            result.Merge(currentPath, nested);
            i++;
        }
    }
}
