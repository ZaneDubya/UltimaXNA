using System;

namespace UltimaXNA.ComponentModel
{
    public static class EqualityHelper
    {
        public static bool IsEqual<T>(T oldValue, T newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return true;
            }

            if (oldValue == null || newValue == null)
            {
                return false;
            }

            Type type = typeof(T);

            if (type.IsValueType())
            {
                return oldValue.Equals(newValue);
            }

            return Equals(oldValue, newValue);
        }
    }
}