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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Data;
using UltimaXNA.Client;
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.InputOld;
using UltimaXNA.Network.Packets.Client;
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
        static IInputService _input;
        static IWorld _world;
        static IUIManager _ui;

        // mouse input variables
        static ClickState[] _mouseButtonClicks;
        static bool _ContinuousMoveCheck = false;
        static float _TimeToShowTip = float.MaxValue;
        const float _TimeHoveringBeforeTip = 1.0f;

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
            _input = game.Services.GetService<IInputService>();
            _world = game.Services.GetService<IWorld>();
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
                ClientVars.CursorDirection = Utility.DirectionFromVectors(new Vector2(400, 300), _input.MousePosition);

                // Show a popup tip if we have hovered over this item for X seconds.
                if (_input.IsCursorMovedSinceLastUpdate())
                    _TimeToShowTip = ClientVars.TheTime + _TimeHoveringBeforeTip;
                else
                    if (_TimeToShowTip >= ClientVars.TheTime && _world.MouseOverObject != null)
                        createHoverLabel(_world.MouseOverObject);

                // check for mouse input which relates to the world.
                if (!_ui.IsMouseOverUI)
                {
                    // movement button
                    if (_input.IsMouseButtonPress(ClientVars.MouseButton_Move))
                        onMoveButtonDown();
                    if (_input.IsMouseButtonRelease(ClientVars.MouseButton_Move))
                        onMoveButtonUp();

                    // targeting
                    if (isTargeting && _input.IsMouseButtonPress(ClientVars.MouseButton_Interact))
                        onTargetingButtonDown();

                    // interaction button
                    onInteractButton(_mouseButtonClicks[(int)ClientVars.MouseButton_Interact]);
                    // special case: if the interaction button has been released, and we are holding anything, we just dropped it.
                    if (_input.IsMouseButtonRelease(ClientVars.MouseButton_Interact))
                        if (_ui.Cursor.IsHolding)
                            checkDropItem();
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
            float distanceFromCenterOfScreen = Vector2.Distance(_input.MousePosition, new Vector2(400, 300));
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
            MapObject mouseOverObject = _world.MouseOverObject;
            if (mouseOverObject == null)
                return;

            // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
            // If not, then we are just clicking the mouse and we need to find out if something is under the mouse cursor.
            switch (_TargettingType)
            {
                case TargetTypes.Object:
                    // Select Object
                    _world.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                    mouseTargetingEventObject(mouseOverObject);
                    break;
                case TargetTypes.Position:
                    // Select X, Y, Z
                    _world.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                    mouseTargetingEventObject(mouseOverObject); // mouseTargetingEventXYZ(mouseOverObject);
                    break;
                case TargetTypes.MultiPlacement:
                    // select X, Y, Z
                    mouseTargetingEventXYZ(mouseOverObject);
                    break;
                default:
                    throw new Exception("Unknown targetting type!");
            }
        }

        static void onInteractButton(ClickState clickState)
        {
            ClickTypes click = clickState.Click;

            // we only need to interact when we click.
            if (click == ClickTypes.None)
                return;
            else
                clickState.Reset();

            // get the object under our mouse cursor. Return if it is null.
            MapObject mouseOverObject = clickState.Object;
            if (mouseOverObject == null)
                return;

            if (mouseOverObject is MapObjectGround)
            {
                // we can't interact with ground tiles.
            }
            else if (mouseOverObject is MapObjectStatic)
            {
                // clicking a static should pop up the name of the static.
                if (click == ClickTypes.SingleClick)
                {

                }
            }
            else if (mouseOverObject is MapObjectItem)
            {
                Item entity = (Item)mouseOverObject.OwnerEntity;
                // single click = tool tip
                // double click = use / open
                // click and drag = pick up
                switch (click)
                {
                    case ClickTypes.SingleClick:
                        // tool tip
                        Interaction.SingleClick(entity);
                        break;
                    case ClickTypes.DoubleClick:
                        Interaction.DoubleClick(entity);
                        break;
                    case ClickTypes.Drag:
                        Interaction.PickUpItem(entity, clickState.ObjectClickPoint.X, clickState.ObjectClickPoint.Y);
                        break;
                }
            }
            else if (mouseOverObject is MapObjectMobile)
            {
                Mobile entity = (Mobile)mouseOverObject.OwnerEntity;
                // single click = tool tip; if npc = request context sensitive menu
                // double click = set last target; if is human open paper doll; if ridable ride; if self and riding, dismount;
                // click and drag = pull off status bar
                switch (click)
                {
                    case ClickTypes.SingleClick:
                        // tool tip
                        Interaction.SingleClick(entity);
                        if (ClientVars.WarMode)
                            UltimaClient.Send(new AttackRequestPacket(entity.Serial));
                        else
                            UltimaClient.Send(new RequestContextMenuPacket(entity.Serial));
                        break;
                    case ClickTypes.DoubleClick:
                        Interaction.DoubleClick(entity);
                        ClientVars.LastTarget = entity.Serial;
                        break;
                    case ClickTypes.Drag:
                        // pull off status bar
                        break;
                }
            }
            else if (mouseOverObject is MapObjectCorpse)
            {
                Corpse entity = (Corpse)mouseOverObject.OwnerEntity;
                // click and drag = nothing
                // single click = tool tip
                // double click = open loot window.
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
                else if (mouseoverObject is MapObjectItem || mouseoverObject is MapObjectItem)
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

        static void parseMouse()
        {
            // must create the object that maintains the clicks.
            if (_mouseButtonClicks == null)
            {
                _mouseButtonClicks = new ClickState[(int)MouseButton.Count];
                for (int i = 0; i < _mouseButtonClicks.Length; i++)
                    _mouseButtonClicks[i] = new ClickState();
            }

            int x = (int)_input.MousePosition.X;
            int y = (int)_input.MousePosition.Y;
            for (MouseButton i = MouseButton.LeftButton; i < MouseButton.Count; i++)
            {
                // Check for mouse button presses ...
                if (_input.IsMouseButtonPress(i))
                {
                    if (_ui.Cursor.IsHolding)
                    {
                        // Special case: if we are holding an item but the mouse button is unpressed
                        // (i.e. failed to drop the item we are holding) then ignore button presses.
                    }
                    else
                    {
                        _mouseButtonClicks[(int)i].Press(x, y, _world.MouseOverObject, _world.MouseOverObjectPoint);
                    }
                }
                
                // ... and releases 
                if (_input.IsMouseButtonRelease(i))
                {
                    _mouseButtonClicks[(int)i].Release(x, y);
                }
                _mouseButtonClicks[(int)i].Update(x, y);
            }
        }

        static void parseKeyboard()
        {
            // If we are targeting, cancel the target cursor if we hit escape.
            if (isTargeting)
                if (_input.IsKeyPress(Keys.Escape))
                    mouseTargetingCancel();

            float _LightRadians = _world.LightDirection;
            if (_input.IsKeyDown(Keys.I))
                 _LightRadians += .001f;
            _world.LightDirection = _LightRadians;

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
