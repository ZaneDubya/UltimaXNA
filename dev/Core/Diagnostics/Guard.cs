using System;
using System.Text;

namespace UltimaXNA.Core.Diagnostics
{
    public static class Guard
    {
        public static void Requires(bool condition)
        {
            if (!condition)
            {
                throw new ArgumentException();
            }
        }

        public static void Requires(bool condition, string message, params object[] parameters)
        {
            if (!condition)
            {
                if (parameters.Length > 0)
                {
                    throw new ArgumentException(String.Format(message, parameters));
                }

                throw new ArgumentException(message);
            }
        }

        public static void Requires<TException>(bool condition, string message = null)
            where TException : Exception
        {
            if (!condition)
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw (TException)Activator.CreateInstance(typeof (TException));
                }

                throw (TException)Activator.CreateInstance(typeof (TException), message);
            }
        }

        public static void RequireIsNotNull(object obj, string message)
        {
            if (obj == null)
            {
                throw new ArgumentException(message);
            }
        }
    }
}