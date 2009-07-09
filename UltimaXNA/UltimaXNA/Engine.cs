#region File Description & Usings
//-----------------------------------------------------------------------------
// Engine.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA
{
    public class Engine : Game
    {
        private Network.GameClient m_Client;
        private Input.InputHandler m_Input;
        private GameState m_GameState;
        private GameObjects.GameObjects m_GameObjects;
        private TileEngine.World m_World;
        private TileEngine.TileEngine m_TileEngine;
        private GUI.EngineGUI m_GUI;

        public Engine()
        {
            m_SetupGraphicsDeviceManager();
        }

        protected override void Initialize()
        {
            // First initialize some of the local data classes with our graphicsdevice so 
            // we don't have to continually pass it to them.
            Data.Gumps.GraphicsDevice = this.GraphicsDevice;
            
            //load the hues texture
            UltimaXNA.Data.HuesXNA.Initialize(GraphicsDevice);
            UltimaXNA.Data.StringList.LoadStringList("enu");

            this.Content.RootDirectory = "Content";

            m_Client = new Network.GameClient(this);
            this.Components.Add(m_Client);

            m_Input = new Input.InputHandler(this);
            this.Components.Add(m_Input);

            m_GameState = new GameState(this);
            this.Components.Add(m_GameState);

            m_GameObjects = new GameObjects.GameObjects(this);
            this.Components.Add(m_GameObjects);

            m_World = new TileEngine.World(this);
            this.Components.Add(m_World);

            m_TileEngine = new TileEngine.TileEngine(this);
            this.Components.Add(m_TileEngine);

            m_GUI = new GUI.EngineGUI(this);
            this.Components.Add(m_GUI);

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            switch (m_Client.Status)
            {
                case UltimaXNA.Network.ClientStatus.Unconnected:
                    m_GUI.LoadLoginGUI();
                    break;
                case UltimaXNA.Network.ClientStatus.LoginServer_Connecting :
                    // do nothing ... wait until we have the server list.
                    break;
                case UltimaXNA.Network.ClientStatus.LoginServer_HasServerList:
                    m_Client.SelectServer(0);
                    break;
                case UltimaXNA.Network.ClientStatus.Error :
                    m_Client.Reset();
                    m_GUI.Reset();
                    m_GUI.LoadLoginGUI();
                    break;
                default :
                    break;
            }

            m_GameState.UpdateAfter();
            m_GUI.DebugMessage = m_GameState.DebugMessage;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        // Some settings to designate a screen size and fps limit.
        private void m_SetupGraphicsDeviceManager()
        {
            GraphicsDeviceManager iGraphicsDeviceManager = new GraphicsDeviceManager(this);
            iGraphicsDeviceManager.PreferredBackBufferWidth = 800;
            iGraphicsDeviceManager.PreferredBackBufferHeight = 600;
            iGraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            iGraphicsDeviceManager.PreparingDeviceSettings += OnPreparingDeviceSettings;
            this.IsFixedTimeStep = false;
            iGraphicsDeviceManager.ApplyChanges();
        }
        private static void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        #region EntryPoint
        [STAThread]
        private static void Main(string[] args)
        {
            using (Engine engine = new Engine())
            {
                engine.Run();
            }
        }
        #endregion
    }
}