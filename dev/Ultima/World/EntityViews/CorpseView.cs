using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.WorldViews;

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

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            CheckDefer(map, drawPosition);

            return DrawInternal(spriteBatch, drawPosition, mouseOverList, map, roofHideFlag);
        }

        public override bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            int facing = MirrorFacingForDraw(Entity.Facing);
            int frameIndex = (int)(Entity.Frame * BodyConverter.DeathAnimationFrameCount(Entity.Body));

            IAnimationFrame animationFrame = getFrame(Entity.Body, facing, frameIndex, Entity.Hue);

            DrawFlip = (MirrorFacingForDraw(Entity.Facing) > 4) ? true : false;

            DrawTexture = animationFrame.Texture;

            DrawArea = new Rectangle(
                animationFrame.Center.X - IsometricRenderer.TILE_SIZE_INTEGER_HALF,
                DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER_HALF + (Entity.Z * 4) + animationFrame.Center.Y, DrawTexture.Width, DrawTexture.Height);

            return base.Draw(spriteBatch, drawPosition, mouseOverList, map, roofHideFlag);
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
