using System.Collections.Generic;
using System.IO;
using System.Linq;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Maps;

namespace ExamplePlugin
{
    /// <summary>
    /// Counts all statics in a given map.
    /// </summary>
    class MapParser
    {
        private Dictionary<int, int> m_StaticCounts;

        public void CreateSeasonalTileInfo()
        {
            m_StaticCounts = new Dictionary<int, int>();

            TileMatrixData tileData = new TileMatrixData(0);

            Map map = new Map(0);

            for (uint y = 0; y < tileData.ChunkHeight; y++)
            {
                Tracer.Info("Map Parser: row {0}.", y);
                for (uint x = 0; x < tileData.ChunkWidth; x++)
                {
                    ParseMapBlock(tileData, x, y);
                }
            }

            var items = from pair in m_StaticCounts
                        orderby pair.Value descending
                        select pair;

            using (FileStream file = new FileStream(@"AllTiles.txt", FileMode.Create))
            {
                StreamWriter stream = new StreamWriter(file);
                foreach (KeyValuePair<int, int> pair in items)
                {
                    ItemData itemData = TileData.ItemData[pair.Key];
                    if ((itemData.IsBackground || itemData.IsFoliage) && !itemData.IsWet && !itemData.IsSurface)
                        stream.WriteLine(string.Format("{0},{1} ; {2}", pair.Key, pair.Value, itemData.Name));
                }
                stream.Flush();
                file.Flush();
            }
        }

        private void ParseMapBlock(TileMatrixData tileData, uint x, uint y)
        {
            byte[] groundData = tileData.GetLandChunk(x, y);
            int staticLength;
            byte[] staticsData = tileData.GetStaticChunk(x, y, out staticLength);

            // load the statics data
            int countStatics = staticLength / 7;
            int staticDataIndex = 0;
            for (int i = 0; i < countStatics; i++)
            {
                int iTileID = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8);
                int iX = staticsData[staticDataIndex++];
                int iY = staticsData[staticDataIndex++];
                int iTileZ = (sbyte)staticsData[staticDataIndex++];
                int hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256);

                if (m_StaticCounts.ContainsKey(iTileID))
                    m_StaticCounts[iTileID]++;
                else
                    m_StaticCounts.Add(iTileID, 1);
            }
        }
    }
}
