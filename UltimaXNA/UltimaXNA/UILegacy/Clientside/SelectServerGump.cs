using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Client;
using UltimaXNA.Network;
using UltimaXNA.Network.Packets.Server;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
{
    class SelectServerGump : Gump
    {
        public CancelLoginEvent OnCancelLogin;

        public SelectServerGump(ServerListPacket p)
            : base(0, 0)
        {
            int hue = 1115;
            _renderFullScreen = false;
            // backdrop
            this.AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            this.AddGumpling(new GumpPic(this, 0, 0, 0, 9003, 0));
            // quit button
            this.AddGumpling(new Button(this, 0, 554, 2, 5513, 5515, 1, 0, (int)LoggingInGumpButtons.QuitButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5514;
            // forward button
            this.AddGumpling(new Button(this, 0, 610, 435, 5540, 5542, 1, 0, (int)LoginGumpButtons.LoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5541;
            // back button
            this.AddGumpling(new Button(this, 0, 586, 435, 5537, 5539, 1, 0, (int)LoginGumpButtons.LoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5538;

            // center message window backdrop
            this.AddGumpling(new ResizePic(this, 0, 152, 90, 3500, 382, 274));
            AddGumpling(new HtmlGump(this, 0, 158, 72, 200, 20, 0, 0, Data.StringList.Table[1044579].ToString()));
            AddGumpling(new HtmlGump(this, 0, 402, 72, 50, 20, 0, 0, Data.StringList.Table[1044577].ToString()));
            AddGumpling(new HtmlGump(this, 0, 472, 72, 80, 20, 0, 0, Data.StringList.Table[1044578].ToString()));
            // display the serverlist the server list.
            foreach (ServerListEntry e in UltimaClient.ServerListPacket.Servers)
            {
                AddGumpling(new HtmlGump(this, 0, 224, 104, 200, 20, 0, 0, "<big><basefont color=#000000><a href=\"selectshard=" + e.Index + "\">" + e.Name + "</a></big>"));
            }

            // Page 1 - Connecting... with cancel login button
            AddGumpling(new TextLabelAscii(this, 1, 166, 143, hue, 2, Data.StringList.Table[3000002].ToString()));
            this.AddGumpling(new Button(this, 1, 305, 342, 1150, 1152, 1, 0, (int)LoggingInGumpButtons.CancelLoginButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1151;
        }

        public override void Activate(Control c)
        {
            switch ((LoggingInGumpButtons)(((Button)c).ButtonID))
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
