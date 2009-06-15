#region File Description & Usings
//-----------------------------------------------------------------------------
// GameState.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
        // Input service
        Input.IInputHandler m_InputService;
        TileEngine.ITileEngine m_TileEngineService;
        GameObjects.IGameObjects m_GameObjectsService;
        TileEngine.IWorld m_WorldService;
        GUI.IGUI m_GUIService;
        Network.IGameClient m_GameClientService;

        // Debug message
        public string DebugMessage { get { return m_DebugMessage(); } }
        private bool m_InWorld;
        public bool InWorld
        {
            get
            {
                return m_InWorld;
            }
            set
            {
                if (value == true)
                {
                    m_GUIService.LoadInWorldGUI();
                }
                m_InWorld = value;
            }
        }
        public bool EngineRunning { get; set; }

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
            m_InputService = (Input.IInputHandler)Game.Services.GetService(typeof(Input.IInputHandler));
            m_TileEngineService = (TileEngine.ITileEngine)Game.Services.GetService(typeof(TileEngine.ITileEngine));
            m_GameObjectsService = (GameObjects.IGameObjects)Game.Services.GetService(typeof(GameObjects.IGameObjects));
            m_WorldService = (TileEngine.IWorld)Game.Services.GetService(typeof(TileEngine.IWorld));
            m_GUIService = (GUI.IGUI)Game.Services.GetService(typeof(GUI.IGUI));
            m_GameClientService = (Network.IGameClient)Game.Services.GetService(typeof(Network.IGameClient));
        }

        public void LoadContent()
        {
            // Don't do anything...
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Do we need to quit?
            if (this.EngineRunning == false)
            {
                m_GameClientService.Disconnect();
                Game.Exit();
                return;
            }

            // Get a pick type for the cursor.
            if (m_GUIService.IsMouseOverGUI(m_InputService.Mouse.Position))
            {
                m_TileEngineService.PickType = UltimaXNA.TileEngine.PickTypes.PickNothing;
            }
            else
            {
                if (m_InputService.Mouse.Buttons[0].Press)
                {
                    m_TileEngineService.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects | TileEngine.PickTypes.PickGroundTiles;
                }
                else if (m_InputService.Mouse.Buttons[1].Press)
                {
                    m_TileEngineService.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects;
                }
                else
                {
                    m_TileEngineService.PickType = TileEngine.PickTypes.PickStatics | TileEngine.PickTypes.PickObjects;
                }
            }

            mParseKeyboard(m_InputService.Keyboard);

            mUpdateFPS(gameTime);
        }

        public void UpdateAfter()
        {
            if (m_InputService.Mouse.Buttons[0].Press)
            {
                // Left button pressed ... move to the tile under the mouse cursor, if there is one...
                TileEngine.IMapObject iGroundTile = m_TileEngineService.MouseOverGroundTile;
                if (iGroundTile != null)
                {
                    ((GameObjects.Unit)m_GameObjectsService.GetObject(m_GameObjectsService.MyGUID)).Move(
                        (int)iGroundTile.Position.X,
                        (int)iGroundTile.Position.Y,
                        (int)iGroundTile.Z);
                }
            }
            else if (m_InputService.Mouse.Buttons[1].Press)
            {
                // Right button pressed ... activate this object.
                TileEngine.IMapObject iMapObject = m_TileEngineService.MouseOverObject;
                if ((iMapObject != null) && (iMapObject.Type != UltimaXNA.TileEngine.MapObjectTypes.StaticTile))
                {
                    GameObjects.BaseObject iObject = m_GameObjectsService.GetObject(iMapObject.OwnerGUID);
                    // default option is to simply 'use' this object, although this will doubtless be more complicated in the future.
                    // Perhaps the use option is based on the type of object? Anyways, for right now, we only interact with gameobjects,
                    // and we send a double-click to the server.
                    switch (iObject.ObjectType)
                    {
                        case UltimaXNA.GameObjects.ObjectType.GameObject:
                            m_GameClientService.Send_UseRequest(iObject.GUID);
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
                bool iMoveEvent = m_GameObjectsService.GetObject(m_GameObjectsService.MyGUID).Movement.GetMoveEvent(ref iDirection, ref iSequence, ref iKey);
                if (iMoveEvent)
                {
                    m_GameClientService.Send_MoveRequest(iDirection, iSequence, iKey);
                }
            }
            catch
            {
                // The player has not yet been loaded
            }
        }

        private Vector3 m_LightDirection = new Vector3(0f, 0f, 1f);
        private double m_LightRadians = -0.5d;

        private void mParseKeyboard(Input.KeyboardHandler nKeyboard)
        {
            if (InWorld)
            {
                if (nKeyboard.IsKeyDown(Keys.I))
                    m_LightRadians += .01f;
                if (nKeyboard.IsKeyDown(Keys.K))
                    m_LightRadians -= .01f;

                m_LightDirection.Z = -(float)Math.Cos(m_LightRadians);
                m_LightDirection.Y = (float)Math.Sin(m_LightRadians);

                m_TileEngineService.SetLightDirection(m_LightDirection);
                #region KeyboardMovement
                GameObjects.Movement iMovement = m_GameObjectsService.GetObject(m_GameObjectsService.MyGUID).Movement;
                if (nKeyboard.IsKeyDown(Keys.W))
                    iMovement.SetPositionInstant(iMovement.TileX - 1, iMovement.TileY - 1, 0);
                if (nKeyboard.IsKeyDown(Keys.A))
                    iMovement.SetPositionInstant(iMovement.TileX - 1, iMovement.TileY + 1, 0);
                if (nKeyboard.IsKeyDown(Keys.S))
                    iMovement.SetPositionInstant(iMovement.TileX + 1, iMovement.TileY + 1, 0);
                if (nKeyboard.IsKeyDown(Keys.D))
                    iMovement.SetPositionInstant(iMovement.TileX + 1, iMovement.TileY - 1, 0);
                if (nKeyboard.IsKeyPressed(Keys.B))
                    m_GameClientService.Send_UseRequest(
                        ((GameObjects.Player)m_GameObjectsService.GetObject(m_GameObjectsService.MyGUID))
                        .Equipment[(int)GameObjects.EquipLayer.Backpack].GUID);
                /*
                if (iMovement.IsMoving == false)
                {
                    if (nKeyboard.IsKeyDown(Keys.W))
                    {
                        if (nKeyboard.IsKeyDown(Keys.A))
                        {
                            iMovement.SetGoalTile(
                                iMovement.TileX - 1,
                                iMovement.TileY, 0);
                        }
                        else if (nKeyboard.IsKeyDown(Keys.D))
                        {
                            iMovement.SetGoalTile(
                                iMovement.TileX,
                                iMovement.TileY - 1, 0);
                        }
                        else
                        {
                            iMovement.SetGoalTile(
                                iMovement.TileX - 1,
                                iMovement.TileY - 1, 0);
                        }
                    }
                    else if (nKeyboard.IsKeyDown(Keys.S))
                    {
                        if (nKeyboard.IsKeyDown(Keys.A))
                        {
                            iMovement.SetGoalTile(
                                iMovement.TileX,
                                iMovement.TileY + 1, 0);
                        }
                        else if (nKeyboard.IsKeyDown(Keys.D))
                        {
                            iMovement.SetGoalTile(
                                iMovement.TileX + 1,
                                iMovement.TileY, 0);
                        }
                        else
                        {
                            iMovement.SetGoalTile(
                                iMovement.TileX + 1,
                                iMovement.TileY + 1, 0);
                        }
                    }
                    else
                    {
                        if (nKeyboard.IsKeyDown(Keys.A))
                        {
                            iMovement.SetGoalTile(
                                iMovement.TileX - 1,
                                iMovement.TileY + 1, 0);

                        }
                        if (nKeyboard.IsKeyDown(Keys.D))
                        {
                            iMovement.SetGoalTile(
                                iMovement.TileX + 1,
                                iMovement.TileY - 1, 0);
                        }
                    }
                }
                */
                #endregion
            }
        }

        // Poplicola 5/9/2009
        private float FPS; private float m_frames = 0; private float m_elapsedSeconds = 0;
        private bool mUpdateFPS(GameTime gameTime)
        {
            m_frames++;
            m_elapsedSeconds += (float)gameTime.ElapsedRealTime.TotalSeconds;
            if (m_elapsedSeconds >= 1)
            {
                FPS = m_frames / m_elapsedSeconds;
                m_elapsedSeconds -= 1;
                m_frames = 0;
                return true;
            }
            return false;
        }

        private string m_DebugMessage()
        {
            String iDebug = "FPS: " + FPS.ToString() + Environment.NewLine;
            iDebug += "Objects on screen: " + m_TileEngineService.ObjectsRendered.ToString() + Environment.NewLine;
            if (m_TileEngineService.MouseOverObject != null)
            {
                iDebug += "OBJECT: " + m_TileEngineService.MouseOverObject.ToString() + Environment.NewLine;
                if (m_TileEngineService.MouseOverObject.Type == TileEngine.MapObjectTypes.StaticTile)
                {
                    iDebug += "ArtID: " + ((TileEngine.StaticItem)m_TileEngineService.MouseOverObject).ID;
                }
                else if (m_TileEngineService.MouseOverObject.Type == TileEngine.MapObjectTypes.MobileTile)
                {
                    iDebug += 
                        "AnimID: " + ((TileEngine.MobileTile)m_TileEngineService.MouseOverObject).ID + Environment.NewLine +
                        "GUID: " + m_TileEngineService.MouseOverObject.OwnerGUID + Environment.NewLine +
                        "Hue: " + ((TileEngine.MobileTile)m_TileEngineService.MouseOverObject).Hue;
                }
                else if (m_TileEngineService.MouseOverObject.Type == TileEngine.MapObjectTypes.GameObjectTile)
                {
                    iDebug +=
                        "ArtID: " + ((TileEngine.GameObjectTile)m_TileEngineService.MouseOverObject).ID + Environment.NewLine +
                        "GUID: " + m_TileEngineService.MouseOverObject.OwnerGUID;
                }
                iDebug += " Z: " + m_TileEngineService.MouseOverObject.Z;
            }
            else
            {
                iDebug += "OVER: " + "null";
            }
            if (m_TileEngineService.MouseOverGroundTile != null)
            {
                iDebug += Environment.NewLine + "GROUND: " + m_TileEngineService.MouseOverGroundTile.Position.ToString();
            }
            else
            {
                iDebug += Environment.NewLine + "GROUND: null";
            }

            // iDebug += Environment.NewLine + m_GameObjectsService.GetObject(m_GameObjectsService.MyGUID).Movement.MoveSequence.ToString();
            iDebug += Environment.NewLine + m_LightRadians.ToString();
            return iDebug;
        }
    }
}
