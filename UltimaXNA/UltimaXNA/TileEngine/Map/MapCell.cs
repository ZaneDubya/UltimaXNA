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
    public sealed class MapCell : IPoint2D
    {
        public MapTile[] _Tiles = new MapTile[64];
        private Map _map;
        private TileMatrixRaw _matrix;
        private bool _isLoaded = false;

        #region XY
        int _x, _y;
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        #endregion

        public MapCell(Map map, TileMatrixRaw matrix, int x, int y)
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
            byte[] tiledata = _matrix.GetLandBlock(_x >> 3, _y >> 3);
            int index = 0;
            for (int i = 0; i < 64; i++)
            {
                int ix = _x + i % 8;
                int iy = _y + (i >> 3);
                // int iz = 0, itop = 255, iavg = 0;

                int iTileID = tiledata[index++] + (tiledata[index++] << 8);
                int iTileZ = (sbyte)tiledata[index++];

                MapObjectGround ground = 
                    new MapObjectGround(iTileID, 
                        new Position3D(ix, iy, iTileZ));
                ground.SortZ = iTileZ;
                MapTile tile = new MapTile(ix, iy);
                tile.Add(ground);
                _Tiles[tile.X % 8 + (tile.Y % 8) * 8] = tile;
            }
        }

        void loadStatics()
        {
            byte[] tiledata = _matrix.GetStaticBlock(_x >> 3, _y >> 3);
            if (tiledata == null)
                return;
            int count = tiledata.Length / 7;
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                int iTileID = tiledata[index++] + (tiledata[index++] << 8);
                int iTileX = tiledata[index++];
                int iTileY = tiledata[index++];
                int iTileZ = (sbyte)tiledata[index++];
                index += 2; // unknown 2 byte data, not used.
                MapTile tile = _Tiles[iTileX + (iTileY << 3)];
                tile.Add(
                    new MapObjectStatic(iTileID, i, 
                        new Position3D(iTileX + _x, iTileY + _y, iTileZ)));
            }
        }

        public MapTile Tile(int x, int y)
        {
            return _Tiles[x % 8 + (y % 8) * 8];
        }

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
                            List<MapObject> o = _Tiles[tileindex].GetSortedObjects();
                            for (int j = o.Count - 1; j >= 0; j--)
                            {
                                if (o[j] is MapObjectStatic || o[j] is MapObjectGround)
                                {
                                    radar = _Tiles[tileindex].Objects[j].ItemID;
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
    }
}
