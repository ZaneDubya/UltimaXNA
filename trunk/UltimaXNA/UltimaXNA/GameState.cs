﻿/***************************************************************************
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Data;
using UltimaXNA.Client;
using UltimaXNA.Entities;
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA
{
    public interface IGameState
    {
        bool InWorld { get; set; }
        bool EngineRunning { get; set; }
        Serial LastTarget { get; set; }
        void MouseTargeting(int nCursorID, int nTargetingType);
        bool WarMode { get; set; }
        Direction CursorDirection { get; }
    }

    public class GameState : GameComponent, IGameState
    {
        public bool WarMode
        {
            get
            {
                return (_Entities.GetPlayerObject() != null) ? ((Mobile)_Entities.GetPlayerObject()).IsWarMode : false;
            }
            set
            {
                ((Mobile)_Entities.GetPlayerObject()).IsWarMode = value;
            }
        }
        Input.IInputService _Input;
        TileEngine.ITileEngine _TileEngine;
        IEntitiesService _Entities;
        TileEngine.IWorld _World;
        GUI.IGUI _GUI;
        Client.IUltimaClient _GameClient;
        private Serial _lastTarget;
        public Serial LastTarget {
            get { return _lastTarget; }
            set
            {
                _lastTarget = value;
                _GameClient.Send(new GetPlayerStatusPacket(0x04, _lastTarget));
            }
        }
        public Direction CursorDirection { get; protected set; }

        public string DebugMessage { get { return generateDebugMessage(); } }

        // added for future interface option, allowing both continuous mouse movement and discrete clicks -BERT
        private bool _MovementFollowsMouse = true;
        private bool _ContinuousMoveCheck = false;
        // Are we asking for a target?
        private int _TargettingType = -1;
        private bool isTargeting
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
        private bool _InWorld;
        public bool InWorld
        {
            get { return _InWorld; }
            set { _InWorld = value; }
        }
        // Set EngineRunning to false to cause the engine to immediately exit.
        public bool EngineRunning { get; set; }
        // These variables move the light source around...
        private Vector3 _LightDirection = new Vector3(0f, 0f, 1f);
        private double _LightRadians = -0.5d;

        public GameState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGameState), this);
            EngineRunning = true;
            InWorld = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            // Load the service objects.
            _Input = (Input.IInputService)Game.Services.GetService(typeof(Input.IInputService));
            _TileEngine = (TileEngine.ITileEngine)Game.Services.GetService(typeof(TileEngine.ITileEngine));
            _Entities = (IEntitiesService)Game.Services.GetService(typeof(IEntitiesService));
            _World = (TileEngine.IWorld)Game.Services.GetService(typeof(TileEngine.IWorld));
            _GUI = (GUI.IGUI)Game.Services.GetService(typeof(GUI.IGUI));
            _GameClient = Game.Services.GetService(typeof(Client.IUltimaClient)) as Client.IUltimaClient;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InWorld)
            {
                // Set the target frame stuff.
                ((GUI.Window_StatusFrame)_GUI.Window("StatusFrame")).MyEntity = (Mobile)_Entities.GetPlayerObject();
                if (LastTarget.IsValid)
                    ((GUI.Window_StatusFrame)_GUI.Window("StatusFrame")).TargetEntity = _Entities.GetObject<Mobile>(LastTarget, false);

                // Parse keyboard input.
                parseKeyboard(_Input.Keyboard);

                // Get a pick type for the cursor.
                if (_GUI.IsMouseOverGUI(_Input.Mouse.Position))
                {
                    _TileEngine.PickType = UltimaXNA.TileEngine.PickTypes.PickNothing;
                }
                else
                {
                    // Check to see if we are actively targetting something, or we have normal mouse interaction.
                    if (isTargeting)
                    {
                        // We are targetting. Based on the targetting type, we will either select an object, or a location.
                        _TileEngine.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects | TileEngine.PickTypes.PickGroundTiles;
                    }
                    else
                    {
                        // Changed to leverage movementFollowsMouse interface option -BERT
                        if (_MovementFollowsMouse ? _Input.Mouse.Buttons[0].IsDown : _Input.Mouse.Buttons[0].Press)
                        {
                            _TileEngine.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects | TileEngine.PickTypes.PickGroundTiles;
                        }
                        else if (_Input.Mouse.Buttons[1].Press)
                        {
                            _TileEngine.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects;
                        }
                        else if (_Input.Mouse.Buttons[0].Release)
                        {
                            _TileEngine.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects | TileEngine.PickTypes.PickGroundTiles;
                        }
                        else
                        {
                            _TileEngine.PickType = TileEngine.PickTypes.PickNothing;
                        }
                    }
                }
            }
            updateFPS(gameTime);
        }

        public void UpdateAfter()
        {
            if (InWorld)
            {
                // Set the cursor direction.
                CursorDirection = mousePositionToDirection(_Input.Mouse.Position);

                // Check to see if we are actively targetting something, or we have normal mouse interaction.
                if (isTargeting)
                {
                    // We are targetting. 
                    TileEngine.IMapObject iMapObject = null;
                    // If we press the left mouse button, we send the targetting event.
                    if (_Input.Mouse.Buttons[0].Press)
                    {
                        switch (_TargettingType)
                        {
                            case 0:
                                // Select Object
                                _TileEngine.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects;
                                iMapObject = _TileEngine.MouseOverObject;
                                MouseTargetingEventObject(iMapObject);
                                break;
                            case 1:
                                // Select X, Y, Z
                                iMapObject = _TileEngine.MouseOverObject;
                                if (iMapObject != null)
                                {
                                    MouseTargetingEventObject(iMapObject);
                                }
                                else
                                {
                                    iMapObject = _TileEngine.MouseOverGroundTile;
                                    if (iMapObject != null)
                                        mouseTargetingEventXYZ(iMapObject);
                                    else
                                        mouseTargetingCancel();
                                }
                                
                                break;
                            default:
                                throw new Exception("Unknown targetting type!");
                        }
                    }
                }
                else
                {
                    // If the left mouse button has been released, and movementFollowsMouse = true, reset mContinuousMovement.
                    if (_Input.Mouse.Buttons[0].Release)
                        _ContinuousMoveCheck = false;

                    // Changed to leverage movementFollowsMouse interface option -BERT
                    if (_ContinuousMoveCheck)
                    {
                        if (_Input.Mouse.Buttons[0].IsDown)
                        {
                            checkMove();
                        }
                    }

                    // If the left mouse button has been released, we check to see if we were holding an item in the cursor.
                    // If we dropped the item into another slot, then the GUI will have already removed the MouseHoldingItem,
                    // and we will never reach this routine. If the MouseHoldingItem is still here, we need to take care of it.
                    #region DropItemFromMouse
                    if (_Input.Mouse.Buttons[0].Release)
                    {
                        if (!(_Input.Keyboard.IsKeyDown(Keys.U)))
                            if (GUI.GUIHelper.MouseHoldingItem != null)
                            {
                                if (!_GUI.IsMouseOverGUI(_Input.Mouse.Position))
                                {
                                    int x, y, z;
                                    IMapObject groundObject = _TileEngine.MouseOverGroundTile;
                                    IMapObject mouseoverObject = _TileEngine.MouseOverObject;
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
                                        ContainerItem iContainer = _Entities.GetObject<ContainerItem>(
                                            (GUI.GUIHelper.MouseHoldingItem).Item_ContainedWithinSerial, false);
                                        iContainer.Contents.RemoveItem(GUI.GUIHelper.MouseHoldingItem.Serial);
                                    }
                                    GUI.GUIHelper.DropItemOntoGround(x, y, z);
                                }

                            }
                    }
                    #endregion

                    // Check if the left mouse button has been pressed. We will either walk to the object under the cursor
                    // or pick it up, depending on what kind of object we are looking at.
                    if (_Input.Mouse.Buttons[0].Press)
                    {
                        if (!_GUI.IsMouseOverGUI(_Input.Mouse.Position))
                        {
                            checkLeftClick();
                        }
                    }

                    if (_Input.Mouse.Buttons[1].Press)
                    {
                        // Right button pressed ... activate this object.
                        checkRightClick();
                    }

                    // Check for a move event from the player ...
                    try
                    {
                        int direction = 0, sequence = 0, key = 0;
                        bool hasMoveEvent = _Entities.GetPlayerObject().Movement.GetMoveEvent(ref direction, ref sequence, ref key);
                        if (hasMoveEvent)
                            _GameClient.Send(new MoveRequestPacket((byte)direction, (byte)sequence, key));
                    }
                    catch
                    {
                        // The player has not yet been loaded
                    }
                }
            }
        }

        private void checkMove()
        {
            Direction moveDirection = CursorDirection;
            float distanceFromCenterOfScreen = Vector2.Distance(new Vector2(_Input.Mouse.Position.X, _Input.Mouse.Position.Y), new Vector2(400, 300));
            if (distanceFromCenterOfScreen >= 200f)
                moveDirection |= Direction.Running;

            ((Mobile)_Entities.GetPlayerObject()).Move(moveDirection);
            
            
            if (_MovementFollowsMouse)
                _ContinuousMoveCheck = true;
        }

        private Direction mousePositionToDirection(Vector2 mousePosition)
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

        private void checkMove_OLD()
        {
            TileEngine.IMapObject iGroundTile = _TileEngine.MouseOverGroundTile;
            TileEngine.IMapObject iTopObject = _TileEngine.MouseOverObject;

            // We check the ground tile first.
            // If there is no objects under the mouse cursor, but there is a groundtile, move to the ground tile.
            // Same thing if the highest object under the mouse cursor is lower than the groundtile.
            if ((iGroundTile != null) && ((iTopObject == null) || (iTopObject.Z < iGroundTile.Z)))
            {
                ((Mobile)_Entities.GetPlayerObject()).Move(
                       (int)iGroundTile.Position.X,
                       (int)iGroundTile.Position.Y,
                       (int)iGroundTile.Z, -1);
                if (_MovementFollowsMouse)
                    _ContinuousMoveCheck = true;
            }
            else if (iTopObject != null)
            {
                Data.ItemData iItemData;

                if (iTopObject is MapObjectStatic)
                {
                    iItemData = Data.TileData.ItemData[iTopObject.ItemID - 0x4000];
                }
                else if (iTopObject is MapObjectItem)
                {
                    Item item = _Entities.GetObject<Item>(iTopObject.OwnerSerial, false);
                    iItemData = item.ItemData;
                }
                else
                    return;

                if (iItemData.Surface)
                {
                    // This is a walkable static or gameobject. Walk on it!
                    ((Mobile)_Entities.GetPlayerObject()).Move(
                           (int)iTopObject.Position.X,
                           (int)iTopObject.Position.Y,
                           (int)iTopObject.Z + iItemData.CalcHeight, -1);
                    if (_MovementFollowsMouse)
                        _ContinuousMoveCheck = true;
                }
                else
                {
                    // This is a static that is not a surface.
                    // We can't interact with it, so do nothing.
                    // (although what about chairs, aren't they interactable?)
                }
            }
        }

        private void checkLeftClick()
        {
            TileEngine.IMapObject iTopObject = _TileEngine.MouseOverObject;

            if (iTopObject != null)
            {
                if (iTopObject is MapObjectMobile)
                {
                    // Proper action: target this mobile.
                    LastTarget = iTopObject.OwnerSerial;
                    _GameClient.Send(new SingleClickPacket(LastTarget));
                    return;
                }
                if (iTopObject is MapObjectItem)
                {
                    // Proper action: pick it up if possible, as long as this is a press event.
                    Item item = _Entities.GetObject<Item>(iTopObject.OwnerSerial, false);
                    if (item.ItemData.Weight != 255)
                    {
                        GUI.GUIHelper.PickUpItem(item);
                        return;
                    }
                }
            }
                checkMove();
        }

        private void checkRightClick()
        {
            TileEngine.IMapObject iMapObject = _TileEngine.MouseOverObject;
            if ((iMapObject != null) && !(iMapObject is MapObjectStatic))
            {
                Entity iObject = _Entities.GetObject<Entity>(iMapObject.OwnerSerial, false);
                // default option is to simply 'use' this object, although this will doubtless be more complicated in the future.
                // Perhaps the use option is based on the type of object? Anyways, for right now, we only interact with gameobjects,
                // and we send a double-click to the server.
                if (iObject is Item)
                {
                    _GameClient.Send(new DoubleClickPacket(iObject.Serial));
                }
                else if (iObject is PlayerMobile)
                {
                    LastTarget = iObject.Serial;
                    if (iObject.Serial == _Entities.MySerial)
                    {
                        // this is my player.
                        // if mounted, dismount.
                        if (((Mobile)iObject).IsMounted)
                        {
                            _GameClient.Send(new DoubleClickPacket(iObject.Serial));
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
                        _GameClient.Send(new AttackRequestPacket(iObject.Serial));
                    }
                    else
                    {
                        _GameClient.Send(new RequestContextMenuPacket(iObject.Serial));
                    }
                }
                else
                {
                    // do nothing?
                }
            }
        }

        private void parseKeyboard(Input.KeyboardHandler keyboard)
        {
            // If we are targeting, cancel the target cursor if we hit escape.
            if (isTargeting)
                if (keyboard.IsKeyPressed(Keys.Escape))
                    mouseTargetingCancel();

            if (keyboard.IsKeyDown(Keys.I))
                _LightRadians += .01f;
            if (keyboard.IsKeyDown(Keys.K))
                _LightRadians -= .01f;

            _LightDirection.Z = -(float)Math.Cos(_LightRadians);
            _LightDirection.Y = (float)Math.Sin(_LightRadians);

            _TileEngine.SetLightDirection(_LightDirection);

            // Toggle for backpack container window.
            if (keyboard.IsKeyPressed(Keys.B) && (keyboard.IsKeyDown(Keys.LeftControl)))
            {
                Serial backpackSerial = ((PlayerMobile)_Entities.GetPlayerObject())
                    .equipment[(int)EquipLayer.Backpack].Serial;
                if (_GUI.Window("Container:" + backpackSerial) == null)
                    _GameClient.Send(new DoubleClickPacket(backpackSerial));
                else
                    _GUI.CloseWindow("Container:" + backpackSerial);
            }

            // Toggle for paperdoll window.
            if (keyboard.IsKeyPressed(Keys.C) && (keyboard.IsKeyDown(Keys.LeftControl)))
            {
                Serial serial = ((PlayerMobile)_Entities.GetPlayerObject())
                    .Serial;
                if (_GUI.Window("PaperDoll:" + serial) == null)
                    _GUI.PaperDoll_Open(_Entities.GetPlayerObject());
                else
                    _GUI.CloseWindow("PaperDoll:" + serial);
            }

            // Toggle for logout
            if (keyboard.IsKeyPressed(Keys.Q) && (keyboard.IsKeyDown(Keys.LeftControl)))
            {
                ((SceneManagement.ISceneService)Game.Services.GetService(typeof(SceneManagement.ISceneService))).CurrentScene = new SceneManagement.LoginScene(Game);
            }

            // toggle for warmode
            if (keyboard.IsKeyPressed(Keys.Tab))
            {
                if (WarMode)
                    _GameClient.Send(new RequestWarModePacket(false));
                else
                    _GameClient.Send(new RequestWarModePacket(true));
            }
        }

        // Maintain an accurate count of frames per second.
        private float _FPS; private float _Frames = 0; private float _ElapsedSeconds = 0;
        private bool updateFPS(GameTime gameTime)
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
        private string generateDebugMessage()
        {
            String debugMessage = "FPS: " + ((int)_FPS).ToString() + Environment.NewLine;
            if (InWorld)
            {
                debugMessage += "Objects on screen: " + _TileEngine.ObjectsRendered.ToString() + Environment.NewLine;
                debugMessage += "WarMode: " + WarMode.ToString() + Environment.NewLine;
                if (_TileEngine.MouseOverObject != null)
                {
                    debugMessage += "OBJECT: " + _TileEngine.MouseOverObject.ToString() + Environment.NewLine;
                    if (_TileEngine.MouseOverObject is MapObjectStatic)
                    {
                        debugMessage += "ArtID: " + ((TileEngine.MapObjectStatic)_TileEngine.MouseOverObject).ItemID;
                    }
                    else if (_TileEngine.MouseOverObject is MapObjectMobile)
                    {
                        Mobile iUnit = _Entities.GetObject<Mobile>(_TileEngine.MouseOverObject.OwnerSerial, false);
                        if (iUnit != null)
                            debugMessage += "Name: " + iUnit.Name + Environment.NewLine;
                        debugMessage +=
                            "AnimID: " + ((TileEngine.MapObjectMobile)_TileEngine.MouseOverObject).BodyID + Environment.NewLine +
                            "Serial: " + _TileEngine.MouseOverObject.OwnerSerial + Environment.NewLine +
                            "Hue: " + ((TileEngine.MapObjectMobile)_TileEngine.MouseOverObject).Hue;
                    }
                    else if (_TileEngine is MapObjectItem)
                    {
                        debugMessage +=
                            "ArtID: " + ((TileEngine.MapObjectItem)_TileEngine.MouseOverObject).ItemID + Environment.NewLine +
                            "Serial: " + _TileEngine.MouseOverObject.OwnerSerial;
                    }
                    debugMessage += " Z: " + _TileEngine.MouseOverObject.Z;
                }
                else
                {
                    debugMessage += "OVER: " + "null";
                }
                if (_TileEngine.MouseOverGroundTile != null)
                {
                    debugMessage += Environment.NewLine + "GROUND: " + _TileEngine.MouseOverGroundTile.Position.ToString();
                }
                else
                {
                    debugMessage += Environment.NewLine + "GROUND: null";
                }
            }
            return debugMessage;
        }


        public void MouseTargeting(int nCursorID, int nTargetingType)
        {
            _TargettingType = nTargetingType;
            // Set the GUI's cursor to a targetting cursor.
            _GUI.TargettingCursor = true;
            // Stop continuous movement.
            _ContinuousMoveCheck = false;
        }

        private void mouseTargetingCancel()
        {
            // Send the cancel target message back to the server.
            _GameClient.Send(new TargetCancelPacket());
            // Clear our target cursor.
            _TargettingType = -1;
            _GUI.TargettingCursor = false;
        }

        private void mouseTargetingEventXYZ(TileEngine.IMapObject selectedObject)
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
            _GameClient.Send(new TargetXYZPacket((ushort)selectedObject.Position.X, (ushort)selectedObject.Position.Y, (sbyte)selectedObject.Z, (ushort)modelNumber));
            // ... and then clear our target cursor.
            _TargettingType = -1;
            _GUI.TargettingCursor = false;
        }

        private void MouseTargetingEventObject(TileEngine.IMapObject selectedObject)
        {
            // If we are passed a null object, keep targeting.
            if (selectedObject == null)
                return;
            Serial serial = selectedObject.OwnerSerial;
            // Send the targetting event back to the server
            _GameClient.Send(new TargetObjectPacket(serial));
            // Clear our target cursor.
            _TargettingType = -1;
            _GUI.TargettingCursor = false;
        }
    }
}
