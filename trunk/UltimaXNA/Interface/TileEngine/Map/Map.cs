/***************************************************************************
 *   Map.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Data;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Interface.TileEngine
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
        const int MaxCellsLoadedPerFrame = 200;
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
            _MapTilesInMemory = ClientVars.EngineVars.MapCellsInMemory * 8;
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
            int idx = (x % _MapTilesInMemory) + (y % _MapTilesInMemory) * _MapTilesInMemory;
            if (idx < 0)
                return null;
            MapTile t = _tiles[idx];
            if (t == null || (x != t.X) || (y != t.Y))
            {
                if (!load && (Math.Abs(x - _x) > _MapTilesDrawRadius || Math.Abs(y - _y) > _MapTilesDrawRadius))
                {
                    return null;
                }
                else if (load && (_numCellsLoadedThisFrame < MaxCellsLoadedPerFrame || LoadEverything_Override))
                {
                    _numCellsLoadedThisFrame++;
                    loadMapCellIntotiles(x - x % 8, y - y % 8);
                }
                else
                {
                    _tiles[idx] = null;
                }
            }

            return _tiles[idx];
        }

        private void loadMapCellIntotiles(int x, int y)
        {
            // get data from the tile Matrix
            byte[] groundData = _tileMatrix.GetLandBlock(x >> 3, y >> 3);
            byte[] staticsData = _tileMatrix.GetStaticBlock(x >> 3, y >> 3);
            int[] indexes = new int[64];
            int thisindex = x % _MapTilesInMemory + (y % _MapTilesInMemory) * _MapTilesInMemory;
            for (int i = 0; i < 64; )
            {
                indexes[i++] = thisindex++;
                if ((i % 8) == 0)
                    thisindex += (_MapTilesInMemory - 8);
            }

            // load the ground data into the tiles.
            int index = 0;
            for (int i = 0; i < 64; i++)
            {
                int iTileID = groundData[index++] + (groundData[index++] << 8);
                int iTileZ = (sbyte)groundData[index++];

                MapObjectGround ground =
                    new MapObjectGround(iTileID, new Position3D(x + i % 8, y + (i >> 3), iTileZ));
                MapTile tile = new MapTile(ground.Position.X, ground.Position.Y);
                tile.AddMapObject(ground);
                _tiles[indexes[i]] = tile;
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
                MapTile tile = _tiles[indexes[iTileIndex]];
                tile.AddMapObject(new MapObjectStatic(iTileID, i, new Position3D(tile.X, tile.Y, iTileZ)));
            }

            // now update this batch of tiles - sets their normals and surroundings as necessary.
            for (int i = 0; i < 64; i++)
            {
                _tiles[indexes[i]].GroundTile.UpdateSurroundingsIfNecessary(this);
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

        public float GetTileZ(int x, int y)
        {
            MapTile t = GetMapTile(x, y, false);
            if (t != null)
                return t.GroundTile.Z;
            else
            {
                int tileID, alt;
                _tileMatrix.GetLandTile(x, y, out tileID, out alt);
                return alt;
            }
        }
    }
}