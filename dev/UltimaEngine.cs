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

using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Patterns.IoC;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.IO.FontsNew;
using UltimaXNA.Ultima.IO.FontsOld;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima;
using UltimaXNA.Ultima.Login;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;

#endregion

namespace UltimaXNA
{
    public interface IEngine : IDisposable
    {
        InputManager Input
        {
            get;
        }
        
        Game Game
        {
            get;
        }

        GraphicsDevice GraphicsDevice
        {
            get;
        }

        GUIManager UserInterface
        {
            get;
        }

        Client Client
        {
            get;
        }

        AUltimaModel QueuedModel
        {
            get;
            set;
        }

        AUltimaModel ActiveModel
        {
            get;
            set;
        }

        GameWindow Window
        {
            get;
        }

        bool IsActive
        {
            get;
        }

        void ActivateQueuedModel();

        void Run();

        void Tick();

        void SuppressDraw();

        void Exit();

        void ResetElapsedTime();

    }

    internal class UltimaEngine : Game, IEngine
    {
        public static double TotalMS = 0d;

        private readonly IContainer m_Container;
        private readonly INetworkClient m_Network;

        private AUltimaModel m_Model;
        private AUltimaModel m_QueuedModel;

        public UltimaEngine(IContainer container)
        {
            m_Container = container;
            m_Network = container.Resolve<INetworkClient>();

            InitializeGraphicsDevice();
        }

        public InputManager Input
        {
            get;
            private set;
        }

        public Game Game
        {
            get { return this; }
        }

        public GUIManager UserInterface
        {
            get;
            private set;
        }

        public Client Client
        {
            get;
            private set;
        }

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

        public bool IsMinimized
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

        public void ActivateQueuedModel()
        {
            if(m_QueuedModel != null)
            {
                ActiveModel = QueuedModel;
                m_QueuedModel = null;
            }
        }

        protected override void Initialize()
        {
            Content.RootDirectory = "Content";

            // Create all the services we need.
            Client = m_Container.Resolve<Client>();
            Input = new InputManager(Window.Handle);
            UserInterface = new GUIManager(m_Container);

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

                ActiveModel = m_Container.Resolve<LoginModel>();
            }
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
                m_Network.Slice();
                ActiveModel.Update(totalMS, frameMS);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if(!IsMinimized)
            {
                SpriteBatch3D.ResetZ();
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

        // Some settings to designate a screen size and fps limit.
        private void InitializeGraphicsDevice()
        {
            Resolution resolution = Settings.Game.Resolution;
            GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = resolution.Width,
                PreferredBackBufferHeight = resolution.Height,
                SynchronizeWithVerticalRetrace = Settings.Game.IsVSyncEnabled
            };

            graphicsDeviceManager.PreparingDeviceSettings += onPreparingDeviceSettings;

            IsFixedTimeStep = false;
            graphicsDeviceManager.ApplyChanges();
        }

        private static void onPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
    }
}