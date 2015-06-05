using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.UI;

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
            return IO.FontsNew.TextUni.GetFont(fontIndex);
        }
    }
}
