/***************************************************************************
 *   Map.cs
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
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
using UltimaXNA.Entity;
using UltimaXNA.UltimaData;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld.View;
#endregion

namespace UltimaXNA.UltimaWorld
{
    public sealed class Map
    {
        public int UpdateTicker;
        MapTile[] m_tiles;
        TileMatrixRaw m_tileMatrix;
        int m_x, m_y;
        bool m_loadAllNearbyCells = false; // set when a map is first loaded.
        bool m_mustResetMap = false;
        public bool LoadEverything_Override = false;

        int m_numCellsLoadedThisFrame = 0;
        const int MaxCellsLoadedPerFrame = 200;
        int m_MapTilesDrawRadius = 0;
        int m_MapTilesInMemory = 0;

        int m_index = -1;
        public int Index { get { return m_index; } }

        public Map(int index)
        {
            m_index = index;
            m_mustResetMap = true;
        }

        private void resetMap()
        {
            m_loadAllNearbyCells = true;
            m_tileMatrix = new TileMatrixRaw(m_index, m_index);
            Height = m_tileMatrix.Height;
            Width = m_tileMatrix.Width;
            m_MapTilesInMemory = UltimaVars.EngineVars.MapCellsInMemory * 8;
            m_MapTilesDrawRadius = ((m_MapTilesInMemory / 2));

            m_tiles = new MapTile[m_MapTilesInMemory * m_MapTilesInMemory];
        }

        public int Height;
        public int Width;

        public int GetAverageZ(int top, int left, int right, int bottom, ref int low, ref int high)
        {
            high = top;
            if (left > high)
                high = left;
            if (right > high)
                high = right;
            if (bottom > high)
                high = bottom;

            low = high;
            if (left < low)
                low = left;
            if (right < low)
                low = right;
            if (bottom < low)
                low = bottom;

            if (Math.Abs(top - bottom) > Math.Abs(left - right))
                return FloorAverage(left, right);
            else
                return FloorAverage(top, bottom);
        }

        public int GetAverageZ(int x, int y, ref int low, ref int top)
        {
            return GetAverageZ(
                (int)GetTileZ(x, y),
                (int)GetTileZ(x, y + 1),
                (int)GetTileZ(x + 1, y),
                (int)GetTileZ(x + 1, y + 1),
                ref low, ref top);
        }

        private static int FloorAverage(int a, int b)
        {
            int v = a + b;

            if (v < 0)
                --v;

            return (v / 2);
        }

        public MapTile GetMapTile(int x, int y, bool load)
        {
            int idx = (x % m_MapTilesInMemory) + (y % m_MapTilesInMemory) * m_MapTilesInMemory;
            if (idx < 0)
                return null;
            MapTile t = m_tiles[idx];
            if (t == null || (x != t.X) || (y != t.Y))
            {
                if (!load && (Math.Abs(x - m_x) > m_MapTilesDrawRadius || Math.Abs(y - m_y) > m_MapTilesDrawRadius))
                {
                    return null;
                }
                else if (load && (m_numCellsLoadedThisFrame < MaxCellsLoadedPerFrame || LoadEverything_Override))
                {
                    m_numCellsLoadedThisFrame++;
                    loadMapCellIntotiles(x - x % 8, y - y % 8);
                }
                else
                {
                    m_tiles[idx] = null;
                }
            }

            return m_tiles[idx];
        }

        private void loadMapCellIntotiles(int x, int y)
        {
            // get data from the tile Matrix
            byte[] groundData = m_tileMatrix.GetLandBlock(x >> 3, y >> 3);
            byte[] staticsData = m_tileMatrix.GetStaticBlock(x >> 3, y >> 3);
            int[] indexes = new int[64];
            int thisindex = x % m_MapTilesInMemory + (y % m_MapTilesInMemory) * m_MapTilesInMemory;
            for (int i = 0; i < 64; )
            {
                indexes[i++] = thisindex++;
                if ((i % 8) == 0)
                    thisindex += (m_MapTilesInMemory - 8);
            }

            // load the ground data into the tiles.
            int index = 0;
            for (int i = 0; i < 64; i++)
            {
                int iTileID = groundData[index++] + (groundData[index++] << 8);
                int iTileZ = (sbyte)groundData[index++];

                Ground ground = new Ground(iTileID, x + i % 8, y + (i >> 3), iTileZ);
                MapTile tile = new MapTile(ground);
                m_tiles[indexes[i]] = tile;
            }

            // load the statics data into the tiles
            int countStatics = staticsData.Length / 7;
            index = 0;
            for (int i = 0; i < countStatics; i++)
            {
                int iTileID = staticsData[index++] + (staticsData[index++] << 8);
                int iTileIndex = staticsData[index++] + (staticsData[index++] * 8);
                int iTileZ = (sbyte)staticsData[index++];
                index += 2; // unknown 2 byte data, not used.
                MapTile tile = m_tiles[indexes[iTileIndex]];
                tile.AddMapObject(new MapObjectStatic(iTileID, i, new Position3D(tile.X, tile.Y, iTileZ)));
            }
        }

        public void Update(int centerX, int centerY)
        {
            if (m_x != centerX || m_y != centerY)
            {
                if ((Math.Abs(m_x - centerX) > 2) || (Math.Abs(m_y - centerY) > 2))
                {
                    m_mustResetMap = true;
                }
                m_x = centerX;
                m_y = centerY;
            }

            if (m_mustResetMap)
            {
                resetMap();
                m_mustResetMap = false;
            }

            if (m_loadAllNearbyCells)
            {
                m_loadAllNearbyCells = true;
                m_numCellsLoadedThisFrame = int.MinValue;
            }
            else
                m_numCellsLoadedThisFrame = 0;
        }

        public float GetTileZ(int x, int y)
        {
            MapTile t = GetMapTile(x, y, false);
            if (t != null)
                return t.Ground.Z;
            else
            {
                int tileID, alt;
                m_tileMatrix.GetLandTile(x, y, out tileID, out alt);
                return alt;
            }
        }
    }
}