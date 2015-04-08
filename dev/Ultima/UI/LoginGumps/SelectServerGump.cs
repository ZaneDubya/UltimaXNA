/***************************************************************************
 *   SelectServerGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Data;
using UltimaXNA.UltimaGUI.Controls;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaVars;

namespace UltimaXNA.UltimaGUI.LoginGumps
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
            m_renderFullScreen = false;
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 0, Settings.Game.Resolution.Width, Settings.Game.Resolution.Height, 9274));
            AddControl(new GumpPic(this, 0, 0, 0, 5500, 0));
            // quit button
            AddControl(new Button(this, 0, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;

            // Page 1 - select a server
            // back button
            AddControl(new Button(this, 1, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.BackButton));
            ((Button)LastControl).GumpOverID = 5538;
            // forward button
            AddControl(new Button(this, 1, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.ForwardButton));
            ((Button)LastControl).GumpOverID = 5541;

            // center message window backdrop
            AddControl(new ResizePic(this, 1, 152, 90, 3500, 382, 274));
            AddControl(new HtmlGump(this, 1, 158, 72, 200, 20, 0, 0, UltimaData.StringData.Entry(1044579)));
            AddControl(new HtmlGump(this, 1, 402, 72, 50, 20, 0, 0, UltimaData.StringData.Entry(1044577)));
            AddControl(new HtmlGump(this, 1, 472, 72, 80, 20, 0, 0, UltimaData.StringData.Entry(1044578)));
            // display the serverlist the server list.
            foreach (ServerListEntry e in Servers.List)
            {
                AddControl(new HtmlGump(this, 1, 224, 104, 200, 20, 0, 0, "<big><a href=\"SHARD=" + e.Index + "\" style=\"colorhue: #1278; hoverhue: #836; activatehue: #796; text-decoration: none\">" + e.Name + "</a></big>"));
            }

            // Page 2 - logging in to server ... with cancel login button
            // center message window backdrop
            AddControl(new ResizePic(this, 2, 116, 95, 2600, 408, 288));
            AddControl(new TextLabelAscii(this, 2, 166, 143, 2017, 2, UltimaData.StringData.Entry(3000053) + "..."));
            AddControl(new Button(this, 2, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.BackButton));
            ((Button)LastControl).GumpOverID = 1151;
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((SelectServerGumpButtons)buttonID)
            {
                case SelectServerGumpButtons.QuitButton:
                    EngineVars.EngineRunning = false;
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
