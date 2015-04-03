/***************************************************************************
 *
 *   LoginScene.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.LoginGumps;
#endregion

namespace UltimaXNA.UltimaLogin.Scenes
{
    public class LoggingInScene : AScene
    {
        private LoggingInGump m_Gump;

        private string m_ServerHost;
        private int m_ServerPort;
        private string m_AccountName;
        private string m_Password;

        private bool m_ErrorReceived = false;

        public LoggingInScene(string server, int port, string account, string password)
        {
            // Todo: Send the accountname and password to the ultimaclient so this gump does not have to save them.
            m_ServerHost = server;
            m_ServerPort = port;
            m_AccountName = account;
            m_Password = password;
        }

        public override void Intitialize(UltimaEngine engine)
        {
            base.Intitialize(engine);
            m_Gump = (LoggingInGump)UltimaEngine.UserInterface.AddControl(new LoggingInGump(), 0, 0);
            m_Gump.OnCancelLogin += this.OnCancelLogin;
            if (Engine.Client.IsConnected)
                Engine.Client.Disconnect();
        }

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if (SceneState == SceneState.Active)
            {
                if (!m_ErrorReceived)
                {
                    switch (Engine.Client.Status)
                    {
                        case UltimaClientStatus.Unconnected:
                            Engine.Client.Connect(m_ServerHost, m_ServerPort);
                            break;
                        case UltimaClientStatus.LoginServer_Connecting:
                            // connecting ...
                            break;
                        case UltimaClientStatus.LoginServer_WaitingForLogin:
                            // show 'verifying account...' gump
                            m_Gump.ActivePage = 9;
                            Engine.Client.SendAccountLogin(m_AccountName, m_Password);
                            break;
                        case UltimaClientStatus.LoginServer_LoggingIn:
                            // logging in ...
                            break;
                        case UltimaClientStatus.LoginServer_HasServerList:
                            Manager.CurrentScene = new SelectServerScene(m_AccountName, m_Password);
                            break;
                        case UltimaClientStatus.Error_CannotConnectToServer:
                            m_Gump.ActivePage = 2;
                            m_ErrorReceived = true;
                            break;
                        case UltimaClientStatus.Error_InvalidUsernamePassword:
                            m_Gump.ActivePage = 3;
                            m_ErrorReceived = true;
                            break;
                        case UltimaClientStatus.Error_InUse:
                            m_Gump.ActivePage = 4;
                            m_ErrorReceived = true;
                            break;
                        case UltimaClientStatus.Error_Blocked:
                            m_Gump.ActivePage = 5;
                            m_ErrorReceived = true;
                            break;
                        case UltimaClientStatus.Error_BadPassword:
                            m_Gump.ActivePage = 6;
                            m_ErrorReceived = true;
                            break;
                        case UltimaClientStatus.Error_Idle:
                            m_Gump.ActivePage = 7;
                            m_ErrorReceived = true;
                            break;
                        case UltimaClientStatus.Error_BadCommunication:
                            m_Gump.ActivePage = 8;
                            m_ErrorReceived = true;
                            break;
                        default:
                            // what's going on here? Add additional error handlers.
                            m_ErrorReceived = true;
                            throw (new Exception("Unknown UltimaClientStatus in LoggingInScene:Update"));
                    }
                }
            }
        }

        public override void Dispose()
        {
            m_Gump.Dispose();
            base.Dispose();
        }

        public void OnCancelLogin()
        {
            Manager.ResetToLoginScreen();
        }
    }
}
