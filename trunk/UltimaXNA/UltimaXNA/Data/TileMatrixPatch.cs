﻿/***************************************************************************
 *                            TileMatrixPatch.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: TileMatrixPatch.cs 252 2007-09-14 07:59:32Z mark $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.IO;
#endregion

namespace UltimaXNA.Data
{
    public class TileMatrixPatch
    {
        private static bool m_Enabled = true;
        public static bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        private int m_LandBlocks;
        public int LandBlocks
        {
            get { return m_LandBlocks; }
        }

        private int m_StaticBlocks;
        public int StaticBlocks
        {
            get { return m_StaticBlocks; }
        }

        public TileMatrixPatch(TileMatrix matrix, int index)
        {
            if (!m_Enabled)
            {
                return;
            }

            m_LandBlocks = PatchLand(matrix, String.Format("mapdif{0}.mul", index), String.Format("mapdifl{0}.mul", index));
            m_StaticBlocks = PatchStatics(matrix, String.Format("stadif{0}.mul", index), String.Format("stadifl{0}.mul", index), String.Format("stadifi{0}.mul", index));
        }

        private unsafe int PatchLand(TileMatrix tileMatrix, string dataPath, string indexPath)
        {
            using (FileStream fsData = FileManager.GetFile(dataPath))
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
                            NativeMethods.Read(fsData.SafeFileHandle.DangerousGetHandle(), pTiles, 192);
                        }

                        tileMatrix.SetLandBlock(x, y, tiles);
                    }

                    indexReader.Close();

                    return count;
                }
            }
        }

        private static StaticTile[] m_TileBuffer = new StaticTile[128];

        private unsafe int PatchStatics(TileMatrix tileMatrix, string dataPath, string indexPath, string lookupPath)
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
                                NativeMethods.Read(fsData.SafeFileHandle.DangerousGetHandle(), pStaticTiles, length);

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