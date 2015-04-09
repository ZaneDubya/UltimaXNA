using System;
using System.Reflection;

namespace UltimaXNA.Patterns.IoC
{
    internal class WeakInstanceFactory : ObjectFactoryBase, IDisposable
    {
        private readonly WeakReference m_instance;
        private readonly Type m_registerImplementation;
        private readonly Type m_registerType;

        public WeakInstanceFactory(Type registerType, Type registerImplementation, object instance)
        {
            if (!Container.IsValidAssignment(registerType, registerImplementation))
            {
                throw new RegistrationTypeException(registerImplementation, "WeakInstanceFactory");
            }

            m_registerType = registerType;
            m_registerImplementation = registerImplementation;
            m_instance = new WeakReference(instance);
        }

        public override Type CreatesType
        {
            get { return m_registerImplementation; }
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get { return new MultiInstanceFactory(m_registerType, m_registerImplementation); }
        }

        public override ObjectFactoryBase StrongReferenceVariant
        {
            get
            {
                object instance = m_instance.Target;

                if (instance == null)
                {
                    throw new WeakReferenceException(m_registerType);
                }

                return new InstanceFactory(m_registerType, m_registerImplementation, instance);
            }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get { return this; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            var disposable = m_instance.Target as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        public override object GetObject(Container container,
            NamedParameterOverloads parameters, ResolveOptions options)
        {
            object instance = m_instance.Target;

            if (instance == null)
            {
                throw new WeakReferenceException(m_registerType);
            }

            return instance;
        }

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new ConstructorResolutionException(
                "Constructor selection is not possible for instance factory registrations");
        }
    }
}