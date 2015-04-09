using System;

namespace UltimaXNA.Core.Patterns.IoC
{
    public class ResolutionException : Exception
    {
        private const string ErrorText = "Unable to resolve type: {0}";

        public ResolutionException(Type type)
            : base(String.Format(ErrorText, type.FullName))
        {
        }

        public ResolutionException(Type type, Exception innerException)
            : base(String.Format(ErrorText, type.FullName), innerException)
        {
        }
    }
}