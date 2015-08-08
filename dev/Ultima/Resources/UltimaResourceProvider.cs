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
        private FontsResource m_Fonts;
        private GumpMulResource m_Gumps;

        public UltimaResourceProvider(Game game)
        {
            m_Art = new ArtMulResource(game.GraphicsDevice);
            m_Fonts = new FontsResource(game.GraphicsDevice);
            m_Gumps = new GumpMulResource(game.GraphicsDevice);
        }

        public Texture2D GetUITexture(int textureIndex, bool replaceMask080808 = false)
        {
            return m_Gumps.GetGumpXNA(textureIndex);
        }

        public Texture2D GetItemTexture(int itemIndex)
        {
            return m_Art.GetStaticTexture(itemIndex);
        }

        public Texture2D GetLandTexture(int landIndex)
        {
            return m_Art.GetLandTexture(landIndex);
        }

        public void GetItemDimensions(int itemIndex, out int width, out int height)
        {
            m_Art.GetStaticDimensions(itemIndex, out width, out height);
        }

        /// <summary>
        /// Returns a Ultima Online Hue index that approximates the passed color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public ushort GetWebSafeHue(Color color)
        {
            return (ushort)HueData.GetWebSafeHue(color);
        }

        public IFont GetUnicodeFont(int fontIndex)
        {
            return m_Fonts.GetUniFont(fontIndex);
        }

        public IFont GetAsciiFont(int fontIndex)
        {
            return m_Fonts.GetAsciiFont(fontIndex);
        }
    }
}
