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
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using UltimaXNA.Core.UI;
using UltimaXNA.Core.UI.Fonts;
#endregion

namespace UltimaXNA.Ultima.IO.FontsNew
{
    internal class FontASCII : AFont
    {
        private CharacterASCII[] m_characters;

        public FontASCII()
        {
            m_characters = new CharacterASCII[224];
        }

        public override void Initialize(BinaryReader reader)
        {
            byte header = reader.ReadByte();

            // space characters have no data in AFont files.
            m_characters[0] = new CharacterASCII();

            // We load all 224 characters; this seeds the font with correct height values.
            for (int i = 0; i < 224; i++)
            {
                CharacterASCII ch = loadCharacter(reader);
                int height = ch.Height + ch.YOffset;
                if (i < 96 && height > Height)
                {
                    Height = height;
                }
                m_characters[i] = ch;
            }

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

        private CharacterASCII NullCharacter = new CharacterASCII();
        CharacterASCII loadCharacter(BinaryReader reader)
        {
            CharacterASCII character = new CharacterASCII(reader);
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
