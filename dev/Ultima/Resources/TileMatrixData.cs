/***************************************************************************
 *   TileMatrixData.cs
 *   Based on TileMatrix.cs from RunUO: https://github.com/runuo/runuo
 *      Copyright (c) 2002 The RunUO Software Team
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
        const int SizeOfInitialStaticTileLoadingBuffer = 16384;
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
            // Map file fallback order: mapX.mul => mapXLegacyMUL.uop => (if trammel / map index 1) => map0.mul => mapXLegacyMUL.uop
            if (!LoadMapStream(MapIndex, out m_MapDataStream, out m_UOPIndex))
            {
                if (MapIndex == 1 && LoadMapStream(0, out m_MapDataStream, out m_UOPIndex))
                {
                    Tracer.Debug("Map file for index 1 did not exist, successfully loaded index 0 instead.");
                }
                else
                {
                    Tracer.Critical($"Unable to load map index {MapIndex}");
                }
            }
            ChunkHeight = MapChunkHeightList[MapIndex];
            ChunkWidth = (uint)m_MapDataStream.Length / (ChunkHeight * SizeOfLandChunk);
            // load map patch and statics
            m_Patch = new TileMatrixDataPatch(this, MapIndex);
            if (!LoadStaticsStream(MapIndex, out m_StaticDataStream, out m_StaticIndexReader))
            {
                if (MapIndex == 1 && LoadStaticsStream(0, out m_StaticDataStream, out m_StaticIndexReader))
                {
                    Tracer.Debug("Statics file for index 1 did not exist, successfully loaded index 0 instead.");
                }
                else
                {
                    Tracer.Critical($"Unable to load static index {MapIndex}");
                }
            }
            // load buffers
            m_BufferedLandChunkKeys = new uint[CountBufferedLandChunk];
            m_BufferedLandChunks = new byte[CountBufferedLandChunk][];
            for (uint i = 0; i < CountBufferedLandChunk; i++)
            {
                m_BufferedLandChunks[i] = new byte[SizeOfLandChunkData];
            }
            m_StaticTileLoadingBuffer = new byte[SizeOfInitialStaticTileLoadingBuffer];
        }

        bool LoadMapStream(uint index, out FileStream mapDataStream, out UOPIndex uopIndex)
        {
            mapDataStream = null;
            uopIndex = null;
            string path = FileManager.GetFilePath($"map{index}.mul");
            if (File.Exists(path))
            {
                mapDataStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return true;
            }
            path = FileManager.GetFilePath($"map{index}LegacyMUL.uop");
            if (File.Exists(path))
            {
                mapDataStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                uopIndex = new UOPIndex(m_MapDataStream);
                return true;
            }
            return false;
        }

        bool LoadStaticsStream(uint index, out FileStream dataStream, out BinaryReader indexReader)
        {
            dataStream = null;
            indexReader = null;
            string pathData = FileManager.GetFilePath($"statics{index}.mul");
            string pathIndex = FileManager.GetFilePath($"staidx{index}.mul");
            if (File.Exists(pathData) && File.Exists(pathIndex))
            {
                dataStream = new FileStream(pathData, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                indexReader = new BinaryReader(new FileStream(pathIndex, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            m_MapDataStream?.Close();
            m_UOPIndex?.Close();
            m_StaticIndexReader?.Close();
            m_StaticDataStream?.Close();
        }

        public byte[] GetLandChunk(uint chunkX, uint chunkY)
        {
            return (m_MapDataStream == null) ? m_InvalidLandChunk : ReadLandChunk(chunkX, chunkY);
        }

        /// <summary>
        /// Retrieve the tileID and altitude of a specific land tile. VERY INEFFECIENT.
        /// </summary>
        public void GetLandTile(uint tileX, uint tileY, out ushort TileID, out sbyte altitude)
        {
            uint index = (((tileX % 8) + (tileY % 8) * 8) * 3);
            byte[] data = ReadLandChunk(tileX >> 3, tileY >> 3);
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
            return ReadStaticChunk(chunkX, chunkY, out length);
        }

        unsafe byte[] ReadStaticChunk(uint chunkX, uint chunkY, out int length)
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

        unsafe byte[] ReadLandChunk(uint chunkX, uint chunkY)
        {
            // bounds check: keep chunk index within bounds of map
            chunkX %= ChunkWidth;
            chunkY %= ChunkHeight;
            // if this chunk is cached in the buffer, return the cached chunk.
            uint key = (chunkX << 16) + chunkY;
            uint index = chunkX % 16 + ((chunkY % 16) * 16);
            if (m_BufferedLandChunkKeys[index] == key)
            {
                return m_BufferedLandChunks[index];
            }
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
