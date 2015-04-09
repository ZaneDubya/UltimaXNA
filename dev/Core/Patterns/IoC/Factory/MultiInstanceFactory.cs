using System;

namespace UltimaXNA.Patterns.IoC
{
    internal class MultiInstanceFactory : ObjectFactoryBase
    {
        private readonly Type _registerImplementation;
        private readonly Type _registerType;

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

            _registerType = registerType;
            _registerImplementation = registerImplementation;
        }

        public override Type CreatesType
        {
            get { return _registerImplementation; }
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get { return this; }
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get { return new SingletonFactory(_registerType, _registerImplementation); }
        }

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(
            IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            return new CustomObjectLifetimeFactory(_registerType, _registerImplementation, lifetimeProvider,
                errorString);
        }

        public override object GetObject(Container container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            try
            {
                return container.ConstructType(_registerImplementation, Constructor, parameters,
                    options);
            }
            catch (ResolutionException ex)
            {
                throw new ResolutionException(_registerType, ex);
            }
        }
    }
}