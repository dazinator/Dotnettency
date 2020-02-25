using System;

namespace Dotnettency
{
    public interface IOptionsProvider<TOptions>
    {
        TOptions CurrentValue { get; }

        IDisposable OnChange(Action<TOptions> handleChange);
    }
}