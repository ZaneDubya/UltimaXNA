#region File Description & Usings
//-----------------------------------------------------------------------------
// Art.cs
//
// Based on UltimaSDK, modifications by Poplicola & ClintXNA
//-----------------------------------------------------------------------------
using System.IO;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UndeadClient.DataLocal
{
    class Art
    {
        private static Texture2D[] m_Cache = new Texture2D[0x10000];
        private static FileIndexClint m_IndexFile = new FileIndexClint("artidx.mul", "art.mul");
        private static ushort[][] m_StaticDimensions = new ushort[0x4000][];

        public static Texture2D GetLandTexture(int index, GraphicsDevice graphicsDevice)
        {
            index &= 0x3FFF;

            Texture2D data = m_Cache[index];

            if (data == null)
            {
                m_Cache[index] = data = ReadLandTexture(index, graphicsDevice);
            }

            return data;
        }

        public static void GetStaticDimensions(int index, out int width, out int height)
        {
            index &= 0x3FFF;

            ushort[] dimensions = m_StaticDimensions[index];

            if (dimensions == null)
            {
                m_StaticDimensions[index] = dimensions = ReadStaticDimensions(index + 0x4000);
            }

            width = dimensions[0];
            height = dimensions[1];
        }

        public static Texture2D GetStaticTexture(int index, GraphicsDevice graphicsDevice)
        {
            index &= 0x3FFF;
            index += 0x4000;

            Texture2D data = m_Cache[index];

            if (data == null)
            {
                m_Cache[index] = data = ReadStaticTexture(index, graphicsDevice);
            }

            return data;
        }

        private static unsafe Texture2D ReadLandTexture(int index, GraphicsDevice graphicsDevice)
        {
            m_IndexFile.Seek(index);

            ushort[] data = new ushort[44 * 44];

            int count = 2;
            int offset = 21;

            fixed (ushort* pData = data)
            {
                ushort* dataRef = pData;

                for (int y = 0; y < 22; y++, count += 2, offset--, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;

                    while (start < end)
                    {
                        *start++ = (ushort)(m_IndexFile.BinaryReader.ReadUInt16() ^ 0x8000);
                    }
                }

                count = 44;
                offset = 0;

                for (int y = 0; y < 22; y++, count -= 2, offset++, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;

                    while (start < end)
                    {
                        *start++ = (ushort)(m_IndexFile.BinaryReader.ReadUInt16() ^ 0x8000);
                    }
                }
            }

            Texture2D texture = new Texture2D(graphicsDevice, 44, 44, 1, TextureUsage.None, SurfaceFormat.Bgra5551);

            texture.SetData<ushort>(data);

            return texture;
        }

        private static ushort[] ReadStaticDimensions(int index)
        {
            m_IndexFile.Seek(index);
            m_IndexFile.BinaryReader.ReadInt32();

            return new ushort[] { (ushort)m_IndexFile.BinaryReader.ReadInt16(), (ushort)m_IndexFile.BinaryReader.ReadInt16() };
        }

        private static unsafe Texture2D ReadStaticTexture(int index, GraphicsDevice graphicsDevice)
        {
            m_IndexFile.Seek(index);
            m_IndexFile.BinaryReader.ReadInt32();

            int width = m_IndexFile.BinaryReader.ReadInt16();
            int height = m_IndexFile.BinaryReader.ReadInt16();

            if (width <= 0 || height <= 0)
            {
                return null;
            }

            if (m_StaticDimensions[index - 0x4000] == null)
            {
                m_StaticDimensions[index - 0x4000] = new ushort[] { (ushort)width, (ushort)height };
            }

            int[] lookups = new int[height];

            int dataStart = (int)m_IndexFile.BinaryReader.BaseStream.Position + (height * 2);

            for (int i = 0; i < height; i++)
            {
                lookups[i] = (int)(dataStart + (m_IndexFile.BinaryReader.ReadUInt16() * 2));
            }

            ushort[] data = new ushort[width * height];

            fixed (ushort* pData = data)
            {
                ushort* dataRef = pData;

                for (int y = 0; y < height; y++, dataRef += width)
                {
                    m_IndexFile.BinaryReader.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                    ushort* start = dataRef;

                    int count, offset;

                    while (((offset = m_IndexFile.BinaryReader.ReadUInt16()) + (count = m_IndexFile.BinaryReader.ReadUInt16())) != 0)
                    {
                        start += offset;
                        ushort* end = start + count;

                        while (start < end)
                        {
                            *start++ = (ushort)(m_IndexFile.BinaryReader.ReadUInt16() ^ 0x8000);
                        }
                    }
                }
            }

            Texture2D texture = new Texture2D(graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Bgra5551);

            texture.SetData<ushort>(data);

            return texture;
        }
    }
}