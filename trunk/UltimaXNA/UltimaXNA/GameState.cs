#region File Description & Usings
//-----------------------------------------------------------------------------
// GameState.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Data;
using UltimaXNA.Client;
using UltimaXNA.Network.Packets.Client;
#endregion

namespace UltimaXNA
{
    public interface IGameState
    {
        bool InWorld { get; set; }
        bool EngineRunning { get; set; }
        void MouseTargeting(int nCursorID, int nTargetingType);
    }

    public class GameState : GameComponent, IGameState
    {
        Input.IInputHandler _Input;
        TileEngine.ITileEngine _TileEngine;
        GameObjects.IGameObjects _GameObjects;
        TileEngine.IWorld _World;
        GUI.IGUI _GUI;
        Client.IUltimaClient _GameClient;

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
            _Input = (Input.IInputHandler)Game.Services.GetService(typeof(Input.IInputHandler));
            _TileEngine = (TileEngine.ITileEngine)Game.Services.GetService(typeof(TileEngine.ITileEngine));
            _GameObjects = (GameObjects.IGameObjects)Game.Services.GetService(typeof(GameObjects.IGameObjects));
            _World = (TileEngine.IWorld)Game.Services.GetService(typeof(TileEngine.IWorld));
            _GUI = (GUI.IGUI)Game.Services.GetService(typeof(GUI.IGUI));
            _GameClient = Game.Services.GetService(typeof(Client.IUltimaClient)) as Client.IUltimaClient;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Do we need to quit?
            if (this.EngineRunning == false)
            {
                _GameClient.Disconnect();
                Game.Exit();
                return;
            }

