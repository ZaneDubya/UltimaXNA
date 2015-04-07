using System;

namespace UltimaXNA.Patterns.IoC
{
    internal class CustomObjectLifetimeFactory : ObjectFactoryBase, IDisposable
    {
        private readonly IObjectLifetimeProvider _lifetimeProvider;
        private readonly Type _registerImplementation;
        private readonly Type _registerType;
        private readonly object _singletonLock = new object();

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

            _registerType = registerType;
            _registerImplementation = registerImplementation;
            _lifetimeProvider = lifetimeProvider;
        }

        public override Type CreatesType
        {
            get { return _registerImplementation; }
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get
            {
                _lifetimeProvider.ReleaseObject();
                return new MultiInstanceFactory(_registerType, _registerImplementation);
            }
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get
            {
                _lifetimeProvider.ReleaseObject();
                return new SingletonFactory(_registerType, _registerImplementation);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _lifetimeProvider.ReleaseObject();
        }

        #endregion

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(
            IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            _lifetimeProvider.ReleaseObject();
            return new CustomObjectLifetimeFactory(_registerType, _registerImplementation, lifetimeProvider,
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

            lock (_singletonLock)
            {
                current = _lifetimeProvider.GetObject();
                if (current == null)
                {
                    current = container.ConstructType(_registerImplementation, Constructor, options);
                    _lifetimeProvider.SetObject(current);
                }
            }

            return current;
        }
    }
}