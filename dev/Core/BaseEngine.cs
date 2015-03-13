/***************************************************************************
 *   BaseEngine.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InterXLib;
#endregion

namespace UltimaXNA.Core
{
    /// <summary>
    /// Encapsulates basic engine functions, allowing the main engine to be kept
    /// clean of extraneous code.
    /// </summary>
    public class BaseEngine : Game
    {
        public static InputState Input = new InputState();
        public static GUIState UserInterface = new GUIState();

        public BaseEngine(int width, int height)
        {
            setupGraphicsDeviceManager(width, height);
        }

        protected override void Initialize()
        {
            this.Content.RootDirectory = "Content";
            // Initialize all the services we need.
            Input.Initialize(this.Window.Handle);
            UserInterface.Initialize(this);
            OnInitialize();
        }

        protected override void Update(GameTime gameTime)
        {
            Rendering.SpriteBatch3D.ResetZ();
            Input.Update(gameTime.TotalGameTime.TotalMilliseconds, gameTime.ElapsedGameTime.TotalMilliseconds);
            OnUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsMinimized)
            {
                this.GraphicsDevice.Clear(Color.Black);
                OnDraw(gameTime);
                this.Window.Title = 
                    (UltimaVars.DebugVars.Flag_DisplayFPS ? string.Format("UltimaXNA FPS:{0}", UltimaVars.EngineVars.FPS) : "UltimaXNA") + 
                    (UltimaVars.EngineVars.MouseEnabled ? "" : "<Alt-M to enable mouse>");
            }
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnUpdate(GameTime gameTime) { }
        protected virtual void OnDraw(GameTime gameTime) { }

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
            this.IsFixedTimeStep = false;
            graphicsDeviceManager.ApplyChanges();
        }

        static void onPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
    }
}