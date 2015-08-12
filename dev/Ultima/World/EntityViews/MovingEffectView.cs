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
    public class MovingEffectView : AEntityView
    {
        MovingEffect Effect
        {
            get
            {
                return (MovingEffect)base.Entity;
            }
        }

        EffectData m_AnimData;
        bool m_Animated;

        int m_DisplayItemID = -1;

        public MovingEffectView(MovingEffect effect)
            : base(effect)
        {
            m_Animated = true;
            m_Animated = TileData.ItemData[Effect.ItemID & 0x3fff].IsAnimation;
            if (m_Animated)
            {
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                m_AnimData = provider.GetResource<EffectData>(Effect.ItemID);
                m_Animated = m_AnimData.FrameCount > 0;
            }
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
                DrawArea = new Rectangle(DrawTexture.Width / 2 - 22, DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER, DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }

            DrawArea.X = 0 - (int)((Entity.Position.X_offset - Entity.Position.Y_offset) * 22);
            DrawArea.Y = 0 + (int)((Entity.Position.Z_offset + Entity.Z) * 4) - (int)((Entity.Position.X_offset + Entity.Position.Y_offset) * 22);

            Rotation = Effect.AngleToTarget;

            // Update hue vector.
            HueVector = Utility.GetHueVector(Entity.Hue);

            return base.Draw(spriteBatch, drawPosition, mouseOverList, map);
        }
    }
}
