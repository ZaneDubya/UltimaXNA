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
        public WorldScene()
        {
        }

        public override void  Intitialize()
        {
             base.Intitialize();
             UserInterface.AddWindow("ChatFrame", new Window_Chat());
             UserInterface.AddWindow("ChatInput", new Window_ChatInput());
             UserInterface.AddWindow("StatusFrame", new Window_StatusFrame());
             GameState.InWorld = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            UltimaClient.Disconnect();
            UserInterface.CloseWindow("ChatFrame");
            UserInterface.CloseWindow("ChatInput");
            UserInterface.CloseWindow("StatusFrame");
            GameState.InWorld = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!UltimaClient.IsConnected && SceneManager.CurrentScene.SceneState == SceneState.Active)
            {
                SceneManager.CurrentScene = new LoginScene();
                UserInterface.ErrorPopup_Modal("You have lost your connection with the server.");
                return;
            }
        }
    }
}
