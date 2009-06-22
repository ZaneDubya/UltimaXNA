#region File Description & Usings
//-----------------------------------------------------------------------------
// GameState.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.DataLocal;
#endregion

namespace UltimaXNA
{
    public interface IGameState
    {
        bool InWorld { get; set; }
        bool EngineRunning { get; set; }
    }

    public class GameState : GameComponent, IGameState
    {
        #region ServiceObjects
        Input.IInputHandler mInputService;
        TileEngine.ITileEngine mTileEngineService;
        GameObjects.IGameObjects mGameObjectsService;
        TileEngine.IWorld mWorldService;
        GUI.IGUI mGUIService;
        Network.IGameClient mGameClientService;
        #endregion

        // The debug message is generated from m_DebugMessage()
        public string DebugMessage { get { return mGenerateDebugMessage(); } }

        // added for future interface option, allowing both continuous mouse movement and discrete clicks -BERT
        bool movementFollowsMouse = true;
        private bool mContinuousMoveCheck = false;

        // InWorld allows us to tell when our character object has been loaded in the world.
        private bool mInWorld;
        public bool InWorld
        { 
            get { return mInWorld; }
            set
            {
                if (value == true)
                    mGUIService.LoadInWorldGUI();
                mInWorld = value;
            }
        }

        // Set EngineRunning to false to cause the engine to immediately exit.
        public bool EngineRunning { get; set; }

        // These variables move the light source around...
        private Vector3 mLightDirection = new Vector3(0f, 0f, 1f);
        private double mLightRadians = -0.5d;



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
            mInputService = (Input.IInputHandler)Game.Services.GetService(typeof(Input.IInputHandler));
            mTileEngineService = (TileEngine.ITileEngine)Game.Services.GetService(typeof(TileEngine.ITileEngine));
            mGameObjectsService = (GameObjects.IGameObjects)Game.Services.GetService(typeof(GameObjects.IGameObjects));
            mWorldService = (TileEngine.IWorld)Game.Services.GetService(typeof(TileEngine.IWorld));
            mGUIService = (GUI.IGUI)Game.Services.GetService(typeof(GUI.IGUI));
            mGameClientService = (Network.IGameClient)Game.Services.GetService(typeof(Network.IGameClient));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Do we need to quit?
            if (this.EngineRunning == false)
            {
                mGameClientService.Disconnect();
                Game.Exit();
                return;
            }

            // Get a pick type for the cursor.
            if (mGUIService.IsMouseOverGUI(mInputService.Mouse.Position))
            {
                mTileEngineService.PickType = UltimaXNA.TileEngine.PickTypes.PickNothing;
            }
            else
            {
                // Changed to leverage movementFollowsMouse interface option -BERT
                if ( movementFollowsMouse ? mInputService.Mouse.Buttons[0].IsDown : mInputService.Mouse.Buttons[0].Press )
                {
                    mTileEngineService.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects | TileEngine.PickTypes.PickGroundTiles;
                }
                else if (mInputService.Mouse.Buttons[1].Press)
                {
                    mTileEngineService.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects;
                }
                else
                {
                    mTileEngineService.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects;
                }
            }

            m_ParseKeyboard(mInputService.Keyboard);

            m_UpdateFPS(gameTime);
        }

