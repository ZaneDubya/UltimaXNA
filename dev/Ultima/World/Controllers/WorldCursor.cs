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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.Entities.Items.Containers;
using UltimaXNA.Ultima.Entities.Mobiles;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Gumps;
#endregion

namespace UltimaXNA.Ultima.World.Controllers
{
    /// <summary>
    /// Handles targeting, holding items, and dropping items (both into the UI and into the world).
    /// Draws the mouse cursor and any held item.
    /// </summary>
    class WorldCursor : UltimaCursor
    {
        protected WorldModel World
        {
            get;
            private set;
        }

        private INetworkClient m_Network;
        private UserInterfaceService m_UserInterface;
        private InputManager m_Input;

        public WorldCursor(WorldModel model)
        {
            m_Network = UltimaServices.GetService<INetworkClient>();
            m_UserInterface = UltimaServices.GetService<UserInterfaceService>();
            m_Input = UltimaServices.GetService<InputManager>();

            World = model;
            InternalRegisterInteraction();
        }

        public override void Update()
        {
            if (IsHoldingItem && m_Input.HandleMouseEvent(MouseEvent.Up, Settings.World.Mouse.InteractionButton))
            {
                if (World.Input.IsMouseOverUI)
                {
                    // mouse over ui
                    AControl target = m_UserInterface.MouseOverControl;
                    // attempt to drop the item onto an interface. The only acceptable targets for dropping items are:
                    // 1. ItemGumplings that represent containers (like a bag icon)
                    // 2. Gumps that represent open Containers (GumpPicContainers, e.g. an open GumpPic of a chest)
                    // 3. Paperdolls for my character.
                    // 4. Backpack gumppics (seen in paperdolls).
                    if (target is ItemGumpling && !(target is ItemGumplingPaperdoll))
                    {
                        Item targetItem = ((ItemGumpling)target).Item;
                        if (targetItem.ItemData.IsContainer)
                        {
                            DropHeldItemToContainer((Container)targetItem);
                        }
                        else if (HeldItem.ItemID == targetItem.ItemID && HeldItem.ItemData.IsGeneric)
                        {
                            MergeHeldItem(targetItem);
                        }
                    }
                    else if (target is GumpPicContainer)
                    {
                        int x = (int)m_Input.MousePosition.X - m_HeldItemOffset.X - (target.X + target.Owner.X);
                        int y = (int)m_Input.MousePosition.Y - m_HeldItemOffset.Y - (target.Y + target.Owner.Y);
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
                else if (World.Input.IsMouseOverWorld)
                {
                    // mouse over world
                    AEntity mouseOverEntity = World.Input.MousePick.MouseOverObject;

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
                if (m_Input.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.Escape, false, false, false))
                {
                    SetTargeting(TargetType.Nothing, 0);
                }

                if (m_Input.HandleMouseEvent(MouseEvent.Click, Settings.World.Mouse.InteractionButton))
                {
                    // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
                    switch (m_Targeting)
                    {
                        case TargetType.Object:
                        case TargetType.Position:
                            if (World.Input.IsMouseOverUI)
                            {
                                // get object under mouse cursor. We can only hue items.
                                // ItemGumping is the base class for all items, containers, and paperdoll items.
                                AControl target = m_UserInterface.MouseOverControl;
                                if (target is ItemGumpling)
                                {
                                    mouseTargetingEventObject(((ItemGumpling)target).Item);
                                }
                            }
                            else if (World.Input.IsMouseOverWorld)
                            {
                                // Send Select Object or Select XYZ packet, depending on the entity under the mouse cursor.
                                World.Input.MousePick.PickOnly = PickType.PickStatics | PickType.PickObjects;
                                mouseTargetingEventObject(World.Input.MousePick.MouseOverObject);
                            }
                            break;
                        case TargetType.MultiPlacement:
                            // select X, Y, Z
                            mouseTargetingEventXYZ(World.Input.MousePick.MouseOverObject);
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

        private HuedTexture m_ItemSprite = null;
        private int m_ItemSpriteArtIndex = -1;

        public int ItemSpriteArtIndex
        {
            get { return m_ItemSpriteArtIndex; }
            set
            {
                if (value != m_ItemSpriteArtIndex)
                {
                    m_ItemSpriteArtIndex = value;

                    Texture2D art = IO.ArtData.GetStaticTexture(m_ItemSpriteArtIndex);
                    if (art == null)
                    {
                        // shouldn't we have a debug texture to show that we are missing this cursor art? !!!
                        m_ItemSprite = null;
                    }
                    else
                    {
                        Rectangle sourceRect = new Rectangle(0, 0, art.Width, art.Height);
                        m_ItemSprite = new HuedTexture(art, Point.Zero, sourceRect, 0);
                    }
                }
            }
        }

        protected override void BeforeDraw(SpriteBatchUI spritebatch, Point position)
        {
            // Hue the cursor if not in warmode and in trammel.
            if (EngineVars.InWorld && !EntityManager.GetPlayerObject().Flags.IsWarMode && (World.MapIndex == 1))
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
                int artworkIndex = 8310;

                if (EngineVars.InWorld && EntityManager.GetPlayerObject().Flags.IsWarMode)
                {
                    // Over the interface or not in world. Display a default cursor.
                    artworkIndex -= 23;
                }

                CursorSpriteArtIndex = artworkIndex;
                CursorOffset = new Point(13, 13);
                // sourceRect = new Rectangle(1, 1, 46, 34);
                /*if (m_targetingMulti != -1)
                {
                    // UNIMPLEMENTED !!! Draw a transparent multi
                }*/
            }
            else if ((World.Input.ContinuousMouseMovementCheck || World.Input.IsMouseOverWorld) && !m_UserInterface.IsModalControlOpen)
            {
                Resolution resolution = Settings.World.GumpResolution;
                Direction mouseDirection = Utility.DirectionFromPoints(new Point(resolution.Width / 2, resolution.Height / 2), World.Input.MouseOverWorldPosition);

                int artIndex = 0;

                switch (mouseDirection)
                {
                    case Direction.North:
                        CursorOffset = new Point(29, 1);
                        artIndex = 8299;
                        break;
                    case Direction.Right:
                        CursorOffset = new Point(41, 9);
                        artIndex = 8300;
                        break;
                    case Direction.East:
                        CursorOffset = new Point(36, 24);
                        artIndex = 8301;break;
                    case Direction.Down:
                        CursorOffset = new Point(14, 33);
                        artIndex = 8302;
                        break;
                    case Direction.South:
                        CursorOffset = new Point(4, 28);
                        artIndex = 8303;
                        break;
                    case Direction.Left:
                        CursorOffset = new Point(2, 10);
                        artIndex = 8304;
                        break;
                    case Direction.West:
                        CursorOffset = new Point(1, 1);
                        artIndex = 8305;
                        break;
                    case Direction.Up:
                        CursorOffset = new Point(4, 2);
                        artIndex = 8298;
                        break;
                    default:
                        CursorOffset = new Point(2, 10);
                        artIndex = 8309;
                        break;
                }

                if (EngineVars.InWorld && EntityManager.GetPlayerObject().Flags.IsWarMode)
                {
                    // Over the interface or not in world. Display a default cursor.
                    artIndex -= 23;
                }

                CursorSpriteArtIndex = artIndex;
            }
            else
            {
                // cursor is over UI or there is a modal message box open. Set up to draw standard cursor sprite.
                base.BeforeDraw(spritebatch, position);
            }
        }

        // ======================================================================
        // Targeting enum and routines
        // ======================================================================

        public enum TargetType
        {
            Nothing = -1,
            Object = 0,
            Position = 1,
            MultiPlacement = 2
        }

        private TargetType m_Targeting = TargetType.Nothing;
        private int m_TargetID = int.MinValue;
        public TargetType Targeting
        {
            get { return m_Targeting; }
        }

        public bool IsTargeting
        {
            get { return m_Targeting != TargetType.Nothing; }
        }

        public void ClearTargetingWithoutTargetCancelPacket()
        {
            m_Targeting = TargetType.Nothing;
        }

        public void SetTargeting(TargetType targeting, int cursorID)
        {
            if (m_Targeting != targeting || cursorID != m_TargetID)
            {
                if (targeting == TargetType.Nothing)
                {
                    m_Network.Send(new TargetCancelPacket());
                }
                else
                {
                    // if we start targeting, we cancel movement.
                    World.Input.ContinuousMouseMovementCheck = false;
                }
                m_Targeting = targeting;
                m_TargetID = cursorID;
            }
        }

        int m_MultiModel;
        public void SetTargetingMulti(Serial deedSerial, int model)
        {
            SetTargeting(TargetType.MultiPlacement, (int)deedSerial);
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
            m_Network.Send(new TargetXYZPacket((short)selectedEntity.Position.X, (short)selectedEntity.Position.Y, (short)selectedEntity.Z, (ushort)modelNumber, m_TargetID));
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
                m_Network.Send(new TargetObjectPacket(selectedEntity, m_TargetID));
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
                m_Network.Send(new TargetXYZPacket((short)selectedEntity.Position.X, (short)selectedEntity.Position.Y, (short)selectedEntity.Z, (ushort)modelNumber, m_TargetID));
            }

            // Clear our target cursor.
            ClearTargetingWithoutTargetCancelPacket();
        }

        // ======================================================================
        // Interaction routines
        // ======================================================================

        private void InternalRegisterInteraction()
        {
            World.Interaction.OnPickupItem += PickUpItem;
            World.Interaction.OnClearHolding += ClearHolding;
        }

        private void InternalUnregisterInteraction()
        {
            World.Interaction.OnPickupItem -= PickUpItem;
            World.Interaction.OnClearHolding -= ClearHolding;
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
                    m_UserInterface.RemoveInputBlocker(this);
                }
                else if (value != null && m_HeldItem == null)
                {
                    m_UserInterface.AddInputBlocker(this);
                }
                m_HeldItem = value;
            }
        }

        private Point m_HeldItemOffset = Point.Zero;

        public bool IsHoldingItem
        {
            get { return HeldItem != null; }
        }

        private void PickUpItem(Item item, int x, int y, int? amount = null)
        {
            // if in a bag and is a quantity, then show the 'lift amount' prompt, else just lift it outright.
            if (item.Parent != null && !amount.HasValue && item.Amount > 1)
            {
                m_UserInterface.AddControl(new SplitItemStackGump(item, new Point(x, y)), m_Input.MousePosition.X - 60, m_Input.MousePosition.Y - 30);
            }
            else
            {
                PickupItemWithoutAmountCheck(item, x, y, amount.HasValue ? amount.Value : item.Amount);
            }
        }

        private void PickupItemWithoutAmountCheck(Item item, int x, int y, int amount)
        {
            // make sure we can pick up the item before actually picking it up!
            if (item.TryPickUp())
            {
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

                // let the server know we're picking up the item. If the server says we can't pick it up, it will send us a cancel pick up message.
                // TEST: what if we can pick something up and drop it in our inventory before the server has a chance to respond?
                m_Network.Send(new PickupItemPacket(item.Serial, amount));
            }
        }

        private void MergeHeldItem(Item target)
        {
            m_Network.Send(new DropItemPacket(HeldItem.Serial, 0, 0, 0, 0, target.Serial));
            ClearHolding();
        }

        private void DropHeldItemToWorld(int X, int Y, int Z)
        {
            Serial serial;
            AEntity entity = World.Input.MousePick.MouseOverObject;
            if (entity is Item && ((Item)entity).ItemData.IsContainer)
            {
                serial = entity.Serial;
                X = Y = 0xFFFF;
                Z = 0;
            }
            else
            {
                serial = Serial.World;
            }
            m_Network.Send(new DropItemPacket(HeldItem.Serial, (ushort)X, (ushort)Y, (byte)Z, 0, serial));
            ClearHolding();
        }

        private void DropHeldItemToContainer(Container container)
        {
            // get random coords and drop the item there.
            Rectangle bounds = IO.ContainerData.GetData(container.ItemID).Bounds;
            int x = Utility.RandomValue(bounds.Left, bounds.Right);
            int y = Utility.RandomValue(bounds.Top, bounds.Bottom);
            DropHeldItemToContainer(container, x, y);
        }

        private void DropHeldItemToContainer(Container container, int x, int y)
        {
            Rectangle containerBounds = IO.ContainerData.GetData(container.ItemID).Bounds;
            Texture2D itemTexture = IO.ArtData.GetStaticTexture(HeldItem.DisplayItemID);
            if (x < containerBounds.Left) x = containerBounds.Left;
            if (x > containerBounds.Right - itemTexture.Width) x = containerBounds.Right - itemTexture.Width;
            if (y < containerBounds.Top) y = containerBounds.Top;
            if (y > containerBounds.Bottom - itemTexture.Height) y = containerBounds.Bottom - itemTexture.Height;
            m_Network.Send(new DropItemPacket(HeldItem.Serial, (ushort)x, (ushort)y, 0, 0, container.Serial));
            ClearHolding();
        }

        private void WearHeldItem()
        {
            m_Network.Send(new DropToLayerPacket(HeldItem.Serial, 0x00, EngineVars.PlayerSerial));
            ClearHolding();
        }

        private void ClearHolding()
        {
            HeldItem = null;
        }
    }
}
