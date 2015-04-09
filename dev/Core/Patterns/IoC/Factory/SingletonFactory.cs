using System;

namespace UltimaXNA.Patterns.IoC
{
    internal class SingletonFactory : ObjectFactoryBase, IDisposable
    {
        private readonly Type _registerImplementation;
        private readonly Type _registerType;
        private readonly object _singletonLock = new object();

        private object _currentObj;

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

            _registerType = registerType;
            _registerImplementation = registerImplementation;
        }

        public override Type CreatesType
        {
            get { return _registerImplementation; }
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get { return new MultiInstanceFactory(_registerType, _registerImplementation); }
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get { return this; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_currentObj == null)
            {
                return;
            }

            var disposable = _currentObj as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(
            IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
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
            if (parameters.Count != 0)
            {
                throw new ArgumentException("Cannot specify parameters for singleton types");
            }

            lock (_singletonLock)
                if (_currentObj == null)
                {
                    _currentObj = container.ConstructType(_registerImplementation, Constructor, options);
                }

            return _currentObj;
        }
    }
}