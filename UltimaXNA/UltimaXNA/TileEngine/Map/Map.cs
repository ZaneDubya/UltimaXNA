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
        TileMatrix _tileMatrix;
        int _x, _y;
        bool _loadAllNearbyCells = false; // set when a map is first loaded.
        public bool LoadEverything_Override = false;

        int _index = -1;
        public int Index { get { return _index; } }

        public Map(int index)
        {
            _index = index;
            _loadAllNearbyCells = true;
            _tileMatrix = new TileMatrix(_index, _index);
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

        public void GetAverageZ(int x, int y, ref int z, ref int avg, ref int top)
        {
            int zTop, zLeft, zRight, zBottom;

            zTop = GetTileZ(x, y);  // GetMapTile(x, y, false).GroundTile.Z;
            zLeft = GetTileZ(x, y + 1); // GetMapTile(x, y + 1, false).GroundTile.Z;
            zRight = GetTileZ(x + 1, y); // GetMapTile(x + 1, y, false).GroundTile.Z;
            zBottom = GetTileZ(x + 1, y + 1); // GetMapTile(x + 1, y + 1, false).GroundTile.Z;

            z = zTop;
            if (zLeft < z)
                z = zLeft;
            if (zRight < z)
                z = zRight;
            if (zBottom < z)
                z = zBottom;

            top = zTop;
            if (zLeft > top)
                top = zLeft;
            if (zRight > top)
                top = zRight;
            if (zBottom > top)
                top = zBottom;

            if (Math.Abs(zTop - zBottom) > Math.Abs(zLeft - zRight))
                avg = FloorAverage(zLeft, zRight);
            else
                avg = FloorAverage(zTop, zBottom);
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

        // This pulls a tile from the TileMatrix.
        public Tile GetLandTile(int x, int y)
        {
            return _tileMatrix.GetLandTile(x, y);
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
            return c.m_Tiles[x % 8 + ((y % 8) << 3)];
        }

        public void Update(int centerX, int centerY)
        {
            if (_x != centerX || _y != centerY)
            {
                _x = centerX;
                _y = centerY;
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
                _tileMatrix.GetLandTile(x, y).Z :
                t.GroundTile.Z;
        }

        public void UpdateSurroundings(MapObjectGround g)
        {
            int x = (int)g.Position.X;
            int y = (int)g.Position.Y;

            int[] zValues = new int[16]; // _matrix.GetElevations(x - 1, y - 1, 4, 4);

            for (int iy = 0; iy < 4; iy++)
            {
                for (int ix = 0; ix < 4; ix++)
                {
                    MapTile t = GetMapTile(x + ix, y + iy, false);
                    zValues[ix + iy * 4] = GetTileZ(x + ix - 1, y + iy - 1);
                }
            }

            g.Surroundings = new Surroundings(
                zValues[2 + 2 * 4],
                zValues[2 + 1 * 4],
                zValues[1 + 2 * 4]);
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