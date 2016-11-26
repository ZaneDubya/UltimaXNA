/***************************************************************************
 *   CoreGame.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using UltimaXNA.Configuration.Properties;
using UltimaXNA.Core.Diagnostics;

namespace UltimaXNA.Core
{
    abstract class CoreGame : Game
    {
        readonly protected GraphicsDeviceManager DeviceManager;
        protected Form GameForm => Control.FromHandle(Window.Handle) as Form;
        protected bool IsMinimized => GameForm.WindowState == FormWindowState.Minimized;
        protected bool IsMaximized => GameForm.WindowState == FormWindowState.Minimized;

        protected CoreGame()
        {
            DeviceManager = new GraphicsDeviceManager(this);
            DeviceManager.PreparingDeviceSettings += OnPreparingDeviceSettings;
        }

        protected void MaximizeWindow()
        {
            GameForm.WindowState = FormWindowState.Maximized;
        }

        protected void RestoreWindow()
        {
            if (GameForm.WindowState != FormWindowState.Normal)
            {
                GameForm.WindowState = FormWindowState.Normal;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Profiler.InContext("OutOfContext"))
            {
                Profiler.ExitContext("OutOfContext");
            }
            Profiler.EnterContext("Update");
            base.Update(gameTime);
            OnUpdate(gameTime.TotalGameTime.TotalMilliseconds, gameTime.ElapsedGameTime.TotalMilliseconds);
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
            OnDraw(gameTime.ElapsedGameTime.TotalMilliseconds);
            Profiler.ExitContext("RenderFrame");
            Profiler.EnterContext("OutOfContext");
            UpdateWindowCaption(gameTime);
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

        protected void CheckWindowSize(int minWidth, int minHeight)
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

        protected void SetGraphicsDeviceWidthHeight(ResolutionProperty resolution)
        {
            DeviceManager.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            DeviceManager.PreferredBackBufferWidth = resolution.Width;
            DeviceManager.PreferredBackBufferHeight = resolution.Height;
            DeviceManager.SynchronizeWithVerticalRetrace = Settings.Engine.IsVSyncEnabled;
            DeviceManager.ApplyChanges();
        }

        void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs args)
        {
            args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        protected abstract void OnUpdate(double totalMS, double frameMS);
        protected abstract void OnDraw(double frameMS);
    }
}
