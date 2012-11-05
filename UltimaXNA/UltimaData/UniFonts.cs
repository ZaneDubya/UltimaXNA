/***************************************************************************
 *   UniFonts.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaData
{
    public sealed class UniCharacter
    {
        bool hasTexture
        {
            get
            {
                return (_textureData == null) ? false : true;
            }
        }
        public int XOffset = 0, YOffset = 0, Width = 0, Height = 0;
        public int OutlinedYOffset = 0, OutlinedWidth = 0, OutlinedHeight = 0;
        public Color[] _textureData;
        public Color[] _textureDataOutlined;

        public UniCharacter()
        {

        }

        public unsafe void WriteToBuffer(Color* rPtr, int dx, int dy, int linewidth, int maxHeight, int baseLine, bool isBold, bool isItalic, bool isUnderlined, bool isOutlined, Color color)
        {
            int iWidth = (isOutlined) ? OutlinedWidth : Width;
            int iHeight = (isOutlined) ? OutlinedHeight : Height;
            int iYOffset = (isOutlined) ? OutlinedYOffset : YOffset;
            Color[] iTexture = (isOutlined) ? _textureDataOutlined : _textureData;
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
                _textureData = new Color[Width * Height];
                _textureDataOutlined = new Color[OutlinedWidth * OutlinedHeight];

                unsafe
                {
                    fixed (Color* p = _textureData)
                    {
                        fixed (Color* p2 = _textureDataOutlined)
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

    public sealed class UniFont
    {
        GraphicsDevice _graphics = null;
        BinaryReader _reader = null;
        private UniCharacter[] _characters;

        private int _height = 0;
        public int Height { get { return _height; } set { _height = value; } }
        private int _baseline = 0;
        public int Baseline { get { return _baseline; } set { _baseline = value; } }
        public int Lineheight { get { return _baseline + 4; } }
        public UniFont()
        {
            _characters = new UniCharacter[0x10000];
        }

        public void Initialize(GraphicsDevice graphicsDevice, BinaryReader reader)
        {
            _graphics = graphicsDevice;
            _reader = reader;
            // We load the first 128 characters to 'seed' the font with correct spacing values.
            for (int iChar = 0; iChar < 128; iChar++)
            {
                GetCharacter(iChar);
            }
            // Determine the width of the space character - arbitrarily .333 the width of capital M (.333 em?).
            GetCharacter(' ').Width = GetCharacter('M').Width / 3;
            Baseline = GetCharacter('M').Height + GetCharacter('M').YOffset;
        }

        public UniCharacter GetCharacter(char character)
        {
            return GetCharacter(((int)character) & 0xFFFFF);
        }

        public UniCharacter GetCharacter(int index)
        {
            if (_characters[index] == null)
            {
                _characters[index] = loadCharacter(index);
                int height = _characters[index].Height + _characters[index].YOffset;
                if (index < 128 && height > Height)
                {
                    Height = height;
                }
            }
            return _characters[index];
        }

        UniCharacter loadCharacter(int index)
        {
            // get the lookup table - 0x10000 ints.
            _reader.BaseStream.Position = index * 4;
            int lookup = _reader.ReadInt32();

            UniCharacter character = new UniCharacter();

            if (lookup == 0)
            {
                // no character - so we just return an empty character
                return character;
            }
            else
            {
                _reader.BaseStream.Position = lookup;
                character.LoadCharacter(_reader, _graphics);
                return character;
            }
        }

        public int GetWidth(char ch)
        {
            return GetCharacter(ch).Width;
        }

        public int GetWidth(string text)
        {
            if (text == null || text.Length == 0) { return 0; }

            int width = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                width += GetCharacter(text[i]).Width;
            }

            return width;
        }
    }

    public static class UniText
    {
        private static UniFont[] _fonts;
        private static bool _initialized;
        private static GraphicsDevice _graphicsDevice;
        public static UniFont[] Fonts
        {
            get { return _fonts; }
        }

        static UniText()
        {

        }

        public static int FontHeight(int index)
        {
            return _fonts[index].Height;
        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (!_initialized)
            {
                _initialized = true;
                _graphicsDevice = graphicsDevice;
                graphicsDevice.DeviceReset += graphicsDevice_DeviceReset;
                loadFonts();
            }
        }

        static void graphicsDevice_DeviceReset(object sender, System.EventArgs e)
        {
            loadFonts();
        }

        static void loadFonts()
        {
            int maxHeight = 0;
            _fonts = new UniFont[7];
            for (int iFont = 0; iFont < 7; iFont++)
            {
                string path = FileManager.GetFilePath("unifont" + (iFont == 0 ? "" : iFont.ToString()) + ".mul");
                if (path != null)
                {
                    _fonts[iFont] = new UniFont();
                    _fonts[iFont].Initialize(_graphicsDevice, new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
                    if (_fonts[iFont].Height > maxHeight)
                        maxHeight = _fonts[iFont].Height;
                }
            }

            for (int iFont = 0; iFont < 7; iFont++)
            {
                if (_fonts[iFont] == null)
                    continue;
                _fonts[iFont].Height = maxHeight;
            }
        }
    }
}
