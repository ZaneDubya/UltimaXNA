/***************************************************************************
 *   LoggingInGump.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.UltimaGUI.Controls;

namespace UltimaXNA.UltimaGUI.Gumps
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
            int hue = 2017;
            m_renderFullScreen = false;
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            AddControl(new GumpPic(this, 0, 0, 0, 5500, 0));
            // quit button
            AddControl(new Button(this, 0, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;
            // center message window backdrop
            AddControl(new ResizePic(this, 0, 116, 95, 2600, 408, 288));

            // Page 1 - Connecting... with cancel login button
            AddControl(new TextLabelAscii(this, 1, 166, 143, hue, 2, UltimaData.StringData.Entry(3000002)));
            AddControl(new Button(this, 1, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.CancelLoginButton));
            ((Button)LastControl).GumpOverID = 1151;

            // Page 2 - Couldn't connect to server
            AddControl(new TextLabelAsciiCropped(this, 2, 166, 143, 308, 308, hue, 2, UltimaData.StringData.Entry(3000016)));
            AddControl(new Button(this, 2, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)LastControl).GumpOverID = 1154;

            // Page 3 - Incorrect username and/or password.
            AddControl(new TextLabelAsciiCropped(this, 3, 166, 143, 308, 308, hue, 2, UltimaData.StringData.Entry(3000036)));
            AddControl(new Button(this, 3, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)LastControl).GumpOverID = 1154;

            // Page 4 - Someone is already using this account.
            AddControl(new TextLabelAsciiCropped(this, 4, 166, 143, 308, 308, hue, 2, UltimaData.StringData.Entry(3000034)));
            AddControl(new Button(this, 4, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)LastControl).GumpOverID = 1154;

            // Page 5 - Your account has been blocked / banned
            AddControl(new TextLabelAsciiCropped(this, 5, 166, 143, 308, 308, 1107, 1, UltimaData.StringData.Entry(3000035)));
            AddControl(new Button(this, 5, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)LastControl).GumpOverID = 1154;

            // Page 6 - Your account credentials are invalid.
            AddControl(new TextLabelAsciiCropped(this, 6, 166, 143, 308, 308, hue, 2, UltimaData.StringData.Entry(3000036)));
            AddControl(new Button(this, 6, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)LastControl).GumpOverID = 1154;

            // Page 7 - Login idle period exceeded (I use "Connection lost")
            AddControl(new TextLabelAsciiCropped(this, 7, 166, 143, 308, 308, hue, 2, UltimaData.StringData.Entry(3000004)));
            AddControl(new Button(this, 7, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)LastControl).GumpOverID = 1154;

            // Page 8 - Communication problem.
            AddControl(new TextLabelAsciiCropped(this, 8, 166, 143, 308, 308, hue, 2, UltimaData.StringData.Entry(3000037)));
            AddControl(new Button(this, 8, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)LastControl).GumpOverID = 1154;

            // Page 9 - Verifying Account... with cancel login button
            AddControl(new TextLabelAscii(this, 9, 166, 143, hue, 2, UltimaData.StringData.Entry(3000003)));
            AddControl(new Button(this, 9, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.CancelLoginButton));
            ((Button)LastControl).GumpOverID = 1151;
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((LoggingInGumpButtons)buttonID)
            {
                case LoggingInGumpButtons.QuitButton:
                    Quit();
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
