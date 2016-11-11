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
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.Entities.Mobiles.Animations;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.WorldViews;
#endregion

namespace UltimaXNA.Ultima.World.EntityViews
{
    /// <summary>
    /// A representation of a mobile object within the isometric world. Draws a separate sprite for each worn item.
    /// </summary>
    public class MobileView : AEntityView
    {
        public Body Body
        {
            get
            {
                if (Entity is Mobile)
                    return (Entity as Mobile).Body;
                if (Entity is Corpse)
                    return (Entity as Corpse).Body;
                return 0;
            }
        }

        public Direction Facing
        {
            get
            {
                if (Entity is Mobile)
                    return (Entity as Mobile).DrawFacing;
                else if (Entity is Corpse)
                    return (Entity as Corpse).Facing;
                return Direction.Nothing;
            }
        }

        public MobileEquipment Equipment
        {
            get
            {
                if (Entity is Mobile)
                    return (Entity as Mobile).Equipment;
                else if (Entity is Corpse)
                    return (Entity as Corpse).Equipment;
                return null;
            }
        }

        // ============================================================================================================
        // Public methods and properties: ctr, update, draw
        // ============================================================================================================

        public MobileView(AEntity entity)
            : base(entity)
        {
            m_MobileLayers = new MobileViewLayer[(int)EquipLayer.LastUserValid];
            PickType = PickType.PickObjects;

            IsShadowCastingView = true;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            if (Body == 0)
                return false;

            if (roofHideFlag)
            {
                int x = (Entity is Mobile) ? (Entity as Mobile).DestinationPosition.X : Entity.X;
                int y = (Entity is Mobile) ? (Entity as Mobile).DestinationPosition.Y : Entity.Y;
                if (CheckUnderSurface(map, x, y))
                    return false;
            }

            CheckDefer(map, drawPosition);

            return DrawInternal(spriteBatch, drawPosition, mouseOverList, map, roofHideFlag);
        }

        public override bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            if (Entity.IsDisposed)
                return false;
            
            // get a z index underneath all this mobile's sprite layers. We will place the shadow on this z index.
            DrawShadowZDepth = spriteBatch.GetNextUniqueZ();
            
            // get running moving and sitting booleans, which are used when drawing mobiles but not corpses.
            bool isRunning = false, isMoving = false, isSitting = false;
            if (Entity is Mobile)
            {
                isRunning = (Entity as Mobile).IsRunning;
                isMoving = (Entity as Mobile).IsMoving;
                isSitting = (Entity as Mobile).IsSitting;
            }

            // flip the facing (anim directions are reversed from the client-server protocol's directions).
            DrawFlip = (MirrorFacingForDraw(Facing) > 4) ? true : false;

            InternalSetupLayers();

            if (m_MobileLayers[0].Frame == null)
                m_MobileLayers[0].Frame = AnimationFrame.NullFrame;
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

            if (isSitting)
            {
                drawX -= 1;
                drawY -= 6 + (Entity as Mobile).ChairData.SittingPixelOffset;
                if (Facing == Direction.North || Facing == Direction.West)
                {
                    drawY -= 16;
                }
            }

            IsShadowCastingView = !isSitting;

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

            // this is where we would draw the reverse of the chair texture.

            bool isMounted = (Entity is Mobile) ? (Entity as Mobile).IsMounted ? true : false : false;
            DrawOverheads(spriteBatch, overheadDrawPosition, mouseOverList, map, isMounted ? yOffset + 16 : yOffset);

            return true;
        }

        // ============================================================================================================
        // Code to get frames for drawing
        // ============================================================================================================

        private IAnimationFrame getFrame(int body, ref int hue, int facing, int action, float frame, out int frameCount)
        {
            // patch light source animations: candles and torches.
            if (body >= 500 && body <= 505)
                patchLightSourceAction(ref action);

            frameCount = 0;

            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            IAnimationFrame[] frames = provider.GetAnimation(body, ref hue, action, facing);
            if (frames == null)
                return null;
            frameCount = frames.Length;
            int iFrame = (int)frame; // frameFromSequence(frame, iFrames.Length);
            if (iFrame >= frameCount)
                iFrame = 0;

            if (frames[iFrame].Texture == null)
                return null;
            return frames[iFrame];
        }

