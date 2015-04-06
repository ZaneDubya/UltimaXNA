/***************************************************************************
 *   MiniMap.cs
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

namespace UltimaXNA.UltimaWorld.Views
{
    // N.B.
    // This code is currently not used by the engine and may well be nonfunctional.
    public class MiniMap
    {

    }
    /*
    class WorldTexture
    {
        Texture2D m_texture;
        bool m_loaded = false;
        Map m_map;
        int m_x, m_y;

        public WorldTexture(GraphicsDevice graphics, Map map, int x, int y)
        {
            m_texture = new Texture2D(graphics, 64, 64);
            m_map = map;
            m_x = x;
            m_y = y;
        }

        public Texture2D Texture()
        {
            if (!m_loaded)
            {
                m_loaded = true;
                uint[] buffer = new uint[64 * 64];

                m_map.LoadEverything_Override = true;

                for (int i = 0; i < 64; i++)
                {
                    int ix = m_x + i % 8;
                    int iy = m_y + i / 8;
                    // MapCell c = m_map.GetMapCell(ix << 3, iy << 3, true);
                    // c.WriteRadarColors(buffer, i % 8 << 3, i / 8 << 3);
                }

                m_map.LoadEverything_Override = false;

                m_texture.SetData<uint>(buffer);
            }
            return m_texture;
        }
    }

    public class MiniMap_DEPRECIATED
    {
        private Texture2D m_texture;
        private GraphicsDevice m_graphics;
        private int m_lastUpdateTicker;

        public Texture2D Texture(Map map, int renderBeginX, int renderBeginY)
        {
            update(map, renderBeginX, renderBeginY);
            return m_texture;
        }

        public MiniMap_DEPRECIATED(GraphicsDevice graphics)
        {
            m_graphics = graphics;
        }

        private unsafe void update(Map map, int renderBeginX, int renderBeginY)
        {
            if ((map.UpdateTicker != m_lastUpdateTicker) || (m_texture == null))
            {
                int size = UltimaVars.EngineVars.RenderSize * 2;
                m_lastUpdateTicker = map.UpdateTicker;
                m_texture = new Texture2D(m_graphics, size, size, false, SurfaceFormat.Bgra5551);
                ushort[] data = new ushort[size * size];
                fixed (ushort* pData = data)
                {
                    for (int y = 0; y < UltimaVars.EngineVars.RenderSize; y++)
                    {
                        ushort* cur = pData + ((size /2 - 1) + (size - 1) * y);
                        for (int x = 0; x < UltimaVars.EngineVars.RenderSize; x++)
                        {
                            MapTile m = map.GetMapTile(renderBeginX + x, renderBeginY + y, true);
                            List<AMapObject> o = m.Items;
                            int i;
                            for (i = o.Count - 1; i > 0; i--)
                            {
                                if (o[i] is MapObjectStatic)
                                {
                                    *cur++ = (ushort)(UltimaData.RadarColorData.Colors[o[i].ItemID] | 0x8000);
                                    *cur = (ushort)(UltimaData.RadarColorData.Colors[o[i].ItemID] | 0x8000);
                                    cur += size;
                                    break;
                                }
                            }
                            if (i == 0)
                            {
                                *cur++ = (ushort)(UltimaData.RadarColorData.Colors[m.GroundTile.ItemID] | 0x8000);
                                *cur = (ushort)(UltimaData.RadarColorData.Colors[o[i].ItemID] | 0x8000);
                                cur += size;
                            }
                        }
                    }
                }
                m_texture.SetData<ushort>(data);
            }
        }
    }
    */
}
