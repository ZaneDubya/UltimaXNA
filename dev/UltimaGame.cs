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
using UltimaXNA.Core;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima;
using UltimaXNA.Ultima.Audio;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.Login;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World;
using UltimaXNA.Configuration.Properties;
using UltimaXNA.Core.Patterns.MVC;
#endregion

namespace UltimaXNA
{
    internal class UltimaGame : Game
    {
        public static double TotalMS;

        GraphicsDeviceManager m_GraphicsDeviceManager;
        AudioService m_Audio;
        InputManager m_Input;
        UserInterfaceService m_UserInterface;
        INetworkClient m_Network;
        PluginManager m_Plugins;
        ModelManager m_Models;
        Form GameForm => Control.FromHandle(Window.Handle) as Form;
        bool m_IsRunning;

        public ModelManager Models => m_Models;

        public UltimaGame()
        {
            m_GraphicsDeviceManager = new GraphicsDeviceManager(this);
            m_GraphicsDeviceManager.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            m_GraphicsDeviceManager.PreparingDeviceSettings += OnPreparingDeviceSettings;
            GameForm.FormClosing += OnFormClosing;
            SetupWindowForLogin();
        }

        protected override void Initialize()
        {
            Content.RootDirectory = "Content";
            ServiceRegistry.Register(this);
            ServiceRegistry.Register(new SpriteBatch3D(this));
            ServiceRegistry.Register(new SpriteBatchUI(this));
            m_Audio = ServiceRegistry.Register(new AudioService());
            m_Network = ServiceRegistry.Register<INetworkClient>(new NetworkClient());
            m_Input = ServiceRegistry.Register(new InputManager(Window.Handle));
            m_UserInterface = ServiceRegistry.Register(new UserInterfaceService());
            m_Plugins = new PluginManager(AppDomain.CurrentDomain.BaseDirectory);
            m_Models = new ModelManager();
            // Make sure we have a UO installation before loading IO.
            if (FileManager.IsUODataPresent)
            {
                // Initialize and load data
                IResourceProvider provider = new ResourceProvider(this);
                provider.RegisterResource(new EffectDataResource());
                ServiceRegistry.Register(provider);
                HueData.Initialize(GraphicsDevice);
                SkillsData.Initialize();
                GraphicsDevice.Textures[1] = HueData.HueTexture0;
                GraphicsDevice.Textures[2] = HueData.HueTexture1;
                m_IsRunning = true;
                WorldModel.IsInWorld = false;
                Models.Current = new LoginModel();
            }
            else
            {
                Tracer.Critical("Did not find a compatible UO Installation. UltimaXNA is compatible with any version of UO through Mondian's Legacy.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            ServiceRegistry.Unregister<UltimaGame>();
            m_UserInterface.Dispose();
            base.Dispose(disposing);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Profiler.InContext("OutOfContext"))
                Profiler.ExitContext("OutOfContext");
            Profiler.EnterContext("Update");

            IsFixedTimeStep = Settings.Engine.IsFixedTimeStep;

            if(!m_IsRunning)
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
                m_Audio.Update();
                m_Input.Update(totalMS, frameMS);
                m_UserInterface.Update(totalMS, frameMS);
                if (m_Network.IsConnected)
                {
                    m_Network.Slice();
                }
                Models.Current.Update(totalMS, frameMS);
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
                if (Models.Current is WorldModel)
                {
                    ResolutionProperty resolution = Settings.UserInterface.PlayWindowGumpResolution;
                    CheckWindowSize(resolution.Width, resolution.Height);
                }
                else
                {
                    CheckWindowSize(800, 600);
                }
                Models.Current.GetView().Draw(gameTime.ElapsedGameTime.TotalMilliseconds);
                m_UserInterface.Draw(gameTime.ElapsedGameTime.TotalMilliseconds);
            }

            Profiler.ExitContext("RenderFrame");
            Profiler.EnterContext("OutOfContext");

            UpdateWindowCaption(gameTime);
        }

        public void Quit()
        {
            m_IsRunning = false;
        }

        void UpdateWindowCaption(GameTime gameTime)
        {
            double timeDraw = Profiler.GetContext("RenderFrame").TimeInContext;
            double timeUpdate = Profiler.GetContext("Update").TimeInContext;
            double timeOutOfContext = Profiler.GetContext("OutOfContext").TimeInContext;
            double timeTotalCheck = timeOutOfContext + timeDraw + timeUpdate;
            double timeTotal = Profiler.TrackedTime;
            double avgDrawMs = Profiler.GetContext("RenderFrame").AverageTime;

            Window.Title = string.Format("UltimaXNA Draw:{0:0.0}% Update:{1:0.0}% AvgDraw:{2:0.0}ms {3}",
                100d * (timeDraw / timeTotal),
                100d * (timeUpdate / timeTotal),
                avgDrawMs,
                gameTime.IsRunningSlowly ? "*" : string.Empty);
        }

        public void SetupWindowForLogin()
        {
            RestoreWindow();
            Window.AllowUserResizing = false;
            SetGraphicsDeviceWidthHeight(new ResolutionProperty(800, 600)); // a wee bit bigger than legacy. Looks nicer.
        }

        public void SetupWindowForWorld()
        {
            Window.AllowUserResizing = true;
            SetGraphicsDeviceWidthHeight(Settings.UserInterface.WindowResolution);
            if (Settings.UserInterface.IsMaximized)
            {
                MaximizeWindow();
            }
        }

        public void SaveResolution()
        {
            if (IsMaximized)
            {
                Settings.UserInterface.IsMaximized = true;
            }
            else
            {
                Settings.UserInterface.WindowResolution = new ResolutionProperty(m_GraphicsDeviceManager.PreferredBackBufferWidth, m_GraphicsDeviceManager.PreferredBackBufferHeight);
            }
        }

        bool IsMinimized => GameForm.WindowState == FormWindowState.Minimized;
        bool IsMaximized => GameForm.WindowState == FormWindowState.Minimized;

        void MaximizeWindow()
        {
            GameForm.WindowState = FormWindowState.Maximized;
        }

        void RestoreWindow()
        {
            if (GameForm.WindowState != FormWindowState.Normal)
            {
                GameForm.WindowState = FormWindowState.Normal;
            }
        }

        void CheckWindowSize(int minWidth, int minHeight)
        {
            GameWindow window = Window; // (sender as GameWindow);
            ResolutionProperty resolution = new ResolutionProperty(window.ClientBounds.Width, window.ClientBounds.Height);
            // this only occurs when the world is active. Make sure that we don't reduce the window size
            // smaller than the world gump size.
            if (resolution.Width < minWidth)
                resolution.Width = minWidth;
            if (resolution.Height < minHeight)
                resolution.Height = minHeight;
            if (resolution.Width != window.ClientBounds.Width || resolution.Height != window.ClientBounds.Height)
                SetGraphicsDeviceWidthHeight(resolution);
        }

        void SetGraphicsDeviceWidthHeight(ResolutionProperty resolution)
        {
            m_GraphicsDeviceManager.PreferredBackBufferWidth = resolution.Width;
            m_GraphicsDeviceManager.PreferredBackBufferHeight = resolution.Height;
            m_GraphicsDeviceManager.SynchronizeWithVerticalRetrace = Settings.Engine.IsVSyncEnabled;
            m_GraphicsDeviceManager.ApplyChanges();
        }

        void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        void OnFormClosing(object sender, CancelEventArgs e)
        {
            // we must dispose of the active model BEFORE we dispose of the window.
            if (Models.Current != null)
            {
                Models.Current = null;
            }
        }
    }
}