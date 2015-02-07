/***************************************************************************
 *   ASCIIFont.cs
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
    //QUERY: Does this really need to be exposed? Shouldnt this be a child class of ASCIIText?
    public sealed class ASCIIFont
    {
        private int m_Height;
        private Texture2D[] m_Characters;

        public int Height { get { return m_Height; } set { m_Height = value; } }
        public Texture2D[] Characters { get { return m_Characters; } set { m_Characters = value; } }

        public ASCIIFont()
        {
            Height = 0;
            Characters = new Texture2D[224];
        }

        public Texture2D GetTexture(char character)
        {
            return m_Characters[(((((int)character) - 0x20) & 0x7FFFFFFF) % 224)];
        }

        public int GetWidth(char ch)
        {
            return GetTexture(ch).Width;
        }

        public int GetWidth(string text)
        {
            if (text == null || text.Length == 0) { return 0; }

            int width = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                width += GetTexture(text[i]).Width;
            }

            return width;
        }

        void getTextDimensions(ref string text, ref int width, ref int height, int wrapwidth)
        {
            width = 0;
            height = Height;
            int biggestwidth = 0;
            List<char> word = new List<char>();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (((int)c) > 32)
                {
                    word.Add(c);
                }

                if (c == ' ' || i == text.Length - 1 || c == '\n')
                {
                    // Size the word, character by character.
                    int wordwidth = 0;

                    if (word.Count > 0)
                    {
                        for (int j = 0; j < word.Count; j++)
                        {
                            int charwidth = GetWidth(word[j]);
                            wordwidth += charwidth;
                        }
                    }

                    // Now make sure this line can fit the word.
                    if (width + wordwidth <= wrapwidth)
                    {
                        width += wordwidth;
                        word.Clear();
                        // if this word is followed by a space, does it fit? If not, drop it entirely and insert \n after the word.
                        if (c == ' ')
                        {
                            int charwidth = GetWidth(c);
                            if (width + charwidth <= wrapwidth)
                            {
                                // we can fit an extra space here.
                                width += charwidth;
                            }
                            else
                            {
                                // can't fit an extra space on the end of the line. replace the space with a \n.
                                text = text.Substring(0, i) + '\n' + text.Substring(i + 1, text.Length - i - 1);
                                i--;
                            }
                        }
                    }
                    else
                    {
                        // The word is too big for the line. SOME words are longer than the entire line, so we have to break them up manually.
                        if (wordwidth > wrapwidth)
                        {
                            int splitwidth = 0;
                            for (int j = 0; j < word.Count; j++)
                            {
                                splitwidth += GetWidth(word[j]) + 1;
                                if (splitwidth > wrapwidth)
                                {
                                    text = text.Insert(i - word.Count + j - 1, "\n");
                                    word.Insert(j - 1, '\n');
                                    j--;
                                    splitwidth = 0;
                                }
                            }
                            i = i - word.Count - 1;
                            word.Clear();
                        }
                        else
                        {
                            // this word is too big, so we insert a \n character before the word... and try again.
                            text = text.Insert(i - word.Count, "\n");
                            i = i - word.Count - 1;
                            word.Clear();
                        }
                    }
                }

                if (c == '\n')
                {
                    if (width > biggestwidth)
                        biggestwidth = width;
                    height += Height;
                    width = 0;
                }
            }

            if (biggestwidth > width)
                width = biggestwidth;
        }

        public void GetTextDimensions(ref string text, ref int width, ref int height, int wrapwidth)
        {
            getTextDimensions(ref text, ref width, ref height, wrapwidth);
        }

        public static ASCIIFont GetFixed(int font)
        {
            if (font < 0 || font > 9)
            {
                return ASCIIText.Fonts[3];
            }

            return ASCIIText.Fonts[font];
        }
    }

    public static class ASCIIText
    {
        private static ASCIIFont[] _fonts = new ASCIIFont[10];
        private static bool _initialized;
        private static GraphicsDevice _graphicsDevice;

        //QUERY: Does this really need to be exposed?
        public static ASCIIFont[] Fonts { get { return ASCIIText._fonts; } set { ASCIIText._fonts = value; } }

        public static int MaxWidth
        {
            get { return _graphicsDevice.Viewport.Width; }
        }

        static ASCIIText()
        {

        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (!_initialized)
            {
                _initialized = true;
                string path = FileManager.GetFilePath("fonts.mul");
                _graphicsDevice = graphicsDevice;
                if (path != null)
                {
                    byte[] buffer;
                    int pos = 0;
                    using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
                    {
                        buffer = reader.ReadBytes((int)reader.BaseStream.Length);
                    }
                    UltimaVars.Metrics.ReportDataRead(buffer.Length);

                    for (int i = 0; i < 10; ++i)
                    {
                        _fonts[i] = new ASCIIFont();

                        byte header = buffer[pos++];

                        for (int k = 0; k < 224; ++k)
                        {
                            int width = buffer[pos++];
                            int height = buffer[pos++];
                            pos++; // byte delimeter?

                            if (width > 0 && height > 0)
                            {
                                if (height > _fonts[i].Height && k < 96)
                                {
                                    _fonts[i].Height = height;
                                }

                                Texture2D texture = new Texture2D(_graphicsDevice, width, height, false, SurfaceFormat.Color);
                                Color[] pixels = new Color[width * height];

                                unsafe
                                {
                                    fixed (Color* p = pixels)
                                    {
                                        for (int y = 0; y < height; ++y)
                                        {
                                            for (int x = 0; x < width; ++x)
                                            {
                                                short pixel = (short)(buffer[pos++] | (buffer[pos++] << 8));
                                                Color color = Color.Transparent;

                                                if (pixel != 0)
                                                {
                                                    color = new Color();
                                                    color.A = 255;
                                                    color.R = (byte)((pixel & 0x7C00) >> 7);
                                                    color.G = (byte)((pixel & 0x3E0) >> 2);
                                                    color.B = (byte)((pixel & 0x1F) << 3);
                                                }

                                                p[x + y * width] = color;
                                            }
                                        }
                                    }
                                }

                                texture.SetData<Color>(pixels);
                                _fonts[i].Characters[k] = texture;
                            }
                        }
                    }
                }
            }
        }

        private static Dictionary<string, Texture2D> _TextTextureCache;
        public static Texture2D GetTextTexture(string text, int fontId)
        {
            string hash = string.Format("<font:{0}>{1}", fontId.ToString(), text);

            if (_TextTextureCache == null)
                _TextTextureCache = new Dictionary<string, Texture2D>();

            if (!_TextTextureCache.ContainsKey(hash))
            {
                Texture2D texture = getTexture(text, fontId, 0);
                _TextTextureCache.Add(hash, texture);
            }
            return _TextTextureCache[hash];
        }

        public static Texture2D GetTextTexture(string text, int fontId, int wrapwidth)
        {
            string hash = string.Format("<font:{0}:w:{1}>{2}", fontId.ToString(), wrapwidth.ToString(), text);

            if (_TextTextureCache == null)
                _TextTextureCache = new Dictionary<string, Texture2D>();

            if (!_TextTextureCache.ContainsKey(hash))
            {
                Texture2D texture = getTexture(text, fontId, wrapwidth);
                _TextTextureCache.Add(hash, texture);
            }
            return _TextTextureCache[hash];
        }

        private unsafe static Texture2D getTexture(string text, int fontId, int wrapwidth)
        {
            ASCIIFont font = ASCIIFont.GetFixed(fontId);

            int width = 0, height = 0;
            if (wrapwidth == 0)
                font.GetTextDimensions(ref text, ref width, ref height, MaxWidth);
            else
                font.GetTextDimensions(ref text, ref width, ref height, wrapwidth);

            if (width == 0) // empty text string
                return new Texture2D(_graphicsDevice, 1, 1);

            Color[] resultData = new Color[width * height];

            int dx = 0;
            int dy = 0;
            text = text.Replace(Environment.NewLine, "\n");

            unsafe
            {
                fixed (Color* rPtr = resultData)
                {
                    for (int i = 0; i < text.Length; ++i)
                    {
                        if (text.Substring(i, 1) == "\n")
                        {
                            dx = 0;
                            dy += font.Height;
                        }
                        else
                        {
                            Texture2D charTexture = font.GetTexture(text[i]);
                            Color[] charData = new Color[charTexture.Width * charTexture.Height];
                            charTexture.GetData<Color>(charData);

                            fixed (Color* cPtr = charData)
                            {
                                int starty = font.Height - charTexture.Height;
                                int maxHeight = (charTexture.Height < font.Height) ? charTexture.Height : font.Height;
                                for (int iy = 0; iy < maxHeight; ++iy)
                                {
                                    Color* src = ((Color*)cPtr) + (charTexture.Width * iy);
                                    Color* dest = (((Color*)rPtr) + (width * (iy + starty + dy)) + dx);

                                    for (int k = 0; k < charTexture.Width; ++k)
                                        *dest++ = *src++;
                                }
                            }
                            dx += charTexture.Width;
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(_graphicsDevice, width, height, false, SurfaceFormat.Color);
            result.SetData<Color>(resultData);
            return result;
        }
    }
}
