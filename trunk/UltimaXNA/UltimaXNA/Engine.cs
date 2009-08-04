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

            //Initialize the Data objects.
            Data.AnimationsXNA.Initialize(GraphicsDevice);
            Data.Art.Initialize(GraphicsDevice);
            Data.ASCIIText.Initialize(GraphicsDevice);
            Data.HuesXNA.Initialize(GraphicsDevice);
            Data.StringList.LoadStringList("enu");
            Data.Texmaps.Initialize(GraphicsDevice);

            this.Content.RootDirectory = "Content";
            _logService = new Diagnostics.Logger("UXNA");
            Services.AddService<Diagnostics.ILoggingService>(_logService);

            SceneManagement.SceneManager.Initialize(this);
            GameState.Initialize(this);
            TileEngine.WorldRenderer.Initialize(this);
            GUI.UserInterface.Initialize(this);

			ParticleEngine.ParticleEngine.Initialize(this, System.IO.Path.Combine(this.Content.RootDirectory, "pfx"));

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
            SceneManagement.SceneManager.Update(gameTime);
            Client.UltimaClient.Update(gameTime);
            Input.InputHandler.Update(gameTime);
            GameState.Update(gameTime);
            Entities.EntitiesCollection.Update(gameTime);
            TileEngine.World.Update(gameTime);
            TileEngine.WorldRenderer.Update(gameTime);
            GUI.UserInterface.Update(gameTime);

			ParticleEngine.ParticleEngine.Update(gameTime);

            if (SceneManagement.SceneManager.CurrentScene == null)
                SceneManagement.SceneManager.CurrentScene = new SceneManagement.LoginScene();

            GameState.UpdateAfter(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

			SceneManagement.SceneManager.Draw(gameTime);
			TileEngine.WorldRenderer.Draw(gameTime);
			GUI.UserInterface.Draw(gameTime);

			ParticleEngine.ParticleEngine.Draw(gameTime);

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