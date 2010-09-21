/***************************************************************************
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
            _cells = new MapCell[ClientVars.MapSizeInMemory * ClientVars.MapSizeInMemory];
        }

        public int Height
        {
            get { return _tileMatrix.Height; }
        }
        public int Width
        {
            get { return _tileMatrix.Width; }
        }

        public int GetAverageZ(int top, int left, int right, int bottom, ref int low, ref int high)
        {
            low = high;
            if (left < low)
                low = left;
            if (right < low)
                low = right;
            if (bottom < low)
                low = bottom;

            high = top;
            if (left > high)
                high = left;
            if (right > high)
                high = right;
            if (bottom > high)
                high = bottom;

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

        int m_LoadedCellThisFrame = 0;
        const int m_MaxCellsLoadedPerFrame = 2;

        public MapCell GetMapCell(int x, int y, bool load)
        {
            if (x < 0) x += this.Width;
            if (x >= this.Width) x -= this.Width;
            if (y < 0) y += this.Height;
            if (y >= this.Height) y -= this.Height;

            int index = ((x >> 3) % ClientVars.MapSizeInMemory) + (((y >> 3) % ClientVars.MapSizeInMemory) * ClientVars.MapSizeInMemory);
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

        private int GetTileZ(int x, int y)
        {
            MapTile t = GetMapTile(x, y, false);
            return
                (t == null) ?
                (sbyte)_tileMatrix.GetLandTile(x, y)[2] :
                t.GroundTile.Z;
        }

        public void UpdateSurroundings(MapObjectGround g)
        {
            int x = (int)g.Position.X;
            int y = (int)g.Position.Y;

            int[] zValues = new int[16];

            for (int iy = 0; iy < 4; iy++)
            {
                for (int ix = 0; ix < 4; ix++)
                {
                    zValues[ix + iy * 4] = GetTileZ(x + ix - 1, y + iy - 1);
                }
            }

            g.Surroundings = new Surroundings(
                zValues[2 + 2 * 4],
                zValues[2 + 1 * 4],
                zValues[1 + 2 * 4]);
            
            if (!g.IsFlat)
            {
                int low = 0, high = 0, sort = 0;
                sort = GetAverageZ(g.Z, g.Surroundings.South, g.Surroundings.East, g.Surroundings.Down, ref low, ref high);
                if (low != g.SortZ)
                {
                    g.SortZ = low;
                    GetMapTile(x, y, false).Resort();
                }
            }
            
            g.CalculateNormals(
                zValues[0 + 1 * 4],
                zValues[0 + 2 * 4],
                zValues[1 + 0 * 4],
                zValues[2 + 0 * 4],
                zValues[1 + 3 * 4],
                zValues[2 + 3 * 4],
                zValues[3 + 1 * 4],
                zValues[3 + 2 * 4]);

            g.MustUpdateSurroundings = false;
        }
    }
}