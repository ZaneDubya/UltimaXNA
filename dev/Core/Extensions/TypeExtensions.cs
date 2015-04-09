using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UltimaXNA.Collections;

public enum TypeCode
{
    Empty = 0,
    Object = 1,
    DBNull = 2,
    Boolean = 3,
    Char = 4,
    SByte = 5,
    Byte = 6,
    Int16 = 7,
    UInt16 = 8,
    Int32 = 9,
    UInt32 = 10,
    Int64 = 11,
    UInt64 = 12,
    Single = 13,
    Double = 14,
    Decimal = 15,
    DateTime = 16,
    String = 18,
}

[Flags]
public enum BindingFlags
{
    Instance = 4,
    Static = 8,
    Public = 16,
    NonPublic = 32,
    FlattenHierarchy = 64,
}

[Flags]
public enum MemberTypes
{
    Constructor = 1,
    Event = 2,
    Field = 4,
    Method = 8,
    Property = 16
}

public static class EventInfoExtensions
{
    public static MethodInfo GetAddMethod(this EventInfo e, bool nonPublic = false)
    {
        if (e.AddMethod != null && (e.AddMethod.IsPublic || nonPublic))
        {
            return e.AddMethod;
        }

        return null;
    }

    public static MethodInfo GetRemoveMethod(this EventInfo e, bool nonPublic = false)
    {
        if (e.RemoveMethod != null && (e.RemoveMethod.IsPublic || nonPublic))
        {
            return e.RemoveMethod;
        }
        return null;
    }
}

public static class MemberInfoExtensions
{
    public static bool CheckBindings(this MemberInfo member, BindingFlags flags, bool inParent)
    {
        if ((member.IsStatic() & (flags & BindingFlags.Static) == BindingFlags.Static) ||
            (!(member.IsStatic()) & (flags & BindingFlags.Instance) == BindingFlags.Instance))
        {
            // if we're static and we're in parent, and we haven't specified flatten hierarchy, we can't match...
            if (inParent && (int)(flags & BindingFlags.FlattenHierarchy) == 0 && member.IsStatic())
            {
                return false;
            }

            if ((member.IsPublic() & (flags & BindingFlags.Public) == BindingFlags.Public) ||
                (!(member.IsPublic()) & (flags & BindingFlags.NonPublic) == BindingFlags.NonPublic))
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public static bool IsPublic(this MemberInfo member)
    {
        MethodBase methodBase = member as MethodBase;

        if (methodBase != null)
        {
            return methodBase.IsPublic;
        }
        PropertyInfo prop = member as PropertyInfo;
        if (prop != null)
        {
            return (prop.GetMethod != null && prop.GetMethod.IsPublic) || (prop.SetMethod != null && prop.SetMethod.IsPublic);
        }

        FieldInfo fieldInfo = member as FieldInfo;
        if (fieldInfo != null)
        {
            return fieldInfo.IsPublic;
        }

        EventInfo evt = member as EventInfo;
        if (evt != null)
        {
            return (evt.AddMethod != null && evt.AddMethod.IsPublic) || (evt.RemoveMethod != null && evt.RemoveMethod.IsPublic);
        }

        throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member.GetType()));
    }

    public static bool IsStatic(this MemberInfo member)
    {
        MethodBase methodBase = member as MethodBase;
        if (methodBase != null)
        {
            return methodBase.IsStatic;
        }
        PropertyInfo prop = member as PropertyInfo;
        if (prop != null)
        {
            return (prop.GetMethod != null && prop.GetMethod.IsStatic) || (prop.SetMethod != null && prop.SetMethod.IsStatic);
        }
        FieldInfo fieldInfo = member as FieldInfo;
        if (fieldInfo != null)
        {
            return fieldInfo.IsStatic;
        }
        EventInfo evt = member as EventInfo;
        if (evt != null)
        {
            return (evt.AddMethod != null && evt.AddMethod.IsStatic) || (evt.RemoveMethod != null && evt.RemoveMethod.IsStatic);
        }
        throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member.GetType()));
    }

    public static MemberTypes MemberType(this MemberInfo member)
    {
        MethodInfo methodInfo = member as MethodInfo;
        if (methodInfo != null)
        {
            return MethodInfoExtensions.MemberType();
        }
        throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member.GetType()));
    }

    public static int MetadataToken(this MemberInfo member)
    {
        return member.GetHashCode();
    }
}

