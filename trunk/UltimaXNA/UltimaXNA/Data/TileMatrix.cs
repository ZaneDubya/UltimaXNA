/***************************************************************************
 *                               TileMatrix.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: TileMatrix.cs 252 2007-09-14 07:59:32Z mark $
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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace UltimaXNA.Data
{
    public class TileMatrix
    {
        private static int[] m_MapBlockHeightList = new int[] { 512, 512, 200, 256, 181 };
        private static StaticTileList[][] m_StaticTileLists;
        private static StaticTile[] m_StaticTileBuffer = new StaticTile[128];

        private StaticTile[][][] m_EmptyStaticsBlock;
        private Tile[] m_InvalidLandBlock;
        private int[][] m_LandPatches;
        private Tile[][][] m_LandTiles;
        private int[][] m_StaticsPatches;
        private StaticTile[][][][][] m_StaticTiles;

        private int m_BlockHeight;
        public int BlockHeight
        {
            get { return m_BlockHeight; }
        }

        private int m_BlockWidth;
        public int BlockWidth
        {
            get { return m_BlockWidth; }
        }

        public StaticTile[][][] EmptyStaticsBlock
        {
            get { return m_EmptyStaticsBlock; }
        }

        private int m_Height;
        public int Height
        {
            get { return m_Height; }
        }

        private BinaryReader m_IndexReader;
        public BinaryReader IndexReader
        {
            get { return m_IndexReader; }
            set { m_IndexReader = value; }
        }

        private FileStream m_IndexStream;
        public FileStream IndexStream
        {
            get { return m_IndexStream; }
            set { m_IndexStream = value; }
        }

        private FileStream m_MapStream;
        public FileStream MapStream
        {
            get { return m_MapStream; }
            set { m_MapStream = value; }
        }

        private TileMatrixPatch m_Patch;
        public TileMatrixPatch Patch
        {
            get { return m_Patch; }
        }

        private FileStream m_Statics;
        public FileStream Statics
        {
            get { return m_Statics; }
            set { m_Statics = value; }
        }

        private int m_Width;
        public int Width
        {
            get { return m_Width; }
        }

        public TileMatrix(int index, int id)
        {
            m_MapStream = FileManager.GetFile("map{0}.mul", index);
            m_IndexStream = FileManager.GetFile("staidx{0}.mul", index);
            m_IndexReader = new BinaryReader(m_IndexStream);
            m_Statics = FileManager.GetFile("statics{0}.mul", index);

            m_Height = m_MapBlockHeightList[index] << 3;
            m_BlockHeight = m_MapBlockHeightList[index];
            m_Width = (int)m_MapStream.Length / (m_BlockHeight * 196) << 3;
            m_BlockWidth = m_Width >> 3;

            m_EmptyStaticsBlock = new StaticTile[8][][];

            for (int i = 0; i < 8; ++i)
            {
                m_EmptyStaticsBlock[i] = new StaticTile[8][];

                for (int j = 0; j < 8; ++j)
                {
                    m_EmptyStaticsBlock[i][j] = new StaticTile[0];
                }
            }

            m_InvalidLandBlock = new Tile[196];
            m_LandPatches = new int[m_BlockWidth][];
            m_LandTiles = new Tile[m_BlockWidth][][];
            m_StaticTiles = new StaticTile[m_BlockWidth][][][][];
            m_StaticsPatches = new int[m_BlockWidth][];

            m_Patch = new TileMatrixPatch(this, id);
        }

        public void SetStaticBlock(int x, int y, StaticTile[][][] value)
        {
            if (x < 0 || y < 0 || x >= m_BlockWidth || y >= m_BlockHeight)
            {
                return;
            }

            if (m_StaticTiles[x] == null)
            {
                m_StaticTiles[x] = new StaticTile[m_BlockHeight][][][];
            }

            m_StaticTiles[x][y] = value;

            if (m_StaticsPatches[x] == null)
            {
                m_StaticsPatches[x] = new int[(m_BlockHeight + 31) >> 5];
            }

            m_StaticsPatches[x][y >> 5] |= 1 << (y & 0x1F);
        }

        public StaticTile[][][] GetStaticBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= m_BlockWidth || y >= m_BlockHeight || m_Statics == null || m_IndexStream == null)
            {
                return m_EmptyStaticsBlock;
            }

            if (m_StaticTiles[x] == null)
            {
                m_StaticTiles[x] = new StaticTile[m_BlockHeight][][][];
            }

            if (m_StaticTiles[x][y] == null)
            {
                m_StaticTiles[x][y] = ReadStaticBlock(x, y);
            }

            return m_StaticTiles[x][y];
        }

        public StaticTile[] GetStaticTiles(int x, int y)
        {
            StaticTile[][][] tiles = GetStaticBlock(x >> 3, y >> 3);

            return tiles[x & 0x07][y & 0x07];
        }

        public void SetLandBlock(int x, int y, Tile[] value)
        {
            if (x < 0 || y < 0 || x >= m_BlockWidth || y >= m_BlockHeight)
            {
                return;
            }

            if (m_LandTiles[x] == null)
            {
                m_LandTiles[x] = new Tile[m_BlockHeight][];
            }

            m_LandTiles[x][y] = value;

            if (m_LandPatches[x] == null)
            {
                m_LandPatches[x] = new int[(m_BlockHeight + 31) >> 5];
            }

            m_LandPatches[x][y >> 5] |= 1 << (y & 0x1F);
        }

        public Tile[] GetLandBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= m_BlockWidth || y >= m_BlockHeight || m_MapStream == null)
            {
                return m_InvalidLandBlock;
            }

            if (m_LandTiles[x] == null)
            {
                m_LandTiles[x] = new Tile[m_BlockHeight][];
            }

            if (m_LandTiles[x][y] == null)
            {
                m_LandTiles[x][y] = ReadLandBlock(x, y);
            }

            return m_LandTiles[x][y];
        }

        public Tile GetLandTile(int x, int y)
        {
            Tile[] tiles = GetLandBlock(x >> 3, y >> 3);

            return tiles[((y & 0x07) << 3) + (x & 0x07)];
        }

        private unsafe StaticTile[][][] ReadStaticBlock(int x, int y)
        {
            try
            {
                m_IndexReader.BaseStream.Seek(((x * m_BlockHeight) + y) * 12, SeekOrigin.Begin);

                int lookup = m_IndexReader.ReadInt32();
                int length = m_IndexReader.ReadInt32();

                if (lookup < 0 || length <= 0)
                {
                    return m_EmptyStaticsBlock;
                }
                else
                {
                    int count = length / 7;

                    m_Statics.Seek(lookup, SeekOrigin.Begin);

                    if (m_StaticTileBuffer.Length < count)
                    {
                        m_StaticTileBuffer = new StaticTile[count];
                    }

                    StaticTile[] staticTiles = m_StaticTileBuffer;

                    fixed (StaticTile* pStaticTiles = staticTiles)
                    {
                        NativeMethods.Read(m_Statics.SafeFileHandle, pStaticTiles, length);

                        if (m_StaticTileLists == null)
                        {
                            m_StaticTileLists = new StaticTileList[8][];

                            for (int i = 0; i < 8; ++i)
                            {
                                m_StaticTileLists[i] = new StaticTileList[8];

                                for (int j = 0; j < 8; ++j)
                                {
                                    m_StaticTileLists[i][j] = new StaticTileList();
                                }
                            }
                        }

                        StaticTileList[][] staticTileLists = m_StaticTileLists;

                        StaticTile* pCurrent = pStaticTiles;
                        StaticTile* pEnd = pStaticTiles + count;

                        while (pCurrent < pEnd)
                        {
                            staticTileLists[pCurrent->X & 0x07][pCurrent->Y & 0x07].Add((short)((pCurrent->ID & 0x3FFF) + 0x4000), pCurrent->Z);

                            pCurrent++;
                        }

                        StaticTile[][][] tiles = new StaticTile[8][][];

                        for (int i = 0; i < 8; ++i)
                        {
                            tiles[i] = new StaticTile[8][];

                            for (int j = 0; j < 8; ++j)
                            {
                                tiles[i][j] = staticTileLists[i][j].ToArray();
                            }
                        }

                        return tiles;
                    }
                }
            }
            catch (EndOfStreamException)
            {
                return m_EmptyStaticsBlock;
            }
        }

        private unsafe Tile[] ReadLandBlock(int x, int y)
        {
            try
            {
                m_MapStream.Seek(((x * m_BlockHeight) + y) * 196 + 4, SeekOrigin.Begin);

                int streamStart = (int)m_MapStream.Position;

                Tile[] tiles = new Tile[64];

                fixed (Tile* pTiles = tiles)
                {
                    NativeMethods.Read(m_MapStream.SafeFileHandle, pTiles, 192);
                }

                Metrics.ReportDataRead((int)m_MapStream.Position - streamStart);

                return tiles;
            }
            catch
            {
                return m_InvalidLandBlock;
            }
        }

        public void Dispose()
        {
            if (m_IndexReader != null)
            {
                m_IndexReader.Close();
            }

            if (m_MapStream != null)
            {
                m_MapStream.Close();
            }

            if (m_Statics != null)
            {
                m_Statics.Close();
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StaticTile : IComparable<StaticTile>
    {
        public short ID;
        public byte X;
        public byte Y;
        public sbyte Z;
        public short Hue;

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("ID: " + ID.ToString());
            stringBuilder.AppendLine("X: " + X.ToString());
            stringBuilder.AppendLine("Y: " + Y.ToString());
            stringBuilder.AppendLine("Z: " + Z.ToString());
            stringBuilder.AppendLine("Hue: " + Hue.ToString());

            return stringBuilder.ToString();
        }

        public int CompareTo(StaticTile t)
        {
            return (Z - t.Z);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Tile
    {
        public short ID;
        public sbyte Z;

        public Tile(short id, sbyte z)
        {
            ID = id;
            Z = z;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("ID: " + ID.ToString());
            stringBuilder.AppendLine("Z: " + Z.ToString());

            return stringBuilder.ToString();
        }
    }
}

/*using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace UltimaXNA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct LandTile
    {
        public short ID;
        public sbyte Z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StaticTile
    {
        public short ID;
        public byte X;
        public byte Y;
        public sbyte Z;
        public short Hue;
    }

    class TileMatrix
    {
        private static int[] m_MapBlockHeightList = new int[] {*/
            //512 /* Felucca */,
            //512 /* Trammel */,
            //200 /* Ilshenar */,
            //256 /* Malas */, 
            //181 /* Tokuno */
        /*};

        private int m_Height;
        private FileStream m_Map;
        private int m_BlockHeight;
        private int m_BlockWidth;
        private LandTile[][][] m_LandTiles;
        private FileStream m_Statics;
        private FileStream m_StaticsIndex;
        private int m_Width;

        public TileMatrix(int index)
        {
            m_Map = FileManager.GetFile("map{0}.mul", index);

            m_Statics = FileManager.GetFile("statics{0}.mul", index);

            m_StaticsIndex = FileManager.GetFile("staidx{0}.mul", index);

            m_Height = m_MapBlockHeightList[index] << 3;

            m_BlockHeight = m_MapBlockHeightList[index];

            m_Width = (int)m_Map.Length / (m_BlockHeight * 196) << 3;

            m_BlockWidth = m_Width >> 3;

            m_LandTiles = new LandTile[m_BlockWidth][][];
        }

        private LandTile[] GetLandBlock(int x, int y)
        {
            if (m_LandTiles[x] == null)
            {
                m_LandTiles[x] = new LandTile[m_BlockHeight][];
            }

            LandTile[] landTiles = m_LandTiles[x][y];

            if (landTiles == null)
            {
                m_LandTiles[x][y] = landTiles = ReadLandBlock(x, y);
            }

            return landTiles;
        }

        public LandTile GetLandTile(int x, int y)
        {
            LandTile[] landTile = GetLandBlock(x >> 3, y >> 3);

            return landTile[((y & 0x07) << 3) + (x & 0x07)];
        }

        private unsafe LandTile[] ReadLandBlock(int x, int y)
        {
            m_Map.Seek(((x * m_BlockHeight) + y) * 196 + 4, SeekOrigin.Begin);

            LandTile[] landTiles = new LandTile[64];

            fixed (LandTile* pLandTiles = landTiles)
            {
                NativeMethods.Read(m_Map.SafeFileHandle.DangerousGetHandle(), pLandTiles, 192);
            }

            return landTiles;
        }
    }
}*/