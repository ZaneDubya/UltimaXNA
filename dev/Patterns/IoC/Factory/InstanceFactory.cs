using System;
using System.Reflection;

namespace UltimaXNA.Patterns.IoC
{
    internal class InstanceFactory : ObjectFactoryBase, IDisposable
    {
        private readonly object _instance;
        private readonly Type _registerImplementation;
        private readonly Type _registerType;

        public InstanceFactory(Type registerType, Type registerImplementation, object instance)
        {
            if (!Container.IsValidAssignment(registerType, registerImplementation))
            {
                throw new RegistrationTypeException(registerImplementation, "InstanceFactory");
            }

            _registerType = registerType;
            _registerImplementation = registerImplementation;
            _instance = instance;
        }

        public override bool AssumeConstruction
        {
            get { return true; }
        }

        public override Type CreatesType
        {
            get { return _registerImplementation; }
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get { return new MultiInstanceFactory(_registerType, _registerImplementation); }
        }

        public override ObjectFactoryBase StrongReferenceVariant
        {
            get { return this; }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get { return new WeakInstanceFactory(_registerType, _registerImplementation, _instance); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            var disposable = _instance as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        public override object GetObject(Container container,
            NamedParameterOverloads parameters, ResolveOptions options)
        {
            return _instance;
        }

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new ConstructorResolutionException(
                "Constructor selection is not possible for instance factory registrations");
        }
    }
}