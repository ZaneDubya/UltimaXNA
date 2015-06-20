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
#endregion

namespace UltimaXNA.Ultima.UI
{
    class UltimaUIResourceProvider : IUIResourceProvider
    {
        public Texture2D GetTexture(int textureID)
        {
            return IO.GumpData.GetGumpXNA(textureID);
        }

        public ushort GetWebSafeHue(Color color)
        {
            return (ushort)IO.HuesXNA.GetWebSafeHue(color);
        }

        public IFont GetUnicodeFont(int fontIndex)
        {
            return IO.FontsNew.TextUni.GetUniFont(fontIndex);
        }

        public IFont GetAsciiFont(int fontIndex)
        {
            return IO.FontsNew.TextUni.GetAsciiFont(fontIndex);
        }
    }
}
