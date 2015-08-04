/***************************************************************************
 *   UltimaUIResourceProvider.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.IO.Fonts;
#endregion

namespace UltimaXNA.Ultima.UI
{
    class UltimaUIResourceProvider : IUIResourceProvider
    {
        public Texture2D GetUITexture(int textureID)
        {
            return GumpData.GetGumpXNA(textureID);
        }

        public Texture2D GetItemTexture(int textureID)
        {
            return ArtData.GetStaticTexture(textureID);
        }

        public ushort GetWebSafeHue(Color color)
        {
            return (ushort)HueData.GetWebSafeHue(color);
        }

        public IFont GetUnicodeFont(int fontIndex)
        {
            return TextUni.GetUniFont(fontIndex);
        }

        public IFont GetAsciiFont(int fontIndex)
        {
            return TextUni.GetAsciiFont(fontIndex);
        }
    }
}
