/***************************************************************************
 *   TileMatrixDataPatch.cs
 *   Based on TileMatrixPatch.cs (c) The RunUO Software Team
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
using UltimaXNA.Core;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public class TileMatrixDataPatch
    {
        private static bool m_Enabled = true;
        public static bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        private FileStream m_LandPatchStream;
        private FileStream m_StaticPatchStream;

        private Dictionary<uint, uint> m_LandPatchPtrs;
        private Dictionary<uint, Tuple<int, int>> m_StaticPatchPtrs;

        public TileMatrixDataPatch(TileMatrixData matrix, uint index)
        {
            if (!m_Enabled)
            {
                return;
            }

            LoadLandPatches(matrix, String.Format("mapdif{0}.mul", index), String.Format("mapdifl{0}.mul", index));
            LoadStaticPatches(matrix, String.Format("stadif{0}.mul", index), String.Format("stadifl{0}.mul", index), String.Format("stadifi{0}.mul", index));
        }

        private uint MakeChunkKey(uint blockX, uint blockY)
        {
            return ((blockY & 0x0000ffff) << 16) | (blockX & 0x0000ffff);
        }

        public unsafe bool TryGetLandPatch(uint blockX, uint blockY, ref byte[] landData)
        {
            uint key = MakeChunkKey(blockX, blockY);
            uint ptr;

            if (m_LandPatchPtrs.TryGetValue(key, out ptr))
            {
                m_LandPatchStream.Seek(ptr, SeekOrigin.Begin);

                landData = new byte[192];
                fixed (byte* pTiles = landData)
                {
                    NativeMethods.Read(m_LandPatchStream.SafeFileHandle, pTiles, 192);
                }
                return true;
            }

            return false;
        }

        private unsafe int LoadLandPatches(TileMatrixData tileMatrix, string landPath, string indexPath)
        {
            m_LandPatchPtrs = new Dictionary<uint, uint>();

            m_LandPatchStream = FileManager.GetFile(landPath);

            using (FileStream fsIndex = FileManager.GetFile(indexPath))
            {
                BinaryReader indexReader = new BinaryReader(fsIndex);

                int count = (int)(indexReader.BaseStream.Length / 4);

                uint ptr = 0;

                for (int i = 0; i < count; ++i)
                {

                    uint blockID = indexReader.ReadUInt32();
                    uint x = blockID / tileMatrix.ChunkHeight;
                    uint y = blockID % tileMatrix.ChunkHeight;
                    uint key = MakeChunkKey(x, y);

                    ptr += 4;

                    m_LandPatchPtrs.Add(key, ptr);

                    ptr += 192;
                }

                indexReader.Close();

                return count;
            }
        }

        public unsafe bool TryGetStaticChunk(uint blockX, uint blockY, ref byte[] staticData, out int length)
        {
            try
            {
                uint key = MakeChunkKey(blockX, blockY);
                Tuple<int, int> ptr; // offset, length
                if (m_StaticPatchPtrs.TryGetValue(key, out ptr))
                {
                    int offset = ptr.Item1;
                    length = ptr.Item2;

                    if (offset < 0 || length <= 0)
                    {
                        return false;
                    }

                    m_StaticPatchStream.Seek(offset, SeekOrigin.Begin);

                    if (length > staticData.Length)
                        staticData = new byte[length];

                    fixed (byte* pStaticTiles = staticData)
                    {
                        NativeMethods.Read(m_StaticPatchStream.SafeFileHandle, pStaticTiles, length);
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
            m_StaticPatchPtrs = new Dictionary<uint, Tuple<int, int>>();

            m_StaticPatchStream = FileManager.GetFile(dataPath);

            using (FileStream fsIndex = FileManager.GetFile(indexPath))
            {
                using (FileStream fsLookup = FileManager.GetFile(lookupPath))
                {
                    BinaryReader indexReader = new BinaryReader(fsIndex);
                    BinaryReader lookupReader = new BinaryReader(fsLookup);

                    int count = (int)(indexReader.BaseStream.Length / 4);

                    for (int i = 0; i < count; ++i)
                    {
                        uint blockID = indexReader.ReadUInt32();
                        uint blockX = blockID / tileMatrix.ChunkHeight;
                        uint blockY = blockID % tileMatrix.ChunkHeight;
                        uint key = MakeChunkKey(blockX, blockY);

                        int offset = lookupReader.ReadInt32();
                        int length = lookupReader.ReadInt32();
                        lookupReader.ReadInt32();

                        if (m_StaticPatchPtrs.ContainsKey(key))
                        {
                            // Tuple<int, int> old = m_StaticPatchPtrs[key];
                            m_StaticPatchPtrs[key] = new Tuple<int, int>(offset, length);
                        }
                        else
                        {
                            m_StaticPatchPtrs.Add(key, new Tuple<int, int>(offset, length));
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