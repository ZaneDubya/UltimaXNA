/***************************************************************************
 *   WorldCursor.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UltimaXNA.UltimaEntities;
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.Controls;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld.Controller;
using UltimaXNA.UltimaWorld.View;
#endregion

namespace UltimaXNA.UltimaWorld.Model
{
    /// <summary>
    /// Handles targeting, holding items, and dropping items (both into the UI and into the world).
    /// Draws the mouse cursor and any held item.
    /// </summary>
    class WorldCursor : UltimaCursor
    {
        private WorldModel m_Model;

        public WorldCursor(WorldModel model)
            : base()
        {
            m_Model = model;
            InternalRegisterInteraction();
        }

        public override void Update()
        {
            if (IsHoldingItem && UltimaEngine.Input.HandleMouseEvent(MouseEvent.Up, UltimaVars.EngineVars.MouseButton_Interact))
            {
                if (UltimaEngine.UserInterface.IsMouseOverUI)
                {
                    Control target = UltimaEngine.UserInterface.MouseOverControl;
                    // attempt to drop the item onto an interface. The only acceptable targets for dropping items are:
                    // 1. ItemGumplings that represent containers (like a bag icon)
                    // 2. Gumps that represent open Containers (GumpPicContainers, e.g. an open GumpPic of a chest)
                    // 3. Paperdolls for my character.
                    // 4. Backpack gumppics (seen in paperdolls).
                    if (target is ItemGumpling && ((ItemGumpling)target).Item.ItemData.IsContainer)
                    {
                        DropHeldItemToContainer((Container)((ItemGumpling)target).Item);
                    }
                    else if (target is GumpPicContainer)
                    {
                        int x = (int)UltimaEngine.Input.MousePosition.X - HeldItemOffset.X - (target.X + target.Owner.X);
                        int y = (int)UltimaEngine.Input.MousePosition.Y - HeldItemOffset.Y - (target.Y + target.Owner.Y);
                        DropHeldItemToContainer((Container)((GumpPicContainer)target).Item, x, y);
                    }
                    else if (target is ItemGumplingPaperdoll || (target is GumpPic && ((GumpPic)target).IsPaperdoll))
                    {
                        WearHeldItem();
                    }
                    else if (target is GumpPicBackpack)
                    {
                        DropHeldItemToContainer((Container)((GumpPicBackpack)target).BackpackItem);
                    }
                }
                else // cursor is over the world display.
                {
                    AEntity mouseOverEntity = IsometricRenderer.MouseOverObject;

                    if (mouseOverEntity != null)
                    {
                        int x, y, z;

                        if (mouseOverEntity is Mobile || mouseOverEntity is Corpse)
                        {
                            // UNIMPLEMENTED: attempt to give this item to the mobile or corpse.
                            return;
                        }
                        else if (mouseOverEntity is Item || mouseOverEntity is StaticItem)
                        {
                            x = (int)mouseOverEntity.Position.X;
                            y = (int)mouseOverEntity.Position.Y;
                            z = (int)mouseOverEntity.Z;
                            if (mouseOverEntity is StaticItem)
                            {
                                z += ((StaticItem)mouseOverEntity).ItemData.Height;
                            }
                            else if (mouseOverEntity is Item)
                            {
                                z += ((Item)mouseOverEntity).ItemData.Height;
                            }
                        }
                        else if (mouseOverEntity is Ground)
                        {
                            x = (int)mouseOverEntity.Position.X;
                            y = (int)mouseOverEntity.Position.Y;
                            z = (int)mouseOverEntity.Z;
                        }
                        else
                        {
                            // over text?
                            return;
                        }
                        DropHeldItemToWorld(x, y, z);
                    }
                }
            }

            if (IsTargeting)
            {
                if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.Escape, false, false, false))
                {
                    SetTargeting(TargetTypes.Nothing, 0);
                }

                if (UltimaEngine.Input.HandleMouseEvent(MouseEvent.Click, UltimaVars.EngineVars.MouseButton_Interact))
                {
                    // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
                    switch (m_Targeting)
                    {
                        case TargetTypes.Object:
                        case TargetTypes.Position:
                            if (UltimaEngine.UserInterface.IsMouseOverUI)
                            {
                                // get object under mouse cursor. We can only hue items.
                                // ItemGumping is the base class for all items, containers, and paperdoll items.
                                Control target = UltimaEngine.UserInterface.MouseOverControl;
                                if (target is ItemGumpling)
                                {
                                    mouseTargetingEventObject(((ItemGumpling)target).Item);
                                }
                            }
                            else
                            {
                                // Send Select Object or Select XYZ packet, depending on the entity under the mouse cursor.
                                IsometricRenderer.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                                mouseTargetingEventObject(IsometricRenderer.MouseOverObject);
                            }
                            break;
                        case TargetTypes.MultiPlacement:
                            // select X, Y, Z
                            mouseTargetingEventXYZ(IsometricRenderer.MouseOverObject);
                            break;
                        default:
                            throw new Exception("Unknown targetting type!");
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        // ======================================================================
        // Drawing routines
        // ======================================================================

        private Sprite m_ItemSprite = null;
        private int m_ItemSpriteArtIndex = -1;

        public int ItemSpriteArtIndex
        {
            get { return m_ItemSpriteArtIndex; }
            set
            {
                if (value != m_ItemSpriteArtIndex)
                {
                    m_ItemSpriteArtIndex = value;

                    Texture2D art = UltimaData.ArtData.GetStaticTexture(m_ItemSpriteArtIndex);
                    if (art == null)
                    {
                        // shouldn't we have a debug texture to show that we are missing this cursor art? !!!
                        m_ItemSprite = null;
                    }
                    else
                    {
                        Rectangle sourceRect = new Rectangle(0, 0, art.Width, art.Height);
                        m_ItemSprite = new Sprite(art, Point.Zero, sourceRect, 0);
                    }
                }
            }
        }

        protected override void BeforeDraw(SpriteBatchUI spritebatch, Point position)
        {
            // Hue the cursor if not in warmode and in trammel.
            if (!UltimaVars.EngineVars.WarMode && (m_Model.MapIndex == 1))
                CursorHue = 2414;
            else
                CursorHue = 0;

            if (IsHoldingItem)
            {
                ItemSpriteArtIndex = HeldItem.DisplayItemID;

                if (m_ItemSprite != null)
                {
                    m_ItemSprite.Hue = HeldItem.Hue;
                    m_ItemSprite.Offset = m_HeldItemOffset;
                    m_ItemSprite.Draw(spritebatch, position);
                }

                // set up to draw standard cursor sprite above item art.
                base.BeforeDraw(spritebatch, position);
            }
            else if (IsTargeting)
            {
                CursorSpriteArtIndex = 8310 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                CursorOffset = new Point(13, 13);
                // sourceRect = new Rectangle(1, 1, 46, 34);
                /*if (m_targetingMulti != -1)
                {
                    // UNIMPLEMENTED !!! Draw a transparent multi
                }*/
            }
            else if ((m_Model.Input.ContinuousMouseMovementCheck || !UltimaEngine.UserInterface.IsMouseOverUI) && 
                !UltimaEngine.UserInterface.IsModalControlOpen)
            {
                switch (UltimaVars.EngineVars.CursorDirection)
                {
                    case Direction.North:
                        CursorOffset = new Point(29, 1);
                        CursorSpriteArtIndex = 8299 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                        break;
                    case Direction.Right:
                        CursorOffset = new Point(41, 9);
                        CursorSpriteArtIndex = 8300 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                        break;
                    case Direction.East:
                        CursorOffset = new Point(36, 24);
                        CursorSpriteArtIndex = 8301 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                        break;
                    case Direction.Down:
                        CursorOffset = new Point(14, 33);
                        CursorSpriteArtIndex = 8302 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                        break;
                    case Direction.South:
                        CursorOffset = new Point(4, 28);
                        CursorSpriteArtIndex = 8303 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                        break;
                    case Direction.Left:
                        CursorOffset = new Point(2, 10);
                        CursorSpriteArtIndex = 8304 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                        break;
                    case Direction.West:
                        CursorOffset = new Point(1, 1);
                        CursorSpriteArtIndex = 8305 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                        break;
                    case Direction.Up:
                        CursorOffset = new Point(4, 2);
                        CursorSpriteArtIndex = 8298 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                        break;
                    default:
                        CursorOffset = new Point(2, 10);
                        CursorSpriteArtIndex = 8309 - ((UltimaVars.EngineVars.WarMode) ? 23 : 0);
                        break;
                }
            }
            else
            {
                // cursor is over UI or there is a modal message box open. Set up to draw standard cursor sprite.
                base.BeforeDraw(spritebatch, position);
            }
        }

        // ======================================================================
        // Targeting routines
        // ======================================================================

        private TargetTypes m_Targeting = TargetTypes.Nothing;
        private int m_TargetID = int.MinValue;
        public TargetTypes Targeting
        {
            get { return m_Targeting; }
        }

        public bool IsTargeting
        {
            get { return m_Targeting != TargetTypes.Nothing; }
        }

        public void ClearTargetingWithoutTargetCancelPacket()
        {
            m_Targeting = TargetTypes.Nothing;
        }

        public void SetTargeting(TargetTypes targeting, int cursorID)
        {
            if (m_Targeting != targeting || cursorID != m_TargetID)
            {
                if (targeting == TargetTypes.Nothing)
                {
                    m_Model.Client.Send(new TargetCancelPacket());
                }
                else
                {
                    // if we start targeting, we cancel movement.
                    m_Model.Input.ContinuousMouseMovementCheck = false;
                }
                m_Targeting = targeting;
                m_TargetID = cursorID;
            }
        }

        int m_MultiModel;
        public void SetTargetingMulti(Serial deedSerial, int model)
        {
            SetTargeting(TargetTypes.MultiPlacement, (int)deedSerial);
            m_MultiModel = model;
        }

        void mouseTargetingEventXYZ(AEntity selectedEntity)
        {
            // Send the targetting event back to the server!
            int modelNumber = 0;
            if (selectedEntity is StaticItem)
            {
                modelNumber = ((StaticItem)selectedEntity).ItemID;
            }
            else
            {
                modelNumber = 0;
            }
            // Send the target ...
            m_Model.Client.Send(new TargetXYZPacket((short)selectedEntity.Position.X, (short)selectedEntity.Position.Y, (short)selectedEntity.Z, (ushort)modelNumber, m_TargetID));
            // ... and clear our targeting cursor.
            ClearTargetingWithoutTargetCancelPacket();
        }

        void mouseTargetingEventObject(AEntity selectedEntity)
        {
            // If we are passed a null object, keep targeting.
            if (selectedEntity == null)
                return;
            Serial serial = selectedEntity.Serial;
            // Send the targetting event back to the server
            if (serial.IsValid)
            {
                m_Model.Client.Send(new TargetObjectPacket(serial));
            }
            else
            {
                int modelNumber = 0;
                if (selectedEntity is StaticItem)
                {
                    modelNumber = ((StaticItem)selectedEntity).ItemID;
                }
                else
                {
                    modelNumber = 0;
                }
                m_Model.Client.Send(new TargetXYZPacket((short)selectedEntity.Position.X, (short)selectedEntity.Position.Y, (short)selectedEntity.Z, (ushort)modelNumber, m_TargetID));
            }

            // Clear our target cursor.
            ClearTargetingWithoutTargetCancelPacket();
        }

        // ======================================================================
        // Interaction routines
        // ======================================================================

        private void InternalRegisterInteraction()
        {
            UltimaInteraction.OnPickupItem += PickUpItem;
            UltimaInteraction.OnClearHolding += ClearHolding;
        }

        private void InternalUnregisterInteraction()
        {
            UltimaInteraction.OnPickupItem -= PickUpItem;
            UltimaInteraction.OnClearHolding -= ClearHolding;
        }

        // ======================================================================
        // Pickup/Drop/Hold item routines
        // ======================================================================

        private Item m_HeldItem = null;
        internal Item HeldItem
        {
            get { return m_HeldItem; }
            set
            {
                if (value == null && m_HeldItem != null)
                {
                    UltimaEngine.UserInterface.RemoveInputBlocker(this);
                }
                else if (value != null && m_HeldItem == null)
                {
                    UltimaEngine.UserInterface.AddInputBlocker(this);
                }
                m_HeldItem = value;
            }
        }

        private Point m_HeldItemOffset = Point.Zero;
        internal Point HeldItemOffset
        {
            get { return m_HeldItemOffset; }
        }

        internal bool IsHoldingItem
        {
            get { return HeldItem != null; }
        }

        private void PickUpItem(Item item, int x, int y)
        {
            // make sure we can pick up the item before actually picking it up!
            if (item.TryPickUp())
            {
                // let the server know we're picking up the item. If the server says we can't pick it up, it will send us a cancel pick up message.
                // TEST: what if we can pick something up and drop it in our inventory before the server has a chance to respond?
                m_Model.Client.Send(new PickupItemPacket(item.Serial, item.Amount));

                // if the item is within a container or worn by a mobile, remove it from that containing entity.
                if (item.Parent != null)
                {
                    // Because we are moving the item from a parent entity into the world, the client will now be checking to see if it is out of range.
                    // To make sure it is 'in range', we set the item's world position to the world postion of the entity it was removed from.
                    item.Position.Set(item.Parent.Position.X, item.Parent.Position.Y, item.Parent.Position.Z);

                    // remove the item from the containing entity.
                    if (item.Parent is Mobile)
                        ((Mobile)item.Parent).RemoveItem(item.Serial);
                    else if (item.Parent is Container)
                        ((Container)item.Parent).RemoveItem(item.Serial);
                    item.Parent = null;
                }

                // set our local holding item variables.
                HeldItem = item;
                m_HeldItemOffset = new Point(x, y);
            }
        }

        private void DropHeldItemToWorld(int X, int Y, int Z)
        {
            Serial serial;
            if (IsometricRenderer.MouseOverObject is Item && ((Item)IsometricRenderer.MouseOverObject).ItemData.IsContainer)
            {
                serial = IsometricRenderer.MouseOverObject.Serial;
                X = Y = 0xFFFF;
                Z = 0;
            }
            else
            {
                serial = Serial.World;
            }
            m_Model.Client.Send(new DropItemPacket(HeldItem.Serial, (ushort)X, (ushort)Y, (byte)Z, 0, serial));
            ClearHolding();
        }

        private void DropHeldItemToContainer(Container container)
        {
            // get random coords and drop the item there.
            Rectangle bounds = UltimaData.ContainerData.GetData(container.ItemID).Bounds;
            int x = Utility.RandomValue(bounds.Left, bounds.Right);
            int y = Utility.RandomValue(bounds.Top, bounds.Bottom);
            DropHeldItemToContainer(container, x, y);
        }

        private void DropHeldItemToContainer(Container container, int x, int y)
        {
            Rectangle containerBounds = UltimaData.ContainerData.GetData(container.ItemID).Bounds;
            Texture2D itemTexture = UltimaData.ArtData.GetStaticTexture(HeldItem.DisplayItemID);
            if (x < containerBounds.Left) x = containerBounds.Left;
            if (x > containerBounds.Right - itemTexture.Width) x = containerBounds.Right - itemTexture.Width;
            if (y < containerBounds.Top) y = containerBounds.Top;
            if (y > containerBounds.Bottom - itemTexture.Height) y = containerBounds.Bottom - itemTexture.Height;
            m_Model.Client.Send(new DropItemPacket(HeldItem.Serial, (ushort)x, (ushort)y, 0, 0, container.Serial));
            ClearHolding();
        }

        private void WearHeldItem()
        {
            m_Model.Client.Send(new DropToLayerPacket(HeldItem.Serial, 0x00, UltimaVars.EngineVars.PlayerSerial));
            ClearHolding();
        }

        private void ClearHolding()
        {
            HeldItem = null;
        }
    }
}
