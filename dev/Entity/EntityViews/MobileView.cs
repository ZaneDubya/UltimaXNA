using Microsoft.Xna.Framework;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Model;
using UltimaXNA.UltimaData;

namespace UltimaXNA.Entity.EntityViews
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
            if (m_AllowDefer == false)
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

            for (int i = 0; i < m_LayerCount; i++)
            {
                if (m_MobileLayers[i].Frame != null)
                {
                    float x = -drawCenterX + (drawX + m_MobileLayers[i].Frame.Center.X);
                    float y = -drawY - (m_MobileLayers[i].Frame.Texture.Height + m_MobileLayers[i].Frame.Center.Y) + drawCenterY;

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

            DrawOverheads(spriteBatch, drawPosition, mouseOverList, map, 100);

            return true;
        }

        // ======================================================================
        // Code to get frames for drawing
        // ======================================================================

        private AnimationFrame getFrame(int bodyID, int hue, int facing, int action, float frame)
        {
            AnimationFrame[] iFrames = UltimaData.AnimationData.GetAnimation(bodyID, action, facing, hue);
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
        // Deferred drawing code
        // ======================================================================
        
        private bool m_AllowDefer = false;

        public void SetAllowDefer()
        {
            m_AllowDefer = true;
        }

        private bool CheckDefer(Map map, Vector3 drawPosition)
        {
            MapTile deferToTile;
            Direction checkDirection;
            if (Entity.IsMoving)
            {
                if (
                    ((Entity.Facing & Direction.FacingMask) == Direction.Left) ||
                    ((Entity.Facing & Direction.FacingMask) == Direction.South) ||
                    ((Entity.Facing & Direction.FacingMask) == Direction.East))
                {
                    deferToTile = map.GetMapTile(Entity.Position.X, Entity.Position.Y + 1);
                    checkDirection = Entity.Facing & Direction.FacingMask;
                }
                else if ((Entity.Facing & Direction.FacingMask) == Direction.Down)
                {
                    deferToTile = map.GetMapTile(Entity.Position.X + 1, Entity.Position.Y + 1);
                    checkDirection = Direction.Down;
                }
                else
                {
                    deferToTile = map.GetMapTile(Entity.Position.X + 1, Entity.Position.Y);
                    checkDirection = Direction.East;
                }
            }
            else
            {
                deferToTile = map.GetMapTile(Entity.Position.X + 1, Entity.Position.Y);
                checkDirection = Direction.East;
            }

            if (deferToTile != null)
            {
                int z;
                if (MobileMovementCheck.CheckMovementForced(Entity, Entity.Position, checkDirection, out z))
                {
                    MobileDeferred deferred = new MobileDeferred(Entity, drawPosition, z);
                    deferToTile.OnEnter(deferred);
                    return true;
                }
            }
            return false;
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
            m_FrameCount = UltimaData.AnimationData.GetAnimationFrameCount(bodyID, animation, facing, hue);
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
