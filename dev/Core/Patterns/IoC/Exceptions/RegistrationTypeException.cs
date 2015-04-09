using System;

namespace UltimaXNA.Core.Patterns.IoC
{
    public class RegistrationTypeException : Exception
    {
        private const string RegisterErrorText = "Cannot register type {0} - abstract classes or interfaces are not valid implementation types for {1}.";

        public RegistrationTypeException(Type type, string factory)
            : base(String.Format(RegisterErrorText, type.FullName, factory))
        {
        }

        public RegistrationTypeException(Type type, string factory, Exception innerException)
            : base(String.Format(RegisterErrorText, type.FullName, factory), innerException)
        {
        }
    }
}