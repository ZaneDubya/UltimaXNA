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
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;
using UltimaXNA.Configuration.Properties;
using UltimaXNA.Core;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Patterns.MVC;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima;
using UltimaXNA.Ultima.Audio;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.Login;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World;
#endregion

namespace UltimaXNA
{
    internal class UltimaGame : CoreGame
    {
        public static double TotalMS;

        AudioService m_Audio;
        InputManager m_Input;
        UserInterfaceService m_UserInterface;
        INetworkClient m_Network;
        PluginManager m_Plugins;
        ModelManager m_Models;
        bool m_IsRunning;

        public ModelManager Models => m_Models;

        public UltimaGame()
        {
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

        protected override void OnUpdate(double totalMS, double frameMS)
        {
            if (!m_IsRunning)
            {
                Settings.Save();
                Exit();
            }
            else
            {
                IsFixedTimeStep = Settings.Engine.IsFixedTimeStep;
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
        }

        protected override void OnDraw(double frameMS)
        {
            if (!IsMinimized)
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
                Models.Current.GetView().Draw(frameMS);
                m_UserInterface.Draw(frameMS);
            }
        }

        public void Quit()
        {
            m_IsRunning = false;
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
                ResolutionProperty res = new ResolutionProperty(DeviceManager.PreferredBackBufferWidth, DeviceManager.PreferredBackBufferHeight);
                Settings.UserInterface.WindowResolution = res;
            }
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