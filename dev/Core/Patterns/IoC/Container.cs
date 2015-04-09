using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UltimaXNA.Collections;
using UltimaXNA.Diagnostics;
using UltimaXNA.Diagnostics.Tracing;

namespace UltimaXNA.Patterns.IoC
{
    public class Container : IDisposable, IContainer
    {
        internal static bool IsValidAssignment(Type registerType, Type registerImplementation)
        {
            if (!registerType.IsGenericTypeDefinition())
            {
                if (!registerType.IsAssignableFrom(registerImplementation))
                {
                    return false;
                }
            }
            else
            {
                if (registerType.IsInterface())
                {
                    if (!registerImplementation.FindInterfaces((t, o) => t.Name == registerType.Name, null).Any())
                    {
                        return false;
                    }
                }
                else if (registerType.IsAbstract() && registerImplementation.BaseType() != registerType)
                {
                    return false;
                }
            }

            return true;
        }

        private readonly Dictionary<Type, IModule> m_modules = new Dictionary<Type, IModule>();
        private readonly object m_autoRegisterLock = new object();
        private readonly Container m_parent;
        private readonly SafeDictionary<TypeRegistration, ObjectFactoryBase> m_registeredTypes;
        private bool m_disposed;

        public Container()
        {
            m_registeredTypes = new SafeDictionary<TypeRegistration, ObjectFactoryBase>();
            registerDefaultTypes();
        }

        private Container(Container parent)
        {
            m_parent = parent;
            m_registeredTypes = new SafeDictionary<TypeRegistration, ObjectFactoryBase>();
            registerDefaultTypes();
        }

        public bool IsDisposed
        {
            get { return m_disposed; }
        }

        public void RegisterModule<TModule>()
            where TModule : class, IModule
        {
            if (m_modules.ContainsKey(typeof (TModule)))
            {
                throw new Exception("Unable to load module " + typeof (TModule).Name + " because it is already registered.");
            }

            Register<TModule>();
            TModule module = Resolve<TModule>();
            module.Load(this);
            m_modules.Add(typeof (TModule), module);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies = null, bool ignoreDuplicateImplementations = true, Func<Type, bool> registrationPredicate = null)
        {
            if (assemblies == null)
            {
                assemblies = new[] {GetType().Assembly()};
            }

            autoRegisterInternal(assemblies, ignoreDuplicateImplementations, registrationPredicate);
        }

        public void BuildUp(object input, ResolveOptions resolveOptions = null)
        {
            if (resolveOptions == null)
            {
                resolveOptions = ResolveOptions.Default;
            }

            buildUpInternal(input, resolveOptions);
        }

        public bool CanResolve(Type resolveType, string name = null, NamedParameterOverloads parameters = null, ResolveOptions options = null)
        {
            Guard.Requires<ArgumentNullException>(resolveType != null, "resolveType");

            if (parameters == null)
            {
                parameters = NamedParameterOverloads.Default;
            }

            if (options == null)
            {
                options = ResolveOptions.Default;
            }

            TypeRegistration registration =
                string.IsNullOrEmpty(name)
                    ? new TypeRegistration(resolveType)
                    : new TypeRegistration(resolveType, name);

            return canResolveInternal(registration, parameters, options);
        }

        public bool CanResolve<TResolveType>(string name = null, NamedParameterOverloads parameters = null, ResolveOptions options = null)
            where TResolveType : class
        {
            if (parameters == null)
            {
                parameters = NamedParameterOverloads.Default;
            }

            if (options == null)
            {
                options = ResolveOptions.Default;
            }

            return CanResolve(typeof (TResolveType), name, parameters, options);
        }

        public Container CreateChildContainer()
        {
            return new Container(this);
        }

        public Container CreateSiblingContainer()
        {
            return new Container(m_parent);
        }

        public RegisterOptions Register(Type registerType)
        {
            return registerInternal(registerType, string.Empty, getDefaultObjectFactory(registerType, registerType));
        }

        public RegisterOptions Register(Type registerType, string name)
        {
            return registerInternal(registerType, name, getDefaultObjectFactory(registerType, registerType));
        }

        public RegisterOptions Register(Type registerType, Type registerImplementation)
        {
            return registerInternal(registerType, string.Empty, getDefaultObjectFactory(registerType, registerImplementation));
        }

