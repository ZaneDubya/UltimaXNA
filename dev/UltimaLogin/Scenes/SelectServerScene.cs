/***************************************************************************
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaGUI.LoginGumps;
#endregion

namespace UltimaXNA.UltimaLogin.Scenes
{
    public class SelectServerScene : AScene
    {
        private SelectServerGump m_SelectServerGump;

        private string m_AccountName;
        private string m_Password;

        public SelectServerScene(string account, string password)
        {
            m_AccountName = account;
            m_Password = password;
        }

        public override void Intitialize(UltimaEngine engine)
        {
            base.Intitialize(engine);
            m_SelectServerGump = (SelectServerGump)UltimaEngine.UserInterface.AddControl(new SelectServerGump(), 0, 0);
            m_SelectServerGump.OnBackToLoginScreen += this.OnBackToLoginScreen;
            m_SelectServerGump.OnSelectLastServer += this.OnSelectLastServer;
            m_SelectServerGump.OnSelectServer += this.OnSelectServer;
        }

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if (SceneState == SceneState.Active)
            {
                switch (Engine.Client.Status)
                {
                    case UltimaClientStatus.LoginServer_HasServerList:
                        // This is where we're supposed to be while waiting to select a server.
                        break;
                    case UltimaClientStatus.LoginServer_WaitingForRelay:
                        // we must now send the relay packet.
                        Engine.Client.SendServerRelay(m_AccountName, m_Password);
                        break;
                    case UltimaClientStatus.LoginServer_Relaying:
                        // relaying to the server we will log in to ...
                        break;
                    case UltimaClientStatus.GameServer_Connecting:
                        // we are logging in to the shard.
                        break;
                    case UltimaClientStatus.GameServer_CharList:
                        // we've got the char list
                        Manager.CurrentScene = new CharacterListScene();
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
            Engine.Client.SelectServer(index);
        }

        public void OnSelectLastServer()
        {
            // select the last server.
        }
    }
}
