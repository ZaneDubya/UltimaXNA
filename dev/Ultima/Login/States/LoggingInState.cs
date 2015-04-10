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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Patterns.IoC;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.LoginGumps;
#endregion

namespace UltimaXNA.Ultima.Login.States
{
    public class LoggingInState : AState
    {
        private readonly INetworkClient m_Network;

        private LoggingInGump m_Gump;
        
        private bool m_ErrorReceived = false;

        public LoggingInState(IContainer container)
            : base(container)
        {
            m_Network = container.Resolve<INetworkClient>();
            // Todo: Send the accountname and password to the ultimaclient so this gump does not have to save them.
        }

        public override void Intitialize()
        {
            base.Intitialize();

            m_Gump = (LoggingInGump)Engine.UserInterface.AddControl(new LoggingInGump(), 0, 0);
            m_Gump.OnCancelLogin += OnCancelLogin;

            if (m_Network.IsConnected)
            {
                m_Network.Disconnect();
            }
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
                            string serverAddress = Settings.Server.ServerAddress;
                            int serverPort = Settings.Server.ServerPort;
                            Engine.Client.Connect(serverAddress, serverPort);
                            break;
                        case UltimaClientStatus.LoginServer_Connecting:
                            // connecting ...
                            break;
                        case UltimaClientStatus.LoginServer_WaitingForLogin:
                            // show 'verifying account...' gump
                            m_Gump.ActivePage = 9;
                            Engine.Client.Login();
                            break;
                        case UltimaClientStatus.LoginServer_LoggingIn:
                            // logging in ...
                            break;
                        case UltimaClientStatus.LoginServer_HasServerList:
                            Manager.CurrentScene = Container.Resolve<SelectServerState>();
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
