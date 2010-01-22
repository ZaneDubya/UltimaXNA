/***************************************************************************
 *   GameState.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Data;
using UltimaXNA.Client;
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.UI;
using UltimaXNA.Input;
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.TileEngine;
using UltimaXNA.UILegacy;
#endregion

namespace UltimaXNA
{
    public delegate void MethodHook();

    public static class GameState
    {
        static IInputService _input;
        static IWorld _worldService;
        static IUIManager _legacyUI;

        public static MethodHook OnLeftClick, OnRightClick, OnUpdate;

        static bool _ContinuousMoveCheck = false;
        static float _TimeToShowTip = float.MaxValue;
        const float _TimeHoveringBeforeTip = 1.0f;

        // Are we asking for a target?
        static int _TargettingType = -1;
        static bool isTargeting
        {
            get
            {
                if (_TargettingType == -1)
                    return false;
                else
                    return true;
            }
        }

        

        public static void Initialize(Game game)
        {
            _input = game.Services.GetService<IInputService>();
            _worldService = game.Services.GetService<IWorld>();
            _legacyUI = game.Services.GetService<IUIManager>();
        }

        public static void Update(GameTime gameTime)
        {
            doUpdate();
            if (OnUpdate != null)
                OnUpdate();
        }

        static void doUpdate()
        {
            if (ClientVars.InWorld)
            {
                // Set the target frame stuff.
                //((UI.Window_StatusFrame)UserInterface.Window("StatusFrame")).MyEntity = (Mobile)EntitiesCollection.GetPlayerObject();
                //if (LastTarget.IsValid)
                //    ((UI.Window_StatusFrame)UserInterface.Window("StatusFrame")).TargetEntity = EntitiesCollection.GetObject<Mobile>(LastTarget, false);

                // Parse keyboard input.
                parseKeyboard();

                // PickType is the kind of objects that will show up as the 'MouseOverObject'
                if (_legacyUI.IsMouseOverUI)
                    _worldService.PickType = PickTypes.PickNothing;
                else
                    _worldService.PickType = PickTypes.PickEverything;

                // Set the cursor direction
                ClientVars.CursorDirection = Utility.DirectionFromVectors(new Vector2(400, 300), _input.CurrentMousePosition);

                // Show a popup tip if we have hovered over this item for X seconds.
                if (_input.IsCursorMovedSinceLastUpdate())
                    _TimeToShowTip = ClientVars.TheTime + _TimeHoveringBeforeTip;
                else
                    if (_TimeToShowTip >= ClientVars.TheTime && _worldService.MouseOverObject != null)
                        createHoverLabel(_worldService.MouseOverObject);

                // check for mouse input which relates to the world.
                if (!_legacyUI.IsMouseOverUI)
                {
                    // interaction button.
                    if (_input.IsMouseButtonPress(ClientVars.MouseButton_Interact))
                    {
                        if (isTargeting)
                            onTargetingButtonDown();
                        else
                            onInteractButtonDown();
                    }
                    if (_input.IsMouseButtonRelease(ClientVars.MouseButton_Interact))
                        onInteractButtonUp();

                    // movement button.
                    if (_input.IsMouseButtonPress(ClientVars.MouseButton_Move))
                        onMoveButtonDown();
                    if (_input.IsMouseButtonRelease(ClientVars.MouseButton_Move))
                        onMoveButtonUp();

                    // hooks for hidden fun stuff.
                    if (_input.IsMouseButtonPress(MouseButtons.LeftButton))
                        if (OnLeftClick != null)
                            OnLeftClick();
                    if (_input.IsMouseButtonPress(MouseButtons.RightButton))
                        if (OnRightClick != null)
                            OnRightClick();
                }

                // Changed to leverage movementFollowsMouse interface option -BERT
                if (_ContinuousMoveCheck)
                    doMovement();

                // Show our target's name
                createHoverLabel(ClientVars.LastTarget);
            }
        }

        static void doMovement()
        {
            // Get the move direction and add the Running offset if the Cursor is far enough away.
            Direction moveDirection = ClientVars.CursorDirection;
            float distanceFromCenterOfScreen = Vector2.Distance(_input.CurrentMousePosition, new Vector2(400, 300));
            if (distanceFromCenterOfScreen >= 150.0f)
                moveDirection |= Direction.Running;

            // Tell the player to Move. If the player has a new move event, send a MoveRequestPacket to the server.
            int direction = 0, sequence = 0, key = 0;
            Mobile m = (Mobile)EntitiesCollection.GetPlayerObject();
            m.Move(moveDirection);
            if (EntitiesCollection.GetPlayerObject().GetMoveEvent(ref direction, ref sequence, ref key))
                UltimaClient.Send(new MoveRequestPacket((byte)direction, (byte)sequence, key));
        }

        static void onMoveButtonDown()
        {
            // keep moving as long as the move button is down.
            _ContinuousMoveCheck = true;
        }

        static void onMoveButtonUp()
        {
            // If the movement mouse button has been released, stop moving.
            _ContinuousMoveCheck = false;
        }

        static void onTargetingButtonDown()
        {
            MapObject mouseOverObject = _worldService.MouseOverObject;
            if (mouseOverObject == null)
                return;

            // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
            // If not, then we are just clicking the mouse and we need to find out if something is under the mouse cursor.
            switch (_TargettingType)
            {
                case 0:
                    // Select Object
                    _worldService.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                    mouseTargetingEventObject(mouseOverObject);
                    break;
                case 1:
                    // Select X, Y, Z
                    mouseTargetingEventXYZ(mouseOverObject);
                    break;
                default:
                    throw new Exception("Unknown targetting type!");
            }
        }

        static void onInteractButtonDown()
        {
            // get the object under our mouse cursor. If it is null, exit.
            MapObject mouseOverObject = _worldService.MouseOverObject;
            if (mouseOverObject == null)
                return;

            if (mouseOverObject is MapObjectGround)
            {
                // we can't interact ground tiles.
            }
            else if (mouseOverObject is MapObjectStatic)
            {
                // clicking a static should pop up the name of the static.
            }
            else if (mouseOverObject is MapObjectItem)
            {
                Item entity = (Item)mouseOverObject.OwnerEntity;
                // click and drag = pick up
                if (entity.ItemData.Weight != 255)
                    UI.UIHelper.PickUpItem(entity);
                // single click = tool tip
                // double click = use / open
                // UltimaClient.Send(new DoubleClickPacket(entity.Serial));
            }
            else if (mouseOverObject is MapObjectMobile)
            {
                // click and drag = pull off status bar
                // single click = tool tip
                // single click npc = request context sensitive menu
                // double click = set last target, open paper doll.
                // double click self = open paper doll
                
                // double click ridable = ride.
                // single click self while mounted = dismount.
                Mobile entity = (Mobile)mouseOverObject.OwnerEntity;
                // ClientVars.LastTarget = entity.Serial;
                if (entity.Serial == EntitiesCollection.MySerial)
                {
                    // this is my player.
                    // if mounted, dismount.
                    if (entity.IsMounted)
                        UltimaClient.Send(new DoubleClickPacket(entity.Serial));
                }
                else
                {
                    // UltimaClient.Send(new SingleClickPacket(ClientVars.LastTarget));
                    if (ClientVars.WarMode)
                        UltimaClient.Send(new AttackRequestPacket(entity.Serial));
                    else
                        UltimaClient.Send(new RequestContextMenuPacket(entity.Serial));
                }

            }
            else if (mouseOverObject is MapObjectCorpse)
            {
                // click and drag = nothing
                // single click = tool tip
                // double click = open loot window.
                Corpse entity = (Corpse)mouseOverObject.OwnerEntity;
            }
            else if (mouseOverObject is MapObjectText)
            {
                // clicking on text should somehow indicate the person speaking.
            }
            else
            {
                throw new Exception("Unknown object type in onInteractButtonDown()");
            }
        }

        static void onInteractButtonUp()
        {
            // If the interaction button has been released, and we are holding anything, we just dropped it.
            if (UI.UIHelper.MouseHoldingItem != null)
                checkDropItem();
        }

        static void checkDropItem()
        {
            // We do not handle dropping the item into the UI in this routine.
            // But we probably should, to consolidate game logic.
            if (!_legacyUI.IsMouseOverUI)
            {
                int x, y, z;
                MapObject groundObject = _worldService.MouseOverGroundTile;
                MapObject mouseoverObject = _worldService.MouseOverObject;
                if (mouseoverObject != null)
                {
                    if (mouseoverObject is MapObjectMobile)
                    {
                        // special case, attempt to give this item.
                        return;
                    }
                    else
                    {
                        x = (int)mouseoverObject.Position.X;
                        y = (int)mouseoverObject.Position.Y;
                        z = mouseoverObject.Z;
                        if (mouseoverObject is MapObjectStatic)
                        {
                            ItemData data = Data.TileData.ItemData[mouseoverObject.ItemID - 0x4000];
                            z += data.Height;
                        }
                        else if (mouseoverObject is MapObjectItem)
                        {
                            z += Data.TileData.ItemData[mouseoverObject.ItemID].Height;
                        }
                    }

                }
                else if (groundObject != null)
                {
                    x = (int)groundObject.Position.X;
                    y = (int)groundObject.Position.Y;
                    z = groundObject.Z;
                }
                else
                {
                    UI.UIHelper.MouseHoldingItem = null;
                    return;
                }
                // We dropped the icon in the world. This means we are trying to drop the item
                // into the world. Let's do it!
                /*
                if ((UI.UIHelper.MouseHoldingItem).Item_ContainedWithinSerial.IsValid)
                {
                    // We must manually remove the item from the container, as RunUO does not do this for us.
                    Container iContainer = EntitiesCollection.GetObject<Container>(
                        (UI.UIHelper.MouseHoldingItem).Item_ContainedWithinSerial, false);
                    iContainer.RemoveItem(UI.UIHelper.MouseHoldingItem);
                }*/
                UI.UIHelper.DropItemOntoGround(x, y, z);
            }
        }

        static void createHoverLabel(MapObject mapObject)
        {
            if (mapObject.OwnerSerial.IsValid)
            {
                // this object is an entity of some kind.
                createHoverLabel(mapObject.OwnerSerial);
            }
            else if (mapObject is MapObjectStatic)
            {
                // since statics have no entity object, we can't easily create a label for them at the moment.
                // surely this will be fixed.
            }
        }

        static void createHoverLabel(Serial serial)
        {
            if (!serial.IsValid)
                return;

            Entity e = EntitiesCollection.GetObject<Entity>(serial, false);

            if (e is Mobile)
            {
                Mobile m = (Mobile)e;
                m.AddOverhead(MessageType.Label, m.Name, 3, m.NotorietyHue);
            }
            else if (e is Corpse)
            {
                // Currently item entities do not Update() so they will not show their label.
            }
            else if (e is Item)
            {
                // Currently item entities do not Update() so they will not show their label.
            }
        }

        static void parseKeyboard()
        {
            // If we are targeting, cancel the target cursor if we hit escape.
            if (isTargeting)
                if (_input.IsKeyPress(Keys.Escape))
                    mouseTargetingCancel();

            float _LightRadians = _worldService.LightDirection;
            if (_input.IsKeyDown(Keys.I))
                 _LightRadians += .001f;
            _worldService.LightDirection = _LightRadians;

            // Toggle for backpack container window.
            if (_input.IsKeyPress(Keys.B) && (_input.IsKeyPress(Keys.LeftControl)))
            {
                Serial backpackSerial = ((PlayerMobile)EntitiesCollection.GetPlayerObject())
                    .equipment[(int)EquipLayer.Backpack].Serial;
                if (UserInterface.Window("Container:" + backpackSerial) == null)
                    UltimaClient.Send(new DoubleClickPacket(backpackSerial));
                else
                    UserInterface.CloseWindow("Container:" + backpackSerial);
            }

            // Toggle for paperdoll window.
            if (_input.IsKeyPress(Keys.C) && (_input.IsKeyDown(Keys.LeftControl)))
            {
                Serial serial = ((PlayerMobile)EntitiesCollection.GetPlayerObject())
                    .Serial;
                if (UserInterface.Window("PaperDoll:" + serial) == null)
                    UserInterface.PaperDoll_Open(EntitiesCollection.GetPlayerObject());
                else
                    UserInterface.CloseWindow("PaperDoll:" + serial);
            }

            // toggle for warmode
            if (_input.IsKeyPress(Keys.Tab))
            {
                if (ClientVars.WarMode)
                    UltimaClient.Send(new RequestWarModePacket(false));
                else
                    UltimaClient.Send(new RequestWarModePacket(true));
            }

            // toggle for all names
            if (_input.IsKeyDown(Keys.LeftShift) && _input.IsKeyDown(Keys.LeftControl))
            {
                List<Mobile> mobiles = EntitiesCollection.GetObjectsByType<Mobile>();
                foreach (Mobile m in mobiles)
                {
                    if (m.Name != string.Empty)
                    {
                        m.AddOverhead(MessageType.Label, m.Name, 3, m.NotorietyHue);
                    }
                }
            }
        }

        public static void MouseTargeting(int nCursorID, int nTargetingType)
        {
            setTargeting(nTargetingType);
        }

        static void setTargeting(int targetingType)
        {
            _TargettingType = targetingType;
            // Set the UI's cursor to a targetting cursor.
            _legacyUI.Cursor.IsTargeting = true;
            // Stop continuous movement.
            _ContinuousMoveCheck = false;
        }

        static void clearTargeting()
        {
            // Clear our target cursor.
            _TargettingType = -1;
            _legacyUI.Cursor.IsTargeting = false;
        }

        static void mouseTargetingCancel()
        {
            // Send the cancel target message back to the server.
            UltimaClient.Send(new TargetCancelPacket());
            clearTargeting();
        }

        static void mouseTargetingEventXYZ(MapObject selectedObject)
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
            UltimaClient.Send(new TargetXYZPacket((short)selectedObject.Position.X, (short)selectedObject.Position.Y, (short)selectedObject.Z, (ushort)modelNumber));
            // ... and clear our targeting cursor.
            clearTargeting();
        }

        static void mouseTargetingEventObject(MapObject selectedObject)
        {
            // If we are passed a null object, keep targeting.
            if (selectedObject == null)
                return;
            Serial serial = selectedObject.OwnerSerial;
            // Send the targetting event back to the server
            if (serial.IsValid)
            {
                UltimaClient.Send(new TargetObjectPacket(serial));
            }
            else
            {
                UltimaClient.Send(new TargetXYZPacket((short)selectedObject.Position.X, (short)selectedObject.Position.Y, (short)selectedObject.Z, (ushort)selectedObject.ItemID));
            }
            
            // Clear our target cursor.
            clearTargeting();
        }

        static void loadDebugAssembly(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                System.Reflection.Assembly debugAssembly = System.Reflection.Assembly.LoadFrom(filename);
                Object o = debugAssembly.CreateInstance("UltimaXNA.UXNADebug", true);
            }
        }
    }
}
