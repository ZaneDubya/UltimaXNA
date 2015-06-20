/***************************************************************************
 *   ASCIIFont.cs
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

namespace UltimaXNA.Ultima.IO.FontsOld
{
    //QUERY: Does this really need to be exposed? Shouldnt this be a child class of ASCIIText?
    public sealed class ASCIIFontOld
    {
        private int m_Height;
        private Texture2D[] m_Characters;

        public int Height { get { return m_Height; } set { m_Height = value; } }
        public Texture2D[] Characters { get { return m_Characters; } set { m_Characters = value; } }

        public ASCIIFontOld()
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

        public static ASCIIFontOld GetFixed(int font)
        {
            if (font < 0 || font > 9)
            {
                return ASCIITextOld.Fonts[3];
            }

            return ASCIITextOld.Fonts[font];
        }
    }
}
