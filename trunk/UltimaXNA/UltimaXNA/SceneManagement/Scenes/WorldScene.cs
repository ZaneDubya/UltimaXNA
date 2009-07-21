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
using UltimaXNA.GUI;
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
             GUI.AddWindow("ChatFrame", new Window_Chat());
             GUI.AddWindow("ChatInput", new Window_ChatInput());
             GUI.AddWindow("StatusFrame", new Window_StatusFrame());
             ((IGameState)Game.Services.GetService(typeof(IGameState))).InWorld = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            Network.Disconnect();
            GUI.CloseWindow("ChatFrame");
            GUI.CloseWindow("ChatInput");
            GUI.CloseWindow("StatusFrame");
            ((IGameState)Game.Services.GetService(typeof(IGameState))).InWorld = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Network.IsConnected && SceneManager.CurrentScene.SceneState == SceneState.Active)
            {
                SceneManager.CurrentScene = new LoginScene(Game);
                GUI.ErrorPopup_Modal("You have lost your connection with the server.");
                return;
            }
        }
    }
}
