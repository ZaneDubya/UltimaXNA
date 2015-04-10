#region Usings

using System;
using System.Threading;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Patterns.IoC;
using UltimaXNA.Ultima.IO;

#endregion

namespace UltimaXNA
{
    internal sealed class CoreModule : IModule
    {
        public string Name
        {
            get { return "UltimaXNA Core Module"; }
        }

        public void Load(IContainer container)
        {

        }

        public void Unload(IContainer container)
        {
        }
    }
}