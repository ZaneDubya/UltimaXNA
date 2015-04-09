using System;

namespace UltimaXNA.Patterns.IoC
{
    public class WeakReferenceException : Exception
    {
        private const string ErrorText = "Unable to instantiate {0} - referenced object has been reclaimed";

        public WeakReferenceException(Type type)
            : base(String.Format(ErrorText, type.FullName))
        {
        }

        public WeakReferenceException(Type type, Exception innerException)
            : base(String.Format(ErrorText, type.FullName), innerException)
        {
        }
    }
}