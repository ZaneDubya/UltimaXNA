using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Effects;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.WorldViews;

namespace UltimaXNA.Ultima.World.EntityViews
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

        EffectData m_AnimData;
        bool m_Animated;
        int m_DisplayItemID = -1;

        public AnimatedItemEffectView(AnimatedItemEffect effect)
            : base(effect)
        {
            m_Animated = true;
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            m_AnimData = provider.GetResource<EffectData>(Effect.ItemID);
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
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                DrawTexture = provider.GetItemTexture(m_DisplayItemID);
                DrawArea = new Rectangle(DrawTexture.Width / 2 - 22, DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER + (Entity.Z * 4), DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }

            // Update hue vector.
            HueVector = Utility.GetHueVector(Entity.Hue);

            return base.Draw(spriteBatch, drawPosition, mouseOverList, map);
        }
    }
}
