using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Patterns;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.World.Maps;
using System.Collections.Generic;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;
using System.IO;
using System.Linq;

namespace ExamplePlugin
{
    internal sealed class ExamplePluginModule : IModule
    {
        public string Name
        {
            get { return "UltimaXNA Example Plugin"; }
        }

        public void Load()
        {
            Tracer.Info("Map Parser loaded.");

            TileMatrixRaw tileData = new TileMatrixRaw(0, 0);

            Map map = new Map(0);

            for (uint y = 0; y < 32; y++)
            {
                Tracer.Info("Map Parser: row {0}.", y);
                for (uint x = 0; x < 896; x++)
                {
                    MapBlock block = new MapBlock(x, y);
                    block.Load(tileData, map);
                    ParseMapBlock(block);
                    block.Unload();
                }
            }

            var items = from pair in m_StaticCounts
		        orderby pair.Value ascending
		        select pair;

            using (StreamWriter tileFile = new StreamWriter(@"\AllTiles.txt"))
            {
                foreach (KeyValuePair<int, int> pair in items)
                    tileFile.WriteLine(string.Format("{0},{1}", pair.Key, pair.Value));
            }
        }

        public void Unload()
        {

        }

        private Dictionary<int, int> m_StaticCounts = new Dictionary<int, int>();

        private void ParseMapBlock(MapBlock block)
        {
            for (int t = 0; t < 64; t++)
            {
                foreach (AEntity e in block.Tiles[t].Entities)
                {
                    if (e is StaticItem)
                    {
                        if (m_StaticCounts.ContainsKey((e as StaticItem).ItemID))
                            m_StaticCounts[(e as StaticItem).ItemID]++;
                        else
                            m_StaticCounts.Add((e as StaticItem).ItemID, 1);
                    }
                }
            }
        }
    }
}
