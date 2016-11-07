/***************************************************************************
 *   TileMatrixDataPatch.cs
 *   Based on TileMatrixPatch.cs from RunUO
 *      (c) The RunUO Software Team
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
using System.IO;
using UltimaXNA.Core.Windows;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.Network.Server.GeneralInfo;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public class TileMatrixDataPatch
    {
        // === Static Data ============================================================================================
        private static MapDiffInfo EnabledDiffs;
        public static void EnableMapDiffs(MapDiffInfo diffs)
        {
            EnabledDiffs = diffs;
        }

        // === Instance data ==========================================================================================
        private FileStream m_LandPatchStream;
        private FileStream m_StaticPatchStream;

        private Dictionary<uint, LandPatchData> m_LandPatchPtrs;
        private Dictionary<uint, StaticPatchData> m_StaticPatchPtrs;

        private class LandPatchData
        {
            public readonly uint Index;
            public readonly uint Pointer;
            public LandPatchData Next;

            public LandPatchData(uint index, uint ptr)
            {
                Index = index;
                Pointer = ptr;
            }
        }

        private class StaticPatchData
        {
            public readonly uint Index;
            public readonly uint Pointer;
            public readonly int Length; // lengths can be negative; if they are, then they should be ignored.
            public StaticPatchData Next;

            public StaticPatchData(uint index, uint ptr, int length)
            {
                Index = index;
                Pointer = ptr;
                Length = length;
            }
        }

        public TileMatrixDataPatch(TileMatrixData matrix, uint index)
        {
            LoadLandPatches(matrix, String.Format("mapdif{0}.mul", index), String.Format("mapdifl{0}.mul", index));
            LoadStaticPatches(matrix, String.Format("stadif{0}.mul", index), String.Format("stadifl{0}.mul", index), String.Format("stadifi{0}.mul", index));
        }

        private uint MakeChunkKey(uint blockX, uint blockY)
        {
            return ((blockY & 0x0000ffff) << 16) | (blockX & 0x0000ffff);
        }

        public unsafe bool TryGetLandPatch(uint map, uint blockX, uint blockY, ref byte[] landData)
        {
            if (ClientVersion.InstallationIsUopFormat)
            {
                return false;
            }
            uint key = MakeChunkKey(blockX, blockY);
            LandPatchData data;
            if (m_LandPatchPtrs.TryGetValue(key, out data))
            {
                if (data.Index >= EnabledDiffs.MapPatches[map])
                {
                    return false;
                }
                while (data.Next != null)
                {
                    if (data.Next.Index >= EnabledDiffs.MapPatches[map])
                        break;
                    data = data.Next;
                }
                m_LandPatchStream.Seek(data.Pointer, SeekOrigin.Begin);
                landData = new byte[192];
                fixed (byte* pTiles = landData)
                {
                    NativeMethods.ReadBuffer(m_LandPatchStream.SafeFileHandle, pTiles, 192);
                }
                return true;
            }

            return false;
        }

        private unsafe int LoadLandPatches(TileMatrixData tileMatrix, string landPath, string indexPath)
        {
            m_LandPatchPtrs = new Dictionary<uint, LandPatchData>();

            if (ClientVersion.InstallationIsUopFormat)
                return 0;

            m_LandPatchStream = FileManager.GetFile(landPath);
            if (m_LandPatchStream == null)
            {
                return 0;
            }

            using (FileStream fsIndex = FileManager.GetFile(indexPath))
            {
                BinaryReader indexReader = new BinaryReader(fsIndex);
                int count = (int)(indexReader.BaseStream.Length / 4);
                uint ptr = 0;
                for (uint i = 0; i < count; ++i)
                {
                    uint blockID = indexReader.ReadUInt32();
                    uint x = blockID / tileMatrix.ChunkHeight;
                    uint y = blockID % tileMatrix.ChunkHeight;
                    uint key = MakeChunkKey(x, y);
                    ptr += 4;
                    if (m_LandPatchPtrs.ContainsKey(key))
                    {
                        LandPatchData current = m_LandPatchPtrs[key];
                        while (current.Next != null)
                            current = current.Next;
                        current.Next = new LandPatchData(i, ptr);
                    }
                    else
                    {
                        m_LandPatchPtrs.Add(key, new LandPatchData(i, ptr));
                    }
                    ptr += 192;
                }

                indexReader.Close();

                return count;
            }
        }

        public unsafe bool TryGetStaticChunk(uint map, uint blockX, uint blockY, ref byte[] staticData, out int length)
        {
            try
            {
                length = 0;
                if (ClientVersion.InstallationIsUopFormat)
                {
                    return false;
                }
                uint key = MakeChunkKey(blockX, blockY);
                StaticPatchData data;
                if (m_StaticPatchPtrs.TryGetValue(key, out data))
                {
                    if (data.Index >= EnabledDiffs.StaticPatches[map])
                    {
                        return false;
                    }
                    while (data.Next != null)
                    {
                        if (data.Next.Index >= EnabledDiffs.StaticPatches[map])
                            break;
                        data = data.Next;
                    }
                    if (data.Pointer == 0 || data.Length <= 0)
                    {
                        return false;
                    }
                    length = data.Length;
                    m_StaticPatchStream.Seek(data.Pointer, SeekOrigin.Begin);
                    if (length > staticData.Length)
                    {
                        staticData = new byte[length];
                    }
                    fixed (byte* pStaticTiles = staticData)
                    {
                        NativeMethods.ReadBuffer(m_StaticPatchStream.SafeFileHandle, pStaticTiles, length);
                    }
                    return true;
                }
                length = 0;
                return false;
            }
            catch (EndOfStreamException)
            {
                throw new Exception("End of stream in static patch block!");
            }
        }

        private unsafe int LoadStaticPatches(TileMatrixData tileMatrix, string dataPath, string indexPath, string lookupPath)
        {
            m_StaticPatchPtrs = new Dictionary<uint, StaticPatchData>();

            m_StaticPatchStream = FileManager.GetFile(dataPath);
            if (m_StaticPatchStream == null)
                return 0;

            using (FileStream fsIndex = FileManager.GetFile(indexPath))
            {
                using (FileStream fsLookup = FileManager.GetFile(lookupPath))
                {
                    BinaryReader indexReader = new BinaryReader(fsIndex);
                    BinaryReader lookupReader = new BinaryReader(fsLookup);

                    int count = (int)(indexReader.BaseStream.Length / 4);

                    for (uint i = 0; i < count; ++i)
                    {
                        uint blockID = indexReader.ReadUInt32();
                        uint blockX = blockID / tileMatrix.ChunkHeight;
                        uint blockY = blockID % tileMatrix.ChunkHeight;
                        uint key = MakeChunkKey(blockX, blockY);
                        uint offset = lookupReader.ReadUInt32();
                        int length = lookupReader.ReadInt32();
                        lookupReader.ReadInt32();
                        if (m_StaticPatchPtrs.ContainsKey(key))
                        {
                            StaticPatchData current = m_StaticPatchPtrs[key];
                            while (current.Next != null)
                                current = current.Next;
                            current.Next = new StaticPatchData(i, offset, length);
                        }
                        else
                        {
                            m_StaticPatchPtrs.Add(key, new StaticPatchData(i, offset, length));
                        }
                    }

                    indexReader.Close();
                    lookupReader.Close();

                    return count;
                }
            }
        }
    }
}