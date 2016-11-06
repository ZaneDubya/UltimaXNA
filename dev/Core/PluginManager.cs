/***************************************************************************
 *   PluginManager.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Patterns;
#endregion

namespace UltimaXNA.Core {
    class PluginManager {
        List<IModule> m_Modules = new List<IModule>();

        public PluginManager(string baseAppPath) {
            Configure(baseAppPath);
        }

        void Configure(string baseAppPath) {
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(baseAppPath, "plugins"));
            if (!directory.Exists) {
                return;
            }
            FileInfo[] assemblies = directory.GetFiles("*.dll");
            foreach (FileInfo file in assemblies) {
                try {
                    Tracer.Info("Loading plugin {0}.", file.Name);
                    Assembly assembly = Assembly.LoadFile(file.FullName);
                    IEnumerable<Type> modules = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IModule)));
                    foreach (Type module in modules) {
                        Tracer.Info("Activating module {0}.", module.FullName);
                        IModule instance = (IModule)Activator.CreateInstance(module);
                        LoadModule(instance);
                    }
                }
                catch (Exception e) {
                    Tracer.Warn("An error occurred while trying to load plugin. [{0}]", file.FullName);
                    Tracer.Warn(e);
                }
            }
        }

        void LoadModule(IModule module) {
            m_Modules.Add(module);
            module.Load();
        }
    }
}
