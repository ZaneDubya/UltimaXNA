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

        public SelectServerGump()
            : base(0, 0)
        {
            _renderFullScreen = false;
            // backdrop
            AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            AddGumpling(new GumpPic(this, 0, 0, 0, 5500, 0));
            // quit button
            AddGumpling(new Button(this, 0, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.QuitButton));
            ((Button)LastGumpling).GumpOverID = 5514;

            // Page 1 - select a server
            // back button
            AddGumpling(new Button(this, 1, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.BackButton));
            ((Button)LastGumpling).GumpOverID = 5538;
            // forward button
            AddGumpling(new Button(this, 1, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.ForwardButton));
            ((Button)LastGumpling).GumpOverID = 5541;

            // center message window backdrop
            AddGumpling(new ResizePic(this, 1, 152, 90, 3500, 382, 274));
            AddGumpling(new HtmlGump(this, 1, 158, 72, 200, 20, 0, 0, Data.StringList.Entry(1044579)));
            AddGumpling(new HtmlGump(this, 1, 402, 72, 50, 20, 0, 0, Data.StringList.Entry(1044577)));
            AddGumpling(new HtmlGump(this, 1, 472, 72, 80, 20, 0, 0, Data.StringList.Entry(1044578)));
            // display the serverlist the server list.
            foreach (ServerListEntry e in ClientVars.ServerListPacket.Servers)
            {
                AddGumpling(new HtmlGump(this, 1, 224, 104, 200, 20, 0, 0, "<big><a href=\"SHARD=" + e.Index + "\" style=\"colorhue: #1278; hoverhue: #836; activatehue: #796; text-decoration: none\">" + e.Name + "</a></big>"));
            }

            // Page 2 - logging in to server ... with cancel login button
            // center message window backdrop
            AddGumpling(new ResizePic(this, 2, 116, 95, 2600, 408, 288));
            AddGumpling(new TextLabelAscii(this, 2, 166, 143, 2017, 2, Data.StringList.Entry(3000053) + "..."));
            AddGumpling(new Button(this, 2, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.BackButton));
            ((Button)LastGumpling).GumpOverID = 1151;
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
