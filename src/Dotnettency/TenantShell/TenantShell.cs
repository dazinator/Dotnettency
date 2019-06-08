using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dotnettency
{
    public class TenantShell<TTenant> : IDisposable
        where TTenant : class
    {

        private CompositeDisposable _disposables = new CompositeDisposable();

        public TenantShell(TTenant tenant, params TenantIdentifier[] distinguishers)
        {
            Id = Guid.NewGuid();
            Tenant = tenant;
            Properties = new ConcurrentDictionary<string, object>();
            Identifiers = new HashSet<TenantIdentifier>();

            if (distinguishers != null)
            {
                foreach (var item in distinguishers)
                {
                    Identifiers.Add(item);
                }
            }
        }

        protected ConcurrentDictionary<string, object> Properties { get; private set; }

        /// <summary>
        /// Gets or adds the item with the specified key from tenant properties. If <paramref name="item"/> implements <see cref="IDisposable"/> it will automatically be disposed of if the <see cref="TenantShell{TTenant}"/> is disposed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public T GetOrAddProperty<T>(string key, T item, bool disposeIfDisposable = true)
        {
            if (disposeIfDisposable)
            {
                EnsureDisposableRegistered(item);
            }

            var getOrAddItem = this.Properties.GetOrAdd(key, item);
            return (T)getOrAddItem;
        }

        public bool TryGetProperty<T>(string key, out T value)
        {
            var success = Properties.TryGetValue(key, out object item);
            value = (T)item;
            return success;
        }

        /// <summary>
        /// Gets or adds the item with the specified key from tenant properties. If <paramref name="item"/> implements <see cref="IDisposable"/> it will automatically be disposed of if the <see cref="TenantShell{TTenant}"/> is disposed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public T GetOrAddProperty<T>(string key, Func<string, T> factory, bool disposeIfDisposable = true)
        {
            var getOrAddItem = this.Properties.GetOrAdd(key, (a) =>
            {
                T newItem = factory.Invoke(a);
                if (disposeIfDisposable)
                {
                    EnsureDisposableRegistered(newItem);
                }
                return newItem;
            });
            return (T)getOrAddItem;
        }

        private void EnsureDisposableRegistered<T>(T item)
        {
            var disposable = item as IDisposable;
            if (disposable != null)
            {
                _disposables.Add(disposable);
            }
        }

        public void RegisterCallbackOnDispose(Action action)
        {
            this._disposables.Add(new DelegateDisposable(action));
        }


        /// <summary>
        /// Uniquely identifies this tenant.
        /// </summary>
        public Guid Id { get; set; }

        public TTenant Tenant { get; set; }

        /// <summary>
        /// Represents identifiers for this tenant. 
        /// A tenant can have multiple identifiers associated with it.
        /// An identifier is returned from <see cref="ITenantIdentifierFactory{TTenant}"/> usally based on information available from HttpContext.
        /// </summary>
        internal HashSet<TenantIdentifier> Identifiers { get; set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                    // TODO: dispose managed state (managed objects).
                    Properties.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
}
