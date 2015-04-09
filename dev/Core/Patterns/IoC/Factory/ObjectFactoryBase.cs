using System;
using System.Reflection;

namespace UltimaXNA.Patterns.IoC
{
    internal abstract class ObjectFactoryBase
    {
        public virtual bool AssumeConstruction
        {
            get { return false; }
        }

        public ConstructorInfo Constructor
        {
            get;
            private set;
        }

        public abstract Type CreatesType
        {
            get;
        }

        public virtual ObjectFactoryBase MultiInstanceVariant
        {
            get { throw new RegistrationException(GetType(), "multi-instance"); }
        }

        public virtual ObjectFactoryBase SingletonVariant
        {
            get { throw new RegistrationException(GetType(), "singleton"); }
        }

        public virtual ObjectFactoryBase StrongReferenceVariant
        {
            get { throw new RegistrationException(GetType(), "strong reference"); }
        }

        public virtual ObjectFactoryBase WeakReferenceVariant
        {
            get { throw new RegistrationException(GetType(), "weak reference"); }
        }

        public virtual ObjectFactoryBase GetCustomObjectLifetimeVariant(
            IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            throw new RegistrationException(GetType(), errorString);
        }

        public virtual ObjectFactoryBase GetFactoryForChildContainer(Container parent)
        {
            return this;
        }

        public abstract object GetObject(Container container,
            NamedParameterOverloads parameters, ResolveOptions options);

        public virtual void SetConstructor(ConstructorInfo constructor)
        {
            Constructor = constructor;
        }
    }
}