            if (!InWorld)
            {
                // Not in the world yet
                switch (_GameClient.Status)
                {
                    case UltimaClientStatus.Unconnected:
                        _GUI.LoadLoginGUI();
                        _GameObjects.Reset();
                        break;
                    case UltimaClientStatus.Error_Undefined:
                        _GameClient.Disconnect();
                        break;
                    case UltimaClientStatus.WorldServer_InWorld:
                        if (this.InWorld == false)
                        {
                            this.InWorld = true;
                            _GUI.LoadInWorldGUI();
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
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
                                if (iMapObject == null)
                                    iMapObject = _TileEngine.MouseOverGroundTile;
                                if (iMapObject != null)
                                    mouseTargetingEventXYZ(iMapObject);
                                else
                                    mouseTargetingCancel();
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
                                    TileEngine.IMapObject groundObject = _TileEngine.MouseOverGroundTile;
                                    TileEngine.IMapObject mouseoverObject = _TileEngine.MouseOverObject;
                                    if (mouseoverObject != null)
                                    {
                                        if (mouseoverObject.Type == UltimaXNA.TileEngine.MapObjectTypes.MobileTile)
                                        {
                                            // special case, attempt to give this item.
                                            return;
                                        }
                                        else
                                        {
                                            x = (int)mouseoverObject.Position.X;
                                            y = (int)mouseoverObject.Position.Y;
                                            z = mouseoverObject.Z;
                                            if (mouseoverObject.Type == UltimaXNA.TileEngine.MapObjectTypes.StaticTile)
                                            {
                                                ItemData data = Data.TileData.ItemData[mouseoverObject.ID - 0x4000];
                                                z += data.Height;
                                            }
                                            else if (mouseoverObject.Type == UltimaXNA.TileEngine.MapObjectTypes.GameObjectTile)
                                            {
                                                z += Data.TileData.ItemData[mouseoverObject.ID].Height;
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
                                    if (((GameObjects.GameObject)GUI.GUIHelper.MouseHoldingItem).Item_ContainedWithinSerial != 0)
                                    {
                                        // We must manually remove the item from the container, as RunUO does not do this for us.
                                        GameObjects.GameObject iContainer = _GameObjects.GetObject(
                                            ((GameObjects.GameObject)GUI.GUIHelper.MouseHoldingItem).Item_ContainedWithinSerial,
                                            UltimaXNA.GameObjects.ObjectType.GameObject) as GameObjects.GameObject;
                                        iContainer.ContainerObject.RemoveItem(GUI.GUIHelper.MouseHoldingItem.Serial);
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
                        checkLeftClick();
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
                        bool hasMoveEvent = _GameObjects.GetPlayerObject().Movement.GetMoveEvent(ref direction, ref sequence, ref key);
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
            TileEngine.IMapObject iGroundTile = _TileEngine.MouseOverGroundTile;
            TileEngine.IMapObject iTopObject = _TileEngine.MouseOverObject;

            // We check the ground tile first.
            // If there is no objects under the mouse cursor, but there is a groundtile, move to the ground tile.
            // Same thing if the highest object under the mouse cursor is lower than the groundtile.
            if ((iGroundTile != null) && ((iTopObject == null) || (iTopObject.Z < iGroundTile.Z)))
            {
                ((GameObjects.Unit)_GameObjects.GetPlayerObject()).Move(
                       (int)iGroundTile.Position.X,
                       (int)iGroundTile.Position.Y,
                       (int)iGroundTile.Z);
                if (_MovementFollowsMouse)
                    _ContinuousMoveCheck = true;
            }
            else if (iTopObject != null)
            {
                Data.ItemData iItemData;
                if (iTopObject.Type == UltimaXNA.TileEngine.MapObjectTypes.StaticTile)
                    iItemData = Data.TileData.ItemData[iTopObject.ID - 0x4000];
                else if (iTopObject.Type == UltimaXNA.TileEngine.MapObjectTypes.GameObjectTile)
                {
                    GameObjects.GameObject iObject =
                        ((iTopObject.Type == TileEngine.MapObjectTypes.GameObjectTile) ?
                        _GameObjects.GetObject(iTopObject.OwnerSerial, UltimaXNA.GameObjects.ObjectType.GameObject) as GameObjects.GameObject :
                        null);
                    iItemData = iObject.ItemData;
                }
                else
                    return;

                if (iItemData.Surface)
                {
                    // This is a walkable static or gameobject. Walk on it!
                    ((GameObjects.Unit)_GameObjects.GetPlayerObject()).Move(
                           (int)iTopObject.Position.X,
                           (int)iTopObject.Position.Y,
                           (int)iTopObject.Z + iItemData.CalcHeight);
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
            // Issue 15 - Mouse left clicks on the wrong topmost object - http://code.google.com/p/ultimaxna/issues/detail?id=15 - Smjert
            // Issue 17 - Player is attempting to 'walk onto' objects? - http://code.google.com/p/ultimaxna/issues/detail?id=17 - ZDW
            // Issue 18 - Player will not walk to a static unless there is a ground tile active - http://code.google.com/p/ultimaxna/issues/detail?id=18 - ZDW

            TileEngine.IMapObject iTopObject = _TileEngine.MouseOverObject;

            if (iTopObject != null)
            {
                if (iTopObject.Type == TileEngine.MapObjectTypes.MobileTile)
                {
                    // Target this mobile.
                    return;
                }
                if (iTopObject.Type == TileEngine.MapObjectTypes.GameObjectTile)
                {
                    // This is a GameObject. Pick it up if possible, as long as this is a press event.

                    GameObjects.GameObject item = _GameObjects.GetObject(iTopObject.OwnerSerial, GameObjects.ObjectType.GameObject) as GameObjects.GameObject;
                    if (item.ItemData.Weight != 255)
                    {
                        // _GameClient.Send(new PickupItemPacket(iObject.Serial, (short)iObject.Item_StackCount));
                        GUI.GUIHelper.MouseHoldingItem = item;
                        return;
                    }
                }
            }

            checkMove();
        }

        private void checkRightClick()
        {
            TileEngine.IMapObject iMapObject = _TileEngine.MouseOverObject;
            if ((iMapObject != null) && (iMapObject.Type != UltimaXNA.TileEngine.MapObjectTypes.StaticTile))
            {
                GameObjects.BaseObject iObject = _GameObjects.GetObject(iMapObject.OwnerSerial, UltimaXNA.GameObjects.ObjectType.Object);
                // default option is to simply 'use' this object, although this will doubtless be more complicated in the future.
                // Perhaps the use option is based on the type of object? Anyways, for right now, we only interact with gameobjects,
                // and we send a double-click to the server.
                switch (iObject.ObjectType)
                {
                    case UltimaXNA.GameObjects.ObjectType.GameObject:
                        _GameClient.Send(new DoubleClickPacket(iObject.Serial));
                        break;
                    case UltimaXNA.GameObjects.ObjectType.Unit:
                        // and we also 'use' this unit.
                        // _GameClient.Send(new DoubleClickPacket(iObject.Serial));
                        // We request a context sensitive menu...
                        _GameClient.Send(new RequestContextMenuPacket(iObject.Serial));
                        break;
                    case UltimaXNA.GameObjects.ObjectType.Player:
                        if (iObject.Serial == _GameObjects.MySerial)
                        {
                            // this is my player.
                            // if mounted, dismount.
                            if (((GameObjects.Unit)iObject).IsMounted)
                            {
                                _GameClient.Send(new DoubleClickPacket(iObject.Serial));
                            }
                        }
                        // else other interaction?
                        break;
                    default:
                        // do nothing?
                        break;

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
                Serial backpackSerial = ((GameObjects.Player)_GameObjects.GetPlayerObject())
                    .Equipment[(int)GameObjects.EquipLayer.Backpack].Serial;
                if (_GUI.Window("Container:" + backpackSerial) == null)
                    _GameClient.Send(new DoubleClickPacket(backpackSerial));
                else
                    _GUI.CloseWindow("Container:" + backpackSerial);
            }

            // Toggle for paperdoll window.
            if (keyboard.IsKeyPressed(Keys.C) && (keyboard.IsKeyDown(Keys.LeftControl)))
            {
                Serial serial = ((GameObjects.Player)_GameObjects.GetPlayerObject())
                    .Serial;
                if (_GUI.Window("PaperDoll:" + serial) == null)
                    _GUI.PaperDoll_Open(_GameObjects.GetPlayerObject());
                else
                    _GUI.CloseWindow("PaperDoll:" + serial);
            }

            // Toggle for logout
            if (keyboard.IsKeyPressed(Keys.Q) && (keyboard.IsKeyDown(Keys.LeftControl)))
            {
                _GameClient.Disconnect();
                InWorld = false;
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
            String debugMessage = "FPS: " + _FPS.ToString() + Environment.NewLine;
            debugMessage += "Objects on screen: " + _TileEngine.ObjectsRendered.ToString() + Environment.NewLine;
            if (_TileEngine.MouseOverObject != null)
            {
                debugMessage += "OBJECT: " + _TileEngine.MouseOverObject.ToString() + Environment.NewLine;
                if (_TileEngine.MouseOverObject.Type == TileEngine.MapObjectTypes.StaticTile)
                {
                    debugMessage += "ArtID: " + ((TileEngine.StaticItem)_TileEngine.MouseOverObject).ID;
                }
                else if (_TileEngine.MouseOverObject.Type == TileEngine.MapObjectTypes.MobileTile)
                {
                    GameObjects.Unit iUnit = _GameObjects.GetObject(_TileEngine.MouseOverObject.OwnerSerial,
                        UltimaXNA.GameObjects.ObjectType.Unit) as GameObjects.Unit;
                    if (iUnit != null)
                        debugMessage += "Name: " + iUnit.Name + Environment.NewLine;
                    debugMessage +=
                        "AnimID: " + ((TileEngine.MobileTile)_TileEngine.MouseOverObject).ID + Environment.NewLine +
                        "Serial: " + _TileEngine.MouseOverObject.OwnerSerial + Environment.NewLine +
                        "Hue: " + ((TileEngine.MobileTile)_TileEngine.MouseOverObject).Hue;
                }
                else if (_TileEngine.MouseOverObject.Type == TileEngine.MapObjectTypes.GameObjectTile)
                {
                    debugMessage +=
                        "ArtID: " + ((TileEngine.GameObjectTile)_TileEngine.MouseOverObject).ID + Environment.NewLine +
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
            switch (selectedObject.Type)
            {
                case UltimaXNA.TileEngine.MapObjectTypes.GroundTile:
                    modelNumber = 0;
                    break;
                case UltimaXNA.TileEngine.MapObjectTypes.StaticTile:
                    modelNumber = selectedObject.ID;
                    break;
                case UltimaXNA.TileEngine.MapObjectTypes.GameObjectTile:
                    modelNumber = 0;
                    break;
                case UltimaXNA.TileEngine.MapObjectTypes.MobileTile:
                    modelNumber = 0;
                    break;
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
            Serial serial = 0;
            switch (selectedObject.Type)
            {
                case UltimaXNA.TileEngine.MapObjectTypes.GroundTile:
                case UltimaXNA.TileEngine.MapObjectTypes.StaticTile:
                    serial = 0;
                    break;
                case UltimaXNA.TileEngine.MapObjectTypes.GameObjectTile:
                case UltimaXNA.TileEngine.MapObjectTypes.MobileTile:
                    serial = selectedObject.OwnerSerial;
                    break;
            }
            // Send the targetting event back to the server
            _GameClient.Send(new TargetObjectPacket(serial));
            // Clear our target cursor.
            _TargettingType = -1;
            _GUI.TargettingCursor = false;
        }
    }
}
