/***************************************************************************
 *   MapCell.cs
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
using UltimaXNA.Data;
using UltimaXNA.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.TileEngine
{
    public sealed class MapCell : Data.IPoint2D
    {
        public MapTile[] m_Tiles = new MapTile[64];
        Map _map;
        Data.TileMatrix _matrix;
        bool _isLoaded = false;

        #region XY
        int _x, _y;
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        #endregion

        public void WriteRadarColors(uint[] buffer, int x, int y)
        {
            unsafe
            {
                fixed (uint* pData = buffer)
                {
                    int radar = 0;
                    int tileindex = 0;

                    for (int iy = 0; iy < 8; iy++)
                    {
                        uint* pDataRef = pData + x + ((y + iy) * 64);
                        for (int ix = 0; ix < 8; ix++)
                        {
                            List<MapObject> o = m_Tiles[tileindex].GetSortedObjects();
                            for (int j = o.Count - 1; j >= 0; j--)
                            {
                                if (o[j] is MapObjectStatic || o[j] is MapObjectGround)
                                {
                                    radar = m_Tiles[tileindex].Objects[j].ItemID;
                                    break;
                                }
                            }
                            *pDataRef++ = Data.Radarcol.Colors[radar];
                            tileindex++;
                        }
                    }
                }
            }
            // texture.SetData<uint>(0, new Rectangle(x, y, 8, 8), data, 0, 64, SetDataOptions.None);
        }

        public MapCell(Map map, Data.TileMatrix matrix, int x, int y)
        {
            _map = map;
            _matrix = matrix;
            _x = x;
            _y = y;
        }

        public void Load()
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                loadGround();
                loadStatics();
            }
        }

        void loadGround()
        {
            Data.Tile[] _tiles;
            _tiles = _matrix.GetLandBlock(_x >> 3, _y >> 3);

            for (int i = 0; i < 64; i++)
            {
                int ix = _x + i % 8;
                int iy = _y + (i >> 3);
                int iz = 0, itop = 255, iavg = 0;
                _map.GetAverageZ(ix, iy, ref iz, ref iavg, ref itop);
                MapObjectGround ground = new MapObjectGround(_tiles[i], new Position3D(ix, iy, _tiles[i].Z));
                ground.SortZ = iavg;
                MapTile tile = new MapTile(ix, iy);
                tile.Add(ground);
                m_Tiles[tile.X % 8 + (tile.Y % 8) * 8] = tile;
            }
        }

        void loadStatics()
        {
            Data.StaticTile[][][] _statics;
            _statics = _matrix.GetStaticBlock(_x >> 3, _y >> 3);
            for (int i = 0; i < 64; i++)
            {
                int ix = _x + i % 8;
                int iy = _y + (i >> 3);
                MapTile tile = m_Tiles[ix % 8 + (iy % 8) * 8];
                foreach (Data.StaticTile s in _statics[ix % 8][iy % 8])
                {
                    tile.Add(new MapObjectStatic(s, i, new Position3D(ix, iy, s.Z)));
                }
            }
        }

        public MapTile Tile(int x, int y)
        {
            return m_Tiles[x % 8 + (y % 8) * 8];
        }
    }
}
