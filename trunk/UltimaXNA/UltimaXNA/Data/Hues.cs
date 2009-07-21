/***************************************************************************
 *   Hues.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.Data
{
    public class HuesXNA
    {
        private static GraphicsDevice graphicsDevice;
        private static Texture2D hueTexture;
        private const int m_HueTextureWidth = 64; // Each hue is 32 pixels wide, so divided by 32 = 2 hues wide.
        private const int m_HueTextureHeight = 2024;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            HuesXNA.graphicsDevice = graphicsDevice;
            graphicsDevice.DeviceReset += graphicsDevice_DeviceReset;
            CreateTexture();
        }

        static void graphicsDevice_DeviceReset(object sender, System.EventArgs e)
        {
            CreateTexture();
        }

        private static void CreateTexture()
        {
            hueTexture = new Texture2D(graphicsDevice, m_HueTextureWidth, m_HueTextureHeight, 1, TextureUsage.None, SurfaceFormat.Bgra5551);
            ushort[] iTextData = GetTextureData();
            hueTexture.SetData(iTextData);
        }

        private static ushort[] GetTextureData()
        {
            BinaryReader reader = new BinaryReader( FileManager.GetFile( "hues.mul" ) );
            int currentHue = 0;
            ushort[] textureData = new ushort[m_HueTextureWidth * m_HueTextureHeight];

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                reader.ReadInt32(); //Header
                
                for (int entry = 0; entry < 8; entry++)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        textureData[currentHue * 32 + i] = (ushort)(reader.ReadUInt16() | 0x8000);
                    }
                    currentHue++;
                    reader.ReadInt16(); //table start
                    reader.ReadInt16(); //table end
                    reader.ReadBytes( 20 ); //name
                }
            }
            reader.Close();
            return textureData;
        }

        public static Texture2D HueTexture
        {
            get
            {
                return hueTexture;
            }
        }
    }

    public class Hues
    {
        private static Hue[] m_List;

        public static Hue[] List { get { return m_List; } }

        static Hues()
        {
            string path = FileManager.GetFilePath("hues.mul");
            int index = 0;

            m_List = new Hue[3000];

            if (path != null)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryReader bin = new BinaryReader(fs);

                    int blockCount = (int)fs.Length / 708;

                    if (blockCount > 375)
                        blockCount = 375;

                    for (int i = 0; i < blockCount; ++i)
                    {
                        bin.ReadInt32();

                        for (int j = 0; j < 8; ++j, ++index)
                            m_List[index] = new Hue(index, bin);
                    }
                }
            }

            for (; index < 3000; ++index)
                m_List[index] = new Hue(index);
        }

        public static Hue GetHue(int index)
        {
            index &= 0x3FFF;

            if (index >= 0 && index < 3000)
                return m_List[index];

            return m_List[0];
        }
    }

    public class Hue
    {
        private int m_Index;
        private short[] m_Colors;
        private string m_Name;

        public int Index { get { return m_Index; } }
        public short[] Colors { get { return m_Colors; } }
        public string Name { get { return m_Name; } }

        public Hue(int index)
        {
            m_Name = "Null";
            m_Index = index;
            m_Colors = new short[34];
        }

        public System.Drawing.Color GetColor(int index)
        {
            int c16 = m_Colors[index];

            return System.Drawing.Color.FromArgb((c16 & 0x7C00) >> 7, (c16 & 0x3E0) >> 2, (c16 & 0x1F) << 3);
        }

        public ushort GetColorUShort(int index)
        {
            return (ushort)m_Colors[index];
        }

        public Hue(int index, BinaryReader bin)
        {
            m_Index = index;
            m_Colors = new short[34];

            for (int i = 0; i < 34; ++i)
                m_Colors[i] = (short)(bin.ReadUInt16() | 0x8000);

            bool nulled = false;

            StringBuilder sb = new StringBuilder(20, 20);

            for (int i = 0; i < 20; ++i)
            {
                char c = (char)bin.ReadByte();

                if (c == 0)
                    nulled = true;
                else if (!nulled)
                    sb.Append(c);
            }

            m_Name = sb.ToString();
        }

        public unsafe void ApplyTo(Bitmap bmp, bool onlyHueGrayPixels)
        {
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format16bppArgb1555);

            int stride = bd.Stride >> 1;
            int width = bd.Width;
            int height = bd.Height;
            int delta = stride - width;

            ushort* pBuffer = (ushort*)bd.Scan0;
            ushort* pLineEnd = pBuffer + width;
            ushort* pImageEnd = pBuffer + (stride * height);

            ushort* pColors = stackalloc ushort[0x40];

            fixed (short* pOriginal = m_Colors)
            {
                ushort* pSource = (ushort*)pOriginal;
                ushort* pDest = pColors;
                ushort* pEnd = pDest + 32;

                while (pDest < pEnd)
                    *pDest++ = 0;

                pEnd += 32;

                while (pDest < pEnd)
                    *pDest++ = *pSource++;
            }

            if (onlyHueGrayPixels)
            {
                int c;
                int r;
                int g;
                int b;

                while (pBuffer < pImageEnd)
                {
                    while (pBuffer < pLineEnd)
                    {
                        c = *pBuffer;
                        r = (c >> 10) & 0x1F;
                        g = (c >> 5) & 0x1F;
                        b = c & 0x1F;

                        if (r == g && r == b)
                            *pBuffer++ = pColors[c >> 10];
                        else
                            ++pBuffer;
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }
            else
            {
                while (pBuffer < pImageEnd)
                {
                    while (pBuffer < pLineEnd)
                    {
                        *pBuffer = pColors[(*pBuffer) >> 10];
                        ++pBuffer;
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }

            bmp.UnlockBits(bd);
        }
    }
}