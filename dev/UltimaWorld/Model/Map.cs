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
using Microsoft.Xna.Framework;
using System;
using UltimaXNA.Entity;
using UltimaXNA.UltimaData;

#endregion

namespace UltimaXNA.UltimaWorld.Model
{
    public sealed class Map
    {
        private MapBlock[] m_Blocks;
        private TileMatrixRaw m_MapData;
        private Point m_Center = new Point(int.MinValue, int.MinValue);

        public int Index;
        public int Height, Width;

        // Any mobile / item beyond this range is removed from the client. RunUO's range is 24 tiles, which would equal 3 cells.
        // We keep 4 cells in memory to allow for drawing further, and also as a safety precaution - don't want to unload an 
        // entity at the edge of what we keep in memory just because of being slightly out of sync with the server.
        private const int c_CellsInMemory = 4;
        private const int c_CellsInMemorySpan = c_CellsInMemory * 2 + 1;

        public Map(int index)
        {
            Index = index;

            m_MapData = new TileMatrixRaw(Index, Index);
            Height = m_MapData.Height;
            Width = m_MapData.Width;

            m_Blocks = new MapBlock[c_CellsInMemorySpan * c_CellsInMemorySpan];
        }

        public Point CenterPosition
        {
            get { return m_Center; }
            set
            {
                if (value != m_Center)
                {
                    m_Center = value;
                }

                InternalCheckCellsInMemory();
            }
        }

        public MapTile GetMapTile(int x, int y)
        {
            int cellX = x / 8, cellY = y / 8;
            int cellIndex = (cellY % c_CellsInMemorySpan) * c_CellsInMemorySpan + (cellX % c_CellsInMemorySpan);

            MapBlock cell = m_Blocks[cellIndex];
            if (cell == null)
                return null;
            return cell.Tiles[(y % 8) * 8 + (x % 8)];
        }

        private void InternalCheckCellsInMemory()
        {
            for (int y = -c_CellsInMemory; y <= c_CellsInMemory; y++)
            {
                int cellY = (CenterPosition.Y / 8) + y;
                if (cellY < 0)
                    cellY += Height / 8;
                for (int x = -c_CellsInMemory; x <= c_CellsInMemory; x++)
                {
                    int cellX = (CenterPosition.X / 8) + x;
                    if (cellX < 0)
                        cellX += Width / 8;

                    int cellIndex = (cellY % c_CellsInMemorySpan) * c_CellsInMemorySpan + cellX % c_CellsInMemorySpan;
                    if (m_Blocks[cellIndex] == null || m_Blocks[cellIndex].X != cellX || m_Blocks[cellIndex].Y != cellY)
                    {
                        if (m_Blocks[cellIndex] != null)
                            m_Blocks[cellIndex].Dispose();
                        m_Blocks[cellIndex] = new MapBlock(cellX, cellY);
                        m_Blocks[cellIndex].LoadTiles(m_MapData);
                    }
                }
            }
        }

        public float GetTileZ(int x, int y)
        {
            MapTile t = GetMapTile(x, y);
            if (t != null)
                return t.Ground.Z;
            else
            {
                int tileID, alt;
                m_MapData.GetLandTile(x, y, out tileID, out alt);
                return alt;
            }
        }

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
    }
}