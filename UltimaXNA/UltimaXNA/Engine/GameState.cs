/***************************************************************************
 *   GameState.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
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
using UltimaXNA.Client;
using UltimaXNA.Client.Packets.Client;
using UltimaXNA.Data;
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
using UltimaXNA.TileEngine;
using UltimaXNA.UILegacy;
#endregion

namespace UltimaXNA
{
    public delegate void MethodHook();

    public enum TargetTypes
    {
        Nothing = -1,
        Object = 0,
        Position = 1,
        MultiPlacement = 2
    }

    static class GameState
    {
        // required services
        static IInputState _inputNew;
        static IIsometricRenderer _world;
        static IUIManager _ui;

        // mouse input variables
        static bool _ContinuousMoveCheck = false;
        const int _TimeHoveringBeforeTipMS = 1000;

        // make sure we drag the correct object variables
        static Point _dragOffset;
        static MapObject _dragObject;

        // Are we asking for a target?
        static TargetTypes _TargettingType = TargetTypes.Nothing;
        static bool isTargeting
        {
            get
            {
                if (_TargettingType == TargetTypes.Nothing)
                    return false;
                else
                    return true;
            }
        }

        public static void Initialize(Game game)
        {
            _inputNew = game.Services.GetService<IInputState>();
            _world = game.Services.GetService<IIsometricRenderer>();
            _ui = game.Services.GetService<IUIManager>();
        }

        public static void Update(GameTime gameTime)
        {
            doUpdate();
        }

        static void doUpdate()
        {
            if (ClientVars.InWorld && !_ui.IsModalMsgBoxOpen && UltimaClient.IsConnected)
            {
                parseKeyboard();
                parseMouse();

                // PickType is the kind of objects that will show up as the 'MouseOverObject'
                if (_ui.IsMouseOverUI)
                    _world.PickType = PickTypes.PickNothing;
                else
                    _world.PickType = PickTypes.PickEverything;

                // Set the cursor direction
                ClientVars.CursorDirection = Utility.DirectionFromVectors(new Vector2(400, 300), _inputNew.MousePosition.ToVector2());

                // Show a popup tip if we have hovered over this item for X seconds.
                if (_inputNew.MouseStationaryMS >= _TimeHoveringBeforeTipMS)
                    if (_world.MouseOverObject != null)
                        createHoverLabel(_world.MouseOverObject);

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
            float distanceFromCenterOfScreen = Vector2.Distance(_inputNew.MousePosition.ToVector2(), new Vector2(400, 300));
            if (distanceFromCenterOfScreen >= 150.0f)
                moveDirection |= Direction.Running;

            // Tell the player to Move. If the player has a new move event, send a MoveRequestPacket to the server.
            int direction = 0, sequence = 0, key = 0;
            Mobile m = (Mobile)EntitiesCollection.GetPlayerObject();
            m.PlayerMobile_Move(moveDirection);
            if (m.PlayerMobile_GetMoveEvent(ref direction, ref sequence, ref key))
            {
                UltimaClient.Send(new MoveRequestPacket((byte)direction, (byte)sequence, key));
            }
        }

        static void onMoveButton(InputEventM e)
        {
            if (e.EventType == MouseEvent.Down)
            {
                // keep moving as long as the move button is down.
                _ContinuousMoveCheck = true;
            }
            else if (e.EventType == MouseEvent.Up)
            {
                // If the movement mouse button has been released, stop moving.
                _ContinuousMoveCheck = false;
            }

            e.Handled = true;
        }

        static void onTargetingButton(MapObject worldObject)
        {
            if (worldObject == null)
                return;

            // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
            // If not, then we are just clicking the mouse and we need to find out if something is under the mouse cursor.
            switch (_TargettingType)
            {
                case TargetTypes.Object:
                    // Select Object
                    _world.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                    mouseTargetingEventObject(worldObject);
                    break;
                case TargetTypes.Position:
                    // Select X, Y, Z
                    _world.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                    mouseTargetingEventObject(worldObject); // mouseTargetingEventXYZ(mouseOverObject);
                    break;
                case TargetTypes.MultiPlacement:
                    // select X, Y, Z
                    mouseTargetingEventXYZ(worldObject);
                    break;
                default:
                    throw new Exception("Unknown targetting type!");
            }
        }

        static void onInteractButton(InputEventM e)
        {
            MapObject overObject = (e.EventType == MouseEvent.DragBegin) ? _dragObject : _world.MouseOverObject;
            Point overObjectOffset = (e.EventType == MouseEvent.DragBegin) ? _dragOffset : _world.MouseOverObjectPoint;

            if (e.EventType == MouseEvent.Down)
            {
                _dragObject = overObject;
                _dragOffset = overObjectOffset;
            }

            if (overObject == null)
                return;

            if (isTargeting && e.EventType == MouseEvent.Click)
            {
                // Special case: targeting
                onTargetingButton(overObject);
            }
            else if (_ui.Cursor.IsHolding && e.EventType == MouseEvent.Up)
            {
                // Special case: if we're holding anything, drop it.
                checkDropItem();
            }
            else
            {
                // standard interaction actions ...
                if (overObject is MapObjectGround)
                {
                    // we can't interact with ground tiles.
                }
                else if (overObject is MapObjectStatic)
                {
                    // clicking a static should pop up the name of the static.
                    if (e.EventType == MouseEvent.Click)
                    {

                    }
                }
                else if (overObject is MapObjectItem)
                {
                    Item entity = (Item)overObject.OwnerEntity;
                    // single click = tool tip
                    // double click = use / open
                    // click and drag = pick up
                    switch (e.EventType)
                    {
                        case MouseEvent.Click:
                            // tool tip
                            Interaction.SingleClick(entity);
                            break;
                        case MouseEvent.DoubleClick:
                            Interaction.DoubleClick(entity);
                            break;
                        case MouseEvent.DragBegin:
                            Interaction.PickUpItem(entity, overObjectOffset.X, overObjectOffset.Y);
                            break;
                    }
                }
                else if (overObject is MapObjectMobile)
                {
                    Mobile entity = (Mobile)overObject.OwnerEntity;
                    // single click = tool tip; if npc = request context sensitive menu
                    // double click = set last target; if is human open paper doll; if ridable ride; if self and riding, dismount;
                    // click and drag = pull off status bar
                    switch (e.EventType)
                    {
                        case MouseEvent.Click:
                            // tool tip
                            Interaction.SingleClick(entity);
                            if (ClientVars.WarMode)
                                UltimaClient.Send(new AttackRequestPacket(entity.Serial));
                            else
                                UltimaClient.Send(new RequestContextMenuPacket(entity.Serial));
                            break;
                        case MouseEvent.DoubleClick:
                            Interaction.DoubleClick(entity);
                            ClientVars.LastTarget = entity.Serial;
                            break;
                        case MouseEvent.DragBegin:
                            // pull off status bar
                            break;
                    }
                }
                else if (overObject is MapObjectCorpse)
                {
                    Corpse entity = (Corpse)overObject.OwnerEntity;
                    // click and drag = nothing
                    // single click = tool tip
                    // double click = open loot window.
                }
                else if (overObject is MapObjectText)
                {
                    // clicking on text should somehow indicate the person speaking.
                }
                else
                {
                    throw new Exception("Unknown object type in onInteractButtonDown()");
                }
            }

            e.Handled = true;
        }

        static void checkDropItem()
        {
            MapObject mouseoverObject = _world.MouseOverObject;

            if (mouseoverObject != null)
            {
                int x, y, z;

                if (mouseoverObject is MapObjectMobile || mouseoverObject is MapObjectCorpse)
                {
                    // special case, attempt to give this item.
                    return;
                }
                else if (mouseoverObject is MapObjectItem || mouseoverObject is MapObjectStatic)
                {
                    x = (int)mouseoverObject.Position.X;
                    y = (int)mouseoverObject.Position.Y;
                    z = mouseoverObject.Z;
                    if (mouseoverObject is MapObjectStatic)
                    {
                        ItemData data = Data.TileData.ItemData[mouseoverObject.ItemID & 0x3FFF];
                        z += data.Height;
                    }
                    else if (mouseoverObject is MapObjectItem)
                    {
                        z += Data.TileData.ItemData[mouseoverObject.ItemID].Height;
                    }
                }
                else if (mouseoverObject is MapObjectGround)
                {
                    x = (int)mouseoverObject.Position.X;
                    y = (int)mouseoverObject.Position.Y;
                    z = mouseoverObject.Z;
                }
                else
                {
                    // over text?
                    return;
                }
                Interaction.DropItemToWorld(_ui.Cursor.HoldingItem, x, y, z);
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
                // Currently corpse entities do not Update() so they will not show their label.
            }
            else if (e is Item)
            {
                // Currently item entities do not Update() so they will not show their label.
            }
        }

        static void  parseMouse()
        {
            // If the mouse is over the world, then interpret mouse input:
            if (!_ui.IsMouseOverUI)
            {
                List<InputEventM> events = _inputNew.GetMouseEvents();
                foreach (InputEventM e in events)
                {
                    if (e.Button == ClientVars.MouseButton_Move)
                    {
                        onMoveButton(e);
                    }
                    else if (e.Button == ClientVars.MouseButton_Interact)
                    {
                        onInteractButton(e);
                    }
                    else
                    {
                        // no handler for this button.
                    }
                }
            }
        }

        static void parseKeyboard()
        {
            List<InputEventKB> events = _inputNew.GetKeyboardEvents();
            foreach (InputEventKB e in events)
            {
                // If we are targeting, cancel the target cursor if we hit escape.
                if (isTargeting)
                    if ((e.EventType == KeyboardEvent.Press) && e.KeyCode == WinKeys.Escape)
                        mouseTargetingCancel();

                // DEBUG: change direction of world light:
                if (e.EventType == KeyboardEvent.Press && e.KeyCode == WinKeys.I)
                {
                    float _LightRadians = _world.LightDirection;
                    _LightRadians += .01f;
                    _world.LightDirection = _LightRadians;
                }

                // Toggle for war mode:
                if (e.EventType == KeyboardEvent.Down && e.KeyCode == WinKeys.Tab)
                {
                    if (ClientVars.WarMode)
                        UltimaClient.Send(new RequestWarModePacket(false));
                    else
                        UltimaClient.Send(new RequestWarModePacket(true));
                }

                // toggle for All Names:
                /*if (_input.IsKeyDown(Keys.LeftShift) && _input.IsKeyDown(Keys.LeftControl))
                {
                    List<Mobile> mobiles = EntitiesCollection.GetObjectsByType<Mobile>();
                    foreach (Mobile m in mobiles)
                    {
                        if (m.Name != string.Empty)
                        {
                            m.AddOverhead(MessageType.Label, m.Name, 3, m.NotorietyHue);
                        }
                    }
                }*/
            }
        }

        public static void MouseTargeting(TargetTypes targetingType, int cursorID)
        {
            setTargeting(targetingType);
        }

        static void setTargeting(TargetTypes targetingType)
        {
            _TargettingType = targetingType;
            // Set the UI's cursor to a targetting cursor. If multi, tell the cursor which multi.
            _ui.Cursor.IsTargeting = true;
            // Stop continuous movement.
            _ContinuousMoveCheck = false;
        }

        static void clearTargeting()
        {
            // Clear our target cursor.
            _TargettingType = TargetTypes.Nothing;
            _ui.Cursor.IsTargeting = false;
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
                UltimaClient.Send(new TargetXYZPacket((short)selectedObject.Position.X, (short)selectedObject.Position.Y, (short)selectedObject.Z, (ushort)modelNumber));
            }
            
            // Clear our target cursor.
            clearTargeting();
        }
    }
}
