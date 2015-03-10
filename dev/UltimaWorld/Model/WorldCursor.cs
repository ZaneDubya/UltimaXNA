﻿using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.Controls;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld.Controller;
using UltimaXNA.UltimaWorld.View;

namespace UltimaXNA.UltimaWorld.Model
{
    class WorldCursor : UltimaCursor
    {
        public override bool BlockingUIMouseEvents
        {
            get
            {
                return IsHoldingItem;
            }
        }

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
                    AMapObject mouseoverObject = IsometricRenderer.MouseOverObject;

                    if (mouseoverObject != null)
                    {
                        int x, y, z;

                        if (mouseoverObject is MapObjectMobile || mouseoverObject is MapObjectCorpse)
                        {
                            // UNIMPLEMENTED: attempt to give this item to the mobile or corpse.
                            return;
                        }
                        else if (mouseoverObject is MapObjectItem || mouseoverObject is MapObjectStatic)
                        {
                            x = (int)mouseoverObject.Position.X;
                            y = (int)mouseoverObject.Position.Y;
                            z = (int)mouseoverObject.Z;
                            if (mouseoverObject is MapObjectStatic)
                            {
                                ItemData itemData = UltimaData.TileData.ItemData[mouseoverObject.ItemID & 0x3FFF];
                                z += itemData.Height;
                            }
                            else if (mouseoverObject is MapObjectItem)
                            {
                                z += UltimaData.TileData.ItemData[mouseoverObject.ItemID].Height;
                            }
                        }
                        else if (mouseoverObject is MapObjectGround)
                        {
                            x = (int)mouseoverObject.Position.X;
                            y = (int)mouseoverObject.Position.Y;
                            z = (int)mouseoverObject.Z;
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

            if (IsTargeting && UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.Escape, false, false, false))
            {
                SetTargeting(TargetTypes.Nothing, 0);
            }

            if (IsTargeting && UltimaEngine.Input.HandleMouseEvent(MouseEvent.Click, UltimaVars.EngineVars.MouseButton_Interact))
            {
                // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
                switch (m_Targeting)
                {
                    case TargetTypes.Object:
                        // Select Object
                        IsometricRenderer.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                        mouseTargetingEventObject(IsometricRenderer.MouseOverObject);
                        break;
                    case TargetTypes.Position:
                        // Select X, Y, Z
                        IsometricRenderer.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                        mouseTargetingEventObject(IsometricRenderer.MouseOverObject); // mouseTargetingEventXYZ(mouseOverObject);
                        break;
                    case TargetTypes.MultiPlacement:
                        // select X, Y, Z
                        mouseTargetingEventXYZ(IsometricRenderer.MouseOverObject);
                        break;
                    default:
                        throw new Exception("Unknown targetting type!");
                }
            }

            base.Update(); // reset image
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
            else if (!UltimaEngine.UserInterface.IsMouseOverUI && !UltimaEngine.UserInterface.IsModalControlOpen)
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
        public TargetTypes Targeting
        {
            get { return m_Targeting; }
        }

        public bool IsTargeting
        {
            get { return m_Targeting != TargetTypes.Nothing; }
        }

        public void SetTargeting(TargetTypes targeting, int cursorID)
        {
            if (m_Targeting != targeting)
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
            }
        }

        void mouseTargetingEventXYZ(AMapObject selectedObject)
        {
            // Send the targetting event back to the server!
            int modelNumber = 0;
            Type type = selectedObject.GetType();
            if (type == typeof(MapObjectStatic))
            {
                modelNumber = selectedObject.ItemID;
            }
            else
            {
                modelNumber = 0;
            }
            // Send the target ...
            m_Model.Client.Send(new TargetXYZPacket((short)selectedObject.Position.X, (short)selectedObject.Position.Y, (short)selectedObject.Z, (ushort)modelNumber));
            // ... and clear our targeting cursor.
            SetTargeting(TargetTypes.Nothing, 0);
        }

        void mouseTargetingEventObject(AMapObject selectedObject)
        {
            // If we are passed a null object, keep targeting.
            if (selectedObject == null)
                return;
            Serial serial = selectedObject.OwnerSerial;
            // Send the targetting event back to the server
            if (serial.IsValid)
            {
                m_Model.Client.Send(new TargetObjectPacket(serial));
            }
            else
            {
                int modelNumber = 0;
                Type type = selectedObject.GetType();
                if (type == typeof(MapObjectStatic))
                {
                    modelNumber = selectedObject.ItemID;
                }
                else
                {
                    modelNumber = 0;
                }
                m_Model.Client.Send(new TargetXYZPacket((short)selectedObject.Position.X, (short)selectedObject.Position.Y, (short)selectedObject.Z, (ushort)modelNumber));
            }

            // Clear our target cursor.
            SetTargeting(TargetTypes.Nothing, 0);
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
        }

        private Point m_HeldItemOffset = Point.Zero;
        internal Point HeldItemOffset
        {
            get { return m_HeldItemOffset; }
        }

        internal bool IsHoldingItem
        {
            get { return m_HeldItem != null; }
        }

        private void PickUpItem(Item item, int x, int y)
        {
            // make sure we can pick up the item before actually picking it up!
            if (item.PickUp())
            {
                // let the server know we're picking up the item. If the server says we can't pick it up, it will send us a cancel pick up message.
                // TEST: what if we can pick something up and drop it in our inventory before the server has a chance to respond?
                m_Model.Client.Send(new PickupItemPacket(item.Serial, item.Amount));

                // if the item is in a container or worn by a mobile, remove it from that container.
                if (item.Parent != null)
                {
                    // set the item's world position to the postion of the container or mobile it is removed from.
                    item.X = item.Parent.WorldPosition.X;
                    item.Y = item.Parent.WorldPosition.Y;
                    item.Z = item.Parent.WorldPosition.Z;
                    if (item.Parent is Mobile)
                    {
                        ((Mobile)item.Parent).RemoveItem(item.Serial);
                    }
                    else if (item.Parent is Container)
                    {
                        ((Container)item.Parent).RemoveItem(item.Serial);
                    }
                    item.Parent = null;
                }

                // set our local holding item variables.
                m_HeldItem = item;
                m_HeldItemOffset = new Point(x, y);
            }
        }

        private void DropHeldItemToWorld(int X, int Y, int Z)
        {
            Serial serial;
            if (IsometricRenderer.MouseOverObject is MapObjectItem && ((Item)IsometricRenderer.MouseOverObject.OwnerEntity).ItemData.IsContainer)
            {
                serial = IsometricRenderer.MouseOverObject.OwnerEntity.Serial;
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
            m_Model.Client.Send(new DropToLayerPacket(HeldItem.Serial, 0x00, EntityManager.MySerial));
            ClearHolding();
        }

        private void ClearHolding()
        {
            m_HeldItem = null;
        }
    }
}
