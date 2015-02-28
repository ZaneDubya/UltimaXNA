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

namespace UltimaXNA.UltimaData.Fonts
{
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
}
