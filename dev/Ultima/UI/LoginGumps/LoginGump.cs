/***************************************************************************
 *   LoginGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System.Security;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Configuration;
using UltimaXNA.Ultima.UI.Controls;

namespace UltimaXNA.Ultima.UI.LoginGumps
{
    public delegate void LoginEvent(string server, int port, string account, SecureString password);

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
            int hue = 1132; // dark brown
            m_renderFullScreen = false;
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 0, Settings.Game.Resolution.Width, Settings.Game.Resolution.Height, 9274));
            AddControl(new GumpPic(this, 0, 0, 0, 9001, 0));
            // quit button
            AddControl(new Button(this, 0, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)LoginGumpButtons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;
            // Log in to Ultima Online
            AddControl(new TextLabelAscii(this, 0, 254, 305, hue, 2, IO.StringData.Entry(3000038)));
            // Account Name
            AddControl(new TextLabelAscii(this, 0, 181, 346, hue, 2, IO.StringData.Entry(3000099)));
            // Password
            AddControl(new TextLabelAscii(this, 0, 181, 386, hue, 2, IO.StringData.Entry(3000103)));
            // name field
            TextEntry g1 = new TextEntry(this, 0, 332, 346, 200, 20, 0, (int)LoginGumpTextFields.AccountName, 32, Settings.Server.UserName);
            g1.HtmlTag = "<basefont color=#000000><big>";
            AddControl(new ResizePic(this, g1));
            AddControl(g1);
            // password field
            TextEntry g2 = new TextEntry(this, 0, 332, 386, 200, 20, 0, (int)LoginGumpTextFields.Password, 32, "");
            g2.IsPasswordField = true;
            g2.HtmlTag = "<basefont color=#000000><big>";
            AddControl(new ResizePic(this, g2));
            AddControl(g2);
            // login button
            AddControl(new Button(this, 0, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)LoginGumpButtons.LoginButton));
            ((Button)LastControl).GumpOverID = 5541;
            // Version information
            AddControl(new TextLabelAscii(this, 0, 183, 421, hue, 9, Utility.VersionString));
        }

        public override void ActivateByButton(int buttonID)
        {
            string accountName = GetTextEntry((int)LoginGumpTextFields.AccountName);
            string password = GetTextEntry((int)LoginGumpTextFields.Password);

            switch ((LoginGumpButtons)buttonID)
            {
                case LoginGumpButtons.QuitButton:
                    EngineVars.EngineRunning = false;
                    break;
                case LoginGumpButtons.LoginButton:
                {
                    var secureStr = new SecureString();

                    if (password.Length > 0)
                    {
                        foreach (var c in password.ToCharArray())
                        {
                            secureStr.AppendChar(c);
                        }
                    }

                    OnLogin(Settings.Server.ServerAddress, Settings.Server.ServerPort, accountName, secureStr);
                    break;
                }
            }

            Settings.Server.UserName = accountName;
        }
        public override void ActivateByKeyboardReturn(int textID, string text)
        {
            ActivateByButton((int)LoginGumpButtons.LoginButton);
        }
    }
}
