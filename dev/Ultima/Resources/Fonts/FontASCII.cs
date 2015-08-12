/***************************************************************************
 *   UniFonts.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   Changes Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.IO;
using UltimaXNA.Core.UI;
using UltimaXNA.Core.UI.Fonts;
#endregion

namespace UltimaXNA.Ultima.Resources.Fonts
{
    internal class FontAscii : AFont
    {
        private CharacterAscii[] m_characters;

        public FontAscii()
        {
            m_characters = new CharacterAscii[224];
        }

        public override void Initialize(BinaryReader reader)
        {
            byte header = reader.ReadByte();

            // space characters have no data in AFont files.
            m_characters[0] = new CharacterAscii();

            // We load all 224 characters; this seeds the font with correct height values.
            for (int i = 0; i < 224; i++)
            {
                CharacterAscii ch = loadCharacter(reader);
                int height = ch.Height;
                if (i > 32 && i < 90 && height > Height)
                    Height = height;
                m_characters[i] = ch;
            }

            for (int i = 0; i < 224; i++)
            {
                m_characters[i].YOffset = Height - (m_characters[i].Height + m_characters[i].YOffset);
            }

            // ascii fonts are so tall! why?
            Height -= 2;

            // Determine the width of the space character - arbitrarily .333 the width of capital M (.333 em?).
            GetCharacter(' ').Width = GetCharacter('M').Width / 3;
        }

        public override ICharacter GetCharacter(char character)
        {
            int index = ((int)character & 0xFFFFF) - 0x20;

            if (index < 0)
                return NullCharacter;
            if (index >= m_characters.Length)
                return NullCharacter;
            return m_characters[index];
        }

        private CharacterAscii NullCharacter = new CharacterAscii();
        CharacterAscii loadCharacter(BinaryReader reader)
        {
            CharacterAscii character = new CharacterAscii(reader);
            return character;
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
