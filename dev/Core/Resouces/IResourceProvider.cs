using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Core.Resources
{
    public interface IResourceProvider
    {
        Texture2D GetUITexture(int textureID);
        Texture2D GetItemTexture(int textureID);
        Texture2D GetLandTexture(int textureID);
        void GetItemDimensions(int textureID, out int width, out int height);

        ushort GetWebSafeHue(Color color);
        IFont GetUnicodeFont(int fontIndex);
        IFont GetAsciiFont(int fontIndex);
    }
}
