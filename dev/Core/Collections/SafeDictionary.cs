using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimaXNA.Core.Collections
{
    public class SafeDictionary<TKey, TValue> : IDisposable
    {
        private readonly Dictionary<TKey, TValue> m_dictionary = new Dictionary<TKey, TValue>();
        private readonly object m_syncRoot = new object();

        public TValue this[TKey key]
        {
            set
            {
                lock (m_syncRoot)
                {
                    TValue current;

                    if (m_dictionary.TryGetValue(key, out current))
                    {
                        IDisposable disposable = current as IDisposable;

                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }

                    m_dictionary[key] = value;
                }
            }
        }

        public IEnumerable<TKey> Keys
        {
            get { return m_dictionary.Keys; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Clear()
        {
            lock (m_syncRoot)
            {
                m_dictionary.Clear();
            }
        }

        public bool Remove(TKey key)
        {
            lock (m_syncRoot)
            {
                return m_dictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (m_syncRoot)
            {
                return m_dictionary.TryGetValue(key, out value);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (m_syncRoot)
                {
                    IEnumerable<IDisposable> disposableItems = m_dictionary.Values.Where(o => o is IDisposable).Cast<IDisposable>().ToArray();

                    foreach (IDisposable item in disposableItems)
                    {
                        item.Dispose();
                    }

                    m_dictionary.Clear();
                }
            }
        }
    }
}