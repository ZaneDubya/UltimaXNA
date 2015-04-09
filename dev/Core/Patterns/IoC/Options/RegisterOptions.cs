using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UltimaXNA.Patterns.IoC
{
    public sealed class RegisterOptions
    {
        public static RegisterOptions ToCustomLifetimeManager(RegisterOptions instance, IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance", "instance is null.");
            }

            if (lifetimeProvider == null)
            {
                throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");
            }

            if (String.IsNullOrEmpty(errorString))
            {
                throw new ArgumentException("errorString is null or empty.", "errorString");
            }

            ObjectFactoryBase currentFactory = instance._container.GetCurrentFactory(instance._registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(instance._registration.Type, errorString);
            }

            return instance._container.AddUpdateRegistration(instance._registration, currentFactory.GetCustomObjectLifetimeVariant(lifetimeProvider, errorString));
        }

        private readonly Container _container;
        private readonly TypeRegistration _registration;

        public RegisterOptions(Container container, TypeRegistration registration)
        {
            _container = container;
            _registration = registration;
        }

        public RegisterOptions AsMultiInstance()
        {
            ObjectFactoryBase currentFactory = _container.GetCurrentFactory(_registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(_registration.Type, "multi-instance");
            }

            return _container.AddUpdateRegistration(_registration, currentFactory.MultiInstanceVariant);
        }

        public RegisterOptions AsSingleton()
        {
            ObjectFactoryBase currentFactory = _container.GetCurrentFactory(_registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(_registration.Type, "singleton");
            }

            return _container.AddUpdateRegistration(_registration, currentFactory.SingletonVariant);
        }

        public RegisterOptions UsingConstructor<RegisterType>(Expression<Func<RegisterType>> constructor)
        {
            var lambda = constructor as LambdaExpression;
            if (lambda == null)
            {
                throw new ConstructorResolutionException(typeof (RegisterType));
            }

            var newExpression = lambda.Body as NewExpression;
            if (newExpression == null)
            {
                throw new ConstructorResolutionException(typeof (RegisterType));
            }

            ConstructorInfo constructorInfo = newExpression.Constructor;
            if (constructorInfo == null)
            {
                throw new ConstructorResolutionException(typeof (RegisterType));
            }

            ObjectFactoryBase currentFactory = _container.GetCurrentFactory(_registration);
            if (currentFactory == null)
            {
                throw new ConstructorResolutionException(typeof (RegisterType));
            }

            currentFactory.SetConstructor(constructorInfo);

            return this;
        }

        public RegisterOptions WithStrongReference()
        {
            ObjectFactoryBase currentFactory = _container.GetCurrentFactory(_registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(_registration.Type, "strong reference");
            }

            return _container.AddUpdateRegistration(_registration, currentFactory.StrongReferenceVariant);
        }

        public RegisterOptions WithWeakReference()
        {
            ObjectFactoryBase currentFactory = _container.GetCurrentFactory(_registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(_registration.Type, "weak reference");
            }

            return _container.AddUpdateRegistration(_registration, currentFactory.WeakReferenceVariant);
        }
    }
}