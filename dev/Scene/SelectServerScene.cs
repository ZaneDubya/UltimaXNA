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
    public class SelectServerScene : BaseScene
    {
        private string m_AccountName;
        private string m_Password;

        public SelectServerScene(Game game, string account, string password)
            : base(game, true)
        {
            m_AccountName = account;
            m_Password = password;
        }

        public override void Intitialize()
        {
            base.Intitialize();
            Gump g = (Gump)UltimaEngine.UserInterface.AddControl(new UltimaGUI.ClientsideGumps.SelectServerGump(), 0, 0);
            ((UltimaGUI.ClientsideGumps.SelectServerGump)g).OnBackToLoginScreen += this.OnBackToLoginScreen;
            ((UltimaGUI.ClientsideGumps.SelectServerGump)g).OnSelectLastServer += this.OnSelectLastServer;
            ((UltimaGUI.ClientsideGumps.SelectServerGump)g).OnSelectServer += this.OnSelectServer;
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
                    case UltimaClientStatus.LoginServer_WaitingForRelay:
                        // we must now send the relay packet.
                        UltimaClient.SendServerRelay(m_AccountName, m_Password);
                        break;
                    case UltimaClientStatus.LoginServer_Relaying:
                        // relaying to the server we will log in to ...
                        break;
                    case UltimaClientStatus.GameServer_Connecting:
                        // we are logging in to the shard.
                        break;
                    case UltimaClientStatus.GameServer_CharList:
                        // we've got the char list
                        SceneManager.CurrentScene = new CharacterListScene(Game);
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
            UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.SelectServerGump>(0).Dispose();
            base.Dispose();
        }

        public void OnBackToLoginScreen()
        {
            UltimaInteraction.DisconnectToLoginScreen();
        }

        public void OnSelectServer(int index)
        {
            UltimaEngine.UserInterface.GetControl<UltimaGUI.ClientsideGumps.SelectServerGump>(0).ActivePage = 2;
            UltimaClient.SelectServer(index);
        }

        public void OnSelectLastServer()
        {
            // select the last server.
        }
    }
}