        public void UpdateAfter()
        {
            // If the left mouse button has been released, and movementFollowsMouse = true, reset mContinuousMovement.
            if (mInputService.Mouse.Buttons[0].Release)
                mContinuousMoveCheck = false;

            // Changed to leverage movementFollowsMouse interface option -BERT
            if (mContinuousMoveCheck)
            {
                if (mInputService.Mouse.Buttons[0].IsDown)
                {
                    m_CheckMovement(false);
                }
            }

            // If the left mouse button has been released, we check to see if we were holding an item in the cursor.
            // If we dropped the item into another slot, then the GUI will have already removed the MouseHoldingItem,
            // and we will never reach this routine. If the MouseHoldingItem is still here, we need to take care of it.
            #region DropItemFromMouse
            if (mInputService.Mouse.Buttons[0].Release)
            {
                if (GUI.GUIHelper.MouseHoldingItem != null)
                {
                    if (mGUIService.IsMouseOverGUI(mInputService.Mouse.Position))
                    {
                        // The mouse is over the GUI. Just get rid of the item, as it hasn't been moved.
                        GUI.GUIHelper.MouseHoldingItem = null;
                    }
                    else
                    {
                        // We dropped the icon in the world. This means we are trying to drop the item
                        // into the world. Let's do it!
                        mGameClientService.Send_PickUpItem(
                            GUI.GUIHelper.MouseHoldingItem.GUID,
                            ((GameObjects.GameObject)GUI.GUIHelper.MouseHoldingItem).Item_StackCount);
                        mGameClientService.Send_DropItem(
                            GUI.GUIHelper.MouseHoldingItem.GUID,
                            mGameObjectsService.GetPlayerObject().Movement.TileX,
                            mGameObjectsService.GetPlayerObject().Movement.TileY,
                            0,
                            -1);
                        if (((GameObjects.GameObject)GUI.GUIHelper.MouseHoldingItem).Item_ContainedWithinGUID != 0)
                        {
                            // We must manually remove the item from the container, as RunUO does not do this for us.
                            GameObjects.GameObject iContainer = mGameObjectsService.GetObject(
                                ((GameObjects.GameObject)GUI.GUIHelper.MouseHoldingItem).Item_ContainedWithinGUID, 
                                UltimaXNA.GameObjects.ObjectType.GameObject) as GameObjects.GameObject;
                            iContainer.ContainerObject.RemoveItem(GUI.GUIHelper.MouseHoldingItem.GUID);
                        }

                        GUI.GUIHelper.MouseHoldingItem = null;

                    }
                    
                }
            }
            #endregion

            // Check if the left mouse button has been pressed. We will either walk to the object under the cursor
            // or pick it up, depending on what kind of object we are looking at.
            if (mInputService.Mouse.Buttons[0].Press)
            {
                m_CheckMovement(true);
            }

            if (mInputService.Mouse.Buttons[1].Press)
            {
                // Right button pressed ... activate this object.
                TileEngine.IMapObject iMapObject = mTileEngineService.MouseOverObject;
                if ((iMapObject != null) && (iMapObject.Type != UltimaXNA.TileEngine.MapObjectTypes.StaticTile))
                {
                    GameObjects.BaseObject iObject = mGameObjectsService.GetObject(iMapObject.OwnerGUID, UltimaXNA.GameObjects.ObjectType.Object);
                    // default option is to simply 'use' this object, although this will doubtless be more complicated in the future.
                    // Perhaps the use option is based on the type of object? Anyways, for right now, we only interact with gameobjects,
                    // and we send a double-click to the server.
                    switch (iObject.ObjectType)
                    {
                        case UltimaXNA.GameObjects.ObjectType.GameObject:
                            mGameClientService.Send_UseRequest(iObject.GUID);
                            break;
                        case UltimaXNA.GameObjects.ObjectType.Unit:
                            // and we also 'use' this unit.
                            mGameClientService.Send_UseRequest(iObject.GUID);
                            // We request a context sensitive menu...
                            mGameClientService.Send_RequestContextMenu(iObject.GUID);
                            break;
                        case UltimaXNA.GameObjects.ObjectType.Player:
                            if (iObject.GUID == mGameObjectsService.MyGUID)
                            {
                                // if mounted, dismount.
                                if (((GameObjects.Unit)iObject).IsMounted)
                                {
                                    mGameClientService.Send_UseRequest(iObject.GUID);
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


            // Check for a move event from the player ...
            try
            {
                int iDirection = 0, iSequence = 0, iKey = 0;
                bool iMoveEvent = mGameObjectsService.GetPlayerObject().Movement.GetMoveEvent(ref iDirection, ref iSequence, ref iKey);
                if (iMoveEvent)
                {
                    mGameClientService.Send_MoveRequest(iDirection, iSequence, iKey);
                }
            }
            catch
            {
                // The player has not yet been loaded
            }

            #region oldmovecode
            /*
            // Changed to leverage movementFollowsMouse interface option -BERT
            if ( movementFollowsMouse ? mInputService.Mouse.Buttons[0].IsDown : mInputService.Mouse.Buttons[0].Press )
            {
				// Issue 15 - Mouse left clicks on the wrong topmost object - http://code.google.com/p/ultimaxna/issues/detail?id=15 - Smjert
                // Left button pressed ... move to the tile under the mouse cursor, if there is one...
				TileEngine.IMapObject iTopMostObject = mTileEngineService.MouseOverGroundTile;

				if ( iTopMostObject != null )
                {
                    int offset = 0;
					if ( mTileEngineService.MouseOverObject != null && mTileEngineService.MouseOverObject.Z >= iTopMostObject.Z )
					{
						iTopMostObject = mTileEngineService.MouseOverObject;
						if(iTopMostObject.Type == UltimaXNA.TileEngine.MapObjectTypes.StaticTile)
							offset = TileData.ItemData[iTopMostObject.ID - 0x4000].CalcHeight;
						else if(iTopMostObject.Type == UltimaXNA.TileEngine.MapObjectTypes.GameObjectTile)
							offset = TileData.ItemData[iTopMostObject.ID].CalcHeight;
					}

                    ((GameObjects.Unit)mGameObjectsService.GetPlayerObject()).Move(
						(int)iTopMostObject.Position.X,
						(int)iTopMostObject.Position.Y,
						(int)iTopMostObject.Z + offset);
                }
				// Issue 15 - End
            }*/
            #endregion
        }

        private void m_CheckMovement(bool nPressEvent)
        {
            // Issue 15 - Mouse left clicks on the wrong topmost object - http://code.google.com/p/ultimaxna/issues/detail?id=15 - Smjert
            // Issue 17 - Player is attempting to 'walk onto' objects? - http://code.google.com/p/ultimaxna/issues/detail?id=17 - ZDW
            // Issue 18 - Player will not walk to a static unless there is a ground tile active - http://code.google.com/p/ultimaxna/issues/detail?id=18 - ZDW
            int iZOffset = 0;
            // We check the ground tile first.
            // If there is no objects under the mouse cursor, but there is a groundtile, move to the ground tile.
            // Same thing if the highest object under the mouse cursor is lower than the groundtile.
            if (
                (mTileEngineService.MouseOverGroundTile != null) &&
                ((mTileEngineService.MouseOverObject == null) ||
                (mTileEngineService.MouseOverObject.Z < mTileEngineService.MouseOverGroundTile.Z))
                )
            {
                TileEngine.IMapObject iGroundTile = mTileEngineService.MouseOverGroundTile;
                if (iGroundTile != null)
                {
                    ((GameObjects.Unit)mGameObjectsService.GetPlayerObject()).Move(
                           (int)iGroundTile.Position.X,
                           (int)iGroundTile.Position.Y,
                           (int)iGroundTile.Z);
                    if (movementFollowsMouse)
                        mContinuousMoveCheck = true;
                }
            }
            else if (mTileEngineService.MouseOverObject != null)
            {
                // Local copy of the top most object.
                TileEngine.IMapObject iTopMostObject = mTileEngineService.MouseOverObject;

                // We react to mobiles differently than we do objects/statics
                if (iTopMostObject.Type == TileEngine.MapObjectTypes.MobileTile)
                {

                }
                else
                {
                    // Retreive the GameObject that owns the object under the cursor. Note that if this is a
                    // static tile, iObject will equal null.
                    GameObjects.GameObject iObject =
                        ((iTopMostObject.Type == TileEngine.MapObjectTypes.GameObjectTile) ?
                        mGameObjectsService.GetObject(iTopMostObject.OwnerGUID, UltimaXNA.GameObjects.ObjectType.GameObject) as GameObjects.GameObject :
                        null);

                    // Retreive the ItemData for this object.
                    DataLocal.ItemData iItemData =
                        ((mTileEngineService.MouseOverObject.OwnerGUID != -1) ?
                        iObject.ItemData :
                        DataLocal.TileData.ItemData[iTopMostObject.ID - 0x4000]);

                    if (iItemData.Surface)
                    {
                        // This is a walkable static or gameobject. Walk on it!

                        if (iTopMostObject.Type == UltimaXNA.TileEngine.MapObjectTypes.StaticTile)
                            iZOffset = TileData.ItemData[iTopMostObject.ID - 0x4000].CalcHeight;
                        else if (iTopMostObject.Type == UltimaXNA.TileEngine.MapObjectTypes.GameObjectTile)
                            iZOffset = TileData.ItemData[iTopMostObject.ID].CalcHeight;

                        ((GameObjects.Unit)mGameObjectsService.GetPlayerObject()).Move(
                               (int)iTopMostObject.Position.X,
                               (int)iTopMostObject.Position.Y,
                               (int)iTopMostObject.Z + iZOffset);
                        if (movementFollowsMouse)
                            mContinuousMoveCheck = true;
                    }
                    else if (iObject != null)
                    {
                        // This is a GameObject. Pick it up if possible, as long as this is a press event.
                        if (nPressEvent)
                        {
                            mGameClientService.Send_PickUpItem(iObject.GUID, iObject.Item_StackCount);
                            // Set this item to be the MouseHoldingItem.
                            GUI.GUIHelper.MouseHoldingItem = iObject;
                        }
                    }
                    else
                    {
                        // This is a static that is not a surface.
                        // We can't interact with it, so do nothing.
                        // (although what about chairs, aren't they interactable?)
                    }
                }
            }
        }

        private void m_ParseKeyboard(Input.KeyboardHandler nKeyboard)
        {
            if (InWorld)
            {
                if (nKeyboard.IsKeyDown(Keys.I))
                    mLightRadians += .01f;
                if (nKeyboard.IsKeyDown(Keys.K))
                    mLightRadians -= .01f;

                mLightDirection.Z = -(float)Math.Cos(mLightRadians);
                mLightDirection.Y = (float)Math.Sin(mLightRadians);

                mTileEngineService.SetLightDirection(mLightDirection);

                // Toggle for backpack container window.
                if (nKeyboard.IsKeyPressed(Keys.B) && (nKeyboard.IsKeyDown(Keys.LeftControl)))
                {
                    int iBackpackGUID = ((GameObjects.Player)mGameObjectsService.GetPlayerObject())
                        .Equipment[(int)GameObjects.EquipLayer.Backpack].GUID;
                    if (mGUIService.Window("Container:" + iBackpackGUID) == null)
                        mGameClientService.Send_UseRequest(iBackpackGUID);
                    else
                        mGUIService.CloseWindow("Container:" + iBackpackGUID);
                }

                // DEBUG MOVEMENT!!! Quickly move around the world without sending a message to the server.
                // Note that if you attempt to move around normally after using this, the server will catch on
                // and will eventually reject your movement.
                #region DEBUG_KeyboardMovement
                    GameObjects.Movement iMovement = mGameObjectsService.GetPlayerObject().Movement;
                    if (nKeyboard.IsKeyDown(Keys.W))
                        iMovement.SetPositionInstant(iMovement.TileX - 1, iMovement.TileY - 1, 0);
                    if (nKeyboard.IsKeyDown(Keys.A))
                        iMovement.SetPositionInstant(iMovement.TileX - 1, iMovement.TileY + 1, 0);
                    if (nKeyboard.IsKeyDown(Keys.S))
                        iMovement.SetPositionInstant(iMovement.TileX + 1, iMovement.TileY + 1, 0);
                    if (nKeyboard.IsKeyDown(Keys.D))
                        iMovement.SetPositionInstant(iMovement.TileX + 1, iMovement.TileY - 1, 0);
                #endregion

            }
        }

        // Maintain an accurate count of frames per second.
        private float FPS; private float mFrames = 0; private float mElapsedSeconds = 0;
        private bool m_UpdateFPS(GameTime gameTime)
        {
            mFrames++;
            mElapsedSeconds += (float)gameTime.ElapsedRealTime.TotalSeconds;
            if (mElapsedSeconds >= 1)
            {
                FPS = mFrames / mElapsedSeconds;
                mElapsedSeconds -= 1;
                mFrames = 0;
                return true;
            }
            return false;
        }

        // Debug message - I put a lot of crap in here to test values.
        // Feel free to add or remove variables.
        private string mGenerateDebugMessage()
        {
            String iDebug = "FPS: " + FPS.ToString() + Environment.NewLine;
            iDebug += "Objects on screen: " + mTileEngineService.ObjectsRendered.ToString() + Environment.NewLine;
            if (mTileEngineService.MouseOverObject != null)
            {
                iDebug += "OBJECT: " + mTileEngineService.MouseOverObject.ToString() + Environment.NewLine;
                if (mTileEngineService.MouseOverObject.Type == TileEngine.MapObjectTypes.StaticTile)
                {
                    iDebug += "ArtID: " + ((TileEngine.StaticItem)mTileEngineService.MouseOverObject).ID;
                }
                else if (mTileEngineService.MouseOverObject.Type == TileEngine.MapObjectTypes.MobileTile)
                {
                    GameObjects.Unit iUnit = mGameObjectsService.GetObject(mTileEngineService.MouseOverObject.OwnerGUID, 
                        UltimaXNA.GameObjects.ObjectType.Unit) as GameObjects.Unit;
                    if (iUnit != null)
                        iDebug += "Name: " + iUnit.Name + Environment.NewLine;
                    iDebug +=
                        "AnimID: " + ((TileEngine.MobileTile)mTileEngineService.MouseOverObject).ID + Environment.NewLine +
                        "GUID: " + mTileEngineService.MouseOverObject.OwnerGUID + Environment.NewLine +
                        "Hue: " + ((TileEngine.MobileTile)mTileEngineService.MouseOverObject).Hue;
                }
                else if (mTileEngineService.MouseOverObject.Type == TileEngine.MapObjectTypes.GameObjectTile)
                {
                    iDebug +=
                        "ArtID: " + ((TileEngine.GameObjectTile)mTileEngineService.MouseOverObject).ID + Environment.NewLine +
                        "GUID: " + mTileEngineService.MouseOverObject.OwnerGUID;
                }
                iDebug += " Z: " + mTileEngineService.MouseOverObject.Z;
            }
            else
            {
                iDebug += "OVER: " + "null";
            }
            if (mTileEngineService.MouseOverGroundTile != null)
            {
                iDebug += Environment.NewLine + "GROUND: " + mTileEngineService.MouseOverGroundTile.Position.ToString();
            }
            else
            {
                iDebug += Environment.NewLine + "GROUND: null";
            }
            return iDebug;
        }
    }
}
