using System;

namespace UltimaXNA.Core.Patterns.IoC
{
    public sealed class TypeRegistration
    {
        private readonly int m_hashCode;

        public TypeRegistration(Type type)
            : this(type, string.Empty)
        {
        }

        public TypeRegistration(Type type, string name)
        {
            Type = type;
            Name = name;

            m_hashCode = String.Concat(Type.FullName, "|", Name).GetHashCode();
        }

        public string Name
        {
            get;
            private set;
        }

        public Type Type
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            TypeRegistration typeRegistration = obj as TypeRegistration;

            if (typeRegistration == null)
            {
                return false;
            }

            if (Type != typeRegistration.Type)
            {
                return false;
            }

            if (String.Compare(Name, typeRegistration.Name, StringComparison.Ordinal) != 0)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }
    }
}