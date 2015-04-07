using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class AssemblyExtender
{
    public static Type[] GetTypes(this Assembly asm)
    {
        return asm.DefinedTypes.Select(type => type.AsType()).ToArray();
    }

    public static IEnumerable<Type> SafeGetTypes(this Assembly assembly)
    {
        Type[] assemblies;

        try
        {
            assemblies = assembly.GetTypes();
        }
        catch (FileNotFoundException)
        {
            assemblies = new Type[] {};
        }
        catch (NotSupportedException)
        {
            assemblies = new Type[] {};
        }

        return assemblies;
    }
}