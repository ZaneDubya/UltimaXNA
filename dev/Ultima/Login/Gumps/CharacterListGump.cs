/***************************************************************************
 *   CharacterListGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Configuration;
using UltimaXNA.Ultima.Data.Accounts;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.Login.Gumps
{
    class CharacterListGump : Gump
    {
        public delegate void EventWithInt(int index);
        public delegate void EventWithNoParams();

        public EventWithInt OnDeleteCharacter;
        public EventWithNoParams OnNewCharacter;
        public EventWithNoParams OnBackToSelectServer;
        public EventWithInt OnLoginWithCharacter;

        enum Buttons
        {
            QuitButton,
            BackButton,
            ForwardButton,
            NewCharacterButton,
            DeleteCharacterButton
        }

        int m_charSelected = -1;
        int m_charListUpdate = -1;
        HtmlGump[] m_characterNames;

        public CharacterListGump()
            : base(0, 0)
        {
            m_renderFullScreen = false;
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 0, Settings.Game.Resolution.Width, Settings.Game.Resolution.Height, 9274));
            AddControl(new GumpPic(this, 0, 0, 0, 5500, 0));
            // quit button
            AddControl(new Button(this, 0, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)Buttons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;

            // Page 1 - select a character
            // back button
            AddControl(new Button(this, 1, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)Buttons.BackButton));
            ((Button)LastControl).GumpOverID = 5538;
            // forward button
            AddControl(new Button(this, 1, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)Buttons.ForwardButton));
            ((Button)LastControl).GumpOverID = 5541;
            // center message window backdrop
            AddControl(new ResizePic(this, 1, 160, 70, 2600, 408, 390));
            AddControl(new TextLabelAscii(this, 1, 266, 112, 2017, 2, IO.StringData.Entry(3000050)));
            // display the character list.
            ReloadCharList();
            // delete button
            AddControl(new Button(this, 1, 224, 398, 5530, 5532, ButtonTypes.Activate, 0, (int)Buttons.DeleteCharacterButton));
            ((Button)LastControl).GumpOverID = 5531;
            // new button
            AddControl(new Button(this, 1, 442, 398, 5533, 5535, ButtonTypes.Activate, 0, (int)Buttons.NewCharacterButton));
            ((Button)LastControl).GumpOverID = 5534;

            // Page 2 - logging in to server
            // center message window backdrop
            AddControl(new ResizePic(this, 2, 116, 95, 2600, 408, 288));
            AddControl(new TextLabelAscii(this, 2, 166, 143, 2017, 2, IO.StringData.Entry(3000001)));
        }

        public void ReloadCharList()
        {
            
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Characters.UpdateValue != m_charListUpdate)
            {
                int entryIndex = 0;
                m_characterNames = new HtmlGump[Characters.Length];
                foreach (CharacterListEntry e in Characters.List)
                {
                    if (e.Name != string.Empty)
                    {
                        m_characterNames[entryIndex] = new HtmlGump(this, 1, 228, 154 + 40 * entryIndex, 272, 22, 0, 0, formatHTMLCharName(entryIndex, e.Name, (m_charSelected == entryIndex ? 431 : 1278)));
                        AddControl(new ResizePic(this, m_characterNames[entryIndex]));
                        AddControl(m_characterNames[entryIndex]);
                    }
                    entryIndex++;
                }
                m_charListUpdate = Characters.UpdateValue;
            }
            base.Update(totalMS, frameMS);
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.QuitButton:
                    EngineVars.EngineRunning = false;
                    break;
                case Buttons.BackButton:
                    OnBackToSelectServer();
                    break;
                case Buttons.ForwardButton:
                    OnLoginWithCharacter(m_charSelected);
                    break;
                case Buttons.NewCharacterButton:
                    OnNewCharacter();
                    break;
                case Buttons.DeleteCharacterButton:
                    OnDeleteCharacter(m_charSelected);
                    break;
            }
        }

        public override void ActivateByHREF(string href)
        {
            if (href.Length > 5 && href.StartsWith("CHAR="))
            {
                int charIndex = int.Parse(href.Substring(5));
                if (charIndex == m_charSelected)
                    OnLoginWithCharacter(charIndex);
                else
                {
                    if ((m_charSelected >= 0) && (m_charSelected < Characters.Length))
                        m_characterNames[m_charSelected].Text = formatHTMLCharName(m_charSelected, Characters.List[m_charSelected].Name, 1278);
                    m_charSelected = charIndex;
                    if ((m_charSelected >= 0) && (m_charSelected < Characters.Length))
                        m_characterNames[m_charSelected].Text = formatHTMLCharName(m_charSelected, Characters.List[m_charSelected].Name, 431);
                }
            }
        }

        static int m_kHoverHue = IO.HuesXNA.GetWebSafeHue("609");
        static int m_kActivateHue = IO.HuesXNA.GetWebSafeHue("F00");
        string formatHTMLCharName(int index, string name, int hue)
        {
            return string.Format("<center><big><a href=\"CHAR={0}\" style=\"colorhue: #{2}; hoverhue: #{3}; activatehue: #{4}; text-decoration: none\">{1}</a></big></center>", 
                index, name, hue, m_kHoverHue, m_kActivateHue);
        }
    }
}
