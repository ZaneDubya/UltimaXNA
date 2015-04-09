using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UltimaXNA.Core.Patterns.IoC
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

            ObjectFactoryBase currentFactory = instance.m_container.GetCurrentFactory(instance.m_registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(instance.m_registration.Type, errorString);
            }

            return instance.m_container.AddUpdateRegistration(instance.m_registration, currentFactory.GetCustomObjectLifetimeVariant(lifetimeProvider, errorString));
        }

        private readonly Container m_container;
        private readonly TypeRegistration m_registration;

        public RegisterOptions(Container container, TypeRegistration registration)
        {
            m_container = container;
            m_registration = registration;
        }

        public RegisterOptions AsMultiInstance()
        {
            ObjectFactoryBase currentFactory = m_container.GetCurrentFactory(m_registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(m_registration.Type, "multi-instance");
            }

            return m_container.AddUpdateRegistration(m_registration, currentFactory.MultiInstanceVariant);
        }

        public RegisterOptions AsSingleton()
        {
            ObjectFactoryBase currentFactory = m_container.GetCurrentFactory(m_registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(m_registration.Type, "singleton");
            }

            return m_container.AddUpdateRegistration(m_registration, currentFactory.SingletonVariant);
        }

        public RegisterOptions UsingConstructor<RegisterType>(Expression<Func<RegisterType>> constructor)
        {
            LambdaExpression lambda = constructor as LambdaExpression;
            if (lambda == null)
            {
                throw new ConstructorResolutionException(typeof (RegisterType));
            }

            NewExpression newExpression = lambda.Body as NewExpression;
            if (newExpression == null)
            {
                throw new ConstructorResolutionException(typeof (RegisterType));
            }

            ConstructorInfo constructorInfo = newExpression.Constructor;
            if (constructorInfo == null)
            {
                throw new ConstructorResolutionException(typeof (RegisterType));
            }

            ObjectFactoryBase currentFactory = m_container.GetCurrentFactory(m_registration);
            if (currentFactory == null)
            {
                throw new ConstructorResolutionException(typeof (RegisterType));
            }

            currentFactory.SetConstructor(constructorInfo);

            return this;
        }

        public RegisterOptions WithStrongReference()
        {
            ObjectFactoryBase currentFactory = m_container.GetCurrentFactory(m_registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(m_registration.Type, "strong reference");
            }

            return m_container.AddUpdateRegistration(m_registration, currentFactory.StrongReferenceVariant);
        }

        public RegisterOptions WithWeakReference()
        {
            ObjectFactoryBase currentFactory = m_container.GetCurrentFactory(m_registration);

            if (currentFactory == null)
            {
                throw new RegistrationException(m_registration.Type, "weak reference");
            }

            return m_container.AddUpdateRegistration(m_registration, currentFactory.WeakReferenceVariant);
        }
    }
}