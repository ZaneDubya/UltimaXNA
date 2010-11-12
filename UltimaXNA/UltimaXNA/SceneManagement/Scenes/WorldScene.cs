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
using UltimaXNA.Client;
using UltimaXNA.UILegacy;
using UltimaXNA.UILegacy.ClientsideGumps;
using UltimaXNA.Input;
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
            UI.AddGump_Local(new TopMenu(0), 0, 0);
            UI.AddGump_Local(new ChatWindow(), 0, 0);
            World.LightDirection = -0.6f;
            ClientVars.InWorld = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            UltimaClient.Disconnect();
            ClientVars.InWorld = false;
            UI.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SceneState == SceneState.Active)
            {
                if (!UltimaClient.IsConnected)
                {
                    if (UI.IsModalMsgBoxOpen == false)
                    {
                        MsgBox g = UI.MsgBox("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
                        g.OnClose = onCloseLostConnectionMsgBox;
                    }
                }
                else
                {
                    World.CenterPosition = Entities.EntitiesCollection.GetPlayerObject().Position;
                    World.Update(gameTime);

                    // Toggle for logout
                    if (Input.HandleKeyPress(WinKeys.Q, false, false, true))
                    {
                        SceneManager.CurrentScene = new LoginScene(Game);
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (SceneState == SceneState.Active)
            {
                World.Draw(gameTime);
            }
        }

        void onCloseLostConnectionMsgBox()
        {
            SceneManager.CurrentScene = new LoginScene(Game);
        }
    }
}
