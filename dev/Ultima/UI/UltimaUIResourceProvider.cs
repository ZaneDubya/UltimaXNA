using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Ultima.UI
{
    class UltimaUIResourceProvider : IUIResourceProvider
    {
        public UltimaUIResourceProvider()
        {

        }

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
            return IO.FontsNew.TextUni.GetFont(fontIndex);
        }
    }
}
