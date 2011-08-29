/***************************************************************************
 *   Dynamic.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   From: http://www.codeproject.com/KB/cs/DynLoadClassInvokeMethod.aspx
 *
 ***************************************************************************/
using System;
using System.Collections;
using System.Reflection;

namespace UltimaXNA.Diagnostics
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
            throw (new System.Exception("Dynamic failed to load."));
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
                    AssemblyReferences.Add(AssemblyName,
                          assembly = Assembly.LoadFrom(AssemblyName));
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
                throw (new System.Exception("Dynamic could not instantiate class."));
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
            return (InvokeMethod(ci, MethodName, args));
        }
    }
}
