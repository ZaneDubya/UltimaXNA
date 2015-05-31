/***************************************************************************
 *   TileMatrixClient.cs
 *   Based on TileMatrix.cs (c) The RunUO Software Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.IO;
using UltimaXNA.Core;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
#endregion

namespace UltimaXNA.Ultima.IO
{
    public class TileMatrixClient
    {
        private static uint[] m_MapBlockHeightList = new uint[] { 512, 512, 200, 256, 181 };
        private const int m_SizeLandBlock = 196;
        private const int m_SizeLandBlockData = 192;

        private byte[] m_EmptyStaticsBlock;
        private byte[] m_InvalidLandBlock;

        private const uint m_bufferedLandBlocksMaxCount = 256; 
        private byte[][] m_bufferedLandBlocks;
        private uint[] m_bufferedLandBlocks_Keys;

        private byte[] m_StaticTileLoadingBuffer;

        private TileMatrixClientPatch m_Patch;

        private readonly FileStream m_MapDataStream;
        private readonly FileStream m_StaticDataStream;
        private readonly BinaryReader m_StaticIndexReader;

        public uint BlockHeight
        {
            get;
            private set;
        }

        public uint BlockWidth
        {
            get;
            private set;
        }

        public TileMatrixClient(uint index)
        {
            FileStream staticIndexStream;

            m_MapDataStream = FileManager.GetFile("map{0}.mul", index);
            staticIndexStream = FileManager.GetFile("staidx{0}.mul", index);
            m_StaticDataStream = FileManager.GetFile("statics{0}.mul", index);

            if (m_MapDataStream == null)
            {
                // the map we tried to load does not exist. Try alternate for felucca / trammel ?
                if (index == 1)
                {
                    uint trammel = 0;
                    m_MapDataStream = FileManager.GetFile("map{0}.mul", trammel);
                    staticIndexStream = FileManager.GetFile("staidx{0}.mul", trammel);
                    m_StaticDataStream = FileManager.GetFile("statics{0}.mul", trammel);
                }
                else
                {
                    Tracer.Critical("Unknown map index {0}", index);
                }
            }

            m_StaticIndexReader = new BinaryReader(staticIndexStream);

            BlockHeight = m_MapBlockHeightList[index];
            BlockWidth = (uint)m_MapDataStream.Length / (BlockHeight * m_SizeLandBlock);

            m_EmptyStaticsBlock = new byte[0];
            m_InvalidLandBlock = new byte[m_SizeLandBlockData];
            m_bufferedLandBlocks_Keys = new uint[m_bufferedLandBlocksMaxCount];
            m_bufferedLandBlocks = new byte[m_bufferedLandBlocksMaxCount][];
            for (uint i = 0; i < m_bufferedLandBlocksMaxCount; i++)
                m_bufferedLandBlocks[i] = new byte[m_SizeLandBlockData];

            m_StaticTileLoadingBuffer = new byte[2048];

            m_Patch = new TileMatrixClientPatch(this, index);
        }

        public byte[] GetLandBlock(uint blockX, uint blockY)
        {
            if (m_MapDataStream == null)
            {
                return m_InvalidLandBlock;
            }
            else
            {
                return readLandBlock(blockX, blockY);
            }
        }

        /// <summary>
        /// Retrieve the tileID and altitude of a specific land tile. VERY INEFFECIENT.
        /// </summary>
        public void GetLandTile(uint tileX, uint tileY, out ushort TileID, out sbyte altitude)
        {
            uint index = (((tileX % 8) + (tileY % 8) * 8) * 3);
            byte[] data = readLandBlock(tileX >> 3, tileY >> 3);
            TileID = BitConverter.ToUInt16(data, (int)index);
            altitude = (sbyte)data[index + 2];
        }

        public byte[] GetStaticBlock(uint blockX, uint blockY, out int length)
        {
            blockX %= BlockWidth;
            blockY %= BlockHeight;

            if (m_StaticDataStream == null || m_StaticIndexReader.BaseStream == null)
            {
                length = 0;
                return m_EmptyStaticsBlock;
            }
            else
            {
                return readStaticBlock(blockX, blockY, out length);
            }
        }

        private unsafe byte[] readStaticBlock(uint blockX, uint blockY, out int length)
        {
            // bounds check: keep block index within bounds of map
            blockX %= BlockWidth;
            blockY %= BlockHeight;

            // load the map block from a file. Check the patch file first (mapdif#.mul), then the base file (map#.mul).
            if (m_Patch.TryGetStaticBlock(blockX, blockY, ref m_StaticTileLoadingBuffer, out length))
            {
                return m_StaticTileLoadingBuffer;
            }
            else
            {
                try
                {
                    m_StaticIndexReader.BaseStream.Seek(((blockX * BlockHeight) + blockY) * 12, SeekOrigin.Begin);

                    int lookup = m_StaticIndexReader.ReadInt32();
                    length = m_StaticIndexReader.ReadInt32();

                    if (lookup < 0 || length <= 0)
                    {
                        return m_EmptyStaticsBlock;
                    }
                    else
                    {
                        m_StaticDataStream.Seek(lookup, SeekOrigin.Begin);

                        if (length > m_StaticTileLoadingBuffer.Length)
                            m_StaticTileLoadingBuffer = new byte[length];

                        fixed (byte* pStaticTiles = m_StaticTileLoadingBuffer)
                        {
                            NativeMethods.Read(m_StaticDataStream.SafeFileHandle, pStaticTiles, length);
                        }
                        return m_StaticTileLoadingBuffer;
                    }
                }
                catch (EndOfStreamException)
                {
                    throw new Exception("End of stream in static block!");
                    // return m_EmptyStaticsBlock;
                }
            }
        }

        private unsafe byte[] readLandBlock(uint blockX, uint blockY)
        {
            // bounds check: keep block index within bounds of map
            blockX %= BlockWidth;
            blockY %= BlockHeight;

            // if this block is cached in the buffer, return the cached block.
            uint key = (blockX << 16) + blockY;
            uint index = blockX % 16 + ((blockY % 16) * 16);
            if (m_bufferedLandBlocks_Keys[index] == key)
                return m_bufferedLandBlocks[index];

            // if it was not cached in the buffer, we will be loading it.
            m_bufferedLandBlocks_Keys[index] = key;

            // load the map block from a file. Check the patch file first (mapdif#.mul), then the base file (map#.mul).
            if (m_Patch.TryGetLandPatch(blockX, blockY, ref m_bufferedLandBlocks[index]))
            {
                return m_bufferedLandBlocks[index];
            }
            else
            {
                uint ptr = ((blockX * BlockHeight) + blockY) * m_SizeLandBlock + 4;
                m_MapDataStream.Seek(ptr, SeekOrigin.Begin);
                fixed (byte* pData = m_bufferedLandBlocks[index])
                {
                    NativeMethods.Read(m_MapDataStream.SafeFileHandle, pData, m_SizeLandBlockData);
                }
                Metrics.ReportDataRead(m_SizeLandBlockData);
                return m_bufferedLandBlocks[index];
            }
        }

        public void Dispose()
        {
            if (m_StaticIndexReader != null)
            {
                m_StaticIndexReader.Close();
            }

            if (m_MapDataStream != null)
            {
                m_MapDataStream.Close();
            }

            if (m_StaticDataStream != null)
            {
                m_StaticDataStream.Close();
            }
        }
    }
}
