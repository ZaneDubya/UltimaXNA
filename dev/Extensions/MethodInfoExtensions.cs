using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class MethodInfoExtensions
{
    public static MethodInfo GetBaseDefinition(this MethodInfo method)
    {
        var flags = BindingFlags.Instance;

        if (method.IsPublic)
        {
            flags |= BindingFlags.Public;
        }
        else
        {
            flags |= BindingFlags.NonPublic;
        }

        // get...
        TypeInfo info = method.DeclaringType.GetTypeInfo();
        var found = new List<MethodInfo>();

        while (true)
        {
            // find...
            MethodInfo inParent = info.AsType().GetMethod(method.Name, flags, method.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
            if (inParent != null)
            {
                found.Add(inParent);
            }

            // up...
            if (info.BaseType == null)
            {
                break;
            }

            info = info.BaseType.GetTypeInfo();
        }

        // return the last one...
        return found.Last();
    }

    public static bool IsAbstract(this MethodBase method)
    {
        return method.IsAbstract;
    }

    public static MemberTypes MemberType()
    {
        return MemberTypes.Method;
    }
}