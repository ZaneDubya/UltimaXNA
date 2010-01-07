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
    public delegate void BackToLoginScreenEvent();
    public delegate void SelectLastServerEvent();
    public delegate void LoginToAServerEvent(int index);

    class SelectServerGump : Gump
    {
        public BackToLoginScreenEvent OnBackToLoginScreen;
        public SelectLastServerEvent OnSelectLastServer;
        public LoginToAServerEvent OnSelectServer;

        enum SelectServerGumpButtons
        {
            QuitButton,
            BackButton,
            ForwardButton
        }

        public SelectServerGump(ServerListPacket p)
            : base(0, 0)
        {
            _renderFullScreen = false;
            // backdrop
            this.AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            this.AddGumpling(new GumpPic(this, 0, 0, 0, 9003, 0));
            // quit button
            this.AddGumpling(new Button(this, 0, 554, 2, 5513, 5515, 1, 0, (int)SelectServerGumpButtons.QuitButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5514;

            // Page 1 - select a server
            // back button
            this.AddGumpling(new Button(this, 1, 586, 435, 5537, 5539, 1, 0, (int)SelectServerGumpButtons.BackButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5538;
            // forward button
            this.AddGumpling(new Button(this, 1, 610, 435, 5540, 5542, 1, 0, (int)SelectServerGumpButtons.ForwardButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5541;

            // center message window backdrop
            this.AddGumpling(new ResizePic(this, 1, 152, 90, 3500, 382, 274));
            AddGumpling(new HtmlGump(this, 1, 158, 72, 200, 20, 0, 0, Data.StringList.Table[1044579].ToString()));
            AddGumpling(new HtmlGump(this, 1, 402, 72, 50, 20, 0, 0, Data.StringList.Table[1044577].ToString()));
            AddGumpling(new HtmlGump(this, 1, 472, 72, 80, 20, 0, 0, Data.StringList.Table[1044578].ToString()));
            // display the serverlist the server list.
            foreach (ServerListEntry e in UltimaClient.ServerListPacket.Servers)
            {
                AddGumpling(new HtmlGump(this, 1, 224, 104, 200, 20, 0, 0, "<big><basefont color=#000000><a href=\"SHARD=" + e.Index + "\">" + e.Name + "</a></big>"));
            }

            // Page 2 - logging in to server ... with cancel login button
            // center message window backdrop
            this.AddGumpling(new ResizePic(this, 2, 116, 95, 2600, 408, 288));
            AddGumpling(new TextLabelAscii(this, 2, 166, 143, 2017, 2, Data.StringList.Table[3000053].ToString() + "..."));
            this.AddGumpling(new Button(this, 2, 305, 342, 1150, 1152, 1, 0, (int)SelectServerGumpButtons.BackButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 1151;
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((SelectServerGumpButtons)buttonID)
            {
                case SelectServerGumpButtons.QuitButton:
                    Quit();
                    break;
                case SelectServerGumpButtons.BackButton:
                    OnBackToLoginScreen();
                    break;
                case SelectServerGumpButtons.ForwardButton:
                    OnSelectLastServer();
                    break;
            }
        }

        public override void ActivateByHREF(string href)
        {
            if (href.Length > 6 && href.StartsWith("SHARD="))
            {
                int serverIndex = int.Parse(href.Substring(6));
                OnSelectServer(serverIndex);
            }
        }
    }
}
