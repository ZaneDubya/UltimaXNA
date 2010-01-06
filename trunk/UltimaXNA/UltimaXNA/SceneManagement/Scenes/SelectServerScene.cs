/***************************************************************************
 *   LoginScene.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Client;
using UltimaXNA.Network;
using UltimaXNA.UILegacy;
#endregion

namespace UltimaXNA.SceneManagement
{
    public class SelectServerScene : BaseScene
    {
        public SelectServerScene(Game game)
            : base(game, true)
        {

        }

        public override void Intitialize()
        {
            base.Intitialize();
            Gump g = UI.AddGump(new UILegacy.Clientside.SelectServerGump(UltimaClient.ServerListPacket), 0, 0);
            ((UILegacy.Clientside.SelectServerGump)g).OnCancelLogin += this.OnCancelLogin;

            UltimaClient.SelectServer(UltimaClient.ServerListPacket.Servers[0].Index);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SceneState == SceneState.Active)
            {
                switch (UltimaClient.Status)
                {
                    case UltimaClientStatus.LoginServer_HasServerList:
                        // This is where we're supposed to be while waiting to select a server.
                        break;
                    case UltimaClientStatus.WorldServer_InWorld:
                        // we've connected!
                        SceneManager.CurrentScene = new WorldScene(Game);
                        break;
                    default:
                        // what's going on here? Add additional error handlers.
                        throw (new Exception("Unknown UltimaClientStatus in ServerSelectScene:Update"));
                }
            }
        }

        public override void Dispose()
        {
            UI.GetGump<UILegacy.Clientside.SelectServerGump>(0).Dispose();
            base.Dispose();
        }

        public void OnCancelLogin()
        {
            UltimaClient.Disconnect();
            SceneManager.CurrentScene = new LoginScene(Game);
        }
    }
}