        public RegisterOptions Register(Type registerType, Type registerImplementation, string name)
        {
            return registerInternal(registerType, name, getDefaultObjectFactory(registerType, registerImplementation));
        }

        public RegisterOptions Register(Type registerType, object instance)
        {
            return registerInternal(registerType, string.Empty, new InstanceFactory(registerType, registerType, instance));
        }

        public RegisterOptions Register(Type registerType, object instance, string name)
        {
            return registerInternal(registerType, name, new InstanceFactory(registerType, registerType, instance));
        }

        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance)
        {
            return registerInternal(registerType, string.Empty, new InstanceFactory(registerType, registerImplementation, instance));
        }

        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name)
        {
            return registerInternal(registerType, name,
                new InstanceFactory(registerType, registerImplementation, instance));
        }

        public RegisterOptions Register(Type registerType, Func<Container, NamedParameterOverloads, object> factory)
        {
            return registerInternal(registerType, string.Empty, new DelegateFactory(registerType, factory));
        }

        public RegisterOptions Register(Type registerType, Func<Container, NamedParameterOverloads, object> factory, string name)
        {
            return registerInternal(registerType, name, new DelegateFactory(registerType, factory));
        }

        public RegisterOptions Register<TRegisterType>()
            where TRegisterType : class
        {
            return Register(typeof (TRegisterType));
        }

        public RegisterOptions Register<TRegisterType>(string name)
            where TRegisterType : class
        {
            return Register(typeof (TRegisterType), name);
        }

        public RegisterOptions Register<TRegisterType, TRegisterImplementation>()
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType
        {
            return Register(typeof (TRegisterType), typeof (TRegisterImplementation));
        }

        public RegisterOptions Register<TRegisterType, TRegisterImplementation>(string name)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType
        {
            return Register(typeof (TRegisterType), typeof (TRegisterImplementation), name);
        }

        public RegisterOptions Register<TRegisterType>(TRegisterType instance)
            where TRegisterType : class
        {
            return Register(typeof (TRegisterType), instance);
        }

        public void Register<TRegisterType, TRegisterType2>(TRegisterType instance)
            where TRegisterType2 : class, TRegisterType
        {
            Register(typeof (TRegisterType), instance);
            Register(typeof (TRegisterType2), instance);
        }

        public RegisterOptions Register<TRegisterType>(TRegisterType instance, string name)
            where TRegisterType : class
        {
            return Register(typeof (TRegisterType), instance, name);
        }

        public RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType
        {
            return Register(typeof (TRegisterType), typeof (TRegisterImplementation), instance);
        }

        public RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance, string name)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType
        {
            return Register(typeof (TRegisterType), typeof (TRegisterImplementation), instance, name);
        }

        public RegisterOptions Register<TRegisterType>(
            Func<Container, NamedParameterOverloads, TRegisterType> factory)
            where TRegisterType : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            return Register(typeof (TRegisterType), factory);
        }

        public RegisterOptions Register<TRegisterType>(Func<Container, NamedParameterOverloads, TRegisterType> factory, string name)
            where TRegisterType : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            return Register(typeof (TRegisterType), factory, name);
        }

        public MultiRegisterOptions RegisterMultiple<TRegisterType>(IEnumerable<Type> implementationTypes)
        {
            return RegisterMultiple(typeof (TRegisterType), implementationTypes);
        }

        public MultiRegisterOptions RegisterMultiple(Type registrationType, IEnumerable<Type> implementationTypes)
        {
            if (implementationTypes == null)
            {
                throw new ArgumentNullException("implementationTypes", "types is null.");
            }

            Type[] enumerable = implementationTypes as Type[] ?? implementationTypes.ToArray();

            foreach (Type type in enumerable)
            {
                if (!registrationType.IsAssignableFrom(type))
                {
                    throw new ArgumentException(String.Format("types: The type {0} is not assignable from {1}",
                        registrationType.FullName, type.FullName));
                }
            }

            if (enumerable.Count() != enumerable.Distinct().Count())
            {
                throw new ArgumentException("types: The same implementation type cannot be specificed multiple times");
            }

            List<RegisterOptions> registerOptions = enumerable.Select(type => Register(registrationType, type, type.FullName)).ToList();

            return new MultiRegisterOptions(registerOptions);
        }

        public object Resolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options)
        {
            Guard.Requires<ArgumentNullException>(resolveType != null, "resolveType");

            if (parameters == null)
            {
                parameters = NamedParameterOverloads.Default;
            }

            if (options == null)
            {
                options = ResolveOptions.Default;
            }

            TypeRegistration registration =
                string.IsNullOrEmpty(name)
                    ? new TypeRegistration(resolveType)
                    : new TypeRegistration(resolveType, name);

            return resolveInternal(registration, parameters, options);
        }

        public TResolveType Resolve<TResolveType>(string name = null, NamedParameterOverloads parameters = null, ResolveOptions options = null)
            where TResolveType : class
        {
            if (parameters == null)
            {
                parameters = NamedParameterOverloads.Default;
            }

            if (options == null)
            {
                options = ResolveOptions.Default;
            }

            TypeRegistration registration =
                string.IsNullOrEmpty(name)
                    ? new TypeRegistration(typeof (TResolveType))
                    : new TypeRegistration(typeof (TResolveType), name);

            return (TResolveType)resolveInternal(registration, parameters, options);
        }

        public IEnumerable<object> ResolveAll(Type resolveType, NamedParameterOverloads parameters, bool includeUnnamed)
        {
            return resolveAllInternal(resolveType, parameters, includeUnnamed);
        }

        public IEnumerable<object> ResolveAll(Type resolveType, bool includeUnnamed)
        {
            return resolveAllInternal(resolveType, NamedParameterOverloads.Default, includeUnnamed);
        }

        public IEnumerable<object> ResolveAll(Type resolveType)
        {
            return ResolveAll(resolveType, false);
        }

        public IEnumerable<TResolveType> ResolveAll<TResolveType>(NamedParameterOverloads parameters)
            where TResolveType : class
        {
            return ResolveAll(typeof (TResolveType), parameters, true).Cast<TResolveType>();
        }

        public IEnumerable<TResolveType> ResolveAll<TResolveType>(bool includeUnnamed)
            where TResolveType : class
        {
            return ResolveAll(typeof (TResolveType), includeUnnamed).Cast<TResolveType>();
        }

        public IEnumerable<TResolveType> ResolveAll<TResolveType>()
            where TResolveType : class
        {
            return ResolveAll<TResolveType>(true);
        }

        public bool TryResolve(Type resolveType, out object resolvedType,
            string name = null,
            NamedParameterOverloads parameters = null,
            ResolveOptions options = null)
        {
            Guard.Requires<ArgumentNullException>(resolveType != null, "resolveType");

            if (parameters == null)
            {
                parameters = NamedParameterOverloads.Default;
            }

            if (options == null)
            {
                options = ResolveOptions.Default;
            }

            TypeRegistration registration =
                string.IsNullOrEmpty(name)
                    ? new TypeRegistration(resolveType)
                    : new TypeRegistration(resolveType, name);

            return tryResolveInternal(registration, parameters, options, out resolvedType);
        }

        private readonly Guid m_containerId = Guid.NewGuid();

        public bool TryResolve<TResolveType>(out TResolveType resolvedType,
            string name = null,
            NamedParameterOverloads parameters = null,
            ResolveOptions options = null)
            where TResolveType : class
        {
            if (parameters == null)
            {
                parameters = NamedParameterOverloads.Default;
            }

            if (options == null)
            {
                options = ResolveOptions.Default;
            }

            object result;
            bool success = TryResolve(typeof (TResolveType), out result, name, parameters, options);
            resolvedType = (TResolveType)result;
            return success;
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                //Tracer.Debug("Disposing container {0}", m_containerId);

                m_disposed = true;
                m_registeredTypes.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        internal RegisterOptions AddUpdateRegistration(TypeRegistration typeRegistration, ObjectFactoryBase factory)
        {
            m_registeredTypes[typeRegistration] = factory;

            return new RegisterOptions(this, typeRegistration);
        }

        internal object ConstructType(Type implementationType, ConstructorInfo constructor, ResolveOptions options)
        {
            return ConstructType(implementationType, constructor, NamedParameterOverloads.Default,
                options);
        }

        internal object ConstructType(Type implementationType, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return ConstructType(implementationType, null, parameters, options);
        }

        internal object ConstructType(Type implementationType, ConstructorInfo constructor, NamedParameterOverloads parameters, ResolveOptions options)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            Type typeToConstruct = implementationType;

            if (constructor == null)
            {
                // Try and get the best constructor that we can construct
                // if we can't construct any then get the constructor
                // with the least number of parameters so we can throw a meaningful
                // resolve exception
                constructor = getBestConstructor(typeToConstruct, parameters, options) ??
                              getTypeConstructors(typeToConstruct).LastOrDefault();
            }

            if (constructor == null)
            {
                throw new ResolutionException(typeToConstruct);
            }

            ParameterInfo[] ctorParams = constructor.GetParameters();
            var args = new object[ctorParams.Count()];

            for (int parameterIndex = 0; parameterIndex < ctorParams.Count(); parameterIndex++)
            {
                ParameterInfo currentParam = ctorParams[parameterIndex];

                try
                {
                    args[parameterIndex] = parameters.ContainsKey(currentParam.Name)
                        ? parameters[currentParam.Name]
                        : resolveInternal(
                            new TypeRegistration(currentParam.ParameterType),
                            NamedParameterOverloads.Default,
                            options);
                }
                catch (ResolutionException ex)
                {
                    // If a constructor parameter can't be resolved
                    // it will throw, so wrap it and throw that this can't
                    // be resolved.
                    throw new ResolutionException(typeToConstruct, ex);
                }
                catch (Exception ex)
                {
                    throw new ResolutionException(typeToConstruct, ex);
                }
            }

            try
            {
                return constructor.Invoke(args);
            }
            catch (Exception ex)
            {
                throw new ResolutionException(typeToConstruct, ex);
            }
        }

        internal ObjectFactoryBase GetCurrentFactory(TypeRegistration registration)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            ObjectFactoryBase current;

            m_registeredTypes.TryGetValue(registration, out current);

            return current;
        }

        internal RegisterOptions RegisterInternal(Type registerType, string name, ObjectFactoryBase factory)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            var typeRegistration = new TypeRegistration(registerType, name);

            return AddUpdateRegistration(typeRegistration, factory);
        }

        internal void RemoveRegistration(TypeRegistration typeRegistration)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            m_registeredTypes.Remove(typeRegistration);
        }

        private static ObjectFactoryBase getDefaultObjectFactory(Type registerType, Type registerImplementation)
        {
            if (registerType.IsInterface() || registerType.IsAbstract())
            {
                return new SingletonFactory(registerType, registerImplementation);
            }

            return new MultiInstanceFactory(registerType, registerImplementation);
        }

        private static IEnumerable<ConstructorInfo> getTypeConstructors(Type type)
        {
            return type.GetConstructors().OrderByDescending(ctor => ctor.GetParameters().Count());
        }

        private static bool isAutomaticLazyFactoryRequest(Type type)
        {
            if (!type.IsGenericType())
            {
                return false;
            }

            Type genericType = type.GetGenericTypeDefinition();

            // Just a func
            if (genericType == typeof (Func<>))
            {
                return true;
            }

            if ((genericType == typeof (Func<,>) && type.GetGenericArguments()[0] == typeof (string)))
            {
                return true;
            }

            if ((genericType == typeof (Func<,,>) && type.GetGenericArguments()[0] == typeof (string) &&
                 type.GetGenericArguments()[1] == typeof (IDictionary<String, object>)))
            {
                return true;
            }

            return false;
        }

        private static bool isIEnumerableRequest(Type type)
        {
            if (!type.IsGenericType())
            {
                return false;
            }

            Type genericType = type.GetGenericTypeDefinition();

            if (genericType == typeof (IEnumerable<>))
            {
                return true;
            }

            return false;
        }

        private static bool isIgnoredType(Type type, Func<Type, bool> registrationPredicate)
        {
            var ignoreChecks = new List<Func<Type, bool>>
                               {
                                   t => t.FullName.StartsWith("System.", StringComparison.Ordinal),
                                   t => t.FullName.StartsWith("Microsoft.", StringComparison.Ordinal),
                                   t => t.IsPrimitive(),
                                   t =>
                                       (t.GetConstructors().Length == 0) &&
                                       !(t.IsInterface() || t.IsAbstract()),
                               };

            if (registrationPredicate != null)
            {
                ignoreChecks.Add(t => !registrationPredicate(t));
            }

            return ignoreChecks.Any(check => check(type));
        }

        private RegisterOptions addUpdateRegistration(TypeRegistration typeRegistration, ObjectFactoryBase factory)
        {
            m_registeredTypes[typeRegistration] = factory;

            return new RegisterOptions(this, typeRegistration);
        }

        private void autoRegisterInternal(IEnumerable<Assembly> assemblies, bool ignoreDuplicateImplementations, Func<Type, bool> registrationPredicate)
        {
            lock (m_autoRegisterLock)
            {
                List<Type> types =
                    assemblies.SelectMany(a => a.SafeGetTypes()).Where(t => !isIgnoredType(t, registrationPredicate)).
                        ToList();

                IEnumerable<Type> concreteTypes = from type in types
                    where
                        type.IsClass() && (type.IsAbstract() == false) &&
                        (type != GetType() && (type.DeclaringType != GetType()) &&
                         (!type.IsGenericTypeDefinition()))
                    select type;

                Type[] enumerable = concreteTypes as Type[] ?? concreteTypes.ToArray();

                foreach (Type type in enumerable)
                {
                    registerInternal(type, string.Empty, getDefaultObjectFactory(type, type));
                }

                IEnumerable<Type> abstractInterfaceTypes = from type in types
                    where
                        ((type.IsInterface() || type.IsAbstract()) &&
                         (type.DeclaringType != GetType()) &&
                         (!type.IsGenericTypeDefinition()))
                    select type;

                foreach (Type type in abstractInterfaceTypes)
                {
                    Type check = type;

                    IEnumerable<Type> implementations = (from implementationType in enumerable
                        where
                            implementationType.GetInterfaces().Contains(check) ||
                            implementationType.BaseType() == check
                        select implementationType).ToArray();

                    if (!ignoreDuplicateImplementations && implementations.Count() > 1)
                    {
                        throw new AutoRegistrationException(type, implementations);
                    }

                    Type firstImplementation = implementations.FirstOrDefault();

                    if (firstImplementation == null)
                    {
                        continue;
                    }

                    registerInternal(type, string.Empty, getDefaultObjectFactory(type, firstImplementation));
                }
            }
        }

        private void buildUpInternal(object input, ResolveOptions resolveOptions)
        {
            IEnumerable<PropertyInfo> properties = from property in input.GetType().GetProperties()
                where
                    (property.GetGetMethod() != null) &&
                    (property.GetSetMethod() != null) &&
                    !property.PropertyType.IsValueType()
                select property;

            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(input, null) == null)
                {
                    try
                    {
                        property.SetValue(input,
                            resolveInternal(new TypeRegistration(property.PropertyType),
                                NamedParameterOverloads.Default, resolveOptions), null);
                    }
                    catch (ResolutionException)
                    {
                        // Catch any resolution errors and ignore them
                    }
                }
            }
        }

        private bool canConstruct(ConstructorInfo ctor, NamedParameterOverloads parameters, ResolveOptions options)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            foreach (ParameterInfo parameter in ctor.GetParameters())
            {
                if (string.IsNullOrEmpty(parameter.Name))
                {
                    return false;
                }

                bool isParameterOverload = parameters.ContainsKey(parameter.Name);

                if (parameter.ParameterType.IsPrimitive() && !isParameterOverload)
                {
                    return false;
                }

                if (!isParameterOverload &&
                    !canResolveInternal(new TypeRegistration(parameter.ParameterType), NamedParameterOverloads.Default,
                        options))
                {
                    return false;
                }
            }

            return true;
        }

        private bool canResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters, ResolveOptions options)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            Type checkType = registration.Type;
            string name = registration.Name;

            ObjectFactoryBase factory;
            if (m_registeredTypes.TryGetValue(new TypeRegistration(checkType, name), out factory))
            {
                if (factory.AssumeConstruction)
                {
                    return true;
                }

                if (factory.Constructor == null)
                {
                    return (getBestConstructor(factory.CreatesType, parameters, options) != null);
                }

                return canConstruct(factory.Constructor, parameters, options);
            }

            // Fail if requesting named resolution and settings set to fail if unresolved
            // Or bubble up if we have a parent
            if (!String.IsNullOrEmpty(name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail)
            {
                return (m_parent != null) && m_parent.canResolveInternal(registration, parameters, options);
            }

            // Attemped unnamed fallback container resolution if relevant and requested
            if (!String.IsNullOrEmpty(name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution)
            {
                if (m_registeredTypes.TryGetValue(new TypeRegistration(checkType), out factory))
                {
                    if (factory.AssumeConstruction)
                    {
                        return true;
                    }

                    return (getBestConstructor(factory.CreatesType, parameters, options) != null);
                }
            }

            // Check if type is an automatic lazy factory request
            if (isAutomaticLazyFactoryRequest(checkType))
            {
                return true;
            }

            // Check if type is an IEnumerable<TResolveType>
            if (isIEnumerableRequest(registration.Type))
            {
                return true;
            }

            // Attempt unregistered construction if possible and requested
            // If we cant', bubble if we have a parent
            if ((options.UnregisteredResolutionAction == UnregisteredResolutionActions.AttemptResolve) ||
                (checkType.IsGenericType() &&
                 options.UnregisteredResolutionAction == UnregisteredResolutionActions.GenericsOnly))
            {
                return (getBestConstructor(checkType, parameters, options) != null) || (m_parent != null) && m_parent.canResolveInternal(registration, parameters, options);
            }

            // Bubble resolution up the container tree if we have a parent
            if (m_parent != null)
            {
                return m_parent.canResolveInternal(registration, parameters, options);
            }

            return false;
        }

        private ConstructorInfo getBestConstructor(Type type, NamedParameterOverloads parameters, ResolveOptions options)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (type.IsValueType())
            {
                return null;
            }

            // Get constructors in reverse order based on the number of parameters
            // i.e. be as "greedy" as possible so we satify the most amount of dependencies possible
            IEnumerable<ConstructorInfo> ctors = getTypeConstructors(type);

            return ctors.FirstOrDefault(ctor => canConstruct(ctor, parameters, options));
        }

        private object getIEnumerableRequest(Type type)
        {
            MethodInfo genericResolveAllMethod = GetType().GetGenericMethod(
                BindingFlags.Public | BindingFlags.Instance, "ResolveAll", type.GetGenericArguments(),
                new[] {typeof (bool)});

            return genericResolveAllMethod.Invoke(this, new object[] {false});
        }

        private ObjectFactoryBase getParentObjectFactory(TypeRegistration registration)
        {
            if (m_parent == null)
            {
                return null;
            }

            ObjectFactoryBase factory;
            if (m_parent.m_registeredTypes.TryGetValue(registration, out factory))
            {
                return factory.GetFactoryForChildContainer(m_parent);
            }

            return m_parent.getParentObjectFactory(registration);
        }

        private IEnumerable<TypeRegistration> getParentRegistrationsForType(Type resolveType)
        {
            if (m_parent == null)
            {
                return new TypeRegistration[] {};
            }

            IEnumerable<TypeRegistration> registrations =
                m_parent.m_registeredTypes.Keys.Where(tr => tr.Type == resolveType);

            return registrations.Concat(m_parent.getParentRegistrationsForType(resolveType));
        }

        private void registerDefaultTypes()
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            Register<IContainer>(this);
        }

        private RegisterOptions registerInternal(Type registerType, string name, ObjectFactoryBase factory)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            var typeRegistration = new TypeRegistration(registerType, name);

            return addUpdateRegistration(typeRegistration, factory);
        }

        private IEnumerable<object> resolveAllInternal(Type resolveType, NamedParameterOverloads parameters, bool includeUnnamed)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            IEnumerable<TypeRegistration> registrations =
                m_registeredTypes.Keys.Where(tr => tr.Type == resolveType).Concat(
                    getParentRegistrationsForType(resolveType));

            if (!includeUnnamed)
            {
                registrations = registrations.Where(tr => tr.Name != string.Empty);
            }

            return
                registrations.Select(
                    registration =>
                        resolveInternal(registration, parameters, ResolveOptions.Default));
        }

        private object resolveInternal(TypeRegistration registration, NamedParameterOverloads parameters, ResolveOptions options)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            ObjectFactoryBase factory;

            // Attempt container resolution
            if (m_registeredTypes.TryGetValue(registration, out factory))
            {
                try
                {
                    return factory.GetObject(this, parameters, options);
                }
                catch (ResolutionException e)
                {
                    Tracer.Error(e);
                    throw;
                }
                catch (Exception ex)
                {
                    Tracer.Error(ex);
                    throw new ResolutionException(registration.Type, ex);
                }
            }

            // Attempt to get a factory from parent if we can
            ObjectFactoryBase bubbledObjectFactory = getParentObjectFactory(registration);
            if (bubbledObjectFactory != null)
            {
                try
                {
                    return bubbledObjectFactory.GetObject(this, parameters, options);
                }
                catch (ResolutionException ex)
                {
                    Tracer.Error(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Tracer.Error(ex);
                    throw new ResolutionException(registration.Type, ex);
                }
            }

            // Fail if requesting named resolution and settings set to fail if unresolved
            if (!String.IsNullOrEmpty(registration.Name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail)
            {
                throw new ResolutionException(registration.Type);
            }

            // Attemped unnamed fallback container resolution if relevant and requested
            if (!String.IsNullOrEmpty(registration.Name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution)
            {
                if (m_registeredTypes.TryGetValue(new TypeRegistration(registration.Type, string.Empty), out factory))
                {
                    try
                    {
                        return factory.GetObject(this, parameters, options);
                    }
                    catch (ResolutionException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new ResolutionException(registration.Type, ex);
                    }
                }
            }

#if EXPRESSIONS

    // Attempt to construct an automatic lazy factory if possible
            if (IsAutomaticLazyFactoryRequest(registration.Type))
                return GetLazyAutomaticFactoryRequest(registration.Type);
#endif
            if (isIEnumerableRequest(registration.Type))
            {
                return getIEnumerableRequest(registration.Type);
            }

            // Attempt unregistered construction if possible and requested
            if ((options.UnregisteredResolutionAction == UnregisteredResolutionActions.AttemptResolve) ||
                (registration.Type.IsGenericType() &&
                 options.UnregisteredResolutionAction == UnregisteredResolutionActions.GenericsOnly))
            {
                if (!registration.Type.IsAbstract() && !registration.Type.IsInterface())
                {
                    return ConstructType(registration.Type, parameters, options);
                }
            }

            // Unable to resolve - throw
            throw new ResolutionException(registration.Type);
        }

        private bool tryResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters, ResolveOptions options, out object result)
        {
            Guard.Requires<InvalidOperationException>(!m_disposed, "Container is disposed.");

            result = null;
            ObjectFactoryBase factory;

            // Attempt container resolution
            if (m_registeredTypes.TryGetValue(registration, out factory))
            {
                result = factory.GetObject(this, parameters, options);
                return true;
            }

            // Attempt to get a factory from parent if we can
            ObjectFactoryBase bubbledObjectFactory = getParentObjectFactory(registration);
            if (bubbledObjectFactory != null)
            {
                result = bubbledObjectFactory.GetObject(this, parameters, options);
                return true;
            }

            // Fail if requesting named resolution and settings set to fail if unresolved
            if (!String.IsNullOrEmpty(registration.Name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail)
            {
                return false;
            }

            // Attemped unnamed fallback container resolution if relevant and requested
            if (!String.IsNullOrEmpty(registration.Name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution)
            {
                if (m_registeredTypes.TryGetValue(new TypeRegistration(registration.Type, string.Empty), out factory))
                {
                    result = factory.GetObject(this, parameters, options);
                    return true;
                }
            }

#if EXPRESSIONS

    // Attempt to construct an automatic lazy factory if possible
            if (IsAutomaticLazyFactoryRequest(registration.Type))
                return GetLazyAutomaticFactoryRequest(registration.Type);
#endif
            if (isIEnumerableRequest(registration.Type))
            {
                result = getIEnumerableRequest(registration.Type);
                return true;
            }

            // Attempt unregistered construction if possible and requested
            if ((options.UnregisteredResolutionAction == UnregisteredResolutionActions.AttemptResolve) ||
                (registration.Type.IsGenericType() &&
                 options.UnregisteredResolutionAction == UnregisteredResolutionActions.GenericsOnly))
            {
                if (!registration.Type.IsAbstract() && !registration.Type.IsInterface())
                {
                    result = ConstructType(registration.Type, parameters, options);
                    return true;
                }
            }

            return false;
        }
    }
}