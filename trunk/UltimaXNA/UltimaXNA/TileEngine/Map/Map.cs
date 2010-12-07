﻿/***************************************************************************
 *   Map.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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
using UltimaXNA.Entities;
using UltimaXNA.Data;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.TileEngine
{
    public sealed class Map
    {
        public int UpdateTicker;
        MapCell[] _cells;
        TileMatrixRaw _tileMatrix;
        int _x, _y;
        bool _loadAllNearbyCells = false; // set when a map is first loaded.
        bool _mustResetMap = false;
        public bool LoadEverything_Override = false;

        int m_LoadedCellThisFrame = 0;
        const int m_MaxCellsLoadedPerFrame = 2;
        int m_MapCellsDrawRadius = 0;
        int m_MapCellsInMemory = 0;

        int _index = -1;
        public int Index { get { return _index; } }

        public Map(int index)
        {
            _index = index;
            _mustResetMap = true;
        }

        private void resetMap()
        {
            _loadAllNearbyCells = true;
            _tileMatrix = new TileMatrixRaw(_index, _index);
            Height = _tileMatrix.Height;
            Width = _tileMatrix.Width;
            m_MapCellsInMemory = ClientVars.MapCellsInMemory;
            m_MapCellsDrawRadius = ((m_MapCellsInMemory / 2) * 8);
            _cells = new MapCell[m_MapCellsInMemory * m_MapCellsInMemory];
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
                GetTileZ(x, y),
                GetTileZ(x, y + 1),
                GetTileZ(x + 1, y),
                GetTileZ(x + 1, y + 1),
                ref low, ref top);
        }

        private static int FloorAverage(int a, int b)
        {
            int v = a + b;

            if (v < 0)
                --v;

            return (v / 2);
        }

        private int GetKey(MapCell cell)
        {
            return GetKey(cell.X, cell.Y);
        }

        private int GetKey(int x, int y)
        {
            return (x << 18) + y;
        }

        public MapCell GetMapCell(int x, int y, bool load)
        {
            if (x < 0) x += this.Width;
            if (x >= this.Width) x -= this.Width;
            if (y < 0) y += this.Height;
            if (y >= this.Height) y -= this.Height;

            if (!load)
            {
                if (Math.Abs(x - _x) > m_MapCellsDrawRadius ||
                    Math.Abs(y - _y) > m_MapCellsDrawRadius)
                {
                    return null;
                }
            }

            int index = ((x >> 3) % m_MapCellsInMemory) + (((y >> 3) % m_MapCellsInMemory) * m_MapCellsInMemory);
            MapCell c = _cells[index];
            if (c == null || 
                (((x - c.X) & 0xFFF8) != 0) ||
                (((y - c.Y) & 0xFFF8) != 0))
            {
                if (load && (m_LoadedCellThisFrame < m_MaxCellsLoadedPerFrame || LoadEverything_Override))
                {
                    m_LoadedCellThisFrame++;
                    c = _cells[index] = new MapCell(this, _tileMatrix, x - x % 8, y - y % 8);
                    c.Load();
                }
                else
                {
                    _cells[index] = null;
                    return null;
                }
            }
            return c;
        }

        public MapTile GetMapTile(int x, int y, bool load)
        {
            MapCell c = GetMapCell(x, y, load);
            if (c == null)
                return null;
            return c._Tiles[x % 8 + ((y % 8) << 3)];
        }

        public void Update(int centerX, int centerY)
        {
            if (_x != centerX || _y != centerY)
            {
                if ((Math.Abs(_x - centerX) > 2) || (Math.Abs(_y - centerY) > 2))
                {
                    _mustResetMap = true;
                }
                _x = centerX;
                _y = centerY;
            }

            if (_mustResetMap)
            {
                resetMap();
                _mustResetMap = false;
            }

            if (_loadAllNearbyCells)
            {
                _loadAllNearbyCells = true;
                m_LoadedCellThisFrame = int.MinValue;
            }
            else
                m_LoadedCellThisFrame = 0;
        }

        public int GetTileZ(int x, int y)
        {
            MapTile t = GetMapTile(x, y, false);
            return
                (t == null) ?
                (sbyte)_tileMatrix.GetLandTile(x, y)[2] :
                t.GroundTile.Z;
        }
    }
}