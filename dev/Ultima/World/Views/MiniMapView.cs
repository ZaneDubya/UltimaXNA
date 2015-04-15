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
        private MiniMapBlock[] m_BlockCache;

        private Texture2D m_Texture;
        private SpriteBatchUI m_SpriteBatch;

        private bool m_MustRedrawEntireTexture;
        private uint m_LastCenterCellX, m_LastCenterCellY;

        private const uint Stride = 256;
        private const uint BlockCacheWidth = 48, BlockCacheHeight = 48;
        private const uint TilesPerBlock = 64;

        public void Initialize()
        {
            m_SpriteBatch = UltimaServices.GetService<SpriteBatchUI>();
            m_Texture = new Texture2D(m_SpriteBatch.GraphicsDevice, (int)Stride, (int)Stride);

            m_TextureData = new uint[Stride * Stride];
            m_BlockColors = new uint[TilesPerBlock];
            m_BlockCache = new MiniMapBlock[BlockCacheWidth * BlockCacheHeight];
            m_MustRedrawEntireTexture = true;
        }

        public void Update(Map map, Position3D center)
        {
            uint centerCellX = (uint)center.X / 8;
            uint centerCellY = (uint)center.Y / 8;

            int centerDiffX = (int)(centerCellX - m_LastCenterCellX);
            int centerDiffY = (int)(centerCellY - m_LastCenterCellY);

            m_LastCenterCellX = centerCellX;
            m_LastCenterCellY = centerCellY;

            if (centerDiffX < -1 || centerDiffX > 1 || centerDiffY < -1 || centerDiffY > 1)
                m_MustRedrawEntireTexture = true;

            bool newTextureData = false;

            if (m_MustRedrawEntireTexture)
            {
                uint firstX = centerCellX - 7;
                uint firstY = centerCellY;
                for (uint y = 0; y < 32; y++)
                {
                    for (uint x = 0; x < 16; x++)
                    {
                        InternalDrawMapBlock(map, firstX + ((y + 1) / 2) + x, firstY + (y / 2) - x);
                    }
                }

                newTextureData = true;
                m_MustRedrawEntireTexture = false;
            }
            else if (centerDiffX != 0 || centerDiffY != 0)
            {
                // draw just enough of the minimap to cover the newly exposed area.
                if (centerDiffX < 0)
                {
                    if (centerDiffY <= 0)
                    {
                        // traveling UP/WEST, draw new rows.
                        uint firstX = centerCellX - 7;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalDrawMapBlock(map, firstX + x + ((y + 1) / 2), centerCellY - x + (y / 2));
                    }

                    if (centerDiffY >= 0)
                    {
                        // traveling LEFT/WEST, draw a new column.
                        uint firstX = centerCellX - 7;
                        for (uint y = 0; y < 32; y++)
                            InternalDrawMapBlock(map, firstX + ((y + 1) / 2), centerCellY + (y / 2));
                    }
                }
                else if (centerDiffX > 0)
                {
                    if (centerDiffY <= 0)
                    {
                        // traveling RIGHT/EAST, draw a new column.
                        uint firstX = centerCellX - 0 + 8;
                        uint firstY = centerCellY - 7 - 8;
                        for (uint y = 0; y < 32; y++)
                            InternalDrawMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));
                    }

                    if (centerDiffY >= 0)
                    {
                        // travelling DOWN/EAST, draw new rows.
                        uint firstX = centerCellX - 0;
                        uint firstY = centerCellY + 23;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalDrawMapBlock(map, firstX + ((y + 1) / 2) + x, firstY - x + (y / 2));
                    }
                }
                else if (centerDiffY != 0)
                {
                    if (centerDiffY < 0)
                    {
                        // traveling NORTH, draw a new row and column.
                        uint firstX = centerCellX - 7;
                        uint firstY = centerCellY;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalDrawMapBlock(map, firstX + x + ((y + 1) / 2), firstY - x + (y / 2));

                        firstX = centerCellX - 0 + 8;
                        firstY = centerCellY - 7 - 8;
                        for (uint y = 0; y < 32; y++)
                            InternalDrawMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));
                    }
                    else if (centerDiffY > 0)
                    {
                        // traveling SOUTH, draw a new row and column.
                        uint firstX = centerCellX - 7;
                        uint firstY = centerCellY;
                        for (uint y = 0; y < 32; y++)
                            InternalDrawMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));

                        firstX = centerCellX - 0;
                        firstY = centerCellY + 23;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalDrawMapBlock(map, firstX + ((y + 1) / 2) + x, firstY - x + (y / 2));
                    }
                }
                newTextureData = true;
            }

            if (newTextureData)
            {
                m_SpriteBatch.GraphicsDevice.Textures[3] = null;
                m_Texture.SetData<uint>(m_TextureData);
                m_SpriteBatch.GraphicsDevice.Textures[3] = m_Texture;
            }
        }

        private void InternalDrawMapBlock(Map map, uint cellx, uint celly)
        {
            uint blockIndex = (cellx % BlockCacheWidth) + (celly % BlockCacheHeight) * BlockCacheWidth;

            MiniMapBlock block = m_BlockCache[blockIndex];
            if (block == null || block.X != cellx || block.Y != celly)
            {
                // the block is not in our cache! Try loading it from the map?
                MapBlock mapBlock = map.GetMapBlock(cellx, celly);
                if (mapBlock == null)
                {
                    // if the block is not loaded in memory, load it from the filesystem.
                    m_BlockCache[blockIndex] = new MiniMapBlock(cellx, celly, map.MapData);
                    m_BlockColors = m_BlockCache[blockIndex].Colors;
                }
                else
                {
                    // get the colors for this block from the map block, which will have already sorted the objects.
                    m_BlockCache[blockIndex] = new MiniMapBlock(mapBlock);
                    m_BlockColors = m_BlockCache[blockIndex].Colors;
                }
            }
            else
            {
                m_BlockColors = block.Colors;
            }

            uint cellX32 = cellx % 32, cellY32 = celly % 32;

            // now draw the block
            if (cellX32 == 0 && cellY32 == 0)
            {
                // draw the block split out over four corners of the texture.
                int blockindex = 0;
                for (uint tiley = 0; tiley < 8; tiley++)
                {
                    uint drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                    uint drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                    for (uint tilex = 0; tilex < 8; tilex++)
                    {
                        m_TextureData[drawy * Stride + drawx] = m_BlockColors[blockindex];
                        if (drawy == 255)
                        {
                            m_TextureData[drawx] = m_BlockColors[blockindex++];
                        }
                        else
                        {
                            m_TextureData[drawy * Stride + Stride + drawx] = m_BlockColors[blockindex++];
                        }
                        drawx = (drawx + 1) % Stride;
                        drawy = (drawy + 1) % Stride;
                    }
                }
            }
            else if (cellX32 + cellY32 == 32)
            {
                // draw the block split on the top and bottom of the texture.
                int blockindex = 0;
                for (uint tiley = 0; tiley < 8; tiley++)
                {
                    uint drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                    uint drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                    for (uint tilex = 0; tilex < 8; tilex++)
                    {
                        m_TextureData[drawy * Stride + drawx] = m_BlockColors[blockindex];
                        if (drawy == 255)
                        {
                            m_TextureData[drawx] = m_BlockColors[blockindex++];
                        }
                        else
                        {
                            m_TextureData[drawy * Stride + Stride + drawx] = m_BlockColors[blockindex++];
                        }
                        drawx = (drawx + 1) % Stride;
                        drawy = (drawy + 1) % Stride;
                    }
                }
            }
            else if (cellX32 == cellY32)
            {
                // draw the block split on the left and right side of the texture.
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
