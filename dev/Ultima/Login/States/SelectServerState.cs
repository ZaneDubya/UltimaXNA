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
using UltimaXNA.Core.Patterns.IoC;
using UltimaXNA.Ultima.UI.LoginGumps;
using UltimaXNA.Ultima.Data.Servers;
#endregion

namespace UltimaXNA.Ultima.Login.States
{
    public class SelectServerState : AState
    {
        private SelectServerGump m_SelectServerGump;

        public SelectServerState(IContainer container)
            : base(container)
        {
            
        }

        public override void Intitialize()
        {
            base.Intitialize();

            m_SelectServerGump = (SelectServerGump)Engine.UserInterface.AddControl(new SelectServerGump(), 0, 0);
            m_SelectServerGump.OnBackToLoginScreen += OnBackToLoginScreen;
            m_SelectServerGump.OnSelectLastServer += OnSelectLastServer;
            m_SelectServerGump.OnSelectServer += OnSelectServer;
        }

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if(SceneState == SceneState.Active)
            {
                switch(Engine.Client.Status)
                {
                    case UltimaClientStatus.LoginServer_HasServerList:
                        if(Servers.List.Length == 1)
                        {
                            OnSelectServer(0);
                        }
                        // This is where we're supposed to be while waiting to select a server.
                        break;
                    case UltimaClientStatus.LoginServer_WaitingForRelay:
                        // we must now send the relay packet.
                        Engine.Client.Relay();
                        break;
                    case UltimaClientStatus.LoginServer_Relaying:
                        // relaying to the server we will log in to ...
                        break;
                    case UltimaClientStatus.GameServer_Connecting:
                        // we are logging in to the shard.
                        break;
                    case UltimaClientStatus.GameServer_CharList:
                        // we've got the char list
                        Manager.CurrentScene = Container.Resolve<CharacterListState>();
                        break;
                    case UltimaClientStatus.WorldServer_InWorld:
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
            Engine.Client.SelectShard(index);
        }

        public void OnSelectLastServer()
        {
            // select the last server.
        }
    }
}