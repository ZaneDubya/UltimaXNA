#region Usings
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UltimaXNA.Core;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Diagnostics.Listeners;
using UltimaXNA.Core.Patterns;
#endregion

namespace UltimaXNA.Core
{
    class PluginManager
    {
        private List<IModule> m_Modules = new List<IModule>();

        public PluginManager(string baseAppPath)
        {
            Configure(baseAppPath);
        }

        private void Configure(string baseAppPath)
        {
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(baseAppPath, "plugins"));

            if (!directory.Exists)
            {
                return;
            }

            FileInfo[] assemblies = directory.GetFiles("*.dll");

            foreach (FileInfo file in assemblies)
            {
                try
                {
                    Tracer.Info("Loading plugin {0}.", file.Name);

                    Assembly assembly = Assembly.LoadFile(file.FullName);
                    IEnumerable<Type> modules = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IModule)));

                    foreach (Type module in modules)
                    {
                        Tracer.Info("Activating module {0}.", module.FullName);

                        IModule instance = (IModule)Activator.CreateInstance(module);

                        LoadModule(instance);
                    }
                }
                catch (Exception e)
                {
                    Tracer.Warn("An error occurred while trying to load plugin. [{0}]", file.FullName);
                    Tracer.Warn(e);
                }
            }
        }

        private void LoadModule(IModule module)
        {
            m_Modules.Add(module);
            module.Load();
        }
    }
}
