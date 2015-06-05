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
using UltimaXNA.Configuration;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.Login.Gumps;
#endregion

namespace UltimaXNA.Ultima.Login.States
{
    public class LoggingInState : AState
    {
        UserInterfaceService m_UserInterface;
        LoginModel m_Login;

        private LoggingInGump m_Gump;
        
        private bool m_ErrorReceived = false;

        public LoggingInState()
            : base()
        {
            // Todo: Send the accountname and password to the ultimaclient so this gump does not have to save them.
        }

        public override void Intitialize()
        {
            base.Intitialize();

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_Login = ServiceRegistry.GetService<LoginModel>();

            m_Gump = (LoggingInGump)m_UserInterface.AddControl(new LoggingInGump(), 0, 0);
            m_Gump.OnCancelLogin += OnCancelLogin;

            m_Login.Client.Disconnect();
        }

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if (SceneState == SceneState.Active)
            {
                if (!m_ErrorReceived)
                {
                    switch (m_Login.Client.Status)
                    {
                        case LoginClientStatus.Unconnected:
                            string serverAddress = Settings.Server.ServerAddress;
                            int serverPort = Settings.Server.ServerPort;
                            m_Login.Client.Connect(serverAddress, serverPort);
                            break;
                        case LoginClientStatus.LoginServer_Connecting:
                            // connecting ...
                            break;
                        case LoginClientStatus.LoginServer_WaitingForLogin:
                            // show 'verifying account...' gump
                            m_Gump.ActivePage = 9;
                            m_Login.Client.Login();
                            break;
                        case LoginClientStatus.LoginServer_LoggingIn:
                            // logging in ...
                            break;
                        case LoginClientStatus.LoginServer_HasServerList:
                            Manager.CurrentState = new SelectServerState();
                            break;
                        case LoginClientStatus.Error_CannotConnectToServer:
                            m_Gump.ActivePage = 2;
                            m_ErrorReceived = true;
                            break;
                        case LoginClientStatus.Error_InvalidUsernamePassword:
                            m_Gump.ActivePage = 3;
                            m_ErrorReceived = true;
                            break;
                        case LoginClientStatus.Error_InUse:
                            m_Gump.ActivePage = 4;
                            m_ErrorReceived = true;
                            break;
                        case LoginClientStatus.Error_Blocked:
                            m_Gump.ActivePage = 5;
                            m_ErrorReceived = true;
                            break;
                        case LoginClientStatus.Error_BadPassword:
                            m_Gump.ActivePage = 6;
                            m_ErrorReceived = true;
                            break;
                        case LoginClientStatus.Error_Idle:
                            m_Gump.ActivePage = 7;
                            m_ErrorReceived = true;
                            break;
                        case LoginClientStatus.Error_BadCommunication:
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
