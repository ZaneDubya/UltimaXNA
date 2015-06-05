/***************************************************************************
 *   LoginScene.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using System;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data.Servers;
using UltimaXNA.Ultima.Login.Gumps;
#endregion

namespace UltimaXNA.Ultima.Login.States
{
    public class SelectServerState : AState
    {
        UserInterfaceService m_UserInterface;
        LoginModel m_Login;

        private SelectServerGump m_SelectServerGump;

        public SelectServerState()
            : base()
        {
            
        }

        public override void Intitialize()
        {
            base.Intitialize();

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_Login = ServiceRegistry.GetService<LoginModel>();

            m_SelectServerGump = (SelectServerGump)m_UserInterface.AddControl(new SelectServerGump(), 0, 0);
            m_SelectServerGump.OnBackToLoginScreen += OnBackToLoginScreen;
            m_SelectServerGump.OnSelectLastServer += OnSelectLastServer;
            m_SelectServerGump.OnSelectServer += OnSelectServer;
        }

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if(SceneState == SceneState.Active)
            {
                switch (m_Login.Client.Status)
                {
                    case LoginClientStatus.LoginServer_HasServerList:
                        if(Servers.List.Length == 1)
                        {
                            OnSelectServer(0);
                        }
                        // This is where we're supposed to be while waiting to select a server.
                        break;
                    case LoginClientStatus.LoginServer_WaitingForRelay:
                        // we must now send the relay packet.
                        m_Login.Client.Relay();
                        break;
                    case LoginClientStatus.LoginServer_Relaying:
                        // relaying to the server we will log in to ...
                        break;
                    case LoginClientStatus.GameServer_Connecting:
                        // we are logging in to the shard.
                        break;
                    case LoginClientStatus.GameServer_CharList:
                        // we've got the char list
                        Manager.CurrentState = new CharacterListState();
                        break;
                    case LoginClientStatus.WorldServer_InWorld:
                        // we've connected! Client takes us into the world and disposes of this Model.
                        break;
                    default:
                        // what's going on here? Add additional error handlers.
                        throw (new Exception("Unknown UltimaClientStatus in ServerSelectScene:Update"));
                }
            }
        }

        public override void Dispose()
        {
            m_SelectServerGump.Dispose();
            base.Dispose();
        }

        public void OnBackToLoginScreen()
        {
            Manager.ResetToLoginScreen();
        }

        public void OnSelectServer(int index)
        {
            m_SelectServerGump.ActivePage = 2;
            m_Login.Client.SelectShard(index);
        }

        public void OnSelectLastServer()
        {
            // select the last server.
        }
    }
}