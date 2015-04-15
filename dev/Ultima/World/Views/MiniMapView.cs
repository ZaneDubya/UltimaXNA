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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Maps;
using Microsoft.Xna.Framework;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;
#endregion

namespace UltimaXNA.Ultima.World.Views
{
    public class MiniMapTexture
    {
        private uint[] m_TextureData;
        private uint[] m_BlockColors;
        private Texture2D m_Texture;
        private SpriteBatch3D m_SpriteBatch;
        private bool m_MustRedrawEntireTexture;
        private uint m_LastCenterCellX, m_LastCenterCellY;

        private const uint Stride = 256;

        public void Initialize()
        {
            m_SpriteBatch = UltimaServices.GetService<SpriteBatch3D>();
            m_Texture = new Texture2D(m_SpriteBatch.GraphicsDevice, (int)Stride, (int)Stride);
            m_TextureData = new uint[Stride * Stride];
            m_BlockColors = new uint[64];
            m_MustRedrawEntireTexture = true;
        }

        public void Draw(Map map, Position3D center)
        {
            uint centerCellX = (uint)center.X / 8, centerCellY = (uint)center.Y / 8;
            int centerDiffX = (int)(m_LastCenterCellX - centerCellX), centerDiffY = (int)(m_LastCenterCellY - center.Y);
            if (centerDiffX < -1 || centerDiffX > 1 || centerDiffY < -1 || centerDiffY > 1)
                m_MustRedrawEntireTexture = true;

            uint mapCellsWidth = (uint)map.Width / 8;
            uint mapCellsHeight = (uint)map.Height / 8;

            if (m_MustRedrawEntireTexture)
            {
                for (uint col = 0; col < 16; col++)
                {
                    for (int row = 0; row < 16; row++)
                    {

                    }
                }

                for (uint y = (uint)(centerCellY - 4) % mapCellsWidth; y <= centerCellY + 4; y++)
                {
                    for (uint x = centerCellX - 4; x <= centerCellX + 4; x++)
                    {
                        InternalDrawMapBlock(map, x, y);
                    }
                }
                m_Texture.SetData<uint>(m_TextureData);
            }
            else if (centerDiffX != 0 || centerDiffY != 0)
            {
                // draw just a row or two.
            }
            m_SpriteBatch.DrawSimple(m_Texture, new Rectangle(32, 32, 256, 256), Vector2.Zero);
        }

        private void InternalDrawMapBlock(Map map, uint cellx, uint celly)
        {
            MapBlock block = map.GetMapBlock(cellx, celly);
            if (block == null)
                return;

            // get the colors for this block
            for (uint tile = 0; tile < 64; tile++)
            {
                uint color = 0xffff00ff;
                // get the topmost static item or ground
                int eIndex = block.Tiles[tile].Entities.Count - 1;
                while (eIndex >= 0)
                {
                    AEntity e = block.Tiles[tile].Entities[eIndex];
                    if (e is Ground)
                    {
                        color = IO.RadarColorData.Colors[(e as Ground).LandDataID];
                        break;
                    }
                    else if (e is StaticItem)
                    {
                        color = IO.RadarColorData.Colors[(e as StaticItem).ItemID + 0x4000];
                        break;
                    }
                    eIndex--;
                }
                m_BlockColors[tile] = color;
            }

            uint cellX32 = cellx % 32, cellY32 = celly % 32;

            // now draw the block
            if (cellX32 == 0 && cellY32 == 0)
            {
                // draw the block split out over four corners of the texture.
            }
            else if (cellX32 + cellY32 == 32)
            {
                // draw the block split on the top and bottom of the texture.
            }
            else if (cellX32 == cellY32)
            {
                // draw the block split on the left and right side of the texture.
            }
            else
            {
                // draw the block normally.
                int blockindex = 0;
                for (uint tiley = 0; tiley < 8; tiley++)
                {
                    uint drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                    uint drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                    for (uint tilex = 0; tilex < 8; tilex++)
                    {
                        m_TextureData[drawy * Stride + drawx] = m_BlockColors[blockindex];
                        m_TextureData[drawy * Stride + Stride + drawx] = m_BlockColors[blockindex++];
                        drawx = (drawx + 1) % Stride;
                        drawy = (drawy + 1) % Stride;
                    }
                }
            }
        }
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
                int size = EngineVars.RenderSize * 2;
                m_lastUpdateTicker = map.UpdateTicker;
                m_texture = new Texture2D(m_graphics, size, size, false, SurfaceFormat.Bgra5551);
                ushort[] data = new ushort[size * size];
                fixed (ushort* pData = data)
                {
                    for (int y = 0; y < EngineVars.RenderSize; y++)
                    {
                        ushort* cur = pData + ((size /2 - 1) + (size - 1) * y);
                        for (int x = 0; x < EngineVars.RenderSize; x++)
                        {
                            MapTile m = map.GetMapTile(renderBeginX + x, renderBeginY + y, true);
                            List<AMapObject> o = m.Items;
                            int i;
                            for (i = o.Count - 1; i > 0; i--)
                            {
                                if (o[i] is MapObjectStatic)
                                {
                                    *cur++ = (ushort)(IO.RadarColorData.Colors[o[i].ItemID] | 0x8000);
                                    *cur = (ushort)(IO.RadarColorData.Colors[o[i].ItemID] | 0x8000);
                                    cur += size;
                                    break;
                                }
                            }
                            if (i == 0)
                            {
                                *cur++ = (ushort)(IO.RadarColorData.Colors[m.GroundTile.ItemID] | 0x8000);
                                *cur = (ushort)(IO.RadarColorData.Colors[o[i].ItemID] | 0x8000);
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
