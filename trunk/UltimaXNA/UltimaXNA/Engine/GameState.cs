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
    public static class GameState
    {
        private static IInputService _input;
        private static IWorld _worldService;
        static IUIManager _legacyUI;

        private static Serial _lastTarget;
        public static Serial LastTarget
        {
            get { return _lastTarget; }
            set
            {
                _lastTarget = value;
                UltimaClient.Send(new GetPlayerStatusPacket(0x04, _lastTarget));
            }
        }
        public static Direction CursorDirection { get; internal set; }
        public static string DebugMessage
        {
            get { return generateDebugMessage(); }
        }
        public static bool WarMode
        {
            get { return (EntitiesCollection.GetPlayerObject() != null) ? ((Mobile)EntitiesCollection.GetPlayerObject()).IsWarMode : false; }
            set { ((Mobile)EntitiesCollection.GetPlayerObject()).IsWarMode = value; }
        }

        static GameTime _theTime;
        public static GameTime TheTime
        {
            get
            {
                if (_theTime == null)
                    return new GameTime();
                else
                    return _theTime;
            }
        }

        public static float BackBufferWidth = 0, BackBufferHeight = 0;
        private static bool _MovementFollowsMouse = true,  _ContinuousMoveCheck = false;
        private static int _cursorHoverTimeMS = 0, _hoverTimeForLabelMS = 1000;
        public static PickTypes DefaultPickType = PickTypes.PickStatics | PickTypes.PickObjects;
        // Are we asking for a target?
        private static int _TargettingType = -1;
        private static bool isTargeting
        {
            get
            {
                if (_TargettingType == -1)
                    return false;
                else
                    return true;
            }
        }
        // InWorld allows us to tell when our character object has been loaded in the world.
        public static bool InWorld { get; set; }
        // Set EngineRunning to false to cause the engine to immediately exit.
        public static bool EngineRunning { get; set; }
        public static bool IsMinimized { get; set; }
        
        // These are for hidden fun stuff
        public static MethodHook OnLeftClick, OnLeftOver, OnRightClick, OnRightOver, OnUpdate;
        public static GraphicsDevice Graphics;

        static bool DEBUG_BreakdownDataRead = false;
        static bool DEBUG_DisplayFPS = false;

        public static void Initialize(Game game)
        {
            _input = game.Services.GetService<IInputService>();
            _worldService = game.Services.GetService<IWorld>();
            _legacyUI = game.Services.GetService<IUIManager>();

            EngineRunning = true;
            InWorld = false;
            BackBufferWidth = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            BackBufferHeight = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            Graphics = game.GraphicsDevice;
            OnLeftClick += doInteractButton;
            OnRightClick += doMoveButton;
            loadDebugAssembly(@"..\..\..\debug.dll");
        }

        public static void Update(GameTime gameTime)
        {
            parseKeyboardAlways();

            if (InWorld)
            {
                // Set the target frame stuff.
                //((UI.Window_StatusFrame)UserInterface.Window("StatusFrame")).MyEntity = (Mobile)EntitiesCollection.GetPlayerObject();
                //if (LastTarget.IsValid)
                //    ((UI.Window_StatusFrame)UserInterface.Window("StatusFrame")).TargetEntity = EntitiesCollection.GetObject<Mobile>(LastTarget, false);

                // Parse keyboard input.
                parseKeyboardInWorld();

                // Get a pick type for the cursor.
                if (_legacyUI.IsMouseOverUI)
                {
                    _worldService.PickType = PickTypes.PickNothing;
                }
                else
                {
                    // Check to see if we are actively targetting something, or we have normal mouse interaction.
                    if (isTargeting)
                    {
                        // We are targetting. Based on the targetting type, we will either select an object, or a location.
                        _worldService.PickType = PickTypes.PickEverything;
                        _worldService.DEBUG_DrawDebug = true;
                    }
                    else
                    {
                        _worldService.PickType = DefaultPickType;
                        if (_input.IsMouseButtonRelease(MouseButtons.LeftButton))
                        {
                            // We need to pick ground tiles in case we are dropping something.
                            _worldService.PickType |= PickTypes.PickGroundTiles;
                        }
                    }
                }

                // Hidden Fun Stuff:
                // if (OnUpdate != null)
                //     OnUpdate();
            }

            _theTime = gameTime;
            updateFPS(gameTime);
        }

        public static void UpdateAfter(GameTime gameTime)
        {
            if (InWorld)
            {
                // Set the cursor direction.
                CursorDirection = mousePositionToDirection(_input.CurrentMousePosition);
                // get the cursor hoverTime and top up a label if enough time has passed.
                _cursorHoverTimeMS = (_input.IsCursorMovedSinceLastUpdate()) ? 0 : _cursorHoverTimeMS + gameTime.ElapsedRealTime.Milliseconds;
                if ((_cursorHoverTimeMS >= _hoverTimeForLabelMS) && (_worldService.MouseOverObject != null))
                {
                    createHoverLabel(_worldService.MouseOverObject);
                }

                // If the movement mouse button has been released, and movementFollowsMouse = true, reset mContinuousMovement.
                if (_input.IsMouseButtonRelease(MouseButtons.RightButton))
                {
                    // We do not move when the mouse cursor is released.
                    _ContinuousMoveCheck = false;
                    // If we are holding anything, we just dropped it.
                    if (UI.UIHelper.MouseHoldingItem != null)
                    {
                        checkDropItem();
                    }
                }

                // Changed to leverage movementFollowsMouse interface option -BERT
                if (_ContinuousMoveCheck)
                {
                    doMovement();
                }

                if (!_legacyUI.IsMouseOverUI)
                {
                    // Check if the left mouse button has been pressed. We will either walk to the object under the cursor
                    // or pick it up, depending on what kind of object we are looking at.
                    if (_input.IsMouseButtonPress(MouseButtons.LeftButton))
                    {
                        if (OnLeftClick != null)
                            OnLeftClick();
                    }
                    if (_input.IsMouseButtonPress(MouseButtons.RightButton))
                    {
                        if (OnRightClick != null)
                            OnRightClick();
                    }

                    if (_input.IsMouseButtonDown(MouseButtons.LeftButton))
                    {
                        if (OnLeftOver != null)
                            OnLeftOver();
                    }

                    if (_input.IsMouseButtonDown(MouseButtons.RightButton))
                    {
                        if (OnRightOver != null)
                            OnRightOver();
                    }
                }

                // Check for a move event from the player ...
                int direction = 0, sequence = 0, key = 0;
                bool hasMoveEvent = EntitiesCollection.GetPlayerObject().Movement.GetMoveEvent(ref direction, ref sequence, ref key);
                if (hasMoveEvent)
                    UltimaClient.Send(new MoveRequestPacket((byte)direction, (byte)sequence, key));

                // Show our target's name
                createHoverLabel(LastTarget);
            }
        }

        private static void doMovement()
        {
            Direction moveDirection = CursorDirection;
            float distanceFromCenterOfScreen = Vector2.Distance(_input.CurrentMousePosition, new Vector2(400, 300));
            if (distanceFromCenterOfScreen >= 150f)
                moveDirection |= Direction.Running;

            ((Mobile)EntitiesCollection.GetPlayerObject()).Move(moveDirection);

            if (_MovementFollowsMouse)
                _ContinuousMoveCheck = true;
        }

        private static Direction mousePositionToDirection(Vector2 mousePosition)
        {
            double Angle = Math.Atan2(mousePosition.Y - 300, mousePosition.X - 400);
            if (Angle < 0)
                Angle = Math.PI + (Math.PI + Angle);
            double piPerSegment = (Math.PI * 2f) / 8f;
            double segmentValue = (Math.PI * 2f) / 16f;
            int direction = int.MaxValue;

            for (int i = 0; i < 8; i++)
            {
                if (Angle >= segmentValue && Angle <= (segmentValue + piPerSegment))
                {
                    direction = i + 1;
                    break;
                }
                segmentValue += piPerSegment;
            }

            if (direction == int.MaxValue)
                direction = 0;

            direction = (direction >= 7) ? (direction - 7) : (direction + 1);

            return (Direction)direction;
        }

        private static void doMoveButton()
        {
            MapObject mouseOverObject = _worldService.MouseOverObject;

            // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
            // If not, then we are just clicking the mouse and we need to find out if something is under the mouse cursor.
            if (isTargeting)
            {
                switch (_TargettingType)
                {
                    case 0:
                        // Select Object
                        _worldService.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
                        mouseTargetingEventObject(mouseOverObject);
                        break;
                    case 1:
                        // Select X, Y, Z
                        if (mouseOverObject != null)
                        {
                            mouseTargetingEventObject(mouseOverObject);
                        }
                        else
                        {
                            MapObject ground = _worldService.MouseOverGroundTile;
                            if (ground != null)
                                mouseTargetingEventXYZ(ground);
                            else
                                mouseTargetingCancel();
                        }

                        break;
                    default:
                        throw new Exception("Unknown targetting type!");
                }
            }
            else
            {
                if (mouseOverObject != null)
                {
                    if (mouseOverObject is MapObjectMobile)
                    {
                        // Proper action: target this mobile.
                        LastTarget = mouseOverObject.OwnerSerial;
                        UltimaClient.Send(new SingleClickPacket(LastTarget));
                        return;
                    }
                    if (mouseOverObject is MapObjectItem)
                    {
                        // Proper action: pick it up if possible, as long as this is a press event.
                        Item item = EntitiesCollection.GetObject<Item>(mouseOverObject.OwnerSerial, false);
                        if (item.ItemData.Weight != 255)
                        {
                            UI.UIHelper.PickUpItem(item);
                            return;
                        }
                    }
                }
            }
            doMovement();
        }

        private static void doInteractButton()
        {
            MapObject o = _worldService.MouseOverObject;
            if ((o != null) && !(o is MapObjectStatic))
            {
                Entity iObject = EntitiesCollection.GetObject<Entity>(o.OwnerSerial, false);
                // default option is to simply 'use' this object, although this will doubtless be more complicated in the future.
                // Perhaps the use option is based on the type of object? Anyways, for right now, we only interact with gameobjects,
                // and we send a double-click to the server.
                if (iObject is Item)
                {
                    UltimaClient.Send(new DoubleClickPacket(iObject.Serial));
                }
                else if (iObject is PlayerMobile)
                {
                    LastTarget = iObject.Serial;
                    if (iObject.Serial == EntitiesCollection.MySerial)
                    {
                        // this is my player.
                        // if mounted, dismount.
                        if (((Mobile)iObject).IsMounted)
                        {
                            UltimaClient.Send(new DoubleClickPacket(iObject.Serial));
                        }
                    }
                    // else other interaction?
                }
                else if (iObject is Mobile)
                {
                    // We request a context sensitive menu. This automatically sends a double click if no context menu is handled. See parseContextMenu...
                    LastTarget = iObject.Serial;
                    if (WarMode)
                    {
                        UltimaClient.Send(new AttackRequestPacket(iObject.Serial));
                    }
                    else
                    {
                        UltimaClient.Send(new RequestContextMenuPacket(iObject.Serial));
                    }
                }
                else
                {
                    // do nothing?
                }
            }
        }

        private static void checkDropItem()
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
                if ((UI.UIHelper.MouseHoldingItem).Item_ContainedWithinSerial.IsValid)
                {
                    // We must manually remove the item from the container, as RunUO does not do this for us.
                    ContainerItem iContainer = EntitiesCollection.GetObject<ContainerItem>(
                        (UI.UIHelper.MouseHoldingItem).Item_ContainedWithinSerial, false);
                    iContainer.Contents.RemoveItem(UI.UIHelper.MouseHoldingItem.Serial);
                }
                UI.UIHelper.DropItemOntoGround(x, y, z);
            }
        }

        private static void createHoverLabel(MapObject mapObject)
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

        private static void createHoverLabel(Serial serial)
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

        static void parseKeyboardAlways()
        {
            if (_input.IsKeyDown(Keys.LeftAlt))
            {
                if (_input.IsKeyPress(Keys.D))
                    DEBUG_BreakdownDataRead = Utility.ToggleBoolean(DEBUG_BreakdownDataRead);
                if (_input.IsKeyPress(Keys.F))
                    DEBUG_DisplayFPS = Utility.ToggleBoolean(DEBUG_DisplayFPS);
            }
        }

        private static void parseKeyboardInWorld()
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
                if (WarMode)
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

        // Maintain an accurate count of frames per second.
        private static float _FPS = 0, _Frames = 0, _ElapsedSeconds = 0;
        private static bool updateFPS(GameTime gameTime)
        {
            _Frames++;
            _ElapsedSeconds += (float)gameTime.ElapsedRealTime.TotalSeconds;
            if (_ElapsedSeconds >= .5f)
            {
                _FPS = _Frames / _ElapsedSeconds;
                _ElapsedSeconds = 0;
                _Frames = 0;
                return true;
            }
            return false;
        }

        // Debug message - I put a lot of crap in here to test values.
        // Feel free to add or remove variables.
        private static string generateDebugMessage()
        {
            String debugMessage = string.Empty;
            
            if (DEBUG_DisplayFPS)
                debugMessage += string.Format("FPS: {0}\n", (int)_FPS);
            

            if (DEBUG_BreakdownDataRead)
                debugMessage += Metrics.DataReadBreakdown;
            else
                debugMessage += "Data Read: " + Metrics.TotalDataRead.ToString() + '\n';

            if (InWorld && !_legacyUI.IsMouseOverUI)
            {
                debugMessage += string.Format("#Objects: {0}\n", _worldService.ObjectsRendered);
                debugMessage += string.Format("Warmode: {0}\n", WarMode);
                if (_worldService.MouseOverObject != null)
                {
                    debugMessage += "OBJECT: " + _worldService.MouseOverObject.ToString() + Environment.NewLine;
                    if (_worldService.MouseOverObject is MapObjectStatic)
                    {
                        debugMessage += "ArtID: " + ((MapObjectStatic)_worldService.MouseOverObject).ItemID;
                    }
                    else if (_worldService.MouseOverObject is MapObjectMobile)
                    {
                        Mobile iUnit = EntitiesCollection.GetObject<Mobile>(_worldService.MouseOverObject.OwnerSerial, false);
                        if (iUnit != null)
                            debugMessage += "Name: " + iUnit.Name + Environment.NewLine;
                        debugMessage +=
                            "AnimID: " + ((MapObjectMobile)_worldService.MouseOverObject).BodyID + Environment.NewLine +
                            "Serial: " + _worldService.MouseOverObject.OwnerSerial + Environment.NewLine +
                            "Hue: " + ((MapObjectMobile)_worldService.MouseOverObject).Hue;
                    }
                    else if (_worldService.MouseOverObject is MapObjectItem)
                    {
                        debugMessage +=
                            "ArtID: " + ((MapObjectItem)_worldService.MouseOverObject).ItemID + Environment.NewLine +
                            "Serial: " + _worldService.MouseOverObject.OwnerSerial;
                    }
                    debugMessage += " Z: " + _worldService.MouseOverObject.Z;
                }
                else
                {
                    debugMessage += "OVER: " + "null";
                }
                if (_worldService.MouseOverGroundTile != null)
                {
                    debugMessage += Environment.NewLine + "GROUND: " + _worldService.MouseOverGroundTile.Position.ToString();
                }
                else
                {
                    debugMessage += Environment.NewLine + "GROUND: null";
                }
            }
            return debugMessage;
        }


        public static void MouseTargeting(int nCursorID, int nTargetingType)
        {
            setTargeting(nTargetingType);
        }

        private static void setTargeting(int targetingType)
        {
            _TargettingType = targetingType;
            // Set the UI's cursor to a targetting cursor.
            _legacyUI.Cursor.IsTargeting = true;
            // Stop continuous movement.
            _ContinuousMoveCheck = false;
        }

        private static void clearTargeting()
        {
            // Clear our target cursor.
            _TargettingType = -1;
            _legacyUI.Cursor.IsTargeting = false;
        }

        private static void mouseTargetingCancel()
        {
            // Send the cancel target message back to the server.
            UltimaClient.Send(new TargetCancelPacket());
            clearTargeting();
        }

        private static void mouseTargetingEventXYZ(MapObject selectedObject)
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

        private static void mouseTargetingEventObject(MapObject selectedObject)
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

        private static void loadDebugAssembly(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                System.Reflection.Assembly debugAssembly = System.Reflection.Assembly.LoadFrom(filename);
                Object o = debugAssembly.CreateInstance("UltimaXNA.UXNADebug", true);
            }
        }
    }
}
