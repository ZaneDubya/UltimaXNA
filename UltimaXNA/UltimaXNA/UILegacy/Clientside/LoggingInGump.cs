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
        CancelLoginButton,
        OKNoLoginButton
    }

    class LoggingInGump : Gump
    {
        public CancelLoginEvent OnCancelLogin;

        public LoggingInGump()
            : base(0, 0)
        {
            int hue = 1115;
            _renderFullScreen = false;
            // backdrop
            this.AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            this.AddGumpling(new GumpPic(this, 0, 0, 0, 9003, 0));
            this.AddGumpling(new ResizePic(this, 0, 116, 95, 2600, 408, 288));

            // Page 1 - Connecting with cancel login button
            AddGumpling(new TextLabelAscii(this, 1, 166, 143, hue, 2, Data.StringList.Table[3000002].ToString()));
            this.AddGumpling(new Button(this, 1, 305, 342, 1150, 1152, 1, 0, (int)LoggingInGumpButtons.CancelLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1151;

            // Page 2 - Couldn't connect with OK button
            AddGumpling(new TextLabelAsciiCropped(this, 2, 166, 143, 308, 208, hue, 2, Data.StringList.Table[3000016].ToString()));
            this.AddGumpling(new Button(this, 2, 305, 342, 1153, 1155, 1, 0, (int)LoggingInGumpButtons.OKNoLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1154;
        }

        public override void Activate(Control c)
        {
            switch ((LoggingInGumpButtons)(((Button)c).ButtonID))
            {
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
