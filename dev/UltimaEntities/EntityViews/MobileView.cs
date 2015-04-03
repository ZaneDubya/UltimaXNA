using Microsoft.Xna.Framework;
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Maps;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaWorld.Controllers;

namespace UltimaXNA.UltimaEntities.EntityViews
{
    public class MobileView : AEntityView
    {
        new Mobile Entity
        {
            get { return (Mobile)base.Entity; }
        }

        public MobileView(Mobile mobile)
            : base(mobile)
        {
            m_Animation = new MobileAnimation(mobile);

            m_MobileLayers = new MobileViewLayer[(int)EquipLayer.LastUserValid];
            PickType = PickTypes.PickObjects;
        }

        public MobileAnimation m_Animation;

        public void Update(double frameMS)
        {
            m_Animation.Update(frameMS);
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

            DrawFlip = (MirrorFacingForDraw(Entity.Facing) > 4) ? true : false;

            if (Entity.IsMoving)
            {
                if (Entity.IsRunning)
                    m_Animation.Animate(MobileAction.Run);
                else
                    m_Animation.Animate(MobileAction.Walk);
            }
            else
            {
                if (!m_Animation.IsAnimating)
                    m_Animation.Animate(MobileAction.Stand);
            }

            InternalSetupLayers();

            int drawCenterX = m_MobileLayers[0].Frame.Center.X;
            int drawCenterY = m_MobileLayers[0].Frame.Center.Y;

            int drawX, drawY;
            if (DrawFlip)
            {
                drawX = drawCenterX - 22 + (int)((Entity.Position.X_offset - Entity.Position.Y_offset) * 22);
                drawY = drawCenterY + (int)((Entity.Position.Z_offset + Entity.Z) * 4) - 22 - (int)((Entity.Position.X_offset + Entity.Position.Y_offset) * 22);
            }
            else
            {
                drawX = drawCenterX - 22 - (int)((Entity.Position.X_offset - Entity.Position.Y_offset) * 22);
                drawY = drawCenterY + (int)((Entity.Position.Z_offset + Entity.Z) * 4) - 22 - (int)((Entity.Position.X_offset + Entity.Position.Y_offset) * 22);
            }

            // override hue based on notoriety if targeting? Not currently implemented.
            Vector2 hue;
            if (UltimaVars.EngineVars.LastTarget != null && UltimaVars.EngineVars.LastTarget == Entity.Serial)
                hue = new Vector2(Entity.NotorietyHue - 1, 1);

            // get the maximum y-extent of this object so we can correctly place overheads.
            int yOffset = 0;

            for (int i = 0; i < m_LayerCount; i++)
            {
                if (m_MobileLayers[i].Frame != null)
                {
                    float x = -drawCenterX + (drawX + m_MobileLayers[i].Frame.Center.X);
                    float y = -drawY - (m_MobileLayers[i].Frame.Texture.Height + m_MobileLayers[i].Frame.Center.Y) + drawCenterY;

                    if (yOffset > y)
                        yOffset = (int)y;

                    DrawTexture = m_MobileLayers[i].Frame.Texture;
                    DrawArea = new Rectangle((int)x, (int)-y, DrawTexture.Width, DrawTexture.Height);
                    HueVector = Utility.GetHueVector(m_MobileLayers[i].Hue);

                    Rectangle screenDest = new Rectangle(
                        DrawFlip ? (int)drawPosition.X + DrawArea.X - DrawArea.Width + 44 : (int)drawPosition.X - DrawArea.X,
                        (int)drawPosition.Y - DrawArea.Y,
                        DrawFlip ? DrawArea.Width : DrawArea.Width,
                        DrawArea.Height);

                    base.Draw(spriteBatch, drawPosition, mouseOverList, map);
                }
            }

            Vector3 overheadDrawPosition = new Vector3(drawPosition.X + (int)((Entity.Position.X_offset - Entity.Position.Y_offset) * 22),
                drawPosition.Y - (int)((Entity.Position.Z_offset + Entity.Z) * 4),
                drawPosition.Z);

            if (m_MobileLayers[0].Frame != null)
            {
                yOffset = m_MobileLayers[0].Frame.Texture.Height + drawY - (int)((Entity.Z + Entity.Position.Z_offset) * 4);
            }
            else
            {
                yOffset = -(yOffset + 44);
            }

            DrawOverheads(spriteBatch, overheadDrawPosition, mouseOverList, map, (int)yOffset);

            return true;
        }

