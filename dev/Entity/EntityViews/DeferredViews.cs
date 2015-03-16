using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld.View;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Model;

namespace UltimaXNA.Entity.EntityViews
{
    static class DeferredViews
    {
        public static void AddDeferredView(MapTile tile, Vector3 drawPosition, Texture2D texture, Rectangle dest, Vector2 hue)
        {
            int deferredY = (int)drawPosition.Y + 22;
            if (deferredY < dest.Bottom)
            {
                int tilesY = 0;
                while (deferredY < dest.Bottom)
                {
                    deferredY += 22;
                    tilesY += 1;
                }
                Point deferred_top = new Point((int)drawPosition.X + 22 - tilesY * 22, (int)drawPosition.Y + tilesY * 22);
                while (!dest.Contains(deferred_top))
                {
                    break;
                }
            }
        }
    }
}