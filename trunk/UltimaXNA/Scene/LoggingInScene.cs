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
using UltimaXNA.Network;
using UltimaXNA.UltimaGUI;
#endregion

namespace UltimaXNA.Scene
{
    public class LoggingInScene : BaseScene
    {
        private string m_ServerHost;
        private int m_ServerPort;
        private string m_AccountName;
        private string m_Password;

        public LoggingInScene(Game game, string server, int port, string account, string password)
            : base(game, true)
        {
            if (UltimaClient.IsConnected)
                UltimaClient.Disconnect();
            // Send the accountname and password to the ultimaclient so this gump does not have to save them.

            m_ServerHost = server;
            m_ServerPort = port;
            m_AccountName = account;
            m_Password = password;
        }

        public override void Intitialize()
        {
            base.Intitialize();
            Gump g = (Gump)UltimaEngine.UserInterface.AddControl(new UltimaGUI.ClientsideGumps.LoggingInGump(), 0, 0);
            ((UltimaGUI.ClientsideGumps.LoggingInGump)g).OnCancelLogin += this.OnCancelLogin;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SceneState == SceneState.Active)
            {
                switch (UltimaClient.Status)
                {
                    case UltimaClientStatus.Unconnected:
                        UltimaClient.Connect(m_ServerHost, m_ServerPort);
                        break;
                    case UltimaClientStatus.LoginServer_Connecting:
                        // connecting ...
                        break;
                    case UltimaClientStatus.LoginServer_WaitingForLogin:
                        // show 'verifying account...' gump
                        UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.LoggingInGump>(0).ActivePage = 9;
                        UltimaClient.SendAccountLogin(m_AccountName, m_Password);
                        break;
                    case UltimaClientStatus.LoginServer_LoggingIn:
                        // logging in ...
                        break;
                    case UltimaClientStatus.LoginServer_HasServerList:
                        SceneManager.CurrentScene = new SelectServerScene(Game, m_AccountName, m_Password);
                        break;
                    case UltimaClientStatus.Error_CannotConnectToServer:
                        UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.LoggingInGump>(0).ActivePage = 2;
                        // could not connect to server.
                        break;
                    case UltimaClientStatus.Error_InvalidUsernamePassword:
                        UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.LoggingInGump>(0).ActivePage = 3;
                        break;
                    case UltimaClientStatus.Error_InUse:
                        UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.LoggingInGump>(0).ActivePage = 4;
                        break;
                    case UltimaClientStatus.Error_Blocked:
                        UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.LoggingInGump>(0).ActivePage = 5;
                        break;
                    case UltimaClientStatus.Error_BadPassword:
                        UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.LoggingInGump>(0).ActivePage = 6;
                        break;
                    case UltimaClientStatus.Error_Idle:
                        UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.LoggingInGump>(0).ActivePage = 7;
                        break;
                    case UltimaClientStatus.Error_BadCommunication:
                        UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.LoggingInGump>(0).ActivePage = 8;
                        break;
                    default:
                        // what's going on here? Add additional error handlers.
                        throw (new Exception("Unknown UltimaClientStatus in LoggingInScene:Update"));
                }
            }
        }

        public override void Dispose()
        {
            UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.LoggingInGump>(0).Dispose();
            base.Dispose();
        }

        public void OnCancelLogin()
        {
            UltimaInteraction.DisconnectToLoginScreen();
        }
    }
}
