using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Client;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
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
            _renderFullScreen = false;
            // backdrop
            AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            AddGumpling(new GumpPic(this, 0, 0, 0, 5500, 0));
            // quit button
            AddGumpling(new Button(this, 0, 554, 2, 5513, 5515, 1, 0, (int)LoggingInGumpButtons.QuitButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5514;
            // center message window backdrop
            AddGumpling(new ResizePic(this, 0, 116, 95, 2600, 408, 288));

            // Page 1 - Connecting... with cancel login button
            AddGumpling(new TextLabelAscii(this, 1, 166, 143, hue, 2, Data.StringList.Entry(3000002)));
            AddGumpling(new Button(this, 1, 305, 342, 1150, 1152, 1, 0, (int)LoggingInGumpButtons.CancelLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1151;

            // Page 2 - Couldn't connect to server
            AddGumpling(new TextLabelAsciiCropped(this, 2, 166, 143, 308, 308, hue, 2, Data.StringList.Entry(3000016)));
            AddGumpling(new Button(this, 2, 305, 342, 1153, 1155, 1, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1154;

            // Page 3 - Incorrect username and/or password.
            AddGumpling(new TextLabelAsciiCropped(this, 3, 166, 143, 308, 308, hue, 2, Data.StringList.Entry(3000036)));
            AddGumpling(new Button(this, 3, 305, 342, 1153, 1155, 1, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1154;

            // Page 4 - Someone is already using this account.
            AddGumpling(new TextLabelAsciiCropped(this, 4, 166, 143, 308, 308, hue, 2, Data.StringList.Entry(3000034)));
            AddGumpling(new Button(this, 4, 305, 342, 1153, 1155, 1, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1154;

            // Page 5 - Your account has been blocked / banned
            AddGumpling(new TextLabelAsciiCropped(this, 5, 166, 143, 308, 308, 1107, 1, Data.StringList.Entry(3000035)));
            AddGumpling(new Button(this, 5, 305, 342, 1153, 1155, 1, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1154;

            // Page 6 - Your account credentials are invalid.
            AddGumpling(new TextLabelAsciiCropped(this, 6, 166, 143, 308, 308, hue, 2, Data.StringList.Entry(3000036)));
            AddGumpling(new Button(this, 6, 305, 342, 1153, 1155, 1, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1154;

            // Page 7 - Login idle period exceeded (I use "Connection lost")
            AddGumpling(new TextLabelAsciiCropped(this, 7, 166, 143, 308, 308, hue, 2, Data.StringList.Entry(3000004)));
            AddGumpling(new Button(this, 7, 305, 342, 1153, 1155, 1, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1154;

            // Page 8 - Communication problem.
            AddGumpling(new TextLabelAsciiCropped(this, 8, 166, 143, 308, 308, hue, 2, Data.StringList.Entry(3000037)));
            AddGumpling(new Button(this, 8, 305, 342, 1153, 1155, 1, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1154;

            // Page 9 - Verifying Account... with cancel login button
            AddGumpling(new TextLabelAscii(this, 9, 166, 143, hue, 2, Data.StringList.Entry(3000003)));
            AddGumpling(new Button(this, 9, 305, 342, 1150, 1152, 1, 0, (int)LoggingInGumpButtons.CancelLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1151;
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
