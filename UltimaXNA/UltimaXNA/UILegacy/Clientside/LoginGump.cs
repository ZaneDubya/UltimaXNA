using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;
using UltimaXNA.Graphics;
using UltimaXNA.Client;

namespace UltimaXNA.UILegacy.Clientside
{
    enum LoginGumpButtons
    {
        QuitButton = 0,
        LoginButton = 1
    }
    enum LoginGumpTextFields
    {
        AccountName = 0,
        Password = 1
    }
    public class LoginGump : Gump
    {
        public LoginGump(Serial serial)
            : base(serial, 0)
        {
            _renderFullScreen = false;
            // backdrop
            this.AddGumpling(new Gumplings.GumpPic(this, 0, 0, 0, 9001,0));
            // quit button
            this.AddGumpling(new Gumplings.Button(this, 0, 554, 2, 5513, 5515, 1, 0, (int)LoginGumpButtons.QuitButton));
            ((Gumplings.Button)_controls[_controls.Count - 1]).GumpOverID = 5514;
            // Log in to Ultima Online
            this.AddGumpling(new Gumplings.HtmlGump(this, 0, 253, 312, 256, 20, 0, 0, "<basefont color=#bdb5bd><big>Login to Ultima Online</big>"));
            this.AddGumpling(new Gumplings.HtmlGump(this, 0, 254, 311, 256, 20, 0, 0, "<basefont color=#423931><big>Login to Ultima Online</big>"));
            // Account Name
            this.AddGumpling(new Gumplings.HtmlGump(this, 0, 182, 352, 256, 20, 0, 0, "<basefont color=#bdb5bd><big>Account Name</big>"));
            this.AddGumpling(new Gumplings.HtmlGump(this, 0, 183, 351, 256, 20, 0, 0, "<basefont color=#423931><big>Account Name</big>"));
            // Password
            this.AddGumpling(new Gumplings.HtmlGump(this, 0, 182, 392, 256, 20, 0, 0, "<basefont color=#bdb5bd><big>Password</big>"));
            this.AddGumpling(new Gumplings.HtmlGump(this, 0, 183, 391, 256, 20, 0, 0, "<basefont color=#423931><big>Password</big>"));
            // name field
            TextEntry g1 = new Gumplings.TextEntry(this, 0, 332, 346, 200, 20, 0, (int)LoginGumpTextFields.AccountName, 32, "Admin");
            this.AddGumpling(new ResizePic(this, g1));
            this.AddGumpling(g1);
            // password field
            TextEntry g2 = new Gumplings.TextEntry(this, 0, 332, 386, 200, 20, 0, (int)LoginGumpTextFields.Password, 32, "Admin");
            this.AddGumpling(new ResizePic(this, g2));
            this.AddGumpling(g2);
            // login button
            this.AddGumpling(new Gumplings.Button(this, 0, 610, 435, 5540, 5542, 1, 0, (int)LoginGumpButtons.LoginButton));
            ((Gumplings.Button)_controls[_controls.Count - 1]).GumpOverID = 5541;
            // Version information
            this.AddGumpling(new Gumplings.HtmlGump(this, 0, 183, 421, 256, 20, 0, 0, Utility.VersionString));
        }

        public override void Activate(Control c)
        {
            switch ((LoginGumpButtons)(((Button)c).ButtonID))
            {
                case LoginGumpButtons.QuitButton:
                    Quit();
                    break;
                case LoginGumpButtons.LoginButton:
                    if (connect("localhost", 2593))
                    {
                        string accountName = getTextEntry((int)LoginGumpTextFields.AccountName);
                        string password = getTextEntry((int)LoginGumpTextFields.Password);
                        login(accountName, password);
                        this.Dispose();
                    }
                    else
                    {
                        reset();
                    }
                    break;
            }
        }

        private void reset()
        {
            UltimaClient.Disconnect();
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
