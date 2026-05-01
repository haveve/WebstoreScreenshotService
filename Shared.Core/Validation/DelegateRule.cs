using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Validation;

public class DelegateRule<T> : IRule<T>
{
    private readonly Action<T, ValidationResult, string> _action;

    public DelegateRule(Action<T, ValidationResult, string> action)
    {
        _action = action;
    }

    public void Validate(T instance, ValidationResult result, string path)
        => _action(instance, result, path);
}
