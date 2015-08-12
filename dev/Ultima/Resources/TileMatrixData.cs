/***************************************************************************
 *   TileMatrixData.cs
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
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public class TileMatrixData
    {
        private static uint[] m_MapChunkHeightList = new uint[] { 512, 512, 200, 256, 181 };
        private const int m_SizeLandChunk = 196;
        private const int m_SizeLandChunkData = 192;

        private byte[] m_EmptyStaticsChunk;
        private byte[] m_InvalidLandChunk;

        private const uint m_bufferedLandChunksMaxCount = 256; 
        private byte[][] m_bufferedLandChunks;
        private uint[] m_bufferedLandChunks_Keys;

        private byte[] m_StaticTileLoadingBuffer;

        private TileMatrixDataPatch m_Patch;

        private readonly FileStream m_MapDataStream;
        private readonly FileStream m_StaticDataStream;
        private readonly BinaryReader m_StaticIndexReader;

        public uint ChunkHeight
        {
            get;
            private set;
        }

        public uint ChunkWidth
        {
            get;
            private set;
        }

        public TileMatrixData(uint index)
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

            ChunkHeight = m_MapChunkHeightList[index];
            ChunkWidth = (uint)m_MapDataStream.Length / (ChunkHeight * m_SizeLandChunk);

            m_EmptyStaticsChunk = new byte[0];
            m_InvalidLandChunk = new byte[m_SizeLandChunkData];
            m_bufferedLandChunks_Keys = new uint[m_bufferedLandChunksMaxCount];
            m_bufferedLandChunks = new byte[m_bufferedLandChunksMaxCount][];
            for (uint i = 0; i < m_bufferedLandChunksMaxCount; i++)
                m_bufferedLandChunks[i] = new byte[m_SizeLandChunkData];

            m_StaticTileLoadingBuffer = new byte[2048];

            m_Patch = new TileMatrixDataPatch(this, index);
        }

        public byte[] GetLandChunk(uint chunkX, uint chunkY)
        {
            if (m_MapDataStream == null)
            {
                return m_InvalidLandChunk;
            }
            else
            {
                return readLandChunk(chunkX, chunkY);
            }
        }

        /// <summary>
        /// Retrieve the tileID and altitude of a specific land tile. VERY INEFFECIENT.
        /// </summary>
        public void GetLandTile(uint tileX, uint tileY, out ushort TileID, out sbyte altitude)
        {
            uint index = (((tileX % 8) + (tileY % 8) * 8) * 3);
            byte[] data = readLandChunk(tileX >> 3, tileY >> 3);
            TileID = BitConverter.ToUInt16(data, (int)index);
            altitude = (sbyte)data[index + 2];
        }

        public byte[] GetStaticChunk(uint chunkX, uint chunkY, out int length)
        {
            chunkX %= ChunkWidth;
            chunkY %= ChunkHeight;

            if (m_StaticDataStream == null || m_StaticIndexReader.BaseStream == null)
            {
                length = 0;
                return m_EmptyStaticsChunk;
            }
            else
            {
                return readStaticChunk(chunkX, chunkY, out length);
            }
        }

        private unsafe byte[] readStaticChunk(uint chunkX, uint chunkY, out int length)
        {
            // bounds check: keep chunk index within bounds of map
            chunkX %= ChunkWidth;
            chunkY %= ChunkHeight;

            // load the map chunk from a file. Check the patch file first (mapdif#.mul), then the base file (map#.mul).
            if (m_Patch.TryGetStaticChunk(chunkX, chunkY, ref m_StaticTileLoadingBuffer, out length))
            {
                return m_StaticTileLoadingBuffer;
            }
            else
            {
                try
                {
                    m_StaticIndexReader.BaseStream.Seek(((chunkX * ChunkHeight) + chunkY) * 12, SeekOrigin.Begin);

                    int lookup = m_StaticIndexReader.ReadInt32();
                    length = m_StaticIndexReader.ReadInt32();

                    if (lookup < 0 || length <= 0)
                    {
                        return m_EmptyStaticsChunk;
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
                    throw new Exception("End of stream in static chunk!");
                    // return m_EmptyStaticsChunk;
                }
            }
        }

        private unsafe byte[] readLandChunk(uint chunkX, uint chunkY)
        {
            // bounds check: keep chunk index within bounds of map
            chunkX %= ChunkWidth;
            chunkY %= ChunkHeight;

            // if this chunk is cached in the buffer, return the cached chunk.
            uint key = (chunkX << 16) + chunkY;
            uint index = chunkX % 16 + ((chunkY % 16) * 16);
            if (m_bufferedLandChunks_Keys[index] == key)
                return m_bufferedLandChunks[index];

            // if it was not cached in the buffer, we will be loading it.
            m_bufferedLandChunks_Keys[index] = key;

            // load the map chunk from a file. Check the patch file first (mapdif#.mul), then the base file (map#.mul).
            if (m_Patch.TryGetLandPatch(chunkX, chunkY, ref m_bufferedLandChunks[index]))
            {
                return m_bufferedLandChunks[index];
            }
            else
            {
                uint ptr = ((chunkX * ChunkHeight) + chunkY) * m_SizeLandChunk + 4;
                m_MapDataStream.Seek(ptr, SeekOrigin.Begin);
                fixed (byte* pData = m_bufferedLandChunks[index])
                {
                    NativeMethods.Read(m_MapDataStream.SafeFileHandle, pData, m_SizeLandChunkData);
                }
                Metrics.ReportDataRead(m_SizeLandChunkData);
                return m_bufferedLandChunks[index];
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
