using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Client;
using UltimaXNA.Network;
using UltimaXNA.Client.Packets;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.ClientsideGumps
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

        int _charSelected = -1;
        HtmlGump[] _characterNames;

        public CharacterListGump()
            : base(0, 0)
        {
            _renderFullScreen = false;
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
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
            AddControl(new TextLabelAscii(this, 1, 266, 112, 2017, 2, Data.StringList.Entry(3000050)));
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
            AddControl(new TextLabelAscii(this, 2, 166, 143, 2017, 2, Data.StringList.Entry(3000001)));
        }

        public void ReloadCharList()
        {
            int entryIndex = 0;
            _characterNames = new HtmlGump[ClientVars.CharacterList.Length];
            foreach (CharacterListEntry e in ClientVars.CharacterList)
            {
                if (e.Name != string.Empty)
                {
                    _characterNames[entryIndex] = new HtmlGump(this, 1, 228, 154 + 40 * entryIndex, 272, 22, 0, 0, formatHTMLCharName(entryIndex, e.Name, (_charSelected == entryIndex ? 431 : 1278)));
                    AddControl(new ResizePic(this, _characterNames[entryIndex]));
                    AddControl(_characterNames[entryIndex]);
                }
                entryIndex++;
            }
            // if (_charSelected == -1)
            //     ActivateByHREF("CHAR=0");
            ClientVars.CharacterList_Reloaded = false;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (ClientVars.CharacterList_Reloaded)
            {
                ReloadCharList();
            }
            base.Update(gameTime);
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.QuitButton:
                    Quit();
                    break;
                case Buttons.BackButton:
                    OnBackToSelectServer();
                    break;
                case Buttons.ForwardButton:
                    OnLoginWithCharacter(_charSelected);
                    break;
                case Buttons.NewCharacterButton:
                    OnNewCharacter();
                    break;
                case Buttons.DeleteCharacterButton:
                    OnDeleteCharacter(_charSelected);
                    break;
            }
        }

        public override void ActivateByHREF(string href)
        {
            if (href.Length > 5 && href.StartsWith("CHAR="))
            {
                int charIndex = int.Parse(href.Substring(5));
                if (charIndex == _charSelected)
                    OnLoginWithCharacter(charIndex);
                else
                {
                    if ((_charSelected >= 0) && (_charSelected < ClientVars.CharacterList.Length))
                        _characterNames[_charSelected].Text = formatHTMLCharName(_charSelected, ClientVars.CharacterList[_charSelected].Name, 1278);
                    _charSelected = charIndex;
                    if ((_charSelected >= 0) && (_charSelected < ClientVars.CharacterList.Length))
                        _characterNames[_charSelected].Text = formatHTMLCharName(_charSelected, ClientVars.CharacterList[_charSelected].Name, 431);
                }
            }
        }

        static int _kHoverHue = Data.HuesXNA.GetWebSafeHue("609");
        static int _kActivateHue = Data.HuesXNA.GetWebSafeHue("F00");
        string formatHTMLCharName(int index, string name, int hue)
        {
            return string.Format("<center><big><a href=\"CHAR={0}\" style=\"colorhue: #{2}; hoverhue: #{3}; activatehue: #{4}; text-decoration: none\">{1}</a></big></center>", 
                index, name, hue, _kHoverHue, _kActivateHue);
        }
    }
}
