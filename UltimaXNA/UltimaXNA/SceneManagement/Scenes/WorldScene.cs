/***************************************************************************
 *   WorldScene.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.UI;
using UltimaXNA.Client;
#endregion

namespace UltimaXNA.SceneManagement
{
    public class WorldScene : BaseScene
    {
        public WorldScene(Game game)
            : base(game)
        {
        }

        public override void  Intitialize()
        {
             base.Intitialize();
             UserInterface.AddWindow("ChatFrame", new Window_Chat());
             UserInterface.AddWindow("ChatInput", new Window_ChatInput());
             World.LightDirection = -0.6f;
             GameState.InWorld = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            UltimaClient.Disconnect();
            UserInterface.CloseWindow("ChatFrame");
            UserInterface.CloseWindow("ChatInput");
            GameState.InWorld = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!UltimaClient.IsConnected && SceneManager.CurrentScene.SceneState == SceneState.Active)
            {
                SceneManager.CurrentScene = new LoginScene(Game);
                UserInterface.ErrorPopup_Modal("You have lost your connection with the server.");
                return;
            }

            World.CenterPosition = Entities.EntitiesCollection.GetPlayerObject().Movement.DrawPosition;
            World.Update(gameTime);

            // Toggle for logout
            if (Input.IsKeyPress(Keys.Q) && (Input.IsKeyDown(Keys.LeftControl)))
            {
                SceneManager.CurrentScene = new LoginScene(Game);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            World.Draw(gameTime);
        }
    }
}
