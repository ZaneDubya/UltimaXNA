/***************************************************************************
 *   LoggingInGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Core.Resources;
#endregion

namespace UltimaXNA.Ultima.UI.LoginGumps
{
    public delegate void CancelLoginEvent();

    enum LoggingInGumpButtons
    {
        QuitButton,
        CancelLoginButton,
        OKNoLoginButton
    }

    class LoggingInGump : Gump
    {
        public CancelLoginEvent OnCancelLogin;

        public LoggingInGump()
            : base(0, 0)
        {
            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            int hue = 902;
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 9274));
            AddControl(new GumpPic(this, 0, 0, 5500, 0));
            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;
            // center message window backdrop
            AddControl(new ResizePic(this, 116, 95, 2600, 408, 288));

            // Page 1 - Connecting... with cancel login button
            AddControl(new TextLabelAscii(this, 166, 143, hue, 2, provider.GetString(3000002)), 1);
            AddControl(new Button(this, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.CancelLoginButton), 1);
            ((Button)LastControl).GumpOverID = 1151;

            // Page 2 - Couldn't connect to server
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, hue, 2, provider.GetString(3000016)), 2);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), 2);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 3 - Incorrect username and/or password.
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, hue, 2, provider.GetString(3000036)), 3);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), 3);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 4 - Someone is already using this account.
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, hue, 2, provider.GetString(3000034)), 4);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), 4);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 5 - Your account has been blocked / banned
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, 1107, 2, provider.GetString(3000035)), 5);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), 5);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 6 - Your account credentials are invalid.
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, hue, 2, provider.GetString(3000036)), 6);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), 6);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 7 - Login idle period exceeded (I use "Connection lost")
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, hue, 2, provider.GetString(3000004)), 7);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), 7);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 8 - Communication problem.
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, hue, 2, provider.GetString(3000037)), 8);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), 8);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 9 - Verifying Account... with cancel login button
            AddControl(new TextLabelAscii(this, 166, 143, hue, 2, provider.GetString(3000003)), 9);
            AddControl(new Button(this, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.CancelLoginButton), 9);
            ((Button)LastControl).GumpOverID = 1151;

            IsUncloseableWithRMB = true;
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((LoggingInGumpButtons)buttonID)
            {
                case LoggingInGumpButtons.QuitButton:
                    UltimaGame.IsRunning = false;
                    break;
                case LoggingInGumpButtons.CancelLoginButton:
                    OnCancelLogin();
                    break;
                case LoggingInGumpButtons.OKNoLoginButton:
                    OnCancelLogin();
                    break;
            }
        }
    }
}
