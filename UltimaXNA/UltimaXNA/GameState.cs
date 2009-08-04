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
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Data;
using UltimaXNA.Client;
using UltimaXNA.Entities;
using UltimaXNA.GUI;
using UltimaXNA.Input;
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA
{
    public static partial class GameState
    {
        public static bool WarMode
        {
            get
            {
                return (EntitiesCollection.GetPlayerObject() != null) ? ((Mobile)EntitiesCollection.GetPlayerObject()).IsWarMode : false;
            }
            set
            {
                ((Mobile)EntitiesCollection.GetPlayerObject()).IsWarMode = value;
            }
        }

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
        public static string DebugMessage { get { return generateDebugMessage(); } }
        public static float BackBufferWidth = 0, BackBufferHeight = 0;
        // added for future interface option, allowing both continuous mouse movement and discrete clicks -BERT
        private static bool _MovementFollowsMouse = true;
        private static bool _ContinuousMoveCheck = false;
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
        // These variables move the light source around...
        private static Vector3 _LightDirection = new Vector3(0f, 0f, 1f);
        private static double _LightRadians = -0.6d;
        private static int _cursorHoverTimeMS = 0, _hoverTimeForLabelMS = 1000;
        private static PickTypes DefaultPickType = PickTypes.PickStatics | PickTypes.PickObjects;

        public static MethodHook OnLeftClick;
        public static MethodHook OnRightClick;
        public static bool HandleMouseInput = true;

        public static void Initialize(Game game)
        {
            EngineRunning = true;
            InWorld = false;
            BackBufferWidth = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            BackBufferHeight = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public static void Update(GameTime gameTime)
        {
            if (InWorld)
            {
                // Set the target frame stuff.
                ((GUI.Window_StatusFrame)UserInterface.Window("StatusFrame")).MyEntity = (Mobile)EntitiesCollection.GetPlayerObject();
                if (LastTarget.IsValid)
                    ((GUI.Window_StatusFrame)UserInterface.Window("StatusFrame")).TargetEntity = EntitiesCollection.GetObject<Mobile>(LastTarget, false);

                // Parse keyboard input.
                parseKeyboard(InputHandler.Keyboard);

                // Get a pick type for the cursor.
                if (UserInterface.IsMouseOverGUI(InputHandler.Mouse.Position))
                {
                    WorldRenderer.PickType = PickTypes.PickNothing;
                }
                else
                {
                    // Check to see if we are actively targetting something, or we have normal mouse interaction.
                    if (isTargeting)
                    {
                        // We are targetting. Based on the targetting type, we will either select an object, or a location.
                        WorldRenderer.PickType = PickTypes.PickEverything;
                        TileEngine.WorldRenderer.DEBUG_DrawTileOver = true;
                    }
                    else
                    {
                        WorldRenderer.PickType = DefaultPickType;
                        if (InputHandler.Mouse.Buttons[0].Release)
                        {
                            // We need to pick ground tiles in case we are dropping something.
                            WorldRenderer.PickType |= PickTypes.PickGroundTiles;
                        }
                    }
                }
            }
            updateFPS(gameTime);
        }

        public static void UpdateAfter(GameTime gameTime)
        {
            if (InWorld)
            {
                // Set the cursor direction.
                CursorDirection = mousePositionToDirection(InputHandler.Mouse.Position);
                // get the cursor hoverTime and top up a label if enough time has passed.
                _cursorHoverTimeMS = (InputHandler.Mouse.MovedSinceLastUpdate) ? 0 : _cursorHoverTimeMS + gameTime.ElapsedRealTime.Milliseconds;
                if ((_cursorHoverTimeMS >= _hoverTimeForLabelMS) && (WorldRenderer.MouseOverObject != null))
                {
                    createHoverLabel(WorldRenderer.MouseOverObject);
                }

                // If the left mouse button has been released, and movementFollowsMouse = true, reset mContinuousMovement.
                if (InputHandler.Mouse.Buttons[0].Release)
                {
                    // We do not move when the mouse cursor is released.
                    _ContinuousMoveCheck = false;
                    // If we are holding anything, we just dropped it.
                    if (GUI.GUIHelper.MouseHoldingItem != null)
                    {
                        checkDropItem();
                    }
                }

                // Changed to leverage movementFollowsMouse interface option -BERT
                if (_ContinuousMoveCheck)
                {
                    checkMove();
                }

                if (!UserInterface.IsMouseOverGUI(InputHandler.Mouse.Position))
                {
                    // Check if the left mouse button has been pressed. We will either walk to the object under the cursor
                    // or pick it up, depending on what kind of object we are looking at.
                    if (InputHandler.Mouse.Buttons[0].Press)
                    {
                        if (HandleMouseInput)
                            checkLeftClick();
                        if (OnLeftClick != null)
                            OnLeftClick();
                    }
                    if (InputHandler.Mouse.Buttons[1].Press)
                    {
                        if (HandleMouseInput)
                            checkRightClick();
                        if (OnRightClick != null)
                            OnRightClick();
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

        private static void checkMove()
        {
            Direction moveDirection = CursorDirection;
            float distanceFromCenterOfScreen = Vector2.Distance(new Vector2(InputHandler.Mouse.Position.X, InputHandler.Mouse.Position.Y), new Vector2(400, 300));
            if (distanceFromCenterOfScreen >= 200f)
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

        private static void checkLeftClick()
        {
            MapObject mouseOverObject = WorldRenderer.MouseOverObject;

            // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
            // If not, then we are just clicking the mouse and we need to find out if something is under the mouse cursor.
            if (isTargeting)
            {
                switch (_TargettingType)
                {
                    case 0:
                        // Select Object
                        WorldRenderer.PickType = PickTypes.PickStatics | PickTypes.PickObjects;
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
                            MapObject ground = WorldRenderer.MouseOverGroundTile;
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
                            GUI.GUIHelper.PickUpItem(item);
                            return;
                        }
                    }
                }
            }

            checkMove();
        }

        private static void checkRightClick()
        {
            MapObject iMapObject = WorldRenderer.MouseOverObject;
            if ((iMapObject != null) && !(iMapObject is MapObjectStatic))
            {
                Entity iObject = EntitiesCollection.GetObject<Entity>(iMapObject.OwnerSerial, false);
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
            // We do not handle dropping the item into the GUI in this routine.
            // But we probably should, to consolidate game logic.
            if (!UserInterface.IsMouseOverGUI(InputHandler.Mouse.Position))
            {
                int x, y, z;
                MapObject groundObject = WorldRenderer.MouseOverGroundTile;
                MapObject mouseoverObject = WorldRenderer.MouseOverObject;
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
                    GUI.GUIHelper.MouseHoldingItem = null;
                    return;
                }
                // We dropped the icon in the world. This means we are trying to drop the item
                // into the world. Let's do it!
                if ((GUI.GUIHelper.MouseHoldingItem).Item_ContainedWithinSerial.IsValid)
                {
                    // We must manually remove the item from the container, as RunUO does not do this for us.
                    ContainerItem iContainer = EntitiesCollection.GetObject<ContainerItem>(
                        (GUI.GUIHelper.MouseHoldingItem).Item_ContainedWithinSerial, false);
                    iContainer.Contents.RemoveItem(GUI.GUIHelper.MouseHoldingItem.Serial);
                }
                GUI.GUIHelper.DropItemOntoGround(x, y, z);
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

        private static void parseKeyboard(Input.KeyboardHandler keyboard)
        {
            // If we are targeting, cancel the target cursor if we hit escape.
            if (isTargeting)
                if (keyboard.IsKeyPressed(Keys.Escape))
                    mouseTargetingCancel();

            // if (keyboard.IsKeyDown(Keys.I))
            //     _LightRadians += .01f;
            // if (keyboard.IsKeyDown(Keys.K))
            //    _LightRadians -= .01f;

            _LightDirection.Z = -(float)Math.Cos(_LightRadians);
            _LightDirection.Y = (float)Math.Sin(_LightRadians);

            WorldRenderer.SetLightDirection(_LightDirection);

            // Toggle for backpack container window.
            if (keyboard.IsKeyPressed(Keys.B) && (keyboard.IsKeyDown(Keys.LeftControl)))
            {
                Serial backpackSerial = ((PlayerMobile)EntitiesCollection.GetPlayerObject())
                    .equipment[(int)EquipLayer.Backpack].Serial;
                if (UserInterface.Window("Container:" + backpackSerial) == null)
                    UltimaClient.Send(new DoubleClickPacket(backpackSerial));
                else
                    UserInterface.CloseWindow("Container:" + backpackSerial);
            }

            // Toggle for paperdoll window.
            if (keyboard.IsKeyPressed(Keys.C) && (keyboard.IsKeyDown(Keys.LeftControl)))
            {
                Serial serial = ((PlayerMobile)EntitiesCollection.GetPlayerObject())
                    .Serial;
                if (UserInterface.Window("PaperDoll:" + serial) == null)
                    UserInterface.PaperDoll_Open(EntitiesCollection.GetPlayerObject());
                else
                    UserInterface.CloseWindow("PaperDoll:" + serial);
            }

            // Toggle for logout
            if (keyboard.IsKeyPressed(Keys.Q) && (keyboard.IsKeyDown(Keys.LeftControl)))
            {
                SceneManagement.SceneManager.CurrentScene = new SceneManagement.LoginScene();
            }

            // toggle for warmode
            if (keyboard.IsKeyPressed(Keys.Tab))
            {
                if (WarMode)
                    UltimaClient.Send(new RequestWarModePacket(false));
                else
                    UltimaClient.Send(new RequestWarModePacket(true));
            }

            // toggle for all names
            if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.LeftControl))
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

			if (keyboard.IsKeyDown(Keys.P) && keyboard.IsKeyDown(Keys.LeftControl))
			{
				ParticleEngine.ParticleEngine.LoadEffectFromString("print(\"hubert\")");
				ParticleEngine.ParticleEngine.LoadEffectFromScript("foobert");
				ParticleEngine.ParticleEngine.LoadEffectFromString("util:Log(\"foobert\")");
			}
        }

        // Maintain an accurate count of frames per second.
        private static float _FPS = 0, _Frames = 0, _ElapsedSeconds = 0;
        private static bool updateFPS(GameTime gameTime)
        {
            _Frames++;
            _ElapsedSeconds += (float)gameTime.ElapsedRealTime.TotalSeconds;
            if (_ElapsedSeconds >= 1)
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
            String debugMessage = "FPS: " + ((int)_FPS).ToString() + Environment.NewLine;
            if (InWorld)
            {
                debugMessage += "Objects on screen: " + WorldRenderer.ObjectsRendered.ToString() + Environment.NewLine;
                debugMessage += "WarMode: " + WarMode.ToString() + Environment.NewLine;
                if (WorldRenderer.MouseOverObject != null)
                {
                    debugMessage += "OBJECT: " + WorldRenderer.MouseOverObject.ToString() + Environment.NewLine;
                    if (WorldRenderer.MouseOverObject is MapObjectStatic)
                    {
                        debugMessage += "ArtID: " + ((MapObjectStatic)WorldRenderer.MouseOverObject).ItemID;
                    }
                    else if (WorldRenderer.MouseOverObject is MapObjectMobile)
                    {
                        Mobile iUnit = EntitiesCollection.GetObject<Mobile>(WorldRenderer.MouseOverObject.OwnerSerial, false);
                        if (iUnit != null)
                            debugMessage += "Name: " + iUnit.Name + Environment.NewLine;
                        debugMessage +=
                            "AnimID: " + ((MapObjectMobile)WorldRenderer.MouseOverObject).BodyID + Environment.NewLine +
                            "Serial: " + WorldRenderer.MouseOverObject.OwnerSerial + Environment.NewLine +
                            "Hue: " + ((MapObjectMobile)WorldRenderer.MouseOverObject).Hue;
                    }
                    else if (WorldRenderer.MouseOverObject is MapObjectItem)
                    {
                        debugMessage +=
                            "ArtID: " + ((MapObjectItem)WorldRenderer.MouseOverObject).ItemID + Environment.NewLine +
                            "Serial: " + WorldRenderer.MouseOverObject.OwnerSerial;
                    }
                    debugMessage += " Z: " + WorldRenderer.MouseOverObject.Z;
                }
                else
                {
                    debugMessage += "OVER: " + "null";
                }
                if (WorldRenderer.MouseOverGroundTile != null)
                {
                    debugMessage += Environment.NewLine + "GROUND: " + WorldRenderer.MouseOverGroundTile.Position.ToString();
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
            _TargettingType = nTargetingType;
            // Set the GUI's cursor to a targetting cursor.
            UserInterface.TargettingCursor = true;
            // Stop continuous movement.
            _ContinuousMoveCheck = false;
        }

        private static void mouseTargetingCancel()
        {
            // Send the cancel target message back to the server.
            UltimaClient.Send(new TargetCancelPacket());
            // Clear our target cursor.
            _TargettingType = -1;
            UserInterface.TargettingCursor = false;
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
            // ... and then clear our target cursor.
            _TargettingType = -1;
            UserInterface.TargettingCursor = false;
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
            _TargettingType = -1;
            UserInterface.TargettingCursor = false;
        }
    }
}
