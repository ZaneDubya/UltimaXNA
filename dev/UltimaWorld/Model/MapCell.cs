using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UltimaData;
using UltimaXNA.Entity;

namespace UltimaXNA.UltimaWorld.Model
{
    class MapCell
    {
        public MapTile[] Tiles;

        public readonly int X, Y;

        public MapCell(int x, int y)
        {
            X = x;
            Y = y;

            Tiles = new MapTile[64];
            for (int i = 0; i < 64; i++)
                Tiles[i] = new MapTile();
        }

        public void LoadTiles(TileMatrixRaw tileData)
        {
            // get data from the tile Matrix
            byte[] groundData = tileData.GetLandBlock(X, Y);
            byte[] staticsData = tileData.GetStaticBlock(X, Y);

            // load the ground data into the tiles.
            int groundDataIndex = 0;
            for (int i = 0; i < 64; i++)
            {
                int iTileID = groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8);
                int iTileZ = (sbyte)groundData[groundDataIndex++];

                Ground ground = new Ground(iTileID);
                ground.Position.Set(X * 8 + i % 8, Y * 8 + (i / 8), iTileZ);
            }

            // load the statics data into the tiles
            int countStatics = staticsData.Length / 7;
            int staticDataIndex = 0;
            for (int i = 0; i < countStatics; i++)
            {
                int iTileID = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8);
                int iX = staticsData[staticDataIndex++];
                int iY = staticsData[staticDataIndex++];
                int iTileZ = (sbyte)staticsData[staticDataIndex++];
                staticDataIndex += 2; // unknown 2 byte data, not used?

                StaticItem item = new StaticItem(iTileID);
                item.Position.Set(X * 8 + iX, Y * 8 + iY, iTileZ);
                item.Z = iTileZ;
            }
        }
    }
}
