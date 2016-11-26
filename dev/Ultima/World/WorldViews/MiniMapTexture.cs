/***************************************************************************
 *   MiniMapTexture.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.WorldViews
{
    public class MiniMapTexture
    {
        private uint[] m_TextureData;
        private uint[] m_BlockColors;
        private MiniMapChunk[] m_BlockCache;

        public Texture2D Texture
        {
            get;
            private set;
        }

        private SpriteBatchUI m_SpriteBatch;

        private bool m_MustRedrawEntireTexture;
        private uint m_LastCenterCellX, m_LastCenterCellY;

        private List<uint> m_QueuedToDrawBlocks;

        private const uint Stride = 256;
        private const uint BlockCacheWidth = 48, BlockCacheHeight = 48;
        private const uint TilesPerBlock = 64;

        public void Initialize()
        {
            m_SpriteBatch = Services.Get<SpriteBatchUI>();
            Texture = new Texture2D(m_SpriteBatch.GraphicsDevice, (int)Stride, (int)Stride);

            m_TextureData = new uint[Stride * Stride];
            m_BlockColors = new uint[TilesPerBlock];
            m_BlockCache = new MiniMapChunk[BlockCacheWidth * BlockCacheHeight];
            m_MustRedrawEntireTexture = true;

            m_QueuedToDrawBlocks = new List<uint>();
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

            if (m_MustRedrawEntireTexture)
            {
                uint firstX = centerCellX - 15;
                uint firstY = centerCellY;
                for (uint y = 0; y < 32; y++)
                {
                    for (uint x = 0; x < 16; x++)
                    {
                        InternalQueueMapBlock(map, firstX + ((y + 1) / 2) + x, firstY + (y / 2) - x);
                    }
                }
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
                        uint firstX = centerCellX - 15;
                        uint firstY = centerCellY;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalQueueMapBlock(map, firstX + x + ((y + 1) / 2), firstY - x + (y / 2));
                    }

                    if (centerDiffY >= 0)
                    {
                        // traveling LEFT/WEST, draw a new column.
                        uint firstX = centerCellX - 15;
                        uint firstY = centerCellY + 0;
                        for (uint y = 0; y < 32; y++)
                            InternalQueueMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));
                    }
                }
                else if (centerDiffX > 0)
                {
                    if (centerDiffY <= 0)
                    {
                        // traveling RIGHT/EAST, draw a new column.
                        uint firstX = centerCellX + 0;
                        uint firstY = centerCellY - 15;
                        for (uint y = 0; y < 32; y++)
                            InternalQueueMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));
                    }

                    if (centerDiffY >= 0)
                    {
                        // travelling DOWN/EAST, draw new rows.
                        uint firstX = centerCellX + 0;
                        uint firstY = centerCellY + 15;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalQueueMapBlock(map, firstX + ((y + 1) / 2) + x, firstY - x + (y / 2));
                    }
                }
                else if (centerDiffY != 0)
                {
                    if (centerDiffY < 0)
                    {
                        // traveling NORTH, draw a new row and column.
                        uint firstX = centerCellX - 15;
                        uint firstY = centerCellY + 0;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalQueueMapBlock(map, firstX + x + ((y + 1) / 2), firstY - x + (y / 2));

                        firstX = centerCellX + 0;
                        firstY = centerCellY - 15;
                        for (uint y = 0; y < 32; y++)
                            InternalQueueMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));
                    }
                    else if (centerDiffY > 0)
                    {
                        // traveling SOUTH, draw a new row and column.
                        uint firstX = centerCellX - 15;
                        uint firstY = centerCellY + 0;
                        for (uint y = 0; y < 32; y++)
                            InternalQueueMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));

                        firstX = centerCellX - 0;
                        firstY = centerCellY + 15;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalQueueMapBlock(map, firstX + ((y + 1) / 2) + x, firstY - x + (y / 2));
                    }
                }
            }

            if (m_QueuedToDrawBlocks.Count > 0)
            {
                InternalDrawQueuedMapBlocks();
                m_SpriteBatch.GraphicsDevice.Textures[3] = null;
                Texture.SetData<uint>(m_TextureData);
                m_SpriteBatch.GraphicsDevice.Textures[3] = Texture;
            }
        }

        private void InternalQueueMapBlock(Map map, uint cellx, uint celly)
        {
            uint chunkIndex = (cellx % BlockCacheWidth) + (celly % BlockCacheHeight) * BlockCacheWidth;

            MiniMapChunk chunk = m_BlockCache[chunkIndex];
            if (chunk == null || chunk.X != cellx || chunk.Y != celly)
            {
                // the chunk is not in our cache! Try loading it from the map?
                MapChunk mapBlock = map.GetMapChunk(cellx, celly);
                if (mapBlock == null)
                {
                    // if the chunk is not loaded in memory, load it from the filesystem.
                    m_BlockCache[chunkIndex] = new MiniMapChunk(cellx, celly, map.MapData);
                }
                else
                {
                    // get the colors for this chunk from the map chunk, which will have already sorted the objects.
                    m_BlockCache[chunkIndex] = new MiniMapChunk(mapBlock);
                }
            }
            else
            {
                m_BlockColors = chunk.Colors;
            }

            m_QueuedToDrawBlocks.Add(chunkIndex);
        }

        private void InternalDrawQueuedMapBlocks()
        {
            IEnumerator<uint> chunks = m_QueuedToDrawBlocks.GetEnumerator();

            for (int i = 0; i < m_QueuedToDrawBlocks.Count; i++)
            {
                MiniMapChunk chunk = m_BlockCache[m_QueuedToDrawBlocks[i]];

                uint cellX32 = chunk.X % 32, cellY32 = chunk.Y % 32;
                m_BlockColors = chunk.Colors;

                // now draw the chunk
                if (cellX32 == 0 && cellY32 == 0)
                {
                    // draw the chunk split out over four corners of the texture.
                    int chunkindex = 0;
                    for (uint tiley = 0; tiley < 8; tiley++)
                    {
                        uint drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                        uint drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                        for (uint tilex = 0; tilex < 8; tilex++)
                        {
                            uint color = m_BlockColors[chunkindex++];
                            m_TextureData[drawy * Stride + drawx] = color;
                            if (drawy == 255)
                                m_TextureData[drawx] = color;
                            else
                                m_TextureData[drawy * Stride + Stride + drawx] = color;
                            drawx = (drawx + 1) % Stride;
                            drawy = (drawy + 1) % Stride;
                        }
                    }
                }
                else if (cellX32 + cellY32 == 32)
                {
                    // draw the chunk split on the top and bottom of the texture.
                    int chunkindex = 0;
                    for (uint tiley = 0; tiley < 8; tiley++)
                    {
                        uint drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                        uint drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                        for (uint tilex = 0; tilex < 8; tilex++)
                        {
                            uint color = m_BlockColors[chunkindex++];
                            m_TextureData[drawy * Stride + drawx] = color;
                            if (drawy == 255)
                                m_TextureData[drawx] = color;
                            else
                                m_TextureData[drawy * Stride + Stride + drawx] = color;
                            drawx = (drawx + 1) % Stride;
                            drawy = (drawy + 1) % Stride;
                        }
                    }
                }
                else if (cellX32 == cellY32)
                {
                    // draw the chunk split on the left and right side of the texture.
                    int chunkindex = 0;
                    for (uint tiley = 0; tiley < 8; tiley++)
                    {
                        uint drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                        uint drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                        for (uint tilex = 0; tilex < 8; tilex++)
                        {
                            uint color = m_BlockColors[chunkindex++];
                            m_TextureData[drawy * Stride + drawx] = color;
                            m_TextureData[drawy * Stride + Stride + drawx] = color;
                            drawx = (drawx + 1) % Stride;
                            drawy = (drawy + 1) % Stride;
                        }
                    }
                }
                else
                {
                    // draw the chunk normally.
                    int chunkindex = 0;
                    for (uint tiley = 0; tiley < 8; tiley++)
                    {
                        uint drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                        uint drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                        for (uint tilex = 0; tilex < 8; tilex++)
                        {
                            uint color = m_BlockColors[chunkindex++];
                            m_TextureData[drawy * Stride + drawx] = color;
                            m_TextureData[drawy * Stride + Stride + drawx] = color;
                            drawx = (drawx + 1) % Stride;
                            drawy = (drawy + 1) % Stride;
                        }
                    }
                }
            }

            m_QueuedToDrawBlocks.Clear();
        }
    }
}
