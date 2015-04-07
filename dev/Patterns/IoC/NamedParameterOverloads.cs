using System.Collections.Generic;

namespace UltimaXNA.Patterns.IoC
{
    public sealed class NamedParameterOverloads : Dictionary<string, object>
    {
        private static readonly NamedParameterOverloads _default = new NamedParameterOverloads();

        public static NamedParameterOverloads Default
        {
            get { return _default; }
        }

        public static NamedParameterOverloads FromIDictionary(IDictionary<string, object> data)
        {
            return data as NamedParameterOverloads ?? new NamedParameterOverloads(data);
        }

        public NamedParameterOverloads()
        {
        }

        public NamedParameterOverloads(IDictionary<string, object> data)
            : base(data)
        {
        }

        public T GetValue<T>(string key)
        {
            return (T)this[key];
        }

        public void SetValue<T>(string key, T value)
        {
            this[key] = value;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);

            object result;
            bool success = base.TryGetValue(key, out result);

            if (!success || !(result is T))
            {
                return false;
            }

            value = (T)result;

            return true;
        }
    }
}