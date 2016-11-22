using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Core.Resources
{
    public interface IResourceProvider
    {
        IAnimationFrame[] GetAnimation(int body, ref int hue, int action, int direction);
        Texture2D GetUITexture(int textureID, bool replaceMask080808 = false);
        Texture2D GetItemTexture(int textureID);
        Texture2D GetLandTexture(int textureID);
        Texture2D GetTexmapTexture(int textureID);

        bool IsPointInUITexture(int textureID, int x, int y);
        bool IsPointInItemTexture(int textureID, int x, int y, int extraRange = 0);
        void GetItemDimensions(int textureID, out int width, out int height);

        ushort GetWebSafeHue(Color color);
        IFont GetUnicodeFont(int fontIndex);
        IFont GetAsciiFont(int fontIndex);
        string GetString(int strIndex);

        void RegisterResource<T>(IResource<T> resource);
        T GetResource<T>(int resourceIndex);
    }
}
