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
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.IO.FontsNew
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
            // space characters have no data in UniFont files.
            m_characters[0] = new CharacterUni();
            // We load the first 96 characters to 'seed' the font with correct height values.
            for (int i = 33; i < 128; i++)
            {
                GetCharacter((char)i);
            }
            // Determine the width of the space character - arbitrarily .333 the width of capital M (.333 em?).
            GetCharacter(' ').Width = GetCharacter('M').Width / 3;
        }

        public override ICharacter GetCharacter(char character)
        {
            int index = ((int)character & 0xFFFFF) - 0x20;
            if (index < 0)
                return NullCharacter;
            if (m_characters[index] == null)
            {
                CharacterUni ch = loadCharacter(index + 0x20);
                int height = ch.Height + ch.YOffset;
                if (index < 128 && height > Height)
                {
                    Height = height;
                }
                m_characters[index] = ch;
            }
            return m_characters[index];
        }

        private CharacterUni NullCharacter = new CharacterUni();
        CharacterUni loadCharacter(int index)
        {
            // get the lookup table - 0x10000 ints.
            m_reader.BaseStream.Position = index * 4;
            int lookup = m_reader.ReadInt32();

            if (lookup == 0)
            {
                // no character - so we just return null
                return NullCharacter;
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
