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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA
{
    public class UltimaEngine : Core.BaseEngine
    {
        private static UltimaClient s_Client = new UltimaClient();

        private static Core.AUltimaModel m_Model;
        internal static Core.AUltimaModel ActiveModel
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
                m_Model.Initialize(s_Client);
            }
        }

        public UltimaEngine(int width, int height)
            :base(width, height)
        {
            UltimaVars.EngineVars.ScreenSize = new Point(width, height);

            // this is copied from IXL.BaseCore - required for finding the mouse coordinate when moving the cursor over the window.
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(this.Window.Handle);
            InterXLib.Settings.ScreenDPI = new Vector2(graphics.DpiX / 96f, graphics.DpiY / 96f);

            s_Client.LogPackets = true;
        }

        protected override void OnInitialize()
        {
            UltimaWorld.View.IsometricRenderer.Initialize(this);

            // Make sure we have a UO installation before loading UltimaData.
            if (UltimaData.FileManager.IsUODataPresent)
            {
                // Initialize and load data
                UltimaData.AnimationData.Initialize(GraphicsDevice);
                UltimaData.ArtData.Initialize(GraphicsDevice);
                UltimaData.Fonts.ASCIIText.Initialize(GraphicsDevice);
                UltimaData.Fonts.UniText.Initialize(GraphicsDevice);
                UltimaData.GumpData.Initialize(GraphicsDevice);
                UltimaData.HuesXNA.Initialize(GraphicsDevice);
                UltimaData.TexmapData.Initialize(GraphicsDevice);
                UltimaData.StringData.LoadStringList("enu");
                UltimaData.SkillsData.Initialize();
                GraphicsDevice.Textures[1] = UltimaXNA.UltimaData.HuesXNA.HueTexture;

                UltimaData.FontsNew.TextASCII.Initialize(GraphicsDevice);
                UltimaData.FontsNew.TextUni.Initialize(GraphicsDevice);

                UltimaVars.EngineVars.EngineRunning = true;
                UltimaVars.EngineVars.InWorld = false;
                UltimaInteraction.Initialize(s_Client);

                ActiveModel = new UltimaLogin.LoginModel();
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            this.IsFixedTimeStep = UltimaVars.EngineVars.LimitFPS;
            if (!UltimaVars.EngineVars.EngineRunning)
            {
                Exit();
            }
            else
            {
                UltimaVars.EngineVars.GameTime = gameTime;
                UserInterface.Update(gameTime);
                s_Client.Update();
                ActiveModel.Update(gameTime.TotalGameTime.TotalMilliseconds, gameTime.ElapsedGameTime.TotalMilliseconds);
            }
        }

        protected override void OnDraw(GameTime gameTime)
        {
            ActiveModel.GetView().Draw(null, gameTime.ElapsedGameTime.TotalMilliseconds);
        }
    }
}