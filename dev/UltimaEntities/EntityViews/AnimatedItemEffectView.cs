using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaEntities.Effects;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Maps;
using UltimaXNA.UltimaWorld.Controllers;

namespace UltimaXNA.UltimaEntities.EntityViews
{
    public class AnimatedItemEffectView : AEntityView
    {
        AnimatedItemEffect Effect
        {
            get
            {
                return (AnimatedItemEffect)base.Entity;
            }
        }

        AnimData.AnimDataEntry m_AnimData;
        bool m_Animated;
        int m_DisplayItemID = -1;

        public AnimatedItemEffectView(AnimatedItemEffect effect)
            : base(effect)
        {
            m_Animated = true;
            m_AnimData = AnimData.GetAnimData(Effect.ItemID & 0x3fff);
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

            int displayItemdID = (m_Animated) ? Effect.ItemID + ((Effect.FramesActive / m_AnimData.FrameInterval) % m_AnimData.FrameCount) : Effect.ItemID;

            if (displayItemdID != m_DisplayItemID)
            {
                m_DisplayItemID = displayItemdID;
                DrawTexture = UltimaData.ArtData.GetStaticTexture(m_DisplayItemID);
                DrawArea = new Rectangle(DrawTexture.Width / 2 - 22, DrawTexture.Height - 44 + (Entity.Z * 4), DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }

            // Update hue vector.
            HueVector = Utility.GetHueVector(Entity.Hue);

            return base.Draw(spriteBatch, drawPosition, mouseOverList, map);
        }
    }
}
