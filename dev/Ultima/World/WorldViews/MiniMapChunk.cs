using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Maps;

namespace UltimaXNA.Ultima.World.WorldViews
{
    class MiniMapChunk
    {
        public uint X, Y;

        public uint[] Colors;
        private static sbyte[] m_Zs = new sbyte[64]; // shared between all instances of MiniMapChunk.

        public MiniMapChunk(uint x, uint y, TileMatrixData tileData)
        {
            X = x;
            Y = y;
            Colors = new uint[64];

            // get data from the tile Matrix
            byte[] groundData = tileData.GetLandChunk(x, y);
            int staticLength;
            byte[] staticsData = tileData.GetStaticChunk(x, y, out staticLength);

            // get the ground colors
            int groundDataIndex = 0;
            for (int i = 0; i < 64; i++)
            {
                Colors[i] = RadarColorData.Colors[groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8)];
                m_Zs[i]= (sbyte)groundData[groundDataIndex++];
            }

            // get the static colors
            int countStatics = staticLength / 7;
            int staticDataIndex = 0;
            for (int i = 0; i < countStatics; i++)
            {
                int itemID = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8);
                int tile = staticsData[staticDataIndex++] + staticsData[staticDataIndex++] * 8;
                sbyte z = (sbyte)staticsData[staticDataIndex++];
                int hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256); // is this used?

                ItemData data = TileData.ItemData[itemID];
                int iz = z + data.Height + (data.IsRoof || data.IsSurface ? 1 : 0);

                if ((x * 8 + (tile % 8) == 1480) && (y * 8 + (tile / 8) == 1608))
                {
                    if (iz > m_Zs[tile])
                    {
                        Colors[tile] = RadarColorData.Colors[itemID + 0x4000];
                        m_Zs[tile] = (sbyte)iz;
                    }
                }

                if (iz > m_Zs[tile])
                {
                    Colors[tile] = RadarColorData.Colors[itemID + 0x4000];
                    m_Zs[tile] = (sbyte)iz;
                }
            }
        }

        public MiniMapChunk(MapChunk block)
        {
            X = (uint)block.ChunkX;
            Y = (uint)block.ChunkY;
            Colors = new uint[64];

            for (uint tile = 0; tile < 64; tile++)
            {
                uint color = 0xffff00ff;
                // get the topmost static item or ground
                int eIndex = block.Tiles[tile].Entities.Count - 1;
                while (eIndex >= 0)
                {
                    AEntity e = block.Tiles[tile].Entities[eIndex];
                    if (e is Ground)
                    {
                        color = RadarColorData.Colors[(e as Ground).LandDataID];
                        break;
                    }
                    else if (e is StaticItem)
                    {
                        color = RadarColorData.Colors[(e as StaticItem).ItemID + 0x4000];
                        break;
                    }
                    eIndex--;
                }
                Colors[tile] = color;
            }
        }
    }
}
