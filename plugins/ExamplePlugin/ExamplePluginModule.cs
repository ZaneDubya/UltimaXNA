using System.Collections.Generic;
using System.IO;
using System.Linq;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Patterns;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.World.Maps;

namespace ExamplePlugin
{
    internal sealed class ExamplePluginModule : IModule
    {
        public string Name
        {
            get { return "Example Plugin - Not for production!"; }
        }

        public void Load()
        {
            CandleObjectDebugger candle = new CandleObjectDebugger();
            candle.OutputAllCandleTextures();
        }

        public void Unload()
        {

        }
    }
}
