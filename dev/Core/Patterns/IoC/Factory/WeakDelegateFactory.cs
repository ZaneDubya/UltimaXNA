using System;
using System.Reflection;

namespace UltimaXNA.Patterns.IoC
{
    internal class WeakDelegateFactory : ObjectFactoryBase
    {
        private readonly WeakReference m_factory;
        private readonly Type m_registerType;

        public WeakDelegateFactory(Type registerType, Func<Container, NamedParameterOverloads, object> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            m_factory = new WeakReference(factory);
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
            get
            {
                var factory =
                    m_factory.Target as Func<Container, NamedParameterOverloads, object>;

                if (factory == null)
                {
                    throw new WeakReferenceException(m_registerType);
                }

                return new DelegateFactory(m_registerType, factory);
            }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get { return this; }
        }

        public override object GetObject(Container container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            var factory = m_factory.Target as Func<Container, NamedParameterOverloads, object>;

            if (factory == null)
            {
                throw new WeakReferenceException(m_registerType);
            }

            try
            {
                return factory.Invoke(container, parameters);
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