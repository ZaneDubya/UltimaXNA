using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;

namespace UltimaXNA.Ultima.World.Views
{
    class MiniMapBlock
    {
        public uint[] Colors = new uint[64];
        private static sbyte[] m_Zs = new sbyte[64]; // shared between all instances of MiniMapBlock.

        public MiniMapBlock(uint X, uint Y, TileMatrixRaw tileData)
        {
            // get data from the tile Matrix
            byte[] groundData = tileData.GetLandBlock((int)X, (int)Y);
            byte[] staticsData = tileData.GetStaticBlock((int)X, (int)Y);

            // get the ground colors
            int groundDataIndex = 0;
            for (int i = 0; i < 64; i++)
            {
                Colors[i] = IO.RadarColorData.Colors[groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8)];
                m_Zs[i]= (sbyte)groundData[groundDataIndex++];
            }

            // get the static colors
            int countStatics = staticsData.Length / 7;
            int staticDataIndex = 0;
            for (int i = 0; i < countStatics; i++)
            {
                int itemID = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8);
                int tile = staticsData[staticDataIndex++] + staticsData[staticDataIndex++] * 8;
                sbyte z = (sbyte)staticsData[staticDataIndex++];
                int hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256); // is this used?

                if (z > m_Zs[tile])
                {
                    Colors[tile] = IO.RadarColorData.Colors[itemID + 0x4000];
                    m_Zs[tile] = z;
                }
            }
        }

        public MiniMapBlock(MapBlock block)
        {
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
                        color = IO.RadarColorData.Colors[(e as Ground).LandDataID];
                        break;
                    }
                    else if (e is StaticItem)
                    {
                        color = IO.RadarColorData.Colors[(e as StaticItem).ItemID + 0x4000];
                        break;
                    }
                    eIndex--;
                }
                Colors[tile] = color;
            }
        }
    }
}
