using System;
using System.Reflection;

namespace UltimaXNA.Core.Patterns.IoC
{
    internal class DelegateFactory : ObjectFactoryBase
    {
        private readonly Func<Container, NamedParameterOverloads, object> m_factory;
        private readonly Type m_registerType;

        public DelegateFactory(Type registerType, Func<Container, NamedParameterOverloads, object> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            m_factory = factory;
            m_registerType = registerType;
        }

        public override bool AssumeConstruction
        {
            get { return true; }
        }

        public override Type CreatesType
        {
            get { return m_registerType; }
        }

        public override ObjectFactoryBase StrongReferenceVariant
        {
            get { return this; }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get { return new WeakDelegateFactory(m_registerType, m_factory); }
        }

        public override object GetObject(Container container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            try
            {
                return m_factory.Invoke(container, parameters);
            }
            catch (Exception ex)
            {
                throw new ResolutionException(m_registerType, ex);
            }
        }

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new ConstructorResolutionException(
                "Constructor selection is not possible for delegate factory registrations");
        }
    }
}