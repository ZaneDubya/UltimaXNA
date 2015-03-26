/***************************************************************************
 *                               TileMatrix.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: TileMatrix.cs 252 2007-09-14 07:59:32Z mark $
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
using UltimaXNA.Core.Diagnostics;
#endregion

namespace UltimaXNA.UltimaData
{
    public class TileMatrixRaw
    {
        private static int[] m_MapBlockHeightList = new int[] { 512, 512, 200, 256, 181 };
        private const int m_size_LandBlock = 196;
        private const int m_size_LandBlockData = 192;

        private byte[] m_EmptyStaticsBlock;
        private byte[] m_InvalidLandBlock;

        private const int m_bufferedLandBlocksMaxCount = 256; 
        private byte[][] m_bufferedLandBlocks;
        private int[] m_bufferedLandBlocks_Keys;

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

        public byte[] EmptyStaticsBlock
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

        private List<TileMatrix> m_FileShare = new List<TileMatrix>();

        public TileMatrixRaw(int index, int id)
        {
            m_MapStream = FileManager.GetFile("map{0}.mul", index);
            m_IndexStream = FileManager.GetFile("staidx{0}.mul", index);
            m_Statics = FileManager.GetFile("statics{0}.mul", index);

            if (m_MapStream == null)
            {
                // the map we tried to load does not exist.
                if (index == 1)
                {
                    index = 0;
                    m_MapStream = FileManager.GetFile("map{0}.mul", index);
                    m_IndexStream = FileManager.GetFile("staidx{0}.mul", index);
                    m_Statics = FileManager.GetFile("statics{0}.mul", index);
                }
            }

            m_IndexReader = new BinaryReader(m_IndexStream);

            m_Height = m_MapBlockHeightList[index] << 3;
            m_BlockHeight = m_MapBlockHeightList[index];
            m_Width = (int)m_MapStream.Length / (m_BlockHeight * m_size_LandBlock) << 3;
            m_BlockWidth = m_Width >> 3;

            m_EmptyStaticsBlock = new byte[0];
            m_InvalidLandBlock = new byte[m_size_LandBlockData];
            m_bufferedLandBlocks_Keys = new int[m_bufferedLandBlocksMaxCount];
            m_bufferedLandBlocks = new byte[m_bufferedLandBlocksMaxCount][];
            for (int i = 0; i < m_bufferedLandBlocksMaxCount; i++)
                m_bufferedLandBlocks[i] = new byte[m_size_LandBlockData];
        }

        public byte[] GetLandBlock(int x, int y)
        {
            if (m_MapStream == null)
            {
                return m_InvalidLandBlock;
            }
            else
            {
                return readLandBlock_Bytes(x, y);
            }
        }

        public void GetLandTile(int x, int y, out int TileID, out int alt)
        {
            int index = (((x % 8) + (y % 8) * 8) * 3);
            byte[] data = readLandBlock_Bytes(x >> 3, y >> 3);
            TileID = BitConverter.ToInt16(data, index);
            alt = (sbyte)data[index + 2];
        }

        public byte[] GetStaticBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= m_BlockWidth || y >= m_BlockHeight || m_Statics == null || m_IndexStream == null)
            {
                return m_EmptyStaticsBlock;
            }
            else
            {
                return readStaticBlock_Bytes(x, y);
            }
        }

        private unsafe byte[] readStaticBlock_Bytes(int x, int y)
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
                    m_Statics.Seek(lookup, SeekOrigin.Begin);

                    byte[] staticTiles = new byte[length];

                    fixed (byte* pStaticTiles = staticTiles)
                    {
                        SharedMethods.Read(m_Statics.SafeFileHandle, pStaticTiles, length);
                    }
                    return staticTiles;
                }
            }
            catch (EndOfStreamException)
            {
                throw new Exception("End of stream in static block!");
                // return m_EmptyStaticsBlock;
            }
        }

        private unsafe byte[] readLandBlock_Bytes(int x, int y)
        {
            if (x < 0) x += this.Width;
            if (x >= this.Width) x -= this.Width;
            if (y < 0) y += this.Height;
            if (y >= this.Height) y -= this.Height;

            int key = (x << 16) + y;
            int index = x % 16 + (y % 16) * 16;
            if (m_bufferedLandBlocks_Keys[index] == key)
                return m_bufferedLandBlocks[index];

            m_bufferedLandBlocks_Keys[index] = key;

            m_MapStream.Seek(((x * m_BlockHeight) + y) * m_size_LandBlock + 4, SeekOrigin.Begin);
            int streamStart = (int)m_MapStream.Position;
            fixed (byte* pData = m_bufferedLandBlocks[index])
            {
                SharedMethods.Read(m_MapStream.SafeFileHandle, pData, m_size_LandBlockData);
            }
            Metrics.ReportDataRead((int)m_MapStream.Position - streamStart);

            return m_bufferedLandBlocks[index];
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
}
