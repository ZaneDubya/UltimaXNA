using System;
using System.Reflection;

namespace UltimaXNA.Core.Patterns.IoC
{
    internal class InstanceFactory : ObjectFactoryBase, IDisposable
    {
        private readonly object m_instance;
        private readonly Type m_registerImplementation;
        private readonly Type m_registerType;

        public InstanceFactory(Type registerType, Type registerImplementation, object instance)
        {
            if (!Container.IsValidAssignment(registerType, registerImplementation))
            {
                throw new RegistrationTypeException(registerImplementation, "InstanceFactory");
            }

            m_registerType = registerType;
            m_registerImplementation = registerImplementation;
            m_instance = instance;
        }

        public override bool AssumeConstruction
        {
            get { return true; }
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
            get { return this; }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get { return new WeakInstanceFactory(m_registerType, m_registerImplementation, m_instance); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            IDisposable disposable = m_instance as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        public override object GetObject(Container container,
            NamedParameterOverloads parameters, ResolveOptions options)
        {
            return m_instance;
        }

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new ConstructorResolutionException(
                "Constructor selection is not possible for instance factory registrations");
        }
    }
}