        // ======================================================================
        // Code to get frames for drawing
        // ======================================================================

        private AnimationFrame getFrame(int bodyID, int hue, int facing, int action, float frame)
        {
            AnimationFrame[] iFrames = UltimaData.Animations.GetAnimation(bodyID, action, facing, hue);
            if (iFrames == null)
                return null;
            int iFrame = frameFromSequence(frame, iFrames.Length);
            if (iFrames[iFrame].Texture == null)
                return null;
            return iFrames[iFrame];
        }

        private int frameFromSequence(float frame, int maxFrames)
        {
            return (int)(frame * (float)maxFrames);
        }

        private int MirrorFacingForDraw(Direction facing)
        {
            int iFacing = (int)((Direction)facing & Direction.FacingMask);
            if (iFacing >= 3)
                return iFacing - 3;
            else
                return iFacing + 5;
        }

        // ======================================================================
        // Layer management
        // ======================================================================

        private int m_LayerCount = 0;
        private int m_FrameCount = 0;
        private MobileViewLayer[] m_MobileLayers;

        private void InternalSetupLayers()
        {
            ClearLayers();

            int[] drawLayers = m_DrawLayerOrder;
            bool hasOuterTorso = Entity.Equipment[(int)EquipLayer.OuterTorso] != null && Entity.Equipment[(int)EquipLayer.OuterTorso].ItemData.AnimID != 0;

            for (int i = 0; i < drawLayers.Length; i++)
            {
                // when wearing something on the outer torso the other torso stuff is not drawn
                if (hasOuterTorso && (drawLayers[i] == (int)EquipLayer.InnerTorso || drawLayers[i] == (int)EquipLayer.MiddleTorso))
                {
                    continue;
                }

                if (drawLayers[i] == (int)EquipLayer.Body)
                {
                    AddLayer(Entity.BodyID, Entity.Hue);
                }
                else if (Entity.Equipment[drawLayers[i]] != null && Entity.Equipment[drawLayers[i]].ItemData.AnimID != 0)
                {
                    AddLayer(Entity.Equipment[drawLayers[i]].ItemData.AnimID, Entity.Equipment[drawLayers[i]].Hue);
                }
            }
        }

        public void AddLayer(int bodyID, int hue)
        {
            int facing = MirrorFacingForDraw(Entity.Facing);
            int animation = m_Animation.ActionIndex;
            float frame = m_Animation.AnimationFrame;

            m_MobileLayers[m_LayerCount++] = new MobileViewLayer(bodyID, hue, getFrame(bodyID, hue, facing, animation, frame));
            m_FrameCount = UltimaData.Animations.GetAnimationFrameCount(bodyID, animation, facing, hue);
        }

        public void ClearLayers()
        {
            m_LayerCount = 0;
        }

        private int[] m_DrawLayerOrder
        {
            get
            {
                int direction = MirrorFacingForDraw(Entity.Facing);
                switch (direction)
                {
                    case 0x00: return m_DrawLayerOrderNorth;
                    case 0x01: return m_DrawLayerOrderRight;
                    case 0x02: return m_DrawLayerOrderEast;
                    case 0x03: return m_DrawLayerOrderDown;
                    case 0x04: return m_DrawLayerOrderSouth;
                    case 0x05: return m_DrawLayerOrderLeft;
                    case 0x06: return m_DrawLayerOrderWest;
                    case 0x07: return m_DrawLayerOrderUp;
                    default:
                        // TODO: Log an Error
                        return m_DrawLayerOrderNorth;
                }
            }
        }

        private static int[] m_DrawLayerOrderNorth = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };
        private static int[] m_DrawLayerOrderRight = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
        private static int[] m_DrawLayerOrderEast = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
        private static int[] m_DrawLayerOrderDown = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Cloak, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded };
        private static int[] m_DrawLayerOrderSouth = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
        private static int[] m_DrawLayerOrderLeft = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
        private static int[] m_DrawLayerOrderWest = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };
        private static int[] m_DrawLayerOrderUp = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };

        struct MobileViewLayer
        {
            public int Hue;
            public UltimaData.AnimationFrame Frame;
            public int BodyID;

            public MobileViewLayer(int bodyID, int hue, UltimaData.AnimationFrame frame)
            {
                BodyID = bodyID;
                Hue = hue;
                Frame = frame;
            }
        }
    }
}
