/***************************************************************************
 *   MobileView.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.WorldViews;
using UltimaXNA.Ultima.World.Entities;
#endregion

namespace UltimaXNA.Ultima.World.EntityViews
{
    /// <summary>
    /// A representation of a mobile object within the isometric world. Draws a separate sprite for each worn item.
    /// </summary>
    public class MobileView : AEntityView
    {
        // ======================================================================
        // Public methods and properties: ctr, update, draw, and Animation property.
        // ======================================================================

        /// <summary>
        /// Manages the animation state (what animation is playing, how far along is the animation, etc.) for this
        /// mobile view. Exposed as public so that we can receive animations from the server (e.g. emotes).
        /// </summary>
        public readonly MobileAnimation Animation;

        private new Mobile Entity
        {
            get { return (Mobile)base.Entity; }
        }

        public MobileView(Mobile mobile)
            : base(mobile)
        {
            Animation = new MobileAnimation(mobile);
            m_MobileLayers = new MobileViewLayer[(int)EquipLayer.LastUserValid];
            PickType = PickType.PickObjects;

            IsShadowCastingView = true;
        }

        public void Update(double frameMS)
        {
            Animation.Update(frameMS);
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            if (roofHideFlag && CheckUnderSurface(map, Entity.DestinationPosition.X, Entity.DestinationPosition.Y))
                return false;

            CheckDefer(map, drawPosition);

            return DrawInternal(spriteBatch, drawPosition, mouseOverList, map, roofHideFlag);
        }

        public override bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            if (Entity.IsDisposed)
                return false;
            if (Entity.Body == 0)
                return false;
            
            // get a z index underneath all this mobile's sprite layers. We will place the shadow on this z index.
            DrawShadowZDepth = spriteBatch.GetNextUniqueZ();
            
            // get/update the animation index.
            if (Entity.IsMoving)
            {
                if (Entity.IsRunning)
                    Animation.Animate(MobileAction.Run);
                else
                    Animation.Animate(MobileAction.Walk);
            }
            else
            {
                if (!Animation.IsAnimating)
                    Animation.Animate(MobileAction.Stand);
            }

            // flip the facing (anim directions are reversed from the client-server protocol's directions).
            DrawFlip = (MirrorFacingForDraw(Entity.DrawFacing) > 4) ? true : false;

            InternalSetupLayers();

            int drawCenterY = m_MobileLayers[0].Frame.Center.Y;

            int drawX, drawY;
            if (DrawFlip)
            {
                drawX = -IsometricRenderer.TILE_SIZE_INTEGER_HALF + (int)((Entity.Position.X_offset - Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF);
                drawY = drawCenterY + (int)((Entity.Position.Z_offset + Entity.Z) * 4) - IsometricRenderer.TILE_SIZE_INTEGER_HALF - (int)((Entity.Position.X_offset + Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF);
            }
            else
            {
                drawX = -IsometricRenderer.TILE_SIZE_INTEGER_HALF - (int)((Entity.Position.X_offset - Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF);
                drawY = drawCenterY + (int)((Entity.Position.Z_offset + Entity.Z) * 4) - IsometricRenderer.TILE_SIZE_INTEGER_HALF - (int)((Entity.Position.X_offset + Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF);
            }

            if (Entity.IsSitting)
            {
                drawX -= 1;
                drawY -= Entity.SittingPixelOffset + 8;
            }

            IsShadowCastingView = !Entity.IsSitting;

            // get the maximum y-extent of this object so we can correctly place overheads.
            int yOffset = 0;

            for (int i = 0; i < m_LayerCount; i++)
            {
                if (m_MobileLayers[i].Frame != null)
                {
                    float x = (drawX + m_MobileLayers[i].Frame.Center.X);
                    float y = -drawY - (m_MobileLayers[i].Frame.Texture.Height + m_MobileLayers[i].Frame.Center.Y) + drawCenterY;

                    if (yOffset > y)
                        yOffset = (int)y;

                    DrawTexture = m_MobileLayers[i].Frame.Texture;
                    DrawArea = new Rectangle((int)x, (int)-y, DrawTexture.Width, DrawTexture.Height);
                    HueVector = Utility.GetHueVector(m_MobileLayers[i].Hue);

                    Rectangle screenDest = new Rectangle(
                        /* x */ DrawFlip ? (int)drawPosition.X + DrawArea.X - DrawArea.Width + IsometricRenderer.TILE_SIZE_INTEGER : (int)drawPosition.X - DrawArea.X,
                        /* y */ (int)drawPosition.Y - DrawArea.Y,
                        /* w */ DrawFlip ? DrawArea.Width : DrawArea.Width,
                        /* h */ DrawArea.Height);

                    base.Draw(spriteBatch, drawPosition, mouseOverList, map, roofHideFlag);
                }
            }

            Vector3 overheadDrawPosition = new Vector3(drawPosition.X + (int)((Entity.Position.X_offset - Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF),
                drawPosition.Y - (int)((Entity.Position.Z_offset + Entity.Z) * 4),
                drawPosition.Z);

            if (m_MobileLayers[0].Frame != null)
            {
                yOffset = m_MobileLayers[0].Frame.Texture.Height + drawY - (int)((Entity.Z + Entity.Position.Z_offset) * 4);
            }
            else
            {
                yOffset = -(yOffset + IsometricRenderer.TILE_SIZE_INTEGER);
            }

            DrawOverheads(spriteBatch, overheadDrawPosition, mouseOverList, map, yOffset);

            return true;
        }

        // ======================================================================
        // Code to get frames for drawing
        // ======================================================================

        private IAnimationFrame getFrame(int bodyID, int hue, int facing, int action, float frame, out int frameCount)
        {
            if (bodyID >= 500 && bodyID <= 505)
                patchLightSourceAction(ref action, ref frame);

            frameCount = 0;

            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            IAnimationFrame[] frames = provider.GetAnimation(bodyID, action, facing, hue);
            if (frames == null)
                return null;
            frameCount = frames.Length;
            int iFrame = (int)frame; // frameFromSequence(frame, iFrames.Length);
            if (frames[iFrame].Texture == null)
                return null;
            return frames[iFrame];
        }

        private void patchLightSourceAction(ref int action, ref float frame)
        {
            if (action == (int)ActionIndexHumanoid.Walk)
                action = (int)ActionIndexHumanoid.Walk_Armed;
            else if (action == (int)ActionIndexHumanoid.Run)
                action = (int)ActionIndexHumanoid.Run_Armed;
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

            if (Entity.Body.IsHumanoid)
            {
                int[] drawLayers = DrawLayerOrder;
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
                        AddLayer(Entity.Body, Entity.Hue);
                    }
                    else if (Entity.Equipment[drawLayers[i]] != null && Entity.Equipment[drawLayers[i]].ItemData.AnimID != 0)
                    {
                        // skip hair/facial hair for ghosts
                        if (!Entity.IsAlive && (drawLayers[i] == (int)EquipLayer.Hair || drawLayers[i] == (int)EquipLayer.FacialHair))
                            continue;
                        AddLayer(Entity.Equipment[drawLayers[i]].ItemData.AnimID, Entity.Equipment[drawLayers[i]].Hue);
                    }
                }
            }
            else
            {
                AddLayer(Entity.Body, Entity.Hue);
            }
        }

        private void AddLayer(int bodyID, int hue)
        {
            int facing = MirrorFacingForDraw(Entity.DrawFacing);
            int animation = Animation.ActionIndex;
            float frame = Animation.AnimationFrame;

            int frameCount;
            m_MobileLayers[m_LayerCount++] = new MobileViewLayer(bodyID, hue, getFrame(bodyID, hue, facing, animation, frame, out frameCount));
            m_FrameCount = frameCount;
        }

        private void ClearLayers()
        {
            m_LayerCount = 0;
        }

        private int[] DrawLayerOrder
        {
            get
            {
                int direction = MirrorFacingForDraw(Entity.DrawFacing);
                switch (direction)
                {
                    case 0x00: return s_DrawLayerOrderDown;
                    case 0x01: return s_DrawLayerOrderSouth;
                    case 0x02: return s_DrawLayerOrderLeft;
                    case 0x03: return s_DrawLayerOrderWest;
                    case 0x04: return s_DrawLayerOrderUp;
                    case 0x05: return s_DrawLayerOrderNorth;
                    case 0x06: return s_DrawLayerOrderRight;
                    case 0x07: return s_DrawLayerOrderEast;
                    default:
                        // MirrorFacingForDraw ands with 0x07, this will never happen.
                        return s_DrawLayerOrderNorth;
                }
            }
        }

        private static int[] s_DrawLayerOrderNorth = new int[] { 
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, 
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, 
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, 
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, 
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, 
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm, 
            (int)EquipLayer.TwoHanded, 
            };
        private static int[] s_DrawLayerOrderRight = new int[] { 
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, 
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, 
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, 
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, 
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair,
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm, 
            (int)EquipLayer.TwoHanded };
        private static int[] s_DrawLayerOrderEast = new int[] { 
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, 
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, 
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, 
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, 
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, 
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm, 
            (int)EquipLayer.TwoHanded };
        private static int[] s_DrawLayerOrderDown = new int[] { 
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Cloak, (int)EquipLayer.Shirt, 
            (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, 
            (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, 
            (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, 
            (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, 
            (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Helm, 
            (int)EquipLayer.TwoHanded };
        private static int[] s_DrawLayerOrderSouth = new int[] { 
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, 
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, 
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, 
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, 
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, 
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm, 
            (int)EquipLayer.TwoHanded };
        private static int[] s_DrawLayerOrderLeft = new int[] { 
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, 
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, 
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, 
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, 
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, 
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm, 
            (int)EquipLayer.TwoHanded };
        private static int[] s_DrawLayerOrderWest = new int[] { 
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, 
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, 
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, 
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, 
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, 
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm, 
            (int)EquipLayer.TwoHanded };
        private static int[] s_DrawLayerOrderUp = new int[] { 
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, 
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, 
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, 
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, 
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, 
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded };

        struct MobileViewLayer
        {
            public int Hue;
            public IAnimationFrame Frame;
            public int BodyID;

            public MobileViewLayer(int bodyID, int hue, IAnimationFrame frame)
            {
                BodyID = bodyID;
                Hue = hue;
                Frame = frame;
            }

            public override string ToString()
            {
                return string.Format("Body:{0}", BodyID);
            }
        }
    }
}
