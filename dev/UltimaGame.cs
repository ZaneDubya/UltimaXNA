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
    class UltimaGame : CoreGame
    {
        UserInterfaceService m_UserInterface;
        PluginManager m_Plugins;
        ModelManager m_Models;
        bool m_IsRunning;
        double m_TotalMS;

        public ModelManager Models => m_Models;
        public double TotalMS => m_TotalMS;

        public UltimaGame()
        {
            GameForm.FormClosing += OnFormClosing;
            SetupWindowForLogin();
        }

        protected override void Initialize()
        {
            Content.RootDirectory = "Content";
            Service.Add(this);
            Service.Add(new SpriteBatch3D(this));
            Service.Add(new SpriteBatchUI(this));
            Service.Add<INetworkClient>(new NetworkClient());
            Service.Add<IInputService>(new InputService(Window.Handle));
            Service.Add(new AudioService());
            m_UserInterface = Service.Add(new UserInterfaceService());
            m_Plugins = new PluginManager(AppDomain.CurrentDomain.BaseDirectory);
            m_Models = new ModelManager();
            // Make sure we have a UO installation before loading IO.
            if (FileManager.IsUODataPresent)
            {
                // Initialize and load data
                IResourceProvider provider = new ResourceProvider(this);
                provider.RegisterResource(new EffectDataResource());
                Service.Add(provider);
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
                m_TotalMS = totalMS;
                Service.Get<AudioService>().Update();
                Service.Get<IInputService>().Update(totalMS, frameMS);
                m_UserInterface.Update(totalMS, frameMS);
                Service.Get<INetworkClient>().Slice();
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