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
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.Login.Servers;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Core.Resources;
#endregion

namespace UltimaXNA.Ultima.UI.LoginGumps
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
            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

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
            foreach (ServerListEntry e in ServerList.List)
            {
                AddControl(new HtmlGumpling(this, 224, 104, 200, 20, 0, 0, "<big><a href=\"SHARD=" + e.Index + "\" style=\"colorhue: #1278; hoverhue: #836; activatehue: #796; text-decoration: none\">" + e.Name + "</a></big>"), 1);
            }

            // Page 2 - logging in to server ... with cancel login button
            // center message window backdrop
            AddControl(new ResizePic(this, 116, 95, 2600, 408, 288), 2);
            AddControl(new TextLabelAscii(this, 166, 143, 2017, 2, provider.GetString(3000053) + "..."), 2);
            AddControl(new Button(this, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)SelectServerGumpButtons.BackButton), 2);
            ((Button)LastControl).GumpOverID = 1151;

            IsUncloseableWithRMB = true;
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((SelectServerGumpButtons)buttonID)
            {
                case SelectServerGumpButtons.QuitButton:
                    UltimaGame.IsRunning = false;
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
