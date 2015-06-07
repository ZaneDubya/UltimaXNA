using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.World.Entities.Effects;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.Input;

namespace UltimaXNA.Ultima.World.EntityViews
{
    class LightningEffectView : AEntityView
    {
        LightningEffect Effect
        {
            get
            {
                return (LightningEffect)base.Entity;
            }
        }

        int m_DisplayItemID = -1;

        public LightningEffectView(LightningEffect effect)
            : base(effect)
        {

        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map)
        {
            if (!m_AllowDefer)
            {
                if (CheckDefer(map, drawPosition))
                    return false;
            }
            else
            {
                m_AllowDefer = false;
            }

            int displayItemdID = 0x4e20 + Effect.FramesActive;

            if (displayItemdID != m_DisplayItemID)
            {
                m_DisplayItemID = displayItemdID;
                DrawTexture = IO.GumpData.GetGumpXNA(displayItemdID);
                Point offset = m_Offsets[m_DisplayItemID - 20000];
                DrawArea = new Rectangle(offset.X, DrawTexture.Height - 33 + (Entity.Z * 4) + offset.Y, DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }

            // Update hue vector.
            HueVector = Utility.GetHueVector(Entity.Hue);

            return base.Draw(spriteBatch, drawPosition, mouseOverList, map);
        }

        static Point[] m_Offsets = new Point[10]
            {
                new Point(48, 0),
                new Point(68, 0),
                new Point(92, 0),
                new Point(72, 0),
                new Point(48, 0),
                new Point(56, 0),
                new Point(76, 0),
                new Point(76, 0),
                new Point(92, 0),
                new Point(80, 0)
            };
    }
}
