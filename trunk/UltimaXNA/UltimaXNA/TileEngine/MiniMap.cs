/***************************************************************************
 *   MiniMap.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MiniMap
    {
        private Texture2D _texture;
        private GraphicsDevice _graphics;
        private int _lastUpdateTicker;

        public Texture2D Texture(Map map, int renderBeginX, int renderBeginY)
        {
            update(map, renderBeginX, renderBeginY);
            return _texture;
        }

        public MiniMap(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }

        private unsafe void update(Map map, int renderBeginX, int renderBeginY)
        {
            if ((map.UpdateTicker != _lastUpdateTicker) || (_texture == null))
            {
                int size = map.GameSize * 2;
                _lastUpdateTicker = map.UpdateTicker;
                _texture = new Texture2D(_graphics, size, size, 1, TextureUsage.None, SurfaceFormat.Bgra5551);
                ushort[] data = new ushort[size * size];
                fixed (ushort* pData = data)
                {
                    for (int y = 0; y < map.GameSize; y++)
                    {
                        ushort* cur = pData + ((size /2 - 1) + (size - 1) * y);
                        for (int x = 0; x < map.GameSize; x++)
                        {
                            MapCell m = map.GetMapCell(renderBeginX + x, renderBeginY + y);
                            int i;
                            for (i = m.Objects.Count - 1; i > 0; i--)
                            {
                                if (m.Objects[i] is MapObjectStatic)
                                {
                                    *cur++ = (ushort)(Data.Radarcol.Colors[m.Objects[i].ItemID] | 0x8000);
                                    *cur = (ushort)(Data.Radarcol.Colors[m.Objects[i].ItemID] | 0x8000);
                                    cur += size;
                                    break;
                                }
                            }
                            if (i == 0)
                            {
                                *cur++ = (ushort)(Data.Radarcol.Colors[m.GroundTile.ItemID] | 0x8000);
                                *cur = (ushort)(Data.Radarcol.Colors[m.Objects[i].ItemID] | 0x8000);
                                cur += size;
                            }
                        }
                    }
                }
                _texture.SetData<ushort>(data);
            }
        }
    }
}
