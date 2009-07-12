using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.GUI;
using UltimaXNA.Client;

namespace UltimaXNA.SceneManagement
{
    public class LoginScene : BaseScene
    {
        public LoginScene(Game game)
            : base(game)
        {

        }

        public override void Intitialize()
        {
            base.Intitialize();
            GUI.AddWindow("LoginBG", new Window_LoginBG());
            Window_Login w = (Window_Login)GUI.AddWindow("LoginWindow", new Window_Login());
            w.OnLogin += this.OnLogin;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SceneState == SceneState.Active)
            {
                switch (Network.Status)
                {
                    case UltimaClientStatus.Error_Undefined:
                        reset();
                        break;
                    case UltimaClientStatus.WorldServer_InWorld:
                        SceneManager.CurrentScene = new WorldScene(Game);
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            GUI.Window("LoginBG").Close();
            GUI.Window("LoginWindow").Close();
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
            Network.Disconnect();
            GUI.Window("LoginBG").Close();
            GUI.Window("LoginWindow").Close();
            SceneManager.CurrentScene = new LoginScene(Game);
        }

        private bool connect(string host, int port)
        {
            return Network.Connect(host, port);
        }

        private void login(string username, string password)
        {
            Network.SetAccountPassword(username, password);
            Network.Send(new Network.Packets.Client.LoginPacket(username, password));
        }
    }
}
