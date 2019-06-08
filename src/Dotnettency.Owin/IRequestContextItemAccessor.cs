using System;

namespace Dotnettency
{
    public interface IRequestContextItemAccessor<TItem>
        where TItem : IDisposable
    {
        TItem GetCurrent();
    }
}
