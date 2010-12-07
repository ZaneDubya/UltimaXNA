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
        MapTile[] _tiles;
        TileMatrixRaw _tileMatrix;
        int _x, _y;
        bool _loadAllNearbyCells = false; // set when a map is first loaded.
        bool _mustResetMap = false;
        public bool LoadEverything_Override = false;

        int _numCellsLoadedThisFrame = 0;
        const int MaxCellsLoadedPerFrame = 2;
        int _MapTilesDrawRadius = 0;
        int _MapTilesInMemory = 0;

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
            _MapTilesInMemory = ClientVars.MapCellsInMemory * 8;
            _MapTilesDrawRadius = ((_MapTilesInMemory / 2));

            _tiles = new MapTile[_MapTilesInMemory * _MapTilesInMemory];
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

        public MapTile GetMapTile(int x, int y, bool load)
        {
            if (x < 0) x += this.Width;
            if (x >= this.Width) x -= this.Width;
            if (y < 0) y += this.Height;
            if (y >= this.Height) y -= this.Height;

            if (!load)
            {
                if (Math.Abs(x - _x) > _MapTilesDrawRadius ||
                    Math.Abs(y - _y) > _MapTilesDrawRadius)
                {
                    return null;
                }
            }

            int idx = (x % _MapTilesInMemory) + (y % _MapTilesInMemory) * _MapTilesInMemory;
            MapTile t = _tiles[idx];
            if (t == null || (x != t.X) || (y != t.Y))
            {
                if (load && (_numCellsLoadedThisFrame < MaxCellsLoadedPerFrame || LoadEverything_Override))
                {
                    _numCellsLoadedThisFrame++;
                    loadMapCellIntotiles(x - x % 8, y - y % 8);
                }
                else
                {
                    _tiles[idx] = null;
                }
            }
            if (_tiles[idx] == null)
                return null;
            return _tiles[idx];
        }

        private void loadMapCellIntotiles(int x, int y)
        {
            MapCell c = new MapCell(this, _tileMatrix, x, y);
            c.Load();
            // load the cell's tiles into the map's tile matrix ...
            for (int iy = 0; iy < 8; iy++)
            {
                int destidx = (c.X % _MapTilesInMemory) + ((iy + c.Y) % _MapTilesInMemory) * _MapTilesInMemory;
                for (int ix = 0; ix < 8; ix++)
                {
                    _tiles[destidx++] = c._Tiles[iy * 8 + ix];
                }
            }
            // now update this batch of tiles - sets their normals and surroundings as necessary.
            for (int iy = 0; iy < 8; iy++)
            {
                int destidx = (c.X % _MapTilesInMemory) + ((iy + c.Y) % _MapTilesInMemory) * _MapTilesInMemory;
                for (int ix = 0; ix < 8; ix++)
                {
                    _tiles[destidx++].GroundTile.UpdateSurroundingsIfNecessary(this);
                }
            }

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
                _numCellsLoadedThisFrame = int.MinValue;
            }
            else
                _numCellsLoadedThisFrame = 0;
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