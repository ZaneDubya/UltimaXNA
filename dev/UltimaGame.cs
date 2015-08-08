/***************************************************************************
 *   UltimaGame.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using UltimaXNA.Configuration;
using UltimaXNA.Core;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima;
using UltimaXNA.Ultima.Audio;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.Resources.Fonts;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA
{
    internal class UltimaGame : Game
    {
        public static bool IsRunning // false = engine immediately quits.
        {
            get;
            set;
        }

        public static double TotalMS;

        public UltimaGame()
        {
            InitializeGraphicsDeviceAndWindow();
            InitializeExitGuard();
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

        protected PluginManager Plugins
        {
            get;
            private set;
        }

        protected override void Initialize()
        {
            Content.RootDirectory = "Content";

            // register this instance as a service
            ServiceRegistry.Register<UltimaGame>(this);

            // Create all the services we need.
            ServiceRegistry.Register<SpriteBatch3D>(new SpriteBatch3D(this));
            ServiceRegistry.Register<SpriteBatchUI>(new SpriteBatchUI(this));
            ServiceRegistry.Register<AudioService>(new AudioService());
            Network = ServiceRegistry.Register<INetworkClient>(new NetworkClient());
            Input = ServiceRegistry.Register<InputManager>(new InputManager(Window.Handle));
            UserInterface = ServiceRegistry.Register<UserInterfaceService>(new UserInterfaceService());
            Plugins = new PluginManager(AppDomain.CurrentDomain.BaseDirectory);

            // Make sure we have a UO installation before loading IO.
            if (FileManager.IsUODataPresent)
            {
                // Initialize and load data
                IResourceProvider provider = new ResourceProvider(this);
                provider.RegisterResource<EffectData>(new EffectDataResource());
                ServiceRegistry.Register<IResourceProvider>(provider);

                HueData.Initialize(GraphicsDevice);
                SkillsData.Initialize();
                GraphicsDevice.Textures[1] = HueData.HueTexture0;
                GraphicsDevice.Textures[2] = HueData.HueTexture1;

                IsRunning = true;
                WorldModel.IsInWorld = false;

                ActiveModel = new LoginModel();
            }
            else
            {
                Tracer.Critical("Did not find a compatible UO Installation. UltimaXNA is compatible with any version of UO through Mondian's Legacy.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            ServiceRegistry.Unregister<UltimaGame>();
            UserInterface.Dispose();
            base.Dispose(disposing);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Profiler.InContext("OutOfContext"))
                Profiler.ExitContext("OutOfContext");
            Profiler.EnterContext("Update");

            IsFixedTimeStep = Settings.Game.IsFixedTimeStep;

            if(!IsRunning)
            {
                Settings.Save();
                Exit();
            }
            else
            {
                base.Update(gameTime);
                double totalMS = gameTime.TotalGameTime.TotalMilliseconds;
                double frameMS = gameTime.ElapsedGameTime.TotalMilliseconds;

                TotalMS = totalMS;
                Input.Update(totalMS, frameMS);
                UserInterface.Update(totalMS, frameMS);
                if (Network.IsConnected)
                    Network.Slice();
                ActiveModel.Update(totalMS, frameMS);
            }

            Profiler.ExitContext("Update");
            Profiler.EnterContext("OutOfContext");
        }

        protected override void Draw(GameTime gameTime)
        {
            Profiler.EndFrame();
            Profiler.BeginFrame();
            if (Profiler.InContext("OutOfContext"))
                Profiler.ExitContext("OutOfContext");
            Profiler.EnterContext("RenderFrame");

            if(!IsMinimized)
            {
                SpriteBatch3D.Reset();
                GraphicsDevice.Clear(Color.Black);  

                ActiveModel.GetView()
                    .Draw(gameTime.ElapsedGameTime.TotalMilliseconds);
                UserInterface.Draw(gameTime.ElapsedGameTime.TotalMilliseconds);
            }

            Profiler.ExitContext("RenderFrame");
            Profiler.EnterContext("OutOfContext");

            UpdateWindowCaption(gameTime);
        }

        private void UpdateWindowCaption(GameTime gameTime)
        {
            double frame_time_drawing = Profiler.GetContext("RenderFrame").TimeSpent * 1000d;
            double frame_time_updating = Profiler.GetContext("Update").TimeSpent * 1000d;
            double frame_time = Profiler.TotalTimeMS;
            double last_draw_ms = Profiler.GetContext("RenderFrame").AverageOfLast60Times * 1000d;
            double other_time = Profiler.GetContext("OutOfContext").TimeSpent * 1000d;

            double total_time_check = other_time + frame_time_drawing + frame_time_updating;

            this.Window.Title = string.Format("UltimaXNA Draw:{0:0.00}% Update:{1:0.00}% AvgDraw:{2:0.00}ms {3}",
                100d * (frame_time_drawing / frame_time),
                100d * (frame_time_updating / frame_time),
                last_draw_ms,
                gameTime.IsRunningSlowly ? "*" : string.Empty);
        }

        public void SetupWindowForLogin()
        {
            Restore();
            Window.AllowUserResizing = false;
            SetGraphicsDeviceWidthHeight(new ResolutionConfig(800, 600)); // a wee bit bigger than legacy. Looks nicer.
        }

        public void SetupWindowForWorld()
        {
            Window.AllowUserResizing = true;
            SetGraphicsDeviceWidthHeight(Settings.World.WindowResolution);
            if (Settings.World.IsMaximized)
            {
                Maximize();
            }
        }

        public void SaveResolution()
        {
            if (IsMaximized)
            {
                Settings.World.IsMaximized = true;
            }
            else
            {
                Settings.World.WindowResolution = new ResolutionConfig(GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight);
            }
        }

        private void InitializeExitGuard()
        {
            Form form = (Form)Form.FromHandle(Window.Handle);
            form.Closing += ExitGuard;
        }

        private void ExitGuard(object sender, CancelEventArgs e)
        {
            // we should dispose of the active model BEFORE we dispose of the window.
            if (ActiveModel != null)
                ActiveModel = null;
        }

        protected bool IsMinimized
        {
            get
            {
                //Get our top level form via the handle.
                Control MainForm = Control.FromHandle(Window.Handle);
                //If we are minimized don't waste time trying to draw, and avoid crash on resume.
                if (((Form)MainForm).WindowState == FormWindowState.Minimized)
                {
                    return true;
                }
                return false;
            }
        }

        protected bool IsMaximized
        {
            get
            {
                // Get our top level form via the handle.
                Control MainForm = Control.FromHandle(Window.Handle);
                if (((Form)MainForm).WindowState == FormWindowState.Maximized)
                {
                    return true;
                }
                return false;
            }
        }

        protected void Maximize()
        {
            // Get our top level form via the handle.
            Control MainForm = Control.FromHandle(Window.Handle);
            ((Form)MainForm).WindowState = FormWindowState.Maximized;
        }

        protected void Restore()
        {
            // Get our top level form via the handle.
            Control MainForm = Control.FromHandle(Window.Handle);
            if (((Form)MainForm).WindowState != FormWindowState.Normal)
                ((Form)MainForm).WindowState = FormWindowState.Normal;
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
            ResolutionConfig resolution = new ResolutionConfig(window.ClientBounds.Width, window.ClientBounds.Height);
            // this only occurs when the world is active. Make sure that we don't reduce the window size
            // smaller than the world gump size.
            if (resolution.Width < Settings.World.PlayWindowGumpResolution.Width)
                resolution.Width = Settings.World.PlayWindowGumpResolution.Width;
            if (resolution.Height < Settings.World.PlayWindowGumpResolution.Height)
                resolution.Height = Settings.World.PlayWindowGumpResolution.Height;
            SetGraphicsDeviceWidthHeight(resolution);
        }

        private void SetGraphicsDeviceWidthHeight(ResolutionConfig resolution)
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