public static class PropertyInfoExtensions
{
    public static MethodInfo GetGetMethod(this PropertyInfo prop, bool nonPublic = false)
    {
        if (prop.GetMethod != null && (prop.GetMethod.IsPublic || nonPublic))
        {
            return prop.GetMethod;
        }
        return null;
    }

    public static MethodInfo GetSetMethod(this PropertyInfo prop, bool nonPublic = false)
    {
        if (prop.SetMethod != null && (prop.SetMethod.IsPublic || nonPublic))
        {
            return prop.SetMethod;
        }
        return null;
    }

    public static Type ReflectedType(this PropertyInfo prop)
    {
        // this isn't right...
        return prop.DeclaringType;
    }
}

public static class TypeExtensions
{
    public static readonly Type[] EmptyTypes = {};

    private static readonly SafeDictionary<GenericMethodCacheKey, MethodInfo> m_genericMethodCache;

    static TypeExtensions()
    {
        m_genericMethodCache = new SafeDictionary<GenericMethodCacheKey, MethodInfo>();
    }

    public static Assembly Assembly(this Type type)
    {
        return type.GetTypeInfo().Assembly;
    }

    public static Type BaseType(this Type type)
    {
        return type.GetTypeInfo().BaseType;
    }

    public static bool ContainsGenericParameters(this Type type)
    {
        return type.GetTypeInfo().ContainsGenericParameters;
    }

    public static MethodBase DeclaringMethod(this Type type)
    {
        return type.GetTypeInfo().DeclaringMethod;
    }

    public static IEnumerable<Type> FindInterfaces(this Type type, Func<Type, object, bool> filter, object criteria)
    {
        List<Type> results = type.GetInterfaces().Where(walk => filter(type, criteria)).ToList();

        return results.ToArray();
    }

    public static GenericParameterAttributes GenericParameterAttributes(this Type type)
    {
        return type.GetTypeInfo().GenericParameterAttributes;
    }

    public static ConstructorInfo GetConstructor(this Type type, Type[] types)
    {
        return type.GetConstructor(BindingFlags.Public, types);
    }

    public static ConstructorInfo GetConstructor(this Type type, BindingFlags flags, Type[] types)
    {
        // can't have static constructors...
        flags |= BindingFlags.Instance | BindingFlags.Static;
        flags ^= BindingFlags.Static;

        // walk...
        return type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(info => info.CheckBindings(flags, false) && CheckParameters(info, types));
    }

