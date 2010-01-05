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
using UltimaXNA.UILegacy;
#endregion

namespace UltimaXNA.SceneManagement
{
    public class LoggingInScene : BaseScene
    {
        public LoggingInScene(Game game, string server, int port, string account, string password)
            : base(game, true)
        {
            // Send the accountname and password to the ultimaclient so this gump does not have to save them.
            UltimaClient.SetLoginData(server, port, account, password);
        }

        public override void Intitialize()
        {
            base.Intitialize();
            Gump g = UI.AddGump(new UILegacy.Clientside.LoggingInGump(), 0, 0);
            ((UILegacy.Clientside.LoggingInGump)g).OnCancelLogin += this.OnCancelLogin;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SceneState == SceneState.Active)
            {
                switch (UltimaClient.Status)
                {
                    case UltimaClientStatus.Unconnected:
                        UltimaClient.Connect();
                        break;
                    case UltimaClientStatus.LoginServer_Connecting:
                        // connecting ...
                        break;
                    case UltimaClientStatus.LoginServer_WaitingForLogin:
                        // show 'verifying account...' gump
                        UI.GetGump<UILegacy.Clientside.LoggingInGump>(0).ActivePage = 9;
                        UltimaClient.Login();
                        break;
                    case UltimaClientStatus.LoginServer_LoggingIn:
                        // logging in ...
                        break;
                    case UltimaClientStatus.WorldServer_InWorld:
                        // we've connected!
                        SceneManager.CurrentScene = new WorldScene(Game);
                        break;
                    case UltimaClientStatus.Error_CannotConnectToServer:
                        UI.GetGump<UILegacy.Clientside.LoggingInGump>(0).ActivePage = 2;
                        // could not connect to server.
                        break;
                    case UltimaClientStatus.Error_InvalidUsernamePassword:
                        UI.GetGump<UILegacy.Clientside.LoggingInGump>(0).ActivePage = 3;
                        break;
                    case UltimaClientStatus.Error_InUse:
                        UI.GetGump<UILegacy.Clientside.LoggingInGump>(0).ActivePage = 4;
                        break;
                    case UltimaClientStatus.Error_Blocked:
                        UI.GetGump<UILegacy.Clientside.LoggingInGump>(0).ActivePage = 5;
                        break;
                    case UltimaClientStatus.Error_BadPassword:
                        UI.GetGump<UILegacy.Clientside.LoggingInGump>(0).ActivePage = 6;
                        break;
                    case UltimaClientStatus.Error_Idle:
                        UI.GetGump<UILegacy.Clientside.LoggingInGump>(0).ActivePage = 7;
                        break;
                    case UltimaClientStatus.Error_BadCommunication:
                        UI.GetGump<UILegacy.Clientside.LoggingInGump>(0).ActivePage = 8;
                        break;
                    default:
                        // what's going on here? Add additional error handlers.
                        break;
                }
            }
        }

        public override void Dispose()
        {
            UI.GetGump<UILegacy.Clientside.LoggingInGump>(0).Dispose();
            base.Dispose();
        }

        public void OnCancelLogin()
        {
            UltimaClient.Disconnect();
            SceneManager.CurrentScene = new LoginScene(Game);
        }
    }
}
