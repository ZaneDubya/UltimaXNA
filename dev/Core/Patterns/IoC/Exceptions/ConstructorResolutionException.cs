using System;

namespace UltimaXNA.Core.Patterns.IoC
{
    public class ConstructorResolutionException : Exception
    {
        private const string ErrorText = "Unable to resolve constructor for {0} using provided Expression.";

        public ConstructorResolutionException(Type type)
            : base(String.Format(ErrorText, type.FullName))
        {
        }

        public ConstructorResolutionException(Type type, Exception innerException)
            : base(String.Format(ErrorText, type.FullName), innerException)
        {
        }

        public ConstructorResolutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ConstructorResolutionException(string message)
            : base(message)
        {
        }
    }
}