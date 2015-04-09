using System;

namespace UltimaXNA.Patterns.IoC
{
    internal class SingletonFactory : ObjectFactoryBase, IDisposable
    {
        private readonly Type m_registerImplementation;
        private readonly Type m_registerType;
        private readonly object m_singletonLock = new object();

        private object m_currentObj;

        public SingletonFactory(Type registerType, Type registerImplementation)
        {
            if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
            {
                throw new RegistrationTypeException(registerImplementation, "SingletonFactory");
            }

            if (!Container.IsValidAssignment(registerType, registerImplementation))
            {
                throw new RegistrationTypeException(registerImplementation, "SingletonFactory");
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
            get { return new MultiInstanceFactory(m_registerType, m_registerImplementation); }
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get { return this; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_currentObj == null)
            {
                return;
            }

            IDisposable disposable = m_currentObj as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(
            IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
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
            if (parameters.Count != 0)
            {
                throw new ArgumentException("Cannot specify parameters for singleton types");
            }

            lock (m_singletonLock)
                if (m_currentObj == null)
                {
                    m_currentObj = container.ConstructType(m_registerImplementation, Constructor, options);
                }

            return m_currentObj;
        }
    }
}