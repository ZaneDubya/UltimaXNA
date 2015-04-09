using System;

namespace UltimaXNA.Core.Patterns.IoC
{
    internal class MultiInstanceFactory : ObjectFactoryBase
    {
        private readonly Type m_registerImplementation;
        private readonly Type m_registerType;

        public MultiInstanceFactory(Type registerType, Type registerImplementation)
        {
            if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
            {
                throw new RegistrationTypeException(registerImplementation, "MultiInstanceFactory");
            }

            if (!Container.IsValidAssignment(registerType, registerImplementation))
            {
                throw new RegistrationTypeException(registerImplementation, "MultiInstanceFactory");
            }

            m_registerType = registerType;
            m_registerImplementation = registerImplementation;
        }

        public override Type CreatesType
        {
            get { return m_registerImplementation; }
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get { return this; }
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get { return new SingletonFactory(m_registerType, m_registerImplementation); }
        }

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(
            IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            return new CustomObjectLifetimeFactory(m_registerType, m_registerImplementation, lifetimeProvider,
                errorString);
        }

        public override object GetObject(Container container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            try
            {
                return container.ConstructType(m_registerImplementation, Constructor, parameters,
                    options);
            }
            catch (ResolutionException ex)
            {
                throw new ResolutionException(m_registerType, ex);
            }
        }
    }
}