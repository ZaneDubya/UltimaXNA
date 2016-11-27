/***************************************************************************
 *   SelectServerGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.Login.Data;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.LoginGumps {
    class SelectServerGump : Gump {
        Action m_OnBackToLoginScreen;
        Action m_OnSelectLastServer;
        readonly Action<int> m_OnSelectServer;

        enum SelectServerGumpButtons {
            QuitButton,
            BackButton,
            ForwardButton
        }

        public SelectServerGump(ServerListEntry[] servers, Action onBackToLogin, Action onSelectLastServer, Action<int> onSelectServer)
            : base(0, 0) {
            m_OnBackToLoginScreen = onBackToLogin;
            m_OnSelectLastServer = onSelectLastServer;
            m_OnSelectServer = onSelectServer;

            // get the resource provider
            IResourceProvider provider = Service.Get<IResourceProvider>();

            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 9274));
            AddControl(new GumpPic(this, 0, 0, 5500, 0));
            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;

            // Page 1 - select a server
            // back button
            AddControl(new Button(this, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.BackButton), 1);
            ((Button)LastControl).GumpOverID = 5538;
            // forward button
            AddControl(new Button(this, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.ForwardButton), 1);
            ((Button)LastControl).GumpOverID = 5541;

            // center message window backdrop
            AddControl(new ResizePic(this, 152, 90, 3500, 382, 274));
            AddControl(new HtmlGumpling(this, 158, 72, 200, 20, 0, 0, provider.GetString(1044579)), 1);
            AddControl(new HtmlGumpling(this, 402, 72, 50, 20, 0, 0, provider.GetString(1044577)), 1);
            AddControl(new HtmlGumpling(this, 472, 72, 80, 20, 0, 0, provider.GetString(1044578)), 1);
            // display the serverlist the server list.
            int idx = 0;
            foreach (ServerListEntry e in servers) {
                // HINT: Do not use e.Index in place of idx: e.Index may non start from 0, or may contain holes, expecially on POL server
                AddControl(new HtmlGumpling(this, 224, 104 + idx * 25, 200, 20, 0, 0, "<big><a href=\"SHARD=" + e.Index + "\" style=\"text-decoration: none\">" + e.Name + "</a></big>"), 1);
                idx++;
            }

            // Page 2 - logging in to server ... with cancel login button
            // center message window backdrop
            AddControl(new ResizePic(this, 116, 95, 2600, 408, 288), 2);
            AddControl(new TextLabelAscii(this, 166, 143, 2, 2017, provider.GetString(3000053) + "..."), 2);
            AddControl(new Button(this, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.BackButton), 2);
            ((Button)LastControl).GumpOverID = 1151;

            IsUncloseableWithRMB = true;
        }

        public override void OnButtonClick(int buttonID) {
            switch ((SelectServerGumpButtons)buttonID) {
                case SelectServerGumpButtons.QuitButton:
                    Service.Get<UltimaGame>().Quit();
                    break;
                case SelectServerGumpButtons.BackButton:
                    m_OnBackToLoginScreen();
                    break;
                case SelectServerGumpButtons.ForwardButton:
                    m_OnSelectLastServer();
                    break;
            }
        }

        public override void OnHtmlInputEvent(string href, MouseEvent e) {
            if (e != MouseEvent.Click)
                return;

            if (href.Length > 6 && href.StartsWith("SHARD=")) {
                int serverIndex = int.Parse(href.Substring(6));
                m_OnSelectServer(serverIndex);
            }
        }
    }
}
