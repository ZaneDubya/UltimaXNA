/***************************************************************************
 *   UltimaEngine.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Windows.Forms;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.IO.FontsNew;
using UltimaXNA.Ultima.IO.FontsOld;
using UltimaXNA.Ultima.Login;
using UltimaXNA.Ultima.UI;
#endregion

namespace UltimaXNA
{
    internal class UltimaEngine : Game
    {
        public static double TotalMS = 0d;

        public UltimaEngine()
        {
            InitializeGraphicsDeviceAndWindow();
            SetupWindowForLogin();
        }

        #region Active & Queued Models
        private AUltimaModel m_Model;
        private AUltimaModel m_QueuedModel;

        public AUltimaModel QueuedModel
        {
            get { return m_QueuedModel; }
            set
            {
                if(m_QueuedModel != null)
                {
                    m_QueuedModel.Dispose();
                    m_QueuedModel = null;
                }
                m_QueuedModel = value;

                if(m_QueuedModel != null)
                {
                    m_QueuedModel.Initialize();
                }
            }
        }

        public AUltimaModel ActiveModel
        {
            get { return m_Model; }
            set
            {
                if(m_Model != null)
                {
                    m_Model.Dispose();
                    m_Model = null;
                }
                m_Model = value;
                if(m_Model != null)
                {
                    m_Model.Initialize();
                }
            }
        }

        public void ActivateQueuedModel()
        {
            if (m_QueuedModel != null)
            {
                ActiveModel = QueuedModel;
                m_QueuedModel = null;
            }
        }
        #endregion

        protected GraphicsDeviceManager GraphicsDeviceManager
        {
            get;
            private set;
        }

        protected bool IsMinimized
        {
            get
            {
                //Get out top level form via the handle.
                Control MainForm = Control.FromHandle(Window.Handle);
                //If we are minimized don't waste time trying to draw, and avoid crash on resume.
                if(((Form)MainForm).WindowState == FormWindowState.Minimized)
                {
                    return true;
                }
                return false;
            }
        }

        protected InputManager Input
        {
            get;
            private set;
        }

        protected UserInterfaceService UserInterface
        {
            get;
            private set;
        }

        protected INetworkClient Network
        {
            get;
            private set;
        }

        protected override void Initialize()
        {
            Content.RootDirectory = "Content";

            // Create all the services we need.
            UltimaServices.Register<SpriteBatch3D>(new SpriteBatch3D(this));
            UltimaServices.Register<SpriteBatchUI>(new SpriteBatchUI(this));
            Network = UltimaServices.Register<INetworkClient>(new NetworkClient());
            Input = UltimaServices.Register<InputManager>(new InputManager(Window.Handle));
            UserInterface = UltimaServices.Register<UserInterfaceService>(new UserInterfaceService());

            // Make sure we have a UO installation before loading IO.
            if(FileManager.IsUODataPresent)
            {
                // Initialize and load data
                AnimData.Initialize();
                Animations.Initialize(GraphicsDevice);
                ArtData.Initialize(GraphicsDevice);

                ASCIIText.Initialize(GraphicsDevice);
                TextUni.Initialize(GraphicsDevice);

                GumpData.Initialize(GraphicsDevice);
                HuesXNA.Initialize(GraphicsDevice);
                TexmapData.Initialize(GraphicsDevice);
                StringData.LoadStringList("enu");
                SkillsData.Initialize();
                GraphicsDevice.Textures[1] = HuesXNA.HueTexture0;
                GraphicsDevice.Textures[2] = HuesXNA.HueTexture1;

                EngineVars.EngineRunning = true;
                EngineVars.InWorld = false;

                ActiveModel = new LoginModel();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (ActiveModel != null)
                ActiveModel = null;
            base.Dispose(disposing);
        }

        protected override void Update(GameTime gameTime)
        {
            IsFixedTimeStep = Settings.Game.IsFixedTimeStep;

            if(!EngineVars.EngineRunning)
            {
                Settings.Save();
                Exit();
            }
            else
            {
                double totalMS = gameTime.TotalGameTime.TotalMilliseconds;
                double frameMS = gameTime.ElapsedGameTime.TotalMilliseconds;

                TotalMS = totalMS;
                Input.Update(totalMS, frameMS);
                UserInterface.Update(totalMS, frameMS);
                Network.Slice();
                ActiveModel.Update(totalMS, frameMS);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if(!IsMinimized)
            {
                SpriteBatch3D.Reset();
                GraphicsDevice.Clear(Color.Black);

                ActiveModel.GetView()
                    .Draw(gameTime.ElapsedGameTime.TotalMilliseconds);
                UserInterface.Draw(gameTime.ElapsedGameTime.TotalMilliseconds);

                EngineVars.UpdateFPS(gameTime.ElapsedGameTime.TotalMilliseconds);
                Window.Title =
                    Settings.Debug.ShowFps ?
                        string.Format("UltimaXNA FPS:{0}", EngineVars.UpdateFPS(gameTime.ElapsedGameTime.TotalMilliseconds)) :
                        "UltimaXNA";
            }
        }

        public void SetupWindowForLogin()
        {
            Window.AllowUserResizing = false;
            SetGraphicsDeviceWidthHeight(new Resolution(800, 600)); // a wee bit bigger than legacy. Looks nicer.
        }

        public void SetupWindowForWorld()
        {
            Window.AllowUserResizing = true;
            if (Settings.World.IsFullScreen)
            {
                // not implemented!
                SetGraphicsDeviceWidthHeight(Settings.World.WindowResolution);
            }
            else
            {
                SetGraphicsDeviceWidthHeight(Settings.World.WindowResolution);
            }
        }

        public void SaveResolution()
        {
            Settings.World.WindowResolution = new Resolution(GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight);
        }

        private void InitializeGraphicsDeviceAndWindow()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.PreparingDeviceSettings += OnPreparingDeviceSettings;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnWindowSizeChanged);
        }

        private void OnWindowSizeChanged(object sender, EventArgs e)
        {
            GameWindow window = (sender as GameWindow);
            Resolution resolution = new Resolution(window.ClientBounds.Width, window.ClientBounds.Height);
            // this only occurs when the world is active. Make sure that we don't reduce the window size
            // smaller than the world gump size.
            if (resolution.Width < Settings.World.GumpResolution.Width)
                resolution.Width = Settings.World.GumpResolution.Width;
            if (resolution.Height < Settings.World.GumpResolution.Height)
                resolution.Height = Settings.World.GumpResolution.Height;
            SetGraphicsDeviceWidthHeight(resolution);
        }

        private void SetGraphicsDeviceWidthHeight(Resolution resolution)
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = resolution.Width;
            GraphicsDeviceManager.PreferredBackBufferHeight = resolution.Height;
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = Settings.Game.IsVSyncEnabled;
            GraphicsDeviceManager.ApplyChanges();
        }

        private static void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
    }
}