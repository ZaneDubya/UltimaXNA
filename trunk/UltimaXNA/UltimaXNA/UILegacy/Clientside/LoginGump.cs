using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;
using UltimaXNA.Client;

namespace UltimaXNA.UILegacy.Clientside
{
    public delegate void LoginEvent(string server, int port, string account, string password);

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
        public LoginEvent OnLogin;

        public LoginGump()
            : base(0, 0)
        {
            int hue = 1115;
            _renderFullScreen = false;
            // backdrop
            this.AddGumpling(new GumpPic(this, 0, 0, 0, 9001,0));
            // quit button
            this.AddGumpling(new Button(this, 0, 554, 2, 5513, 5515, 1, 0, (int)LoginGumpButtons.QuitButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5514;
            // Log in to Ultima Online
            AddGumpling(new TextLabelAscii(this, 0, 254, 305, hue, 2, Data.StringList.Table[1077841].ToString()));
            // Account Name
            AddGumpling(new TextLabelAscii(this, 0, 181, 346, hue, 2, Data.StringList.Table[1077842].ToString()));
            // Password
            AddGumpling(new TextLabelAscii(this, 0, 181, 386, hue, 2, Data.StringList.Table[3000103].ToString()));
            // name field
            TextEntry g1 = new TextEntry(this, 0, 332, 346, 200, 20, 0, (int)LoginGumpTextFields.AccountName, 32, "Admin");
            this.AddGumpling(new ResizePic(this, g1));
            this.AddGumpling(g1);
            // password field
            TextEntry g2 = new TextEntry(this, 0, 332, 386, 200, 20, 0, (int)LoginGumpTextFields.Password, 32, "Admin");
            g2.IsPasswordField = true;
            this.AddGumpling(new ResizePic(this, g2));
            this.AddGumpling(g2);
            // login button
            this.AddGumpling(new Button(this, 0, 610, 435, 5540, 5542, 1, 0, (int)LoginGumpButtons.LoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5541;
            // Version information
            AddGumpling(new TextLabelAscii(this, 0, 183, 421, hue, 9, Utility.VersionString));
        }

        public override void Activate(Control c)
        {
            switch ((LoginGumpButtons)(((Button)c).ButtonID))
            {
                case LoginGumpButtons.QuitButton:
                    Quit();
                    break;
                case LoginGumpButtons.LoginButton:
                    string accountName = getTextEntry((int)LoginGumpTextFields.AccountName);
                    string password = getTextEntry((int)LoginGumpTextFields.Password);
                    OnLogin("localhost", 2593, accountName, password);
                    break;
            }
        }
    }
}
