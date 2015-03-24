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
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Diagnostics;
using System.IO;
using UltimaXNA.UltimaData.FontsNew;
#endregion

namespace UltimaXNA.UltimaData.FontsUnused
{
    internal class FontASCII : AFont
    {
        public FontASCII(GraphicsDevice device, BinaryReader reader)
        {
            Height = 0;
            m_Characters = new CharacterASCII[224];

            byte header = reader.ReadByte();

            // get the sprite data and get a place for each character in the texture, then write the character data to an array.
            for (int k = 0; k < 224; k++)
            {
                CharacterASCII ch = new CharacterASCII(reader);
                if (k < 96 && ch.Height > Height)
                {
                    Height = ch.Height;
                }

                m_Characters[k] = ch;
            }

            // InitializeTexture(device, false);
        }

        public override ACharacter GetCharacter(char character)
        {
            int index = ((int)character & 0xFF) - 0x20;
            return m_Characters[index];
        }
    }
}
