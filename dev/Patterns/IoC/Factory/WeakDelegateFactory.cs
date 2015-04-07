using System;
using System.Reflection;

namespace UltimaXNA.Patterns.IoC
{
    internal class WeakDelegateFactory : ObjectFactoryBase
    {
        private readonly WeakReference _factory;
        private readonly Type _registerType;

        public WeakDelegateFactory(Type registerType, Func<Container, NamedParameterOverloads, object> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            _factory = new WeakReference(factory);
            _registerType = registerType;
        }

        public override bool AssumeConstruction
        {
            get { return true; }
        }

        public override Type CreatesType
        {
            get { return _registerType; }
        }

        public override ObjectFactoryBase StrongReferenceVariant
        {
            get
            {
                var factory =
                    _factory.Target as Func<Container, NamedParameterOverloads, object>;

                if (factory == null)
                {
                    throw new WeakReferenceException(_registerType);
                }

                return new DelegateFactory(_registerType, factory);
            }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get { return this; }
        }

        public override object GetObject(Container container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            var factory = _factory.Target as Func<Container, NamedParameterOverloads, object>;

            if (factory == null)
            {
                throw new WeakReferenceException(_registerType);
            }

            try
            {
                return factory.Invoke(container, parameters);
            }
            catch (Exception ex)
            {
                throw new ResolutionException(_registerType, ex);
            }
        }

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new ConstructorResolutionException(
                "Constructor selection is not possible for delegate factory registrations");
        }
    }
}