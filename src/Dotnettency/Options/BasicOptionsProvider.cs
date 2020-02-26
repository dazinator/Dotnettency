using System;

namespace Dotnettency
{
    /// <summary>
    /// An options provider that has it's TOptions passed to it in the constructor. This means if you are relying on DI,
    /// you must register the <see cref="TOptions"/> for DI also.
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class BasicOptionsProvider<TOptions> : IOptionsProvider<TOptions>
    {
        private readonly TOptions _options;

        public BasicOptionsProvider(TOptions options)
        {
            _options = options;
        }

        public TOptions CurrentValue { get => _options; }

        public IDisposable OnChange(Action<TOptions> handleChange)
        {
            return null;
            // this implementations does not support change notifications. Use something like the microsoft options system if you need those.
            // throw new NotSupportedException();
        }
    }
}