    public static ConstructorInfo[] GetConstructors(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
    {
        return GetMembers<ConstructorInfo>(type, flags).ToArray();
    }

    public static ConstructorInfo GetDefaultConstructor(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
    {
        return GetMembers<ConstructorInfo>(type, flags).First(x => x.GetParameters().Length == 0);
    }

    public static T GetCustomAttribute<T>(this Type type, bool inherit = false)
        where T : Attribute
    {
        return type.GetTypeInfo().GetCustomAttribute<T>(inherit);
    }

    public static Attribute[] GetCustomAttributes(this Type type, bool inherit = false)
    {
        return type.GetTypeInfo().GetCustomAttributes(inherit).Cast<Attribute>().ToArray();
    }

    public static Attribute[] GetCustomAttributes(this Type type, Type attributeType, bool inherit = false)
    {
        return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).Cast<Attribute>().ToArray();
    }

    public static EventInfo GetEvent(this Type type, string name, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
    {
        return GetMember<EventInfo>(type, name, flags);
    }

    public static EventInfo[] GetEvents(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
    {
        return GetMembers<EventInfo>(type, flags).ToArray();
    }

    public static FieldInfo GetField(this Type type, string name, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
    {
        return GetMember<FieldInfo>(type, name, flags);
    }

    public static FieldInfo[] GetFields(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
    {
        return GetMembers<FieldInfo>(type, flags).ToArray();
    }

    public static Type[] GetGenericArguments(this Type type)
    {
        return type.GetTypeInfo().GenericTypeArguments;
    }

    public static MethodInfo GetGenericMethod(this Type sourceType, BindingFlags bindingFlags, string methodName, Type[] genericTypes, Type[] parameterTypes)
    {
        MethodInfo method;
        GenericMethodCacheKey cacheKey = new GenericMethodCacheKey(sourceType, methodName, genericTypes, parameterTypes);

        // Shouldn't need any additional locking
        // we don't care if we do the method info generation
        // more than once before it gets cached.
        if (!m_genericMethodCache.TryGetValue(cacheKey, out method))
        {
            method = GetMethod(sourceType, bindingFlags, methodName, genericTypes, parameterTypes);
            m_genericMethodCache[cacheKey] = method;
        }

        return method;
    }

    public static Type[] GetGenericParameterConstraints(this Type type)
    {
        return type.GetTypeInfo().GetGenericParameterConstraints();
    }

    public static Type GetInterface(this Type type, string name, bool ignoreCase = false)
    {
        // walk up the hierarchy...
        TypeInfo info = type.GetTypeInfo();
        while (true)
        {
            foreach (Type iface in type.GetInterfaces())
            {
                if (ignoreCase)
                {
                    // this matches just the name...
                    if (string.Compare(iface.Name, name, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        return iface;
                    }
                }
                else
                {
                    if (iface.FullName == name || iface.Name == name)
                    {
                        return iface;
                    }
                }
            }

            // up...
            if (info.BaseType == null)
            {
                break;
            }
            info = info.BaseType.GetTypeInfo();
        }

        return null;
    }

    public static InterfaceMapping GetInterfaceMap(this Type type, Type interfaceType)
    {
        return type.GetTypeInfo().GetRuntimeInterfaceMap(interfaceType);
    }

    public static IEnumerable<Type> GetInterfaces(this Type type)
    {
        return type.GetTypeInfo().ImplementedInterfaces.ToArray();
    }

    public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
    {
        return GetMember<MethodInfo>(type, name, flags);
    }

    public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags, Type[] parameters)
    {
        return type.GetMethods(flags).FirstOrDefault(method => method.Name == name && CheckParameters(method, parameters));
    }

    public static IEnumerable<MethodInfo> GetMethods(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
    {
        return GetMembers<MethodInfo>(type, flags).ToArray();
    }

    public static IEnumerable<PropertyInfo> GetProperties(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
    {
        return GetMembers<PropertyInfo>(type, flags).ToArray();
    }

    public static PropertyInfo GetProperty(this Type type, string name, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
    {
        return GetMember<PropertyInfo>(type, name, flags);
    }

    public static TypeCode GetTypeCode(this Type type)
    {
        if (type == null)
        {
            return TypeCode.Empty;
        }

        if (typeof (bool).IsAssignableFrom(type))
        {
            return TypeCode.Boolean;
        }
        if (typeof (char).IsAssignableFrom(type))
        {
            return TypeCode.Char;
        }
        if (typeof (sbyte).IsAssignableFrom(type))
        {
            return TypeCode.SByte;
        }
        if (typeof (byte).IsAssignableFrom(type))
        {
            return TypeCode.Byte;
        }
        if (typeof (short).IsAssignableFrom(type))
        {
            return TypeCode.Int16;
        }
        if (typeof (ushort).IsAssignableFrom(type))
        {
            return TypeCode.UInt16;
        }
        if (typeof (int).IsAssignableFrom(type))
        {
            return TypeCode.Int32;
        }
        if (typeof (uint).IsAssignableFrom(type))
        {
            return TypeCode.UInt32;
        }
        if (typeof (long).IsAssignableFrom(type))
        {
            return TypeCode.Int64;
        }
        if (typeof (ulong).IsAssignableFrom(type))
        {
            return TypeCode.UInt64;
        }
        if (typeof (float).IsAssignableFrom(type))
        {
            return TypeCode.Single;
        }
        if (typeof (double).IsAssignableFrom(type))
        {
            return TypeCode.Double;
        }
        if (typeof (decimal).IsAssignableFrom(type))
        {
            return TypeCode.Decimal;
        }
        if (typeof (DateTime).IsAssignableFrom(type))
        {
            return TypeCode.DateTime;
        }
        if (typeof (string).IsAssignableFrom(type))
        {
            return TypeCode.String;
        }

        return TypeCode.Object;
    }

    public static bool IsNullable(this Type type)
    {
        return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    public static bool IsAbstract(this Type type)
    {
        return type.GetTypeInfo().IsAbstract;
    }

    public static bool IsArray(this Type type)
    {
        return type.GetTypeInfo().IsArray;
    }

    public static bool IsAssignableFrom(this Type type, Type toCheck)
    {
        return type.GetTypeInfo().IsAssignableFrom(toCheck.GetTypeInfo());
    }

    public static bool IsClass(this Type type)
    {
        return type.GetTypeInfo().IsClass;
    }

    public static bool IsEnum(this Type type)
    {
        return type.GetTypeInfo().IsEnum;
    }

    public static bool IsGenericType(this Type type)
    {
        return type.GetTypeInfo().IsGenericType;
    }

    public static bool IsGenericTypeDefinition(this Type type)
    {
        return type.GetTypeInfo().IsGenericTypeDefinition;
    }

    public static bool IsInterface(this Type type)
    {
        return type.GetTypeInfo().IsInterface;
    }

    public static bool IsNestedAssembly(this Type type)
    {
        return type.GetTypeInfo().IsNestedAssembly;
    }

    public static bool IsNestedFamORAssem(this Type type)
    {
        return type.GetTypeInfo().IsNestedFamORAssem;
    }

    public static bool IsNestedPublic(this Type type)
    {
        return type.GetTypeInfo().IsNestedPublic;
    }

    public static bool IsPrimitive(this Type type)
    {
        return type.GetTypeInfo().IsPrimitive;
    }

    public static bool IsPublic(this Type type)
    {
        return type.GetTypeInfo().IsPublic;
    }

    public static bool IsSealed(this Type type)
    {
        return type.GetTypeInfo().IsSealed;
    }

    public static bool IsSubclassOf(this Type type, Type toCheck)
    {
        return type.GetTypeInfo().IsSubclassOf(toCheck);
    }

    public static bool IsValueType(this Type type)
    {
        return type.GetTypeInfo().IsValueType;
    }

    public static bool IsVisible(this Type type)
    {
        return type.GetTypeInfo().IsVisible;
    }

    public static int MetadataToken(this Type type)
    {
        // @mbrit - 2012-06-01 - no idea what to do with this...
        return type.GetHashCode();
    }

    public static Module Module(this Type type)
    {
        return type.GetTypeInfo().Module;
    }

    public static Type UnderlyingSystemType(this Type type)
    {
        // @mbrit - 2012-05-30 - this needs more science... UnderlyingSystemType isn't supported
        // in WinRT, but unclear why this was used...
        return type;
    }

    private static bool CheckParameters(MethodBase method, Type[] parameters)
    {
        ParameterInfo[] methodParameters = method.GetParameters();
        if (methodParameters.Length == parameters.Length)
        {
            if (parameters.Length == 0)
            {
                return true;
            }

            return !parameters.Where((t, index) => t != methodParameters[index].ParameterType).Any();
        }

        return false;
    }

    private static T GetMember<T>(Type type, string name, BindingFlags flags)
        where T : MemberInfo
    {
        return GetMembers<T>(type, flags).FirstOrDefault(member => member.Name == name);
    }

    private static List<T> GetMembers<T>(Type type, BindingFlags flags)
        where T : MemberInfo
    {
        List<T> results = new List<T>();

        TypeInfo info = type.GetTypeInfo();
        bool inParent = false;
        while (true)
        {
            bool parent = inParent;

            results.AddRange(info.DeclaredMembers.Where(v => v is T).Cast<T>().Where(member => member.CheckBindings(flags, parent)));

            // constructors never walk the hierarchy...
            if (typeof (T) == typeof (ConstructorInfo))
            {
                break;
            }

            // up...
            if (info.BaseType == null)
            {
                break;
            }
            info = info.BaseType.GetTypeInfo();
            inParent = true;
        }

        return results;
    }

    private static MethodInfo GetMethod(Type sourceType, BindingFlags flags, string methodName, Type[] genericTypes, Type[] parameterTypes)
    {
        List<MethodInfo> methods =
            sourceType.GetMethods(flags).Where(
                mi => string.Equals(methodName, mi.Name, StringComparison.Ordinal)).Where(
                    mi => mi.ContainsGenericParameters).Where(mi => mi.GetGenericArguments().Length == genericTypes.Length).
                Where(mi => mi.GetParameters().Length == parameterTypes.Length).Select(
                    mi => mi.MakeGenericMethod(genericTypes)).Where(
                        mi => mi.GetParameters().Select(pi => pi.ParameterType).SequenceEqual(parameterTypes)).ToList();

        if (methods.Count > 1)
        {
            throw new AmbiguousMatchException();
        }

        return methods.FirstOrDefault();
    }

    #region Nested type: GenericMethodCacheKey

    private sealed class GenericMethodCacheKey
    {
        private readonly Type[] m_genericTypes;
        private readonly int m_hashCode;
        private readonly string m_methodName;
        private readonly Type[] m_parameterTypes;
        private readonly Type m_sourceType;

        public GenericMethodCacheKey(Type sourceType, string methodName, Type[] genericTypes, Type[] parameterTypes)
        {
            m_sourceType = sourceType;
            m_methodName = methodName;
            m_genericTypes = genericTypes;
            m_parameterTypes = parameterTypes;
            m_hashCode = GenerateHashCode();
        }

        public override bool Equals(object obj)
        {
            GenericMethodCacheKey cacheKey = obj as GenericMethodCacheKey;
            if (cacheKey == null)
            {
                return false;
            }

            if (m_sourceType != cacheKey.m_sourceType)
            {
                return false;
            }

            if (!String.Equals(m_methodName, cacheKey.m_methodName, StringComparison.Ordinal))
            {
                return false;
            }

            if (m_genericTypes.Length != cacheKey.m_genericTypes.Length)
            {
                return false;
            }

            if (m_parameterTypes.Length != cacheKey.m_parameterTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < m_genericTypes.Length; ++i)
            {
                if (m_genericTypes[i] != cacheKey.m_genericTypes[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < m_parameterTypes.Length; ++i)
            {
                if (m_parameterTypes[i] != cacheKey.m_parameterTypes[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }

        private int GenerateHashCode()
        {
            unchecked
            {
                int result = m_sourceType.GetHashCode();

                result = (result * 397) ^ m_methodName.GetHashCode();

                for (int i = 0; i < m_genericTypes.Length; ++i)
                {
                    result = (result * 397) ^ m_genericTypes[i].GetHashCode();
                }

                for (int i = 0; i < m_parameterTypes.Length; ++i)
                {
                    result = (result * 397) ^ m_parameterTypes[i].GetHashCode();
                }

                return result;
            }
        }
    }

    #endregion
}