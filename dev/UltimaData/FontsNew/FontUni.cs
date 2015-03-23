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
    internal class FontUni : AFont
    {
        GraphicsDevice m_graphics = null;
        BinaryReader m_reader = null;
        private CharacterUni[] m_characters;

        public FontUni()
        {
            m_characters = new CharacterUni[0x10000];
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
        }

        internal CharacterUni GetCharacter(char character)
        {
            return GetCharacter(((int)character) & 0xFFFFF);
        }

        internal CharacterUni GetCharacter(int index)
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

        CharacterUni loadCharacter(int index)
        {
            // get the lookup table - 0x10000 ints.
            m_reader.BaseStream.Position = index * 4;
            int lookup = m_reader.ReadInt32();

            if (lookup == 0)
            {
                // no character - so we just return null
                return null;
            }
            else
            {
                m_reader.BaseStream.Position = lookup;
                CharacterUni character = new CharacterUni(m_reader);
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
