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
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources.Fonts;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    class UltimaResourceProvider : IResourceProvider
    {
        private ArtMulResource m_Art;

        public UltimaResourceProvider(Game game)
        {
            m_Art = new ArtMulResource(game.GraphicsDevice);
        }

        public Texture2D GetUITexture(int textureID)
        {
            return GumpData.GetGumpXNA(textureID);
        }

        public Texture2D GetItemTexture(int textureID)
        {
            return m_Art.GetStaticTexture(textureID);
        }

        public Texture2D GetLandTexture(int textureID)
        {
            return m_Art.GetLandTexture(textureID);
        }

        public void GetItemDimensions(int textureID, out int width, out int height)
        {
            m_Art.GetStaticDimensions(textureID, out width, out height);
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
