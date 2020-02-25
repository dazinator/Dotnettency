using Microsoft.Extensions.Options;
using System;

namespace Dotnettency
{
    public class OptionsMonitorOptionsProvider<TOptions> : IOptionsProvider<TOptions>
    {
        private readonly IOptionsMonitor<TOptions> _optionsMonitor;

        public OptionsMonitorOptionsProvider(IOptionsMonitor<TOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public TOptions CurrentValue { get => _optionsMonitor.CurrentValue; }

        public IDisposable OnChange(Action<TOptions> handleChange)
        {
            return _optionsMonitor.OnChange(handleChange);
        }
    }
}