#region File Description & Usings
//-----------------------------------------------------------------------------
// Engine.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UndeadClient
{
    public class Engine : Game
    {
        string Username = "Poplicola";
        string Password = "sexytime";

        private Network.GameClient m_Client;
        // private Network.GameServer m_Server;
        private Input.InputHandler m_Input;
        private GameState m_GameState;
        private GameObjects.GameObjects m_GameObjects;
        private TileEngine.World m_World;
        private TileEngine.TileEngine m_TileEngine;
        private GUI.EngineGUI m_GUI;
        private MiscUtil.Screenshot m_Screenshot;
        
        private GraphicsDeviceManager m_GraphicsDeviceManager;

        public Engine()
        {
            m_GraphicsDeviceManager = new GraphicsDeviceManager(this);
            m_GraphicsDeviceManager.PreferredBackBufferWidth = 800;
            m_GraphicsDeviceManager.PreferredBackBufferHeight = 600;
            m_GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            this.IsFixedTimeStep = false;
            m_GraphicsDeviceManager.ApplyChanges();
        }

        protected override void Initialize()
        {
            this.Content.RootDirectory = "Content";

            m_Screenshot = new MiscUtil.Screenshot(this);
            this.Components.Add(m_Screenshot);

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
            m_GameState.LoadContent();
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            m_Client.Disconnect();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            switch (m_Client.Status)
            {
                case UndeadClient.Network.ClientStatus.Unconnected :
                    m_Client.ConnectToLoginServer("localhost", 2593, Username, Password);
                    break;
                case UndeadClient.Network.ClientStatus.LoginServer_Connecting :
                    // do nothing ... wait until we have the server list.
                    break;
                case UndeadClient.Network.ClientStatus.LoginServer_HasServerList:
                    m_Client.SelectServer(0);
                    break;
                default :
                    break;
            }

            m_GameState.UpdateAfter();
            m_GUI.DebugMessage = m_GameState.DebugMessage;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (m_GameState.TakeScreenshot)
            {
                m_Screenshot.PrepareForCapture();
                base.Draw(gameTime);
                m_Screenshot.CaptureScreenshot();
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
                base.Draw(gameTime);
            }
        }

        #region EntryPoint
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