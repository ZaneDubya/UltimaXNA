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
            uint centerCellX = (uint)center.X / 8;
            uint centerCellY = (uint)center.Y / 8;

            int centerDiffX = (int)(m_LastCenterCellX - centerCellX);
            int centerDiffY = (int)(m_LastCenterCellY - center.Y);

            m_LastCenterCellX = centerCellX;
            m_LastCenterCellY = centerCellY;

            if (centerDiffX < -1 || centerDiffX > 1 || centerDiffY < -1 || centerDiffY > 1)
                m_MustRedrawEntireTexture = true;

            uint mapCellsWidth = (uint)map.Width / 8;
            uint mapCellsHeight = (uint)map.Height / 8;

            if (m_MustRedrawEntireTexture)
            {
                uint firstX = centerCellX - 7, firstY = centerCellY;
                for (uint y = 0; y < 32; y++)
                {
                    for (uint x = 0; x < 16; x++)
                    {
                        InternalDrawMapBlock(map, firstX + ((y + 1) / 2) + x, firstY + (y / 2) - x);
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
            {
                // if the block is not loaded in memory, load it from the filesystem.
                MiniMapBlock mmb = new MiniMapBlock(cellx, celly, map.MapData);
                m_BlockColors = mmb.Colors;
            }
            else
            {
                // get the colors for this block from the data loaded in memory.
                MiniMapBlock mmb = new MiniMapBlock(block);
                m_BlockColors = mmb.Colors;
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
}
