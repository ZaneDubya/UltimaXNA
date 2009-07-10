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
        private Client.UltimaClient _Client;
        private Input.InputHandler _Input;
        private GameState _GameState;
        private GameObjects.GameObjects _GameObjects;
        private TileEngine.World _World;
        private TileEngine.TileEngine _TileEngine;
        private GUI.EngineGUI _GUI;

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

            _Client = new Client.UltimaClient(this);
            this.Components.Add(_Client);

            _Input = new Input.InputHandler(this);
            this.Components.Add(_Input);

            _GameState = new GameState(this);
            this.Components.Add(_GameState);

            _GameObjects = new GameObjects.GameObjects(this);
            this.Components.Add(_GameObjects);

            _World = new TileEngine.World(this);
            this.Components.Add(_World);

            _TileEngine = new TileEngine.TileEngine(this);
            this.Components.Add(_TileEngine);

            _GUI = new GUI.EngineGUI(this);
            this.Components.Add(_GUI);

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
            _GameState.UpdateAfter();
            _GUI.DebugMessage = _GameState.DebugMessage;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        // Some settings to designate a screen size and fps limit.
        private void m_SetupGraphicsDeviceManager()
        {
            GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferWidth = 800;
            graphicsDeviceManager.PreferredBackBufferHeight = 600;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            graphicsDeviceManager.PreparingDeviceSettings += OnPreparingDeviceSettings;
            this.IsFixedTimeStep = false;
            graphicsDeviceManager.ApplyChanges();
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