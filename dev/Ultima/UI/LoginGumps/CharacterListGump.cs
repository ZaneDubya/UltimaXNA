/***************************************************************************
 *   CharacterListGump.cs
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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Windows;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.Login.Accounts;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.LoginGumps {
    class CharacterListGump : Gump {
        Action m_OnBackToSelectServer;
        Action<int> m_OnDeleteCharacter;
        Action<int> m_OnLoginWithCharacter;
        Action m_OnNewCharacter;

        bool m_isWorldLoading;
        int m_charSelected = -1;
        int m_charListUpdate = -1;
        HtmlGumpling[] m_characterNames;
        GumpPicTiled m_Background;

        enum Buttons {
            QuitButton,
            BackButton,
            ForwardButton,
            NewCharacterButton,
            DeleteCharacterButton
        }

        public CharacterListGump(Action onBack, Action<int> onLogin, Action<int> onDelete, Action onNew)
            : base(0, 0) {
            m_OnBackToSelectServer = onBack;
            m_OnLoginWithCharacter = onLogin;
            m_OnDeleteCharacter = onDelete;
            m_OnNewCharacter = onNew;

            // get the resource provider
            IResourceProvider provider = Services.Get<IResourceProvider>();

            // backdrop
            AddControl(m_Background = new GumpPicTiled(this, 0, 0, 800, 600, 9274));
            AddControl(new GumpPic(this, 0, 0, 5500, 0));
            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)Buttons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;

            // Page 1 - select a character
            // back button
            AddControl(new Button(this, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)Buttons.BackButton), 1);
            ((Button)LastControl).GumpOverID = 5538;
            // forward button
            AddControl(new Button(this, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)Buttons.ForwardButton), 1);
            ((Button)LastControl).GumpOverID = 5541;
            // center message window backdrop
            AddControl(new ResizePic(this, 160, 70, 2600, 408, 390), 1);
            AddControl(new TextLabelAscii(this, 266, 112, 2, 2016, provider.GetString(3000050)), 1);
            // delete button
            AddControl(new Button(this, 224, 398, 5530, 5532, ButtonTypes.Activate, 0, (int)Buttons.DeleteCharacterButton), 1);
            ((Button)LastControl).GumpOverID = 5531;
            // new button
            AddControl(new Button(this, 442, 398, 5533, 5535, ButtonTypes.Activate, 0, (int)Buttons.NewCharacterButton), 1);
            ((Button)LastControl).GumpOverID = 5534;

            // Page 2 - logging in to server
            // center message window backdrop
            AddControl(new ResizePic(this, 116, 95, 2600, 408, 288), 2);
            AddControl(new TextLabelAscii(this, 166, 143, 2, 2016, provider.GetString(3000001)), 2);

            IsUncloseableWithRMB = true;
        }

        public override void Update(double totalMS, double frameMS) {
            SpriteBatch3D sb = Services.Get<SpriteBatch3D>();
            if (m_Background.Width != sb.GraphicsDevice.Viewport.Width || m_Background.Height != sb.GraphicsDevice.Viewport.Height) {
                m_Background.Width = sb.GraphicsDevice.Viewport.Width;
                m_Background.Height = sb.GraphicsDevice.Viewport.Height;
            }

            if (Characters.UpdateValue != m_charListUpdate) {
                int entryIndex = 0;
                m_characterNames = new HtmlGumpling[Characters.Length];
                foreach (CharacterListEntry e in Characters.List) {
                    if (e.Name != string.Empty) {
                        m_characterNames[entryIndex] = new HtmlGumpling(this, 228, 154 + 40 * entryIndex, 272, 22, 0, 0, formatHTMLCharName(entryIndex, e.Name, (m_charSelected == entryIndex ? 431 : 1278)));
                        AddControl(new ResizePic(this, m_characterNames[entryIndex]), 1);
                        AddControl(m_characterNames[entryIndex], 1);
                    }
                    entryIndex++;
                }
                m_charListUpdate = Characters.UpdateValue;
            }

            InputManager input = Services.Get<InputManager>();
            if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Enter, false, false, false) && !m_isWorldLoading) {
                if (m_characterNames.Length > 0) {
                    m_OnLoginWithCharacter(0);
                    m_isWorldLoading = true;
                }
            }

            base.Update(totalMS, frameMS);
        }

        public override void OnButtonClick(int buttonID) {
            switch ((Buttons)buttonID) {
                case Buttons.QuitButton:
                    Services.Get<UltimaGame>().Quit();
                    break;
                case Buttons.BackButton:
                    m_OnBackToSelectServer();
                    break;
                case Buttons.ForwardButton:
                    m_OnLoginWithCharacter(m_charSelected);
                    break;
                case Buttons.NewCharacterButton:
                    m_OnNewCharacter();
                    break;
                case Buttons.DeleteCharacterButton:
                    m_OnDeleteCharacter(m_charSelected);
                    break;
            }
        }

        public override void OnHtmlInputEvent(string href, MouseEvent e) {
            int charIndex;
            if (href.Length > 5 && href.StartsWith("CHAR="))
                charIndex = int.Parse(href.Substring(5));
            else
                return;

            if (e == MouseEvent.Click) {
                if (href.Length > 5 && href.StartsWith("CHAR=")) {
                    if ((m_charSelected >= 0) && (m_charSelected < Characters.Length))
                        m_characterNames[m_charSelected].Text = formatHTMLCharName(m_charSelected, Characters.List[m_charSelected].Name, 1278);
                    m_charSelected = charIndex;
                    if ((m_charSelected >= 0) && (m_charSelected < Characters.Length))
                        m_characterNames[m_charSelected].Text = formatHTMLCharName(m_charSelected, Characters.List[m_charSelected].Name, 431);
                }
            }
            else if (e == MouseEvent.DoubleClick) {
                if (charIndex == m_charSelected)
                    m_OnLoginWithCharacter(charIndex);
            }
        }

        string formatHTMLCharName(int index, string name, int hue) {
            // add a single char to the left so the width doesn't change.
            return string.Format("<left> </left><center><big><a href=\"CHAR={0}\" color='#543' hovercolor='#345' activecolor='#222' style=\"text-decoration: none\">{1}</a></big></center>",
                index, name);
        }
    }
}
