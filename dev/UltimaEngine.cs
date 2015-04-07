/***************************************************************************
 *   UltimaEngine.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings

using UltimaXNA.Data;
using UltimaXNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaWorld;
#endregion

namespace UltimaXNA
{
    public class UltimaEngine : Game
    {
        public static double TotalMS = 0d;

        public InputManager Input
        {
            get;
            private set;
        }

        public GUIManager UserInterface
        {
            get;
            private set;
        }

        public UltimaClient Client
        {
            get;
            private set;
        }

        private AUltimaModel m_QueuedModel;
        internal AUltimaModel QueuedModel
        {
            get { return m_QueuedModel; }
            set
            {
                if (m_QueuedModel != null)
                {
                    m_QueuedModel.Dispose();
                    m_QueuedModel = null;
                }
                m_QueuedModel = value;
                if (m_QueuedModel != null && m_QueuedModel.Engine == null)
                {
                    m_QueuedModel.Initialize(this);
                }
            }
        }

        private AUltimaModel m_Model;
        internal AUltimaModel ActiveModel
        {
            get { return m_Model; }
            set
            {
                if (m_Model != null)
                {
                    m_Model.Dispose();
                    m_Model = null;
                }
                m_Model = value;
                if (m_Model != null && m_Model.Engine == null)
                {
                    m_Model.Initialize(this);
                }
            }
        }

        internal void ActivateQueuedModel()
        {
            if (m_QueuedModel != null)
            {
                ActiveModel = QueuedModel;
                m_QueuedModel = null;
            }
        }

        public UltimaEngine(int width, int height)
        {
            setupGraphicsDeviceManager(width, height);
            UltimaVars.EngineVars.ScreenSize = new Point(width, height);

            // this is copied from IXL.BaseCore - required for finding the mouse coordinate when moving the cursor over the window.
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(Window.Handle);
            UltimaVars.EngineVars.ScreenDPI = new Vector2(graphics.DpiX / 96f, graphics.DpiY / 96f);
        }

        protected override void Initialize()
        {
            Content.RootDirectory = "Content";

            // Create all the services we need.
            Client = new UltimaClient(this);
            Client.IsLoggingPackets = true;
            Input = new InputManager(Window.Handle);
            UserInterface = new GUIManager(this);

            // Make sure we have a UO installation before loading UltimaData.
            if (UltimaData.FileManager.IsUODataPresent)
            {
                // Initialize and load data
                UltimaData.AnimData.Initialize();
                UltimaData.Animations.Initialize(GraphicsDevice);
                UltimaData.ArtData.Initialize(GraphicsDevice);

                UltimaData.FontsOld.ASCIIText.Initialize(GraphicsDevice);
                UltimaData.FontsNew.TextUni.Initialize(GraphicsDevice);

                UltimaData.GumpData.Initialize(GraphicsDevice);
                UltimaData.HuesXNA.Initialize(GraphicsDevice);
                UltimaData.TexmapData.Initialize(GraphicsDevice);
                UltimaData.StringData.LoadStringList("enu");
                UltimaData.SkillsData.Initialize();
                GraphicsDevice.Textures[1] = UltimaData.HuesXNA.HueTexture0;
                GraphicsDevice.Textures[2] = UltimaData.HuesXNA.HueTexture1;

                UltimaVars.EngineVars.EngineRunning = true;
                UltimaVars.EngineVars.InWorld = false;

                ActiveModel = new UltimaLogin.LoginModel();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            IsFixedTimeStep = UltimaVars.EngineVars.LimitFPS;

            if (!UltimaVars.EngineVars.EngineRunning)
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
                Client.Update();
                ActiveModel.Update(totalMS, frameMS);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsMinimized)
            {
                Core.Rendering.SpriteBatch3D.ResetZ();
                GraphicsDevice.Clear(Color.Black);
                ActiveModel.GetView().Draw(gameTime.ElapsedGameTime.TotalMilliseconds);
                UserInterface.Draw(gameTime.ElapsedGameTime.TotalMilliseconds);

                UltimaVars.EngineVars.UpdateFPS(gameTime.ElapsedGameTime.TotalMilliseconds);
                Window.Title =
                    UltimaVars.DebugVars.Flag_DisplayFPS ? 
                    string.Format("UltimaXNA FPS:{0}", UltimaVars.EngineVars.UpdateFPS(gameTime.ElapsedGameTime.TotalMilliseconds)) : 
                    "UltimaXNA";
            }
        }

        public bool IsMinimized
        {
            get
            {
                //Get out top level form via the handle.
                System.Windows.Forms.Control MainForm = System.Windows.Forms.Form.FromHandle(Window.Handle);
                //If we are minimized don't waste time trying to draw, and avoid crash on resume.
                if (((System.Windows.Forms.Form)MainForm).WindowState == System.Windows.Forms.FormWindowState.Minimized)
                    return true;
                return false;
            }
        }

        // Some settings to designate a screen size and fps limit.
        void setupGraphicsDeviceManager(int width, int height)
        {
            GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferWidth = width;
            graphicsDeviceManager.PreferredBackBufferHeight = height;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            graphicsDeviceManager.PreparingDeviceSettings += onPreparingDeviceSettings;
            IsFixedTimeStep = false;
            graphicsDeviceManager.ApplyChanges();
        }

        static void onPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
    }
}