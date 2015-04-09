using System;

namespace UltimaXNA.Patterns.IoC
{
    internal class CustomObjectLifetimeFactory : ObjectFactoryBase, IDisposable
    {
        private readonly IObjectLifetimeProvider m_lifetimeProvider;
        private readonly Type m_registerImplementation;
        private readonly Type m_registerType;
        private readonly object m_singletonLock = new object();

        public CustomObjectLifetimeFactory(Type registerType, Type registerImplementation,
            IObjectLifetimeProvider lifetimeProvider, string errorMessage)
        {
            if (lifetimeProvider == null)
            {
                throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");
            }

            if (!Container.IsValidAssignment(registerType, registerImplementation))
            {
                throw new RegistrationTypeException(registerImplementation, "SingletonFactory");
            }

            if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
            {
                throw new RegistrationTypeException(registerImplementation, errorMessage);
            }

            m_registerType = registerType;
            m_registerImplementation = registerImplementation;
            m_lifetimeProvider = lifetimeProvider;
        }

        public override Type CreatesType
        {
            get { return m_registerImplementation; }
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get
            {
                m_lifetimeProvider.ReleaseObject();
                return new MultiInstanceFactory(m_registerType, m_registerImplementation);
            }
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get
            {
                m_lifetimeProvider.ReleaseObject();
                return new SingletonFactory(m_registerType, m_registerImplementation);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_lifetimeProvider.ReleaseObject();
        }

        #endregion

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(
            IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            m_lifetimeProvider.ReleaseObject();
            return new CustomObjectLifetimeFactory(m_registerType, m_registerImplementation, lifetimeProvider,
                errorString);
        }

        public override ObjectFactoryBase GetFactoryForChildContainer(Container parent)
        {
            // We make sure that the singleton is constructed before the child container takes the factory.
            // Otherwise the results would vary depending on whether or not the parent container had resolved
            // the type before the child container does.
            GetObject(parent, NamedParameterOverloads.Default, ResolveOptions.Default);
            return this;
        }

        public override object GetObject(Container container,
            NamedParameterOverloads parameters, ResolveOptions options)
        {
            object current;

            lock (m_singletonLock)
            {
                current = m_lifetimeProvider.GetObject();
                if (current == null)
                {
                    current = container.ConstructType(m_registerImplementation, Constructor, options);
                    m_lifetimeProvider.SetObject(current);
                }
            }

            return current;
        }
    }
}