/***************************************************************************
 *   UniFonts.cs
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

namespace UltimaXNA.UltimaData.FontsNew
{
    public sealed class UniFont
    {
        GraphicsDevice m_graphics = null;
        BinaryReader m_reader = null;
        private UniCharacter[] m_characters;

        private int m_height = 0;
        public int Height { get { return m_height; } set { m_height = value; } }
        private int m_baseline = 0;
        public int Baseline { get { return m_baseline; } set { m_baseline = value; } }
        public int Lineheight { get { return m_baseline + 4; } }
        public UniFont()
        {
            m_characters = new UniCharacter[0x10000];
        }

        public void Initialize(GraphicsDevice graphicsDevice, BinaryReader reader)
        {
            m_graphics = graphicsDevice;
            m_reader = reader;
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
            if (m_characters[index] == null)
            {
                m_characters[index] = loadCharacter(index);
                int height = m_characters[index].Height + m_characters[index].YOffset;
                if (index < 128 && height > Height)
                {
                    Height = height;
                }
            }
            return m_characters[index];
        }

        UniCharacter loadCharacter(int index)
        {
            // get the lookup table - 0x10000 ints.
            m_reader.BaseStream.Position = index * 4;
            int lookup = m_reader.ReadInt32();

            UniCharacter character = new UniCharacter();

            if (lookup == 0)
            {
                // no character - so we just return an empty character
                return character;
            }
            else
            {
                m_reader.BaseStream.Position = lookup;
                character.LoadCharacter(m_reader, m_graphics);
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
}
