using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld;

namespace UltimaXNA.Entity.EntityViews
{
    class DeferredView : AEntityView
    {
        public AEntityView View;
        public Vector3 DrawPosition;
        public Point Tile;

        public DeferredView(AEntityView view, Vector3 drawPosition)
            : base(view.Entity)
        {
            View = view;
            DrawPosition = drawPosition;
            Tile = new Point(view.Entity.Position.X + 1, view.Entity.Position.Y + 1);
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, PickTypes pickType)
        {
            if (View is MobileView) // only MobileView implements draw_Deferred
            {
                return ((MobileView)View).DrawDeferred(spriteBatch, DrawPosition, mouseOverList, pickType);
            }
            return false;
        }
    }
}
