using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;

namespace UltimaXNA.Core.UI
{
    interface ICursor
    {
        void Update();
        void Draw(SpriteBatchUI spriteBatch, Point mousePosition);
    }
}
