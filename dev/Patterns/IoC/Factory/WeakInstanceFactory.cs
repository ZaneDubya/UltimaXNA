using System;
using System.Reflection;

namespace UltimaXNA.Patterns.IoC
{
    internal class WeakInstanceFactory : ObjectFactoryBase, IDisposable
    {
        private readonly WeakReference _instance;
        private readonly Type _registerImplementation;
        private readonly Type _registerType;

        public WeakInstanceFactory(Type registerType, Type registerImplementation, object instance)
        {
            if (!Container.IsValidAssignment(registerType, registerImplementation))
            {
                throw new RegistrationTypeException(registerImplementation, "WeakInstanceFactory");
            }

            _registerType = registerType;
            _registerImplementation = registerImplementation;
            _instance = new WeakReference(instance);
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
            get
            {
                object instance = _instance.Target;

                if (instance == null)
                {
                    throw new WeakReferenceException(_registerType);
                }

                return new InstanceFactory(_registerType, _registerImplementation, instance);
            }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get { return this; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            var disposable = _instance.Target as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        public override object GetObject(Container container,
            NamedParameterOverloads parameters, ResolveOptions options)
        {
            object instance = _instance.Target;

            if (instance == null)
            {
                throw new WeakReferenceException(_registerType);
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