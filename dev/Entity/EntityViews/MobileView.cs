using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld.View;
using UltimaXNA.UltimaWorld;

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
            m_animation = new MobileAnimation(mobile);
            m_layers = new MobileViewLayer[(int)EquipLayer.LastUserValid];
            PickType = PickTypes.PickObjects;
        }

        public MobileAnimation m_animation;

        public void Update(double frameMS)
        {
            m_animation.Update(frameMS);
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, PickTypes pickType, int maxAlt)
        {
            DrawFlip = (MirrorFacingForDraw(Entity.Facing) > 4) ? true : false;

            if (Entity.IsMoving)
            {
                if (Entity.IsRunning)
                    m_animation.Animate(MobileAction.Run);
                else
                    m_animation.Animate(MobileAction.Walk);
            }
            else
            {
                if (!m_animation.IsAnimating)
                    m_animation.Animate(MobileAction.Stand);
            }

            int[] drawLayers = m_DrawLayerOrder;
            bool hasOuterTorso = Entity.Equipment[(int)EquipLayer.OuterTorso] != null && Entity.Equipment[(int)EquipLayer.OuterTorso].ItemData.AnimID != 0;

            ClearLayers();

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

            InternalPreRender(spriteBatch);

            return base.Draw(spriteBatch, drawPosition, mouseOverList, pickType, maxAlt);
        }

        protected override Vector2 HueVector
        {
            get
            {
                if (m_layerCount == 1)
                    return base.HueVector;
                else
                    return Utility.GetHueVector(0);
            }
        }

        private int m_mobile_drawCenterX, m_mobile_drawCenterY;

        private void InternalPreRender(SpriteBatch3D sb)
        {
            if (m_layerCount == 1)
            {
                DrawTexture = m_layers[0].Frame.Texture;
                DrawArea = new Rectangle(0, 0, DrawTexture.Width, DrawTexture.Height);
                m_mobile_drawCenterX = m_layers[0].Frame.Center.X;
                m_mobile_drawCenterY = m_layers[0].Frame.Center.Y;
            }
            else
            {
                long hash = createHashFromLayers();
                DrawTexture = MapObjectPrerendered.RestorePrerenderedTexture(hash, out m_mobile_drawCenterX, out m_mobile_drawCenterY);
                if (DrawTexture == null)
                {
                    int minX = 0, minY = 0;
                    int maxX = 0, maxY = 0;
                    for (int i = 0; i < m_layerCount; i++)
                    {
                        if (m_layers[i].Frame != null)
                        {
                            int x, y, w, h;
                            x = m_layers[i].Frame.Center.X;
                            y = m_layers[i].Frame.Center.Y;
                            w = m_layers[i].Frame.Texture.Width;
                            h = m_layers[i].Frame.Texture.Height;

                            if (minX < x)
                                minX = x;
                            if (maxX < w - x)
                                maxX = w - x;

                            if (minY < h + y)
                                minY = h + y;
                            if (maxY > y)
                                maxY = y;
                        }
                    }

                    m_mobile_drawCenterX = minX;
                    m_mobile_drawCenterY = maxY;

                    RenderTarget2D renderTarget = new RenderTarget2D(sb.GraphicsDevice,
                            minX + maxX, minY - maxY, false, SurfaceFormat.Color, DepthFormat.None);

                    sb.GraphicsDevice.SetRenderTarget(renderTarget);
                    sb.GraphicsDevice.Clear(Color.Transparent);

                    for (int i = 0; i < m_layerCount; i++)
                        if (m_layers[i].Frame != null)
                            sb.DrawSimple(m_layers[i].Frame.Texture,
                                new Vector3(
                                    minX - m_layers[i].Frame.Center.X,
                                    renderTarget.Height - m_layers[i].Frame.Texture.Height + maxY - m_layers[i].Frame.Center.Y,
                                    0),
                                    Utility.GetHueVector(m_layers[i].Hue));

                    sb.Flush();
                    DrawTexture = renderTarget;
                    MapObjectPrerendered.SavePrerenderedTexture(DrawTexture, hash, m_mobile_drawCenterX, m_mobile_drawCenterY);
                    sb.GraphicsDevice.SetRenderTarget(null);
                }
                DrawArea = new Rectangle(0, 0, DrawTexture.Width, DrawTexture.Height);
            }
        }

        private int m_layerCount = 0;
        private int m_frameCount = 0;
        private MobileViewLayer[] m_layers;

        public void AddLayer(int bodyID, int hue)
        {
            m_layers[m_layerCount++] = new MobileViewLayer(
                bodyID, hue, getFrame(bodyID, hue, MirrorFacingForDraw(Entity.Facing), m_animation.ActionIndex, m_animation.AnimationFrame));
            m_frameCount = UltimaData.AnimationData.GetAnimationFrameCount(
                bodyID, m_animation.ActionIndex, MirrorFacingForDraw(Entity.Facing), hue);
        }

        public void ClearLayers()
        {
            m_layerCount = 0;
        }

        private UltimaData.AnimationFrame getFrame(int bodyID, int hue, int facing, int action, float frame)
        {
            UltimaData.AnimationFrame[] iFrames = UltimaData.AnimationData.GetAnimation(bodyID, action, facing, hue);
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

        private long createHashFromLayers()
        {
            int[] hashArray = new int[m_layerCount * 2 + 3];
            hashArray[0] = m_animation.ActionIndex;
            hashArray[1] = MirrorFacingForDraw(Entity.Facing);
            hashArray[2] = frameFromSequence(m_animation.AnimationFrame, m_frameCount);
            for (int i = 0; i < m_layerCount; i++)
            {
                hashArray[3 + i * 2] = m_layers[i].BodyID;
                hashArray[4 + i * 2] = m_layers[i].Hue;
            }

            long hash = 0;
            for (int i = 0; i < hashArray.Length; i++)
                hash = unchecked(hash * 31 + hashArray[i]);
            return hash;
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

        private int MirrorFacingForDraw(Direction facing)
        {
            int iFacing = (int)((Direction)facing & Direction.FacingMask);
            if (iFacing >= 3)
                return iFacing - 3;
            else
                return iFacing + 5;
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
