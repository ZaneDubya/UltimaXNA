using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimaXNA.Core.Patterns.IoC
{
    public class AutoRegistrationException : Exception
    {
        private const string ErrorText = "Duplicate implementation of type {0} found ({1}).";

        private static string getTypesString(IEnumerable<Type> types)
        {
            IEnumerable<string> typeNames = from type in types
                select type.FullName;

            return string.Join(",", typeNames.ToArray());
        }

        public AutoRegistrationException(Type registerType, IEnumerable<Type> types)
            : base(String.Format(ErrorText, registerType, getTypesString(types)))
        {
        }

        public AutoRegistrationException(Type registerType, IEnumerable<Type> types, Exception innerException)
            : base(String.Format(ErrorText, registerType, getTypesString(types)), innerException)
        {
        }
    }
}