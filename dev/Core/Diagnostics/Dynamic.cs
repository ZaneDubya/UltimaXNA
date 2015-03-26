/***************************************************************************
 *   Dynamic.cs
 *   From: http://www.codeproject.com/KB/cs/DynLoadClassInvokeMethod.aspx
 *
 ***************************************************************************/
using System;
using System.Collections;
using System.Reflection;

namespace UltimaXNA.Core.Diagnostics
{
    public class Dynamic
    {
        public static Object InvokeMethodSlow(string AssemblyName,
               string ClassName, string MethodName, Object[] args)
        {
            Assembly assembly = Assembly.LoadFrom(AssemblyName);
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass == true)
                {
                    if (type.FullName.EndsWith("." + ClassName))
                    {
                        object ClassObj = Activator.CreateInstance(type);
                        object Result = type.InvokeMember(MethodName,
                          BindingFlags.Default | BindingFlags.InvokeMethod,
                               null,
                               ClassObj,
                               args);
                        return (Result);
                    }
                }
            }
            return null;
        }

        public class DynaClassInfo
        {
            public Type type;
            public Object ClassObject;

            public DynaClassInfo()
            {
            }

            public DynaClassInfo(Type t, Object c)
            {
                type = t;
                ClassObject = c;
            }
        }


        public static Hashtable AssemblyReferences = new Hashtable();
        public static Hashtable ClassReferences = new Hashtable();

        public static DynaClassInfo
               GetClassReference(string AssemblyName, string ClassName)
        {
            if (ClassReferences.ContainsKey(AssemblyName) == false)
            {
                Assembly assembly;
                if (AssemblyReferences.ContainsKey(AssemblyName) == false)
                {
                    try
                    {
                        AssemblyReferences.Add(AssemblyName,
                              assembly = Assembly.LoadFrom(AssemblyName));
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                    assembly = (Assembly)AssemblyReferences[AssemblyName];

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass == true)
                    {
                        if (type.FullName.EndsWith("." + ClassName))
                        {
                            DynaClassInfo ci = new DynaClassInfo(type,
                                               Activator.CreateInstance(type));
                            ClassReferences.Add(AssemblyName, ci);
                            return (ci);
                        }
                    }
                }
            }
            return ((DynaClassInfo)ClassReferences[AssemblyName]);
        }

        public static Object InvokeMethod(DynaClassInfo ci,
                             string MethodName, Object[] args)
        {
            Object Result = ci.type.InvokeMember(MethodName,
              BindingFlags.Default | BindingFlags.InvokeMethod,
                   null,
                   ci.ClassObject,
                   args);
            return (Result);
        }

        public static Object InvokeMethod(string AssemblyName,
               string ClassName, string MethodName, Object[] args)
        {
            DynaClassInfo ci = GetClassReference(AssemblyName, ClassName);
            if (ci == null)
            {
                Type type = Type.GetType(AssemblyName + "." + ClassName);
                if (type != null)
                    ci = new DynaClassInfo(type, Activator.CreateInstance(type));
            }
            return (InvokeMethod(ci, MethodName, args));
        }

        public static void InvokeDebug()
        {
            System.Object[] args = { };
            InvokeMethod("../../../UltimaEdit/bin/Debug/UltimaEdit.dll", "Main", "Toggle", args);
        }
    }
}
