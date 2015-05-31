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

        private BinaryReader m_LandPatches;
        private BinaryReader m_StaticPatches;

        public TileMatrixClientPatch(TileMatrixClient matrix, uint index)
        {
            if (!m_Enabled)
            {
                return;
            }

            m_LandBlocks = LoadLandPatches(matrix, String.Format("mapdif{0}.mul", index), String.Format("mapdifl{0}.mul", index));
            m_StaticBlocks = LoadStaticPatches(matrix, String.Format("stadif{0}.mul", index), String.Format("stadifl{0}.mul", index), String.Format("stadifi{0}.mul", index));
        }

        public bool TryGetLandBlock(uint blockX, uint blockY, out byte[] landData)
        {
            landData = null;

            return false;
        }

        public bool TryGetStaticBlock(uint blockX, uint blockY, out byte[] staticData)
        {
            staticData = null;

            return false;
        }

        private unsafe int LoadLandPatches(TileMatrix tileMatrix, string landPath, string indexPath)
        {
            using (FileStream fsData = FileManager.GetFile(landPath))
            {
                using (FileStream fsIndex = FileManager.GetFile(indexPath))
                {
                    BinaryReader indexReader = new BinaryReader(fsIndex);

                    int count = (int)(indexReader.BaseStream.Length / 4);

                    for (int i = 0; i < count; ++i)
                    {
                        int blockID = indexReader.ReadInt32();
                        int x = blockID / tileMatrix.BlockHeight;
                        int y = blockID % tileMatrix.BlockHeight;

                        fsData.Seek(4, SeekOrigin.Current);

                        Tile[] tiles = new Tile[64];

                        fixed (Tile* pTiles = tiles)
                        {
                            NativeMethods.Read(fsData.SafeFileHandle, pTiles, 192);
                        }

                        tileMatrix.SetLandBlock(x, y, tiles);
                    }

                    indexReader.Close();

                    return count;
                }
            }
        }

        private static StaticTile[] m_TileBuffer = new StaticTile[128];

        private unsafe int LoadStaticPatches(TileMatrix tileMatrix, string dataPath, string indexPath, string lookupPath)
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
                            int blockID = indexReader.ReadInt32();
                            int blockX = blockID / tileMatrix.BlockHeight;
                            int blockY = blockID % tileMatrix.BlockHeight;

                            int offset = lookupReader.ReadInt32();
                            int length = lookupReader.ReadInt32();
                            lookupReader.ReadInt32();

                            if (offset < 0 || length <= 0)
                            {
                                tileMatrix.SetStaticBlock(blockX, blockY, tileMatrix.EmptyStaticsBlock);

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

                                tileMatrix.SetStaticBlock(blockX, blockY, tiles);
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