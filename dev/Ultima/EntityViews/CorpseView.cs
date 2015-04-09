using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.Controllers;
using UltimaXNA.Ultima.Entities.Items.Containers;

namespace UltimaXNA.Ultima.EntityViews
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
            int bodyID = Entity.BodyID;
            int frameIndex = (int)(Entity.Frame * BodyConverter.DeathAnimationFrameCount(bodyID));

            AnimationFrame animationFrame = getFrame(bodyID, facing, frameIndex, Entity.Hue);

            DrawTexture = animationFrame.Texture;
            DrawArea = new Rectangle(0, 0, DrawTexture.Width, DrawTexture.Height);
            DrawFlip = false;

            return base.Draw(spriteBatch, drawPosition, mouseOverList, map);
        }

        private AnimationFrame getFrame(int bodyID, int facing, int frameIndex, int hue)
        {
            AnimationFrame[] iFrames = Animations.GetAnimation(bodyID, IO.BodyConverter.DeathAnimationIndex(bodyID), facing, hue);
            if (iFrames == null)
                return null;
            if (iFrames[frameIndex].Texture == null)
                return null;
            return iFrames[frameIndex];
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
