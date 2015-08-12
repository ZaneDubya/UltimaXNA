using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Core.Resources;

namespace UltimaXNA.Ultima.World.EntityViews
{
    class CorpseView : AEntityView
    {
        new Corpse Entity
        {
            get { return (Corpse)base.Entity; }
        }

        public CorpseView(Corpse entity)
            : base(entity)
        {
            PickType = PickType.PickObjects;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map)
        {
            int facing = MirrorFacingForDraw(Entity.Facing);
            int frameIndex = (int)(Entity.Frame * BodyConverter.DeathAnimationFrameCount(Entity.Body));

            IAnimationFrame animationFrame = getFrame(Entity.Body, facing, frameIndex, Entity.Hue);

            DrawTexture = animationFrame.Texture;
            DrawArea = new Rectangle(0, 0, DrawTexture.Width, DrawTexture.Height);
            DrawFlip = false;

            return base.Draw(spriteBatch, drawPosition, mouseOverList, map);
        }

        private IAnimationFrame getFrame(Body body, int facing, int frameIndex, int hue)
        {
            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            IAnimationFrame[] frames = provider.GetAnimation(body, BodyConverter.DeathAnimationIndex(body), facing, hue);
            if (frames == null)
                return null;
            if (frames[frameIndex].Texture == null)
                return null;
            return frames[frameIndex];
        }

        private int MirrorFacingForDraw(Direction facing)
        {
            int iFacing = (int)((Direction)facing & Direction.FacingMask);
            if (iFacing >= 3)
                return iFacing - 3;
            else
                return iFacing + 5;
        }
    }
}
