/***************************************************************************
 *   Gumps.cs
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
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Data
{
    public class Gumps
    {
        private static GraphicsDevice _graphicsDevice;

        private static FileIndex m_FileIndex = new FileIndex("Gumpidx.mul", "Gumpart.mul", 0x10000, 12);
        public static FileIndex FileIndex { get { return m_FileIndex; } }

        private static byte[] m_PixelBuffer;
        private static byte[] m_StreamBuffer;
        private static byte[] m_ColorTable;

        public static void Initialize(GraphicsDevice graphics)
        {
            _graphicsDevice = graphics;
        }

        public unsafe static Bitmap GetGump(int index, Hue hue, bool onlyHueGrayPixels)
        {
            int length, extra;
            bool patched;
            Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);

            if (stream == null)
                return null;

            int width = (extra >> 16) & 0xFFFF;
            int height = extra & 0xFFFF;

            if (width <= 0 || height <= 0)
                return null;

            int bytesPerLine = width << 1;
            int bytesPerStride = (bytesPerLine + 3) & ~3;
            int bytesForImage = height * bytesPerStride;

            int pixelsPerStride = (width + 1) & ~1;
            int pixelsPerStrideDelta = pixelsPerStride - width;

            byte[] pixelBuffer = m_PixelBuffer;

            if (pixelBuffer == null || pixelBuffer.Length < bytesForImage)
                m_PixelBuffer = pixelBuffer = new byte[(bytesForImage + 2047) & ~2047];

            byte[] streamBuffer = m_StreamBuffer;

            if (streamBuffer == null || streamBuffer.Length < length)
                m_StreamBuffer = streamBuffer = new byte[(length + 2047) & ~2047];

            byte[] colorTable = m_ColorTable;

            if (colorTable == null)
                m_ColorTable = colorTable = new byte[128];

            stream.Read(streamBuffer, 0, length);

            fixed (short* psHueColors = hue.Colors)
            {
                fixed (byte* pbStream = streamBuffer)
                {
                    fixed (byte* pbPixels = pixelBuffer)
                    {
                        fixed (byte* pbColorTable = colorTable)
                        {
                            ushort* pHueColors = (ushort*)psHueColors;
                            ushort* pHueColorsEnd = pHueColors + 32;

                            ushort* pColorTable = (ushort*)pbColorTable;

                            ushort* pColorTableOpaque = pColorTable;

                            while (pHueColors < pHueColorsEnd)
                                *pColorTableOpaque++ = *pHueColors++;

                            ushort* pPixelDataStart = (ushort*)pbPixels;

                            int* pLookup = (int*)pbStream;
                            int* pLookupEnd = pLookup + height;
                            int* pPixelRleStart = pLookup;
                            int* pPixelRle;

                            ushort* pPixel = pPixelDataStart;
                            ushort* pRleEnd = pPixel;
                            ushort* pPixelEnd = pPixel + width;

                            ushort color, count;

                            if (onlyHueGrayPixels)
                            {
                                while (pLookup < pLookupEnd)
                                {
                                    pPixelRle = pPixelRleStart + *pLookup++;
                                    pRleEnd = pPixel;

                                    while (pPixel < pPixelEnd)
                                    {
                                        color = *(ushort*)pPixelRle;
                                        count = *(1 + (ushort*)pPixelRle);
                                        ++pPixelRle;

                                        pRleEnd += count;

                                        if (color != 0 && (color & 0x1F) == ((color >> 5) & 0x1F) && (color & 0x1F) == ((color >> 10) & 0x1F))
                                            color = pColorTable[color >> 10];
                                        else if (color != 0)
                                            color ^= 0x8000;

                                        while (pPixel < pRleEnd)
                                            *pPixel++ = color;
                                    }

                                    pPixel += pixelsPerStrideDelta;
                                    pPixelEnd += pixelsPerStride;
                                }
                            }
                            else
                            {
                                while (pLookup < pLookupEnd)
                                {
                                    pPixelRle = pPixelRleStart + *pLookup++;
                                    pRleEnd = pPixel;

                                    while (pPixel < pPixelEnd)
                                    {
                                        color = *(ushort*)pPixelRle;
                                        count = *(1 + (ushort*)pPixelRle);
                                        ++pPixelRle;

                                        pRleEnd += count;

                                        if (color != 0)
                                            color = pColorTable[color >> 10];

                                        while (pPixel < pRleEnd)
                                            *pPixel++ = color;
                                    }

                                    pPixel += pixelsPerStrideDelta;
                                    pPixelEnd += pixelsPerStride;
                                }
                            }

                            Metrics.ReportDataRead(sizeof(UInt16) * height * width);

                            return new Bitmap(width, height, bytesPerStride, PixelFormat.Format16bppArgb1555, (IntPtr)pPixelDataStart);
                        }
                    }
                }
            }
        }

        public unsafe static Bitmap GetGump(int index)
        {
            int length, extra;
            bool patched;
            Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);

            if (stream == null)
                return null;

            int width = (extra >> 16) & 0xFFFF;
            int height = extra & 0xFFFF;

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format16bppRgb555);
            BitmapData bd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppRgb555);
            BinaryReader bin = new BinaryReader(stream);

            int[] lookups = new int[height];
            int start = (int)bin.BaseStream.Position;

            for (int i = 0; i < height; ++i)
                lookups[i] = start + (bin.ReadInt32() * 4);

            ushort* line = (ushort*)bd.Scan0;
            int delta = bd.Stride >> 1;

            for (int y = 0; y < height; ++y, line += delta)
            {
                bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                ushort* cur = line;
                ushort* end = line + bd.Width;

                while (cur < end)
                {
                    ushort color = bin.ReadUInt16();
                    ushort* next = cur + bin.ReadUInt16();

                    if (color == 0)
                    {
                        cur = next;
                    }
                    else
                    {
                        color ^= 0x8000;

                        while (cur < next)
                            *cur++ = color;
                    }
                }
            }

            Metrics.ReportDataRead(sizeof(UInt16) * height * width);

            bmp.UnlockBits(bd);
            return bmp;
        }

        public unsafe static Texture2D GetGumpXNA(int index, int hueIndex, bool onlyHueGreyPixels)
        {
            int length, extra;
            bool patched;
            Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);

            if (stream == null)
                return null;

            int width = (extra >> 16) & 0xFFFF;
            int height = extra & 0xFFFF;

            ushort[] pixels = new ushort[width * height];
            BinaryReader bin = new BinaryReader(stream);

            int[] lookups = new int[height];
            int start = (int)bin.BaseStream.Position;

            for (int i = 0; i < height; ++i)
                lookups[i] = start + (bin.ReadInt32() * 4);

            Hue hueObject = null;
            hueIndex = (hueIndex & 0x3FFF) - 1;
            if (hueIndex >= 0 && hueIndex < Hues.List.Length)
            {
                hueObject = Hues.List[hueIndex];
            }

            fixed (ushort* line = &pixels[0])
            {
                for (int y = 0; y < height; ++y)
                {
                    bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                    ushort* cur = line + (y * width);
                    ushort* end = cur + (width);

                    while (cur < end)
                    {
                        uint color = bin.ReadUInt16();
                        ushort* next = cur + bin.ReadUInt16();

                        if (color == 0)
                        {
                            cur = next;
                        }
                        else
                        {
                            if (hueIndex < 0)
                            {
                                color |= 0x8000;
                            }
                            else
                            {
                                if (onlyHueGreyPixels)
                                {
                                    if (((color & 0x1F) == ((color >> 5) & 0x1F)) && ((color & 0x1F) == (color >> 10)))
                                    {
                                        color = hueObject.GetColorUShort((int)(color >> 10));
                                    }
                                    else
                                    {
                                        color |= 0x8000;
                                    }
                                }
                                else
                                {
                                    color = hueObject.GetColorUShort((int)(color >> 10));
                                }
                            }
                            while (cur < next)
                                *cur++ = (ushort)color;
                        }
                    }
                }
            }

            Metrics.ReportDataRead(sizeof(UInt16) * height * width);

            Texture2D iTexture = new Texture2D(_graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Bgra5551);
            iTexture.SetData(pixels);
            return iTexture;
        }

        public unsafe static Texture2D GetGumpXNA(int index)
        {
            int length, extra;
            bool patched;
            Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);

            if (stream == null)
                return null;

            int width = (extra >> 16) & 0xFFFF;
            int height = extra & 0xFFFF;

            uint[] pixels = new uint[width * height];
            BinaryReader bin = new BinaryReader(stream);

            int[] lookups = new int[height];
            int start = (int)bin.BaseStream.Position;

            for (int i = 0; i < height; ++i)
                lookups[i] = start + (bin.ReadInt32() * 4);

            fixed (uint* line = &pixels[0])
            {
                for (int y = 0; y < height; ++y)
                {
                    bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                    uint* cur = line + (y * width);
                    uint* end = cur + (width);

                    while (cur < end)
                    {
                        uint color = bin.ReadUInt16();
                        uint* next = cur + bin.ReadUInt16();

                        if (color == 0)
                        {
                            cur = next;
                        }
                        else
                        {
                            uint color32 = 0xff000000 + (
                                ((((color >> 10) & 0x1F) * 0xFF / 0x1F) << 16) |
                                ((((color >> 5) & 0x1F) * 0xFF / 0x1F) << 8) |
                                (((color & 0x1F) * 0xFF / 0x1F))
                                );
                            while (cur < next)
                                *cur++ = color32;
                        }
                    }
                }
            }

            Metrics.ReportDataRead(sizeof(UInt16) * height * width);

            Texture2D iTexture = new Texture2D(_graphicsDevice, width, height);
            iTexture.SetData(pixels);
            return iTexture;
        }
    }
}