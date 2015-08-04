using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Core.UI
{
    public interface IUIResourceProvider
    {
        Texture2D GetUITexture(int textureID);
        Texture2D GetItemTexture(int textureID);
        ushort GetWebSafeHue(Color color);
        IFont GetUnicodeFont(int fontIndex);
        IFont GetAsciiFont(int fontIndex);
    }
}
