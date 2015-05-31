/***************************************************************************
 *   TileMatrixClientPatch.cs
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
using System.IO;
using UltimaXNA.Core;
using System.Collections.Generic;
#endregion

namespace UltimaXNA.Ultima.IO
{
    public class TileMatrixClientPatch
    {
        private static bool m_Enabled = true;
        public static bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        private FileStream m_LandPatchStream;

        private Dictionary<uint, uint> m_LandPatchPtrs;
        private Dictionary<uint, uint> m_StaticPatchPtrs;

        public TileMatrixClientPatch(TileMatrixClient matrix, uint index)
        {
            if (!m_Enabled)
            {
                return;
            }

            LoadLandPatches(matrix, String.Format("mapdif{0}.mul", index), String.Format("mapdifl{0}.mul", index));
            // LoadStaticPatches(matrix, String.Format("stadif{0}.mul", index), String.Format("stadifl{0}.mul", index), String.Format("stadifi{0}.mul", index));
        }

        public unsafe bool TryGetLandPatch(uint blockX, uint blockY, ref byte[] landData)
        {
            uint key = ((blockY & 0x0000ffff) << 16) | (blockX & 0x0000ffff);
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

        private unsafe int LoadLandPatches(TileMatrixClient tileMatrix, string landPath, string indexPath)
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
                    uint x = blockID / tileMatrix.BlockHeight;
                    uint y = blockID % tileMatrix.BlockHeight;
                    uint key = ((y & 0x0000ffff) << 16) | (x & 0x0000ffff);

                    ptr += 4;

                    m_LandPatchPtrs.Add(key, ptr);

                    ptr += 192;
                }

                indexReader.Close();

                return count;
            }
        }

        public bool TryGetStaticBlock(uint blockX, uint blockY, out byte[] staticData)
        {
            staticData = null;

            uint key = ((blockY & 0x0000ffff) << 16) | (blockX & 0x0000ffff);
            uint ptr;
            if (m_StaticPatchPtrs.TryGetValue(key, out ptr))
            {

                return true;
            }

            return false;
        }

        private static StaticTile[] m_TileBuffer = new StaticTile[128];

        private unsafe int LoadStaticPatches(TileMatrixClient tileMatrix, string dataPath, string indexPath, string lookupPath)
        {
            using (FileStream fsData = FileManager.GetFile(dataPath))
            {
                using (FileStream fsIndex = FileManager.GetFile(indexPath))
                {
                    using (FileStream fsLookup = FileManager.GetFile(lookupPath))
                    {
                        BinaryReader indexReader = new BinaryReader(fsIndex);
                        BinaryReader lookupReader = new BinaryReader(fsLookup);

                        int count = (int)(indexReader.BaseStream.Length / 4);

                        StaticTileList[][] lists = new StaticTileList[8][];

                        for (int x = 0; x < 8; ++x)
                        {
                            lists[x] = new StaticTileList[8];

                            for (int y = 0; y < 8; ++y)
                            {
                                lists[x][y] = new StaticTileList();
                            }
                        }

                        for (int i = 0; i < count; ++i)
                        {
                            uint blockID = indexReader.ReadUInt32();
                            uint blockX = blockID / tileMatrix.BlockHeight;
                            uint blockY = blockID % tileMatrix.BlockHeight;

                            int offset = lookupReader.ReadInt32();
                            int length = lookupReader.ReadInt32();
                            lookupReader.ReadInt32();

                            if (offset < 0 || length <= 0)
                            {
                                // tileMatrix.SetStaticBlock(blockX, blockY, tileMatrix.EmptyStaticsBlock);

                                continue;
                            }

                            fsData.Seek(offset, SeekOrigin.Begin);

                            int tileCount = length / 7;

                            if (m_TileBuffer.Length < tileCount)
                            {
                                m_TileBuffer = new StaticTile[tileCount];
                            }

                            StaticTile[] staticTiles = m_TileBuffer;

                            fixed (StaticTile* pStaticTiles = staticTiles)
                            {
                                NativeMethods.Read(fsData.SafeFileHandle, pStaticTiles, length);

                                StaticTile* pCur = pStaticTiles, pEnd = pStaticTiles + tileCount;

                                while (pCur < pEnd)
                                {
                                    lists[pCur->X & 0x07][pCur->Y & 0x07].Add((short)((pCur->ID & 0x3FFF) + 0x4000), pCur->Z);

                                    pCur = pCur + 1;
                                }

                                StaticTile[][][] tiles = new StaticTile[8][][];

                                for (int x = 0; x < 8; ++x)
                                {
                                    tiles[x] = new StaticTile[8][];

                                    for (int y = 0; y < 8; ++y)
                                    {
                                        tiles[x][y] = lists[x][y].ToArray();
                                    }
                                }

                                // tileMatrix.SetStaticBlock(blockX, blockY, tiles);
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
}