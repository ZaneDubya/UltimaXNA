using System;
using System.Reflection;

namespace UltimaXNA.Patterns.IoC
{
    internal class DelegateFactory : ObjectFactoryBase
    {
        private readonly Func<Container, NamedParameterOverloads, object> _factory;
        private readonly Type _registerType;

        public DelegateFactory(Type registerType, Func<Container, NamedParameterOverloads, object> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            _factory = factory;
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
            get { return this; }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get { return new WeakDelegateFactory(_registerType, _factory); }
        }

        public override object GetObject(Container container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            try
            {
                return _factory.Invoke(container, parameters);
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