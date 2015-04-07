using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimaXNA.Collections
{
    public class SafeDictionary<TKey, TValue> : IDisposable
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private readonly object _syncRoot = new object();

        public TValue this[TKey key]
        {
            set
            {
                lock (_syncRoot)
                {
                    TValue current;

                    if (_dictionary.TryGetValue(key, out current))
                    {
                        var disposable = current as IDisposable;

                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }

                    _dictionary[key] = value;
                }
            }
        }

        public IEnumerable<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                _dictionary.Clear();
            }
        }

        public bool Remove(TKey key)
        {
            lock (_syncRoot)
            {
                return _dictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_syncRoot)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_syncRoot)
                {
                    IEnumerable<IDisposable> disposableItems = _dictionary.Values.Where(o => o is IDisposable).Cast<IDisposable>().ToArray();

                    foreach (IDisposable item in disposableItems)
                    {
                        item.Dispose();
                    }

                    _dictionary.Clear();
                }
            }
        }
    }
}