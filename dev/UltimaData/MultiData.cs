/***************************************************************************
 *   Multis.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using InterXLib;
#endregion

namespace UltimaXNA.UltimaData
{
    public class MultiData
    {
        private static MultiComponentList[] m_Components = new MultiComponentList[0x4000];
        public static MultiComponentList[] Cache { get { return m_Components; } }

        private static FileIndex m_FileIndex = new FileIndex("Multi.idx", "Multi.mul", 0x4000, 14);
        public static FileIndex FileIndex { get { return m_FileIndex; } }

        public static MultiComponentList GetComponents(int index)
        {
            MultiComponentList mcl;

            index &= 0x3FFF;

            if (index >= 0 && index < m_Components.Length)
            {
                mcl = m_Components[index];

                if (mcl == null)
                    m_Components[index] = mcl = Load(index);
            }
            else
            {
                mcl = MultiComponentList.Empty;
            }

            return mcl;
        }

        public static MultiComponentList Load(int index)
        {
            try
            {
                int length, extra;
                bool patched;
                BinaryFileReader reader = m_FileIndex.Seek(index, out length, out extra, out patched);
                if (reader == null)
                    return MultiComponentList.Empty;

                return new MultiComponentList(reader, length / 12);
            }
            catch
            {
                return MultiComponentList.Empty;
            }
        }
    }

    public sealed class MultiComponentList
    {
        private Point m_Min, m_Max, m_Center;
        private int m_Width, m_Height;
        private StaticTile[][][] m_Tiles;

        public static readonly MultiComponentList Empty = new MultiComponentList();

        public Point Min { get { return m_Min; } }
        public Point Max { get { return m_Max; } }
        public Point Center { get { return m_Center; } }
        public int Width { get { return m_Width; } }
        public int Height { get { return m_Height; } }
        public StaticTile[][][] Tiles { get { return m_Tiles; } }

        private struct MultiTileEntry
        {
            public short m_ItemID;
            public short m_OffsetX, m_OffsetY, m_OffsetZ;
            public int m_Flags;
        }

        public MultiComponentList(BinaryFileReader reader, int count)
        {
            int metrics_dataread_start = (int)reader.Position;

            m_Min = m_Max = Point.Empty;

            MultiTileEntry[] allTiles = new MultiTileEntry[count];

            for (int i = 0; i < count; ++i)
            {
                allTiles[i].m_ItemID = reader.ReadShort();
                allTiles[i].m_OffsetX = reader.ReadShort();
                allTiles[i].m_OffsetY = reader.ReadShort();
                allTiles[i].m_OffsetZ = reader.ReadShort();
                allTiles[i].m_Flags = reader.ReadInt();

                MultiTileEntry e = allTiles[i];

                if (e.m_OffsetX < m_Min.X)
                    m_Min.X = e.m_OffsetX;

                if (e.m_OffsetY < m_Min.Y)
                    m_Min.Y = e.m_OffsetY;

                if (e.m_OffsetX > m_Max.X)
                    m_Max.X = e.m_OffsetX;

                if (e.m_OffsetY > m_Max.Y)
                    m_Max.Y = e.m_OffsetY;
            }

            m_Center = new Point(-m_Min.X, -m_Min.Y);
            m_Width = (m_Max.X - m_Min.X) + 1;
            m_Height = (m_Max.Y - m_Min.Y) + 1;

            StaticTileList[][] tiles = new StaticTileList[m_Width][];
            m_Tiles = new StaticTile[m_Width][][];

            for (int x = 0; x < m_Width; ++x)
            {
                tiles[x] = new StaticTileList[m_Height];
                m_Tiles[x] = new StaticTile[m_Height][];

                for (int y = 0; y < m_Height; ++y)
                    tiles[x][y] = new StaticTileList();
            }

            for (int i = 0; i < allTiles.Length; ++i)
            {
                int xOffset = allTiles[i].m_OffsetX + m_Center.X;
                int yOffset = allTiles[i].m_OffsetY + m_Center.Y;

                tiles[xOffset][yOffset].Add((short)((allTiles[i].m_ItemID & 0x3FFF) + 0x4000), (sbyte)allTiles[i].m_OffsetZ);
            }

            for (int x = 0; x < m_Width; ++x)
            {
                for (int y = 0; y < m_Height; ++y)
                {
                    m_Tiles[x][y] = tiles[x][y].ToArray();
                    // 
                    // This is unnecessary, since TileEngine has to sort/resort anyways.
                    // 
                    // if (m_Tiles[x][y].Length > 1)
                    // {
                    //     Array.Sort(m_Tiles[x][y]);
                    // }
                }
            }

            Diagnostics.Metrics.ReportDataRead((int)reader.Position - metrics_dataread_start);
        }

        private MultiComponentList()
        {
            m_Tiles = new StaticTile[0][][];
        }
    }
}