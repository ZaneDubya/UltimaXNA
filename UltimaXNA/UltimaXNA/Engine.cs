/***************************************************************************
 *   Engine.cs
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
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA
{
    public class Engine : Game
    {
        private Client.UltimaClient _Client;
        private Input.InputHandler _Input;
        private GameState _GameState;
        private Entities.EntitiesCollection _GameObjects;
        private TileEngine.World _World;
        private TileEngine.TileEngine _TileEngine;
        private GUI.EngineGUI _GUI;
        private SceneManagement.SceneManager _sceneManager;
        private Diagnostics.Logger _logService;

        public Engine()
        {
            m_SetupGraphicsDeviceManager();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            // WavePlayer opens a thread for each sound.
            WavePlayer.Player.EndEverything();
            base.OnExiting(sender, args);
        }

        protected override void Initialize()
        {
            // First initialize some of the local data classes with our graphicsdevice so 
            // we don't have to continually pass it to them.
            Data.Gumps.GraphicsDevice = this.GraphicsDevice;

            //load the hues texture
            UltimaXNA.Data.HuesXNA.Initialize(GraphicsDevice);
            UltimaXNA.Data.ASCIIText.Initialize(GraphicsDevice);
            UltimaXNA.Data.StringList.LoadStringList("enu");

            this.Content.RootDirectory = "Content";

            _logService = new Diagnostics.Logger("UXNA");
            Services.AddService<Diagnostics.ILoggingService>(_logService);

            _sceneManager = new SceneManagement.SceneManager(this);
            Services.AddService<SceneManagement.ISceneService>(_sceneManager);
            this.Components.Add(_sceneManager);

            _Client = new Client.UltimaClient(this);
            this.Components.Add(_Client);

            _Input = new Input.InputHandler(this);
            this.Components.Add(_Input);

            _GameState = new GameState(this);
            this.Components.Add(_GameState);

            _GameObjects = new Entities.EntitiesCollection(this);
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
            if (_sceneManager.CurrentScene == null)
            {
                _sceneManager.CurrentScene = new SceneManagement.LoginScene(this);
            }
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
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            using (Engine engine = new Engine())
            {
                engine.Run();
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Diagnostics.Logger log = new Diagnostics.Logger(typeof(Engine));
            log.Fatal(e.ExceptionObject);
        }

        #endregion
    }
}