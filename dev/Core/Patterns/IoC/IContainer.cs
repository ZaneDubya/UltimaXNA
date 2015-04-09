using System;
using System.Collections.Generic;
using System.Reflection;

namespace UltimaXNA.Core.Patterns.IoC
{
    public interface IContainer
    {
        bool IsDisposed
        {
            get;
        }

        void RegisterModule<TModule>() where TModule : class, IModule;

        void AutoRegister(IEnumerable<Assembly> assemblies = null, bool ignoreDuplicateImplementations = true, Func<Type, bool> registrationPredicate = null);

        void BuildUp(object input, ResolveOptions resolveOptions = null);

        bool CanResolve(Type resolveType, string name = null, NamedParameterOverloads parameters = null, ResolveOptions options = null);

        bool CanResolve<TResolveType>(string name = null, NamedParameterOverloads parameters = null, ResolveOptions options = null)
            where TResolveType : class;

        Container CreateChildContainer();

        Container CreateSiblingContainer();

        void Dispose();

        RegisterOptions Register(Type registerType);

        RegisterOptions Register(Type registerType, string name);

        RegisterOptions Register(Type registerType, Type registerImplementation);

        RegisterOptions Register(Type registerType, Type registerImplementation, string name);

        RegisterOptions Register(Type registerType, object instance);

        RegisterOptions Register(Type registerType, object instance, string name);

        RegisterOptions Register(Type registerType, Type registerImplementation, object instance);

        RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name);

        RegisterOptions Register(Type registerType, Func<Container, NamedParameterOverloads, object> factory);

        RegisterOptions Register(Type registerType, Func<Container, NamedParameterOverloads, object> factory, string name);

        RegisterOptions Register<TRegisterType>()
            where TRegisterType : class;

        RegisterOptions Register<TRegisterType>(string name)
            where TRegisterType : class;

        RegisterOptions Register<TRegisterType, TRegisterImplementation>()
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType;

        RegisterOptions Register<TRegisterType, TRegisterImplementation>(string name)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType;

        RegisterOptions Register<TRegisterType>(TRegisterType instance)
            where TRegisterType : class;

        void Register<TRegisterType, TRegisterType2>(TRegisterType instance)
            where TRegisterType2 : class, TRegisterType;

        RegisterOptions Register<TRegisterType>(TRegisterType instance, string name)
            where TRegisterType : class;

        RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType;

        RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance, string name)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType;

        RegisterOptions Register<TRegisterType>(
            Func<Container, NamedParameterOverloads, TRegisterType> factory)
            where TRegisterType : class;

        RegisterOptions Register<TRegisterType>(Func<Container, NamedParameterOverloads, TRegisterType> factory, string name)
            where TRegisterType : class;

        MultiRegisterOptions RegisterMultiple<TRegisterType>(IEnumerable<Type> implementationTypes);

        MultiRegisterOptions RegisterMultiple(Type registrationType, IEnumerable<Type> implementationTypes);

        object Resolve(Type resolveType, string name = null, NamedParameterOverloads parameters = null, ResolveOptions options = null);

        TResolveType Resolve<TResolveType>(string name = null, NamedParameterOverloads parameters = null, ResolveOptions options = null)
            where TResolveType : class;

        IEnumerable<object> ResolveAll(Type resolveType, bool includeUnnamed);

        IEnumerable<object> ResolveAll(Type resolveType);

        IEnumerable<TResolveType> ResolveAll<TResolveType>(bool includeUnnamed)
            where TResolveType : class;

        IEnumerable<TResolveType> ResolveAll<TResolveType>()
            where TResolveType : class;

        IEnumerable<TResolveType> ResolveAll<TResolveType>(NamedParameterOverloads parameters)
            where TResolveType : class;

        IEnumerable<object> ResolveAll(Type resolveType, NamedParameterOverloads parameters, bool includeUnnamed);

        bool TryResolve(Type resolveType, out object resolvedType,
                        string name = null,
                        NamedParameterOverloads parameters = null,
                        ResolveOptions options = null);

        bool TryResolve<TResolveType>(out TResolveType resolvedType,
                                      string name = null,
                                      NamedParameterOverloads parameters = null,
                                      ResolveOptions options = null)
            where TResolveType : class;
    }
}