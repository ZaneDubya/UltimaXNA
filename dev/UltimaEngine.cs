/***************************************************************************
 *   Engine.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Scenes;
using UltimaXNA.UltimaNetwork;
using UltimaXNA.UltimaWorld;
#endregion

namespace UltimaXNA
{
    public class UltimaEngine : BaseEngine
    {
        public static UltimaGUIState UltimaUI = new UltimaGUIState();

        public UltimaEngine(int width, int height)
            :base(width, height)
        {
            UltimaVars.EngineVars.ScreenSize = new Point2D(width, height);
        }

        protected override void OnInitialize()
        {
            SceneManager.Initialize(this);
            IsometricRenderer.Initialize(this);

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
                SceneManager.Reset();
                UltimaVars.EngineVars.EngineRunning = true;
                UltimaVars.EngineVars.InWorld = false;
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            this.IsFixedTimeStep = UltimaVars.EngineVars.LimitFPS;
            if (!UltimaVars.EngineVars.EngineRunning)
                Exit();

            UltimaUI.Update();
            UltimaClient.Update(gameTime);
            EntityManager.Update(gameTime);
            UltimaGameState.Update(gameTime);
            SceneManager.Update(gameTime);
        }

        protected override void OnDraw(GameTime gameTime)
        {
            SceneManager.Draw(gameTime);
            UltimaUI.Draw();
        }
    }
}