        private int patchMountAction(int action)
        {
            switch ((ActionIndexHumanoid)action)
            {
                case ActionIndexHumanoid.Mounted_RideFast:
                    return (int)ActionIndexAnimal.Run;
                case ActionIndexHumanoid.Mounted_RideSlow:
                    return (int)ActionIndexAnimal.Walk;
                case ActionIndexHumanoid.Mounted_Attack_1H:
                case ActionIndexHumanoid.Mounted_Attack_Bow:
                case ActionIndexHumanoid.Mounted_Attack_BowX:
                case ActionIndexHumanoid.Block_WithShield:
                    return (int)ActionIndexAnimal.Attack3;
                default:
                    return (int)ActionIndexAnimal.Stand;
            }
        }

        private void patchLightSourceAction(ref int action)
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

        // ============================================================================================================
        // Layer management
        // ============================================================================================================

        private int m_LayerCount = 0;
        private int m_FrameCount = 0;
        private MobileViewLayer[] m_MobileLayers;

        private void InternalSetupLayers()
        {
            ClearLayers();

            if (Body.IsHumanoid)
            {
                int[] drawLayers = DrawLayerOrder;
                bool hasOuterTorso = Equipment[(int)EquipLayer.OuterTorso] != null && Equipment[(int)EquipLayer.OuterTorso].ItemData.AnimID != 0;

                for (int i = 0; i < drawLayers.Length; i++)
                {
                    // when wearing something on the outer torso the other torso equipment is not drawn in the world.
                    if (hasOuterTorso && (drawLayers[i] == (int)EquipLayer.InnerTorso || drawLayers[i] == (int)EquipLayer.MiddleTorso))
                    {
                        continue;
                    }

                    if (drawLayers[i] == (int)EquipLayer.Body)
                    {
                        AddLayer(Body, Entity.Hue);
                    }

                    else if (Equipment[drawLayers[i]] != null)
                    {
                        // special handling for mounts.
                        if (drawLayers[i] == (int)EquipLayer.Mount)
                        {
                            int body = Equipment[drawLayers[i]].ItemID;
                            if (BodyConverter.CheckIfItemIsMount(ref body))
                            {
                                AddLayer(body, Equipment[drawLayers[i]].Hue, true);
                            }
                        }
                        else
                        {
                            if (Equipment[drawLayers[i]].ItemData.AnimID != 0)
                            {
                                // skip hair/facial hair for ghosts
                                if (((Entity is Mobile) && !(Entity as Mobile).IsAlive) &&
                                    (drawLayers[i] == (int)EquipLayer.Hair || drawLayers[i] == (int)EquipLayer.FacialHair))
                                {
                                    continue;
                                }
                                AddLayer(Equipment[drawLayers[i]].ItemData.AnimID, Equipment[drawLayers[i]].Hue);
                            }
                        }
                    }
                }
            }
            else
            {
                AddLayer(Body, Entity.Hue);
            }
        }

        private void AddLayer(int bodyID, int hue, bool asMount = false)
        {
            int facing = MirrorFacingForDraw(Facing);
            int animation = 0;
            float frame = 0;
            if (Entity is Mobile)
            {
                animation = (Entity as Mobile).Animation.ActionIndex;
                if (asMount)
                    animation = patchMountAction(animation);
                frame = (Entity as Mobile).Animation.AnimationFrame;
            }
            else if (Entity is Corpse)
            {
                animation = ActionTranslator.GetActionIndex(Entity, MobileAction.Death);
                frame = (Entity as Corpse).Frame * BodyConverter.DeathAnimationFrameCount(Body);
            }

            int frameCount;
            IAnimationFrame animframe = getFrame(bodyID, ref hue, facing, animation, frame, out frameCount);
            m_MobileLayers[m_LayerCount++] = new MobileViewLayer(bodyID, hue, animframe);
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
                int direction = MirrorFacingForDraw(Facing);
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
