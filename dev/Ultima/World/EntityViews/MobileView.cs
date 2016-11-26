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
                if (Entity is Corpse)
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
                if (Entity is Corpse)
                    return (Entity as Corpse).Equipment;
                return null;
            }
        }

        // ============================================================================================================
        // ctor, pick
        // ============================================================================================================

        public MobileView(AEntity entity)
            : base(entity)
        {
            m_MobileLayers = new MobileViewLayer[(int)EquipLayer.LastUserValid];
            PickType = PickType.PickObjects;

            IsShadowCastingView = true;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (Body == 0)
                return false;

            if (roofHideFlag)
            {
                int x = (Entity is Mobile) ? (Entity as Mobile).DestinationPosition.X : Entity.X;
                int y = (Entity is Mobile) ? (Entity as Mobile).DestinationPosition.Y : Entity.Y;
                if (CheckUnderSurface(map, x, y))
                {
                    return false;
                }
            }
            CheckDefer(map, drawPosition);
            return DrawInternal(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }

        void MobilePick(MouseOverList mouseOver, Vector3 drawPosition, Rectangle area, AAnimationFrame frame)
        {
            int x, y;
            if (DrawFlip)
            {
                // when flipped, the upper right pixel = drawPosition.x + DrawArea.x + 44
                // the upper left pixel = drawposition.x + drawarea.x + 44 - drawarea.width.
                // don't forget to reverse the mouse position!
                x = (int)drawPosition.X + area.X + IsometricRenderer.TILE_SIZE_INTEGER - mouseOver.MousePosition.X;
            }
            else
            {
                // ul pixel = (drawposition - drawarea.x)
                x = mouseOver.MousePosition.X - (int)drawPosition.X + area.X;
            }
            y = mouseOver.MousePosition.Y - ((int)drawPosition.Y - area.Y);
            if (frame.IsPointInTexture(x, y))
            {
                mouseOver.AddItem(Entity, drawPosition);
            }
        }

        // ============================================================================================================
        // draw
        // ============================================================================================================

        public override bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (Entity.IsDisposed)
            {
                return false;
            }
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
            DrawFlip = (MirrorFacingForDraw(Facing) > 4);
            InternalSetupLayers();
            if (m_MobileLayers[0].Frame == null)
            {
                m_MobileLayers[0].Frame = AnimationFrame.NullFrame;
            }
            int drawCenterY = m_MobileLayers[0].Frame.Center.Y;
            int drawX;
            int drawY;
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
            int yOffset = 0;
            for (int i = 0; i < m_LayerCount; i++)
            {
                if (m_MobileLayers[i].Frame != null)
                {
                    AAnimationFrame frame = m_MobileLayers[i].Frame;
                    int x = (drawX + frame.Center.X);
                    int y = -drawY - (frame.Texture.Height + frame.Center.Y) + drawCenterY;
                    if (yOffset > y)
                    {
                        yOffset = y;
                    }
                    DrawTexture = frame.Texture;
                    DrawArea = new Rectangle(x, -y, DrawTexture.Width, DrawTexture.Height);
                    HueVector = Utility.GetHueVector(m_MobileLayers[i].Hue);
                    base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
                    MobilePick(mouseOver, drawPosition, DrawArea, frame);
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

            bool isMounted = (Entity is Mobile) && (Entity as Mobile).IsMounted;
            DrawOverheads(spriteBatch, overheadDrawPosition, mouseOver, map, isMounted ? yOffset + 16 : yOffset);

            return true;
        }

        // ============================================================================================================
        // Code to get frames for drawing
        // ============================================================================================================

        AAnimationFrame getFrame(int body, ref int hue, int facing, int action, float frame, out int frameCount)
        {
            // patch light source animations: candles and torches.
            if (body >= 500 && body <= 505)
            {
                PatchLightSourceAction(ref action);
            }
            frameCount = 0;
            AAnimationFrame[] frames = Provider.GetAnimation(body, ref hue, action, facing);
            if (frames == null)
            {
                return null;
            }
            frameCount = frames.Length;
            int iFrame = (int)frame; // frameFromSequence(frame, iFrames.Length);
            if (iFrame >= frameCount)
            {
                iFrame = 0;
            }
            if (frames[iFrame].Texture == null)
            {
                return null;
            }
            return frames[iFrame];
        }

        int PatchMountAction(int action)
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

        void PatchLightSourceAction(ref int action)
        {
            if (action == (int)ActionIndexHumanoid.Walk)
                action = (int)ActionIndexHumanoid.Walk_Armed;
            else if (action == (int)ActionIndexHumanoid.Run)
                action = (int)ActionIndexHumanoid.Run_Armed;
        }

        int GetFrameFromSequence(float frame, int maxFrames)
        {
            return (int)(frame * maxFrames);
        }

        int MirrorFacingForDraw(Direction facing)
        {
            int iFacing = (int)(facing & Direction.FacingMask);
            return (iFacing >= 3) ? iFacing - 3 : iFacing + 5;
        }

        // ============================================================================================================
        // Layer management
        // ============================================================================================================

        int m_LayerCount;
        int m_FrameCount;
        MobileViewLayer[] m_MobileLayers;

        void InternalSetupLayers()
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

        void AddLayer(int bodyID, int hue, bool asMount = false)
        {
            int facing = MirrorFacingForDraw(Facing);
            int animation = 0;
            float frame = 0;
            if (Entity is Mobile)
            {
                animation = (Entity as Mobile).Animation.ActionIndex;
                if (asMount)
                    animation = PatchMountAction(animation);
                frame = (Entity as Mobile).Animation.AnimationFrame;
            }
            else if (Entity is Corpse)
            {
                animation = ActionTranslator.GetActionIndex(Entity, MobileAction.Death);
                frame = (Entity as Corpse).Frame * BodyConverter.DeathAnimationFrameCount(Body);
            }

            int frameCount;
            AAnimationFrame animframe = getFrame(bodyID, ref hue, facing, animation, frame, out frameCount);
            m_MobileLayers[m_LayerCount++] = new MobileViewLayer(bodyID, hue, animframe);
            m_FrameCount = frameCount;
        }

        void ClearLayers()
        {
            m_LayerCount = 0;
        }

        int[] DrawLayerOrder
        {
            get
            {
                int direction = MirrorFacingForDraw(Facing);
                switch (direction)
                {
                    case 0x00:
                        return s_DrawLayerOrderDown;
                    case 0x01:
                        return s_DrawLayerOrderSouth;
                    case 0x02:
                        return s_DrawLayerOrderLeft;
                    case 0x03:
                        return s_DrawLayerOrderWest;
                    case 0x04:
                        return s_DrawLayerOrderUp;
                    case 0x05:
                        return s_DrawLayerOrderNorth;
                    case 0x06:
                        return s_DrawLayerOrderRight;
                    case 0x07:
                        return s_DrawLayerOrderEast;
                    default:
                        // MirrorFacingForDraw ands with 0x07, this will never happen.
                        return s_DrawLayerOrderNorth;
                }
            }
        }

        static int[] s_DrawLayerOrderNorth = {
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants,
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring,
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms,
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck,
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair,
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded
            };
        static int[] s_DrawLayerOrderRight = {
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants,
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring,
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms,
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck,
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair,
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded };
        static int[] s_DrawLayerOrderEast = {
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants,
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring,
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms,
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck,
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair,
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded };
        static int[] s_DrawLayerOrderDown = {
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Cloak, (int)EquipLayer.Shirt,
            (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso,
            (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF,
            (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso,
            (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist,
            (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded };
        static int[] s_DrawLayerOrderSouth = {
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants,
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring,
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms,
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck,
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair,
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded };
        static int[] s_DrawLayerOrderLeft = {
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants,
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring,
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms,
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck,
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair,
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded };
        static int[] s_DrawLayerOrderWest = {
            (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants,
            (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring,
            (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms,
            (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck,
            (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair,
            (int)EquipLayer.Earrings, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded };
        static int[] s_DrawLayerOrderUp = {
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
            public AAnimationFrame Frame;
            public int BodyID;

            public MobileViewLayer(int bodyID, int hue, AAnimationFrame frame)
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
