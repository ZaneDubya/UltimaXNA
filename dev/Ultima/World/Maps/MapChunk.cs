/***************************************************************************
 *   MapBlock.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
#endregion

namespace UltimaXNA.Ultima.World.Maps
{
    public class MapChunk
    {
        public MapTile[] Tiles;

        public readonly uint ChunkX;
        public readonly uint ChunkY;

        public MapChunk(uint x, uint y)
        {
            ChunkX = x;
            ChunkY = y;

            Tiles = new MapTile[64];
            for (int i = 0; i < 64; i++)
                Tiles[i] = new MapTile();
        }

        /// <summary>
        /// Unloads all tiles and entities from memory.
        /// </summary>
        public void Unload()
        {
            for (int i = 0; i < 64; i++)
            {
                if (Tiles[i] != null)
                {
                    for (int j = 0; j < Tiles[i].Entities.Count; j++)
                    {
                        if (Tiles[i].Entities[j].IsClientEntity)
                        {
                            // Never dispose of the client entity.
                        }
                        else
                        {
                            int entityCount = Tiles[i].Entities.Count;
                            Tiles[i].Entities[j].Dispose();
                            if (entityCount == Tiles[i].Entities.Count)
                            {
                                Tiles[i].Entities.RemoveAt(j);
                            }
                            j--; // entity will dispose, removing it from collection.
                        }
                    }
                    Tiles[i] = null;
                }
            }
            Tiles = null;
        }

        public void Load(TileMatrixData tileData, Map map)
        {
            // get data from the tile Matrix
            byte[] groundData = tileData.GetLandChunk(ChunkX, ChunkY);
            int staticLength;
            byte[] staticsData = tileData.GetStaticChunk(ChunkX, ChunkY, out staticLength);

            // load the ground data into the tiles.
            int groundDataIndex = 0;
            for (int i = 0; i < 64; i++)
            {
                int tileID = groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8);
                int tileZ = (sbyte)groundData[groundDataIndex++];

                Ground ground = new Ground(tileID, map);
                ground.Position.Set((int)ChunkX * 8 + i % 8, (int)ChunkY * 8 + (i / 8), tileZ);
            }

            // load the statics data into the tiles
            int countStatics = staticLength / 7;
            int staticDataIndex = 0;
            for (int i = 0; i < countStatics; i++)
            {
                int iTileID = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8);
                int iX = staticsData[staticDataIndex++];
                int iY = staticsData[staticDataIndex++];
                int iTileZ = (sbyte)staticsData[staticDataIndex++];
                int hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256);

                StaticItem item = new StaticItem(iTileID, hue, i, map);
                item.Position.Set((int)ChunkX * 8 + iX, (int)ChunkY * 8 + iY, iTileZ);
            }
        }
    }
}
