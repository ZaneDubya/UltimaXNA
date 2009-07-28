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
using UltimaXNA.GUI;
using UltimaXNA.Client;
#endregion

namespace UltimaXNA.SceneManagement
{
    public class LoginScene : BaseScene
    {
        public LoginScene()
        {

        }

        public override void Intitialize()
        {
            base.Intitialize();
            UserInterface.Reset();
            UserInterface.AddWindow("LoginBG", new Window_LoginBG());
            Window_Login w = (Window_Login)UserInterface.AddWindow("LoginWindow", new Window_Login());
            w.OnLogin += this.OnLogin;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SceneState == SceneState.Active)
            {
                switch (UltimaClient.Status)
                {
                    case UltimaClientStatus.Error_Undefined:
                        reset();
                        break;
                    case UltimaClientStatus.WorldServer_InWorld:
                        SceneManager.CurrentScene = new WorldScene();
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            UserInterface.Window("LoginBG").Close();
            UserInterface.Window("LoginWindow").Close();
        }

        public void OnLogin(string server, int port, string account, string password)
        {
            if (connect(server, port))
                login(account,password);
            else
            {
                reset();
            }
        }

        private void reset()
        {
            UltimaClient.Disconnect();
            UserInterface.Window("LoginBG").Close();
            UserInterface.Window("LoginWindow").Close();
            SceneManager.CurrentScene = new LoginScene();
        }

        private bool connect(string host, int port)
        {
            return UltimaClient.Connect(host, port);
        }

        private void login(string username, string password)
        {
            UltimaClient.SetAccountPassword(username, password);
            UltimaClient.Send(new Network.Packets.Client.LoginPacket(username, password));
        }
    }
}
