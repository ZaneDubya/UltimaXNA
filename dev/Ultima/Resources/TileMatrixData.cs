/***************************************************************************
 *   TileMatrixData.cs
 *   Based on TileMatrix.cs from RunUO
 *      (c) The RunUO Software Team
 *   And on code from OpenUO: https://github.com/jeffboulanger/OpenUO
 *      Copyright (c) 2011 OpenUO Software Team.
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
using UltimaXNA.Core.Windows;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public class TileMatrixData
    {
        // === Constant Data ==========================================================================================
        readonly uint[] MapChunkHeightList = { 512, 512, 200, 256, 181 };
        const int SizeOfLandChunk = 196;
        const int SizeOfLandChunkData = 192;
        const uint CountBufferedLandChunk = 256;
        static byte[] m_EmptyStaticsChunk = new byte[0];
        static byte[] m_InvalidLandChunk = new byte[SizeOfLandChunkData];
        // === Instance data ==========================================================================================
        public readonly uint ChunkHeight;
        public readonly uint ChunkWidth;
        public readonly uint MapIndex;
        byte[][] m_BufferedLandChunks;
        uint[] m_BufferedLandChunkKeys;
        byte[] m_StaticTileLoadingBuffer;
        readonly TileMatrixDataPatch m_Patch;
        readonly FileStream m_MapDataStream;
        readonly FileStream m_StaticDataStream;
        readonly BinaryReader m_StaticIndexReader;
        readonly UOPIndex m_UOPIndex;

        public TileMatrixData(uint index)
        {
            MapIndex = index;
            FileStream staticIndexStream;
            string mapPath = FileManager.GetFilePath(String.Format("map{0}.mul", index));
            if (File.Exists(mapPath))
            {
                m_MapDataStream = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            else
            {
                mapPath = FileManager.GetFilePath(String.Format("map{0}LegacyMUL.uop", index));
                if (File.Exists(mapPath))
                {
                    m_MapDataStream = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    m_UOPIndex = new UOPIndex(m_MapDataStream);
                }
            }
            if (m_MapDataStream == null)
            {
                // the map we tried to load does not exist. Try alternate for felucca / trammel ?
                if (index == 1)
                {
                    uint trammel = 0;
                    string mapPath2 = FileManager.GetFilePath(String.Format("map{0}.mul", trammel));

                    if (File.Exists(mapPath2))
                    {
                        m_MapDataStream = new FileStream(mapPath2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    }
                    else
                    {
                        mapPath2 = FileManager.GetFilePath(String.Format("map{0}LegacyMUL.uop", trammel));
                        if (File.Exists(mapPath2))
                        {
                            m_MapDataStream = new FileStream(mapPath2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            m_UOPIndex = new UOPIndex(m_MapDataStream);
                        }
                    }
                    staticIndexStream = FileManager.GetFile("staidx{0}.mul", trammel);
                    m_StaticDataStream = FileManager.GetFile("statics{0}.mul", trammel);
                }
                else
                {
                    Tracer.Critical("Unknown map index {0}", index);
                }
            }

            staticIndexStream = FileManager.GetFile("staidx{0}.mul", index);
            m_StaticDataStream = FileManager.GetFile("statics{0}.mul", index);
            m_StaticIndexReader = new BinaryReader(staticIndexStream);

            ChunkHeight = MapChunkHeightList[index];
            ChunkWidth = (uint)m_MapDataStream.Length / (ChunkHeight * SizeOfLandChunk);

            m_BufferedLandChunkKeys = new uint[CountBufferedLandChunk];
            m_BufferedLandChunks = new byte[CountBufferedLandChunk][];
            for (uint i = 0; i < CountBufferedLandChunk; i++)
            {
                m_BufferedLandChunks[i] = new byte[SizeOfLandChunkData];
            }
            m_StaticTileLoadingBuffer = new byte[2048];
            m_Patch = new TileMatrixDataPatch(this, index);
        }

        public void Dispose()
        {
            m_StaticIndexReader?.Close();
            m_MapDataStream?.Close();
            m_StaticDataStream?.Close();
        }

        public byte[] GetLandChunk(uint chunkX, uint chunkY)
        {
            return (m_MapDataStream == null) ? m_InvalidLandChunk : readLandChunk(chunkX, chunkY);
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
            return readStaticChunk(chunkX, chunkY, out length);
        }

        unsafe byte[] readStaticChunk(uint chunkX, uint chunkY, out int length)
        {
            // bounds check: keep chunk index within bounds of map
            chunkX %= ChunkWidth;
            chunkY %= ChunkHeight;

            // load the map chunk from a file. Check the patch file first (mapdif#.mul), then the base file (map#.mul).
            if (m_Patch.TryGetStaticChunk(MapIndex, chunkX, chunkY, ref m_StaticTileLoadingBuffer, out length))
            {
                return m_StaticTileLoadingBuffer;
            }
            try
            {
                m_StaticIndexReader.BaseStream.Seek(((chunkX * ChunkHeight) + chunkY) * 12, SeekOrigin.Begin);
                int lookup = m_StaticIndexReader.ReadInt32();
                length = m_StaticIndexReader.ReadInt32();
                if (lookup < 0 || length <= 0)
                {
                    return m_EmptyStaticsChunk;
                }
                m_StaticDataStream.Seek(lookup, SeekOrigin.Begin);
                if (length > m_StaticTileLoadingBuffer.Length)
                {
                    m_StaticTileLoadingBuffer = new byte[length];
                }
                fixed (byte* pStaticTiles = m_StaticTileLoadingBuffer)
                {
                    NativeMethods.ReadBuffer(m_StaticDataStream.SafeFileHandle, pStaticTiles, length);
                }
                return m_StaticTileLoadingBuffer;
            }
            catch (EndOfStreamException)
            {
                throw new Exception("End of stream in static chunk!");
            }
        }

        unsafe byte[] readLandChunk(uint chunkX, uint chunkY)
        {
            // bounds check: keep chunk index within bounds of map
            chunkX %= ChunkWidth;
            chunkY %= ChunkHeight;

            // if this chunk is cached in the buffer, return the cached chunk.
            uint key = (chunkX << 16) + chunkY;
            uint index = chunkX % 16 + ((chunkY % 16) * 16);
            if (m_BufferedLandChunkKeys[index] == key)
                return m_BufferedLandChunks[index];

            // if it was not cached in the buffer, we will be loading it.
            m_BufferedLandChunkKeys[index] = key;

            // load the map chunk from a file. Check the patch file first (mapdif#.mul), then the base file (map#.mul).
            if (m_Patch.TryGetLandPatch(MapIndex, chunkX, chunkY, ref m_BufferedLandChunks[index]))
            {
                return m_BufferedLandChunks[index];
            }
            int ptr = (int)((chunkX * ChunkHeight) + chunkY) * SizeOfLandChunk + 4;
            if (m_UOPIndex != null)
            {
                ptr = m_UOPIndex.Lookup(ptr);
            }
            m_MapDataStream.Seek(ptr, SeekOrigin.Begin);
            fixed (byte* pData = m_BufferedLandChunks[index])
            {
                NativeMethods.ReadBuffer(m_MapDataStream.SafeFileHandle, pData, SizeOfLandChunkData);
            }
            Metrics.ReportDataRead(SizeOfLandChunkData);
            return m_BufferedLandChunks[index];
        }
    }
}
