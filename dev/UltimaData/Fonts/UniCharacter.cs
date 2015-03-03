using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaData.Fonts
{
    public sealed class UniCharacter
    {
        bool hasTexture
        {
            get
            {
                return (m_textureData == null) ? false : true;
            }
        }
        public int XOffset = 0, YOffset = 0, Width = 0, Height = 0;
        public int OutlinedYOffset = 0, OutlinedWidth = 0, OutlinedHeight = 0;
        public Color[] m_textureData;
        public Color[] m_textureDataOutlined;

        public UniCharacter()
        {

        }

        public unsafe void WriteToBuffer(Color* rPtr, int dx, int dy, int linewidth, int maxHeight, int baseLine, bool isBold, bool isItalic, bool isUnderlined, bool isOutlined, Color color)
        {
            int iWidth = (isOutlined) ? OutlinedWidth : Width;
            int iHeight = (isOutlined) ? OutlinedHeight : Height;
            int iYOffset = (isOutlined) ? OutlinedYOffset : YOffset;
            Color[] iTexture = (isOutlined) ? m_textureDataOutlined : m_textureData;
            Color iColor = Color.Transparent;
            if (hasTexture)
            {
                fixed (Color* cPtr = iTexture)
                {
                    for (int iy = 0; (iy < iHeight) && (iy + dy < maxHeight); iy++)
                    {
                        Color* src = ((Color*)cPtr) + (iWidth * iy);
                        Color* dest = (((Color*)rPtr) + (linewidth * (iy + dy + iYOffset)) + dx);
                        if (isItalic)
                        {
                            dest += (baseLine - iYOffset - iy - 1) / 2;
                        }

                        for (int k = 0; k < iWidth; k++)
                        {
                            if (*src != Color.Transparent)
                            {
                                if (*src == Color.White)
                                    iColor = color;
                                else if (*src == Color.Black)
                                    iColor = Color.Black;

                                *dest = iColor;
                                if (isBold)
                                    *(dest + 1) = iColor;
                            }
                            dest++;
                            src++;
                        }
                    }
                }
            }

            if (isUnderlined)
            {
                int underlineAtY = dy + baseLine + 2;
                if (underlineAtY >= maxHeight)
                    return;
                Color* dest = (((Color*)rPtr) + (linewidth * (underlineAtY)) + dx);
                int w = isBold ? iWidth + 2 : iWidth + 1;
                for (int k = 0; k < w; k++)
                {
                    *dest++ = color;
                }
            }
        }

        public void LoadCharacter(BinaryReader reader, GraphicsDevice graphics)
        {
            int readerStart = (int)reader.BaseStream.Position;

            this.XOffset = reader.ReadByte();
            this.YOffset = reader.ReadByte();
            this.Width = reader.ReadByte();
            this.Height = reader.ReadByte();
            this.OutlinedYOffset = this.YOffset;
            this.OutlinedWidth = this.Width + 2;
            this.OutlinedHeight = this.Height + 2;

            // only read data if there is UltimaData...
            if ((this.Width > 0) && (this.Height > 0))
            {
                // At this point, we know we have data, so go ahead and start reading!
                m_textureData = new Color[Width * Height];
                m_textureDataOutlined = new Color[OutlinedWidth * OutlinedHeight];

                unsafe
                {
                    fixed (Color* p = m_textureData)
                    {
                        fixed (Color* p2 = m_textureDataOutlined)
                        {
                            for (int y = 0; y < Height; ++y)
                            {
                                byte[] scanline = reader.ReadBytes(((Width - 1) / 8) + 1);
                                int bitX = 7;
                                int byteX = 0;
                                for (int x = 0; x < Width; ++x)
                                {
                                    Color color = Color.Transparent;
                                    if ((scanline[byteX] & (byte)Math.Pow(2, bitX)) != 0)
                                        color = Color.White;

                                    // the not outlined font is easy:
                                    p[x + y * Width] = color;

                                    // the outlined font requires a little more work:
                                    if (color == Color.White)
                                    {
                                        p2[(x + 1) + (y + 1) * OutlinedWidth] = color;

                                        for (int ix = -1; ix < 2; ix++)
                                            for (int iy = -1; iy < 2; iy++)
                                            {
                                                int index = (x + 1 + ix) + (y + 1 + iy) * (OutlinedWidth);
                                                if (p2[index] != Color.White)
                                                    p2[index] = Color.Black;
                                            }
                                    }

                                    bitX--;
                                    if (bitX < 0)
                                    {
                                        bitX = 7;
                                        byteX++;
                                    }
                                }
                            }
                        }
                    }
                }

                UltimaVars.Metrics.ReportDataRead((int)reader.BaseStream.Position - readerStart);
            }
        }
    }
}
