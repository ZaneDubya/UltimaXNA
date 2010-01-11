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

        public CharacterListGump(ServerListPacket p)
            : base(0, 0)
        {
            _renderFullScreen = false;
            // backdrop
            AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            AddGumpling(new GumpPic(this, 0, 0, 0, 5500, 0));
            // quit button
            AddGumpling(new Button(this, 0, 554, 2, 5513, 5515, 1, 0, (int)Buttons.QuitButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5514;

            // Page 1 - select a character
            // back button
            AddGumpling(new Button(this, 1, 586, 435, 5537, 5539, 1, 0, (int)Buttons.BackButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5538;
            // forward button
            AddGumpling(new Button(this, 1, 610, 435, 5540, 5542, 1, 0, (int)Buttons.ForwardButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5541;
            // center message window backdrop
            AddGumpling(new ResizePic(this, 1, 160, 70, 2600, 408, 390));
            AddGumpling(new TextLabelAscii(this, 1, 266, 112, 2017, 2, Data.StringList.Entry(3000050)));
            // display the character list.
            int entryIndex = 0;
            foreach (CharacterListEntry e in UltimaClient.CharacterListPacket.Characters)
            {
                if (e.Name != string.Empty)
                {
                    Control c = new HtmlGump(this, 1, 228, 154 + 40 * entryIndex, 272, 22, 0, 0, "<center><big><basefont color=#000000><a href=\"CHAR=" + entryIndex + "\">" + e.Name + "</a></big></center>");
                    AddGumpling(new ResizePic(this, c));
                    AddGumpling(c);
                }
                entryIndex++;
            }
            // delete button
            AddGumpling(new Button(this, 1, 224, 398, 5530, 5532, 1, 0, (int)Buttons.DeleteCharacterButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5531;
            // new button
            AddGumpling(new Button(this, 1, 442, 398, 5533, 5535, 1, 0, (int)Buttons.NewCharacterButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5534;

            // Page 2 - logging in to server
            // center message window backdrop
            AddGumpling(new ResizePic(this, 2, 116, 95, 2600, 408, 288));
            AddGumpling(new TextLabelAscii(this, 2, 166, 143, 2017, 2, Data.StringList.Entry(3000001)));
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
                    OnLoginWithCharacter(0);
                    break;
                case Buttons.NewCharacterButton:
                    OnNewCharacter();
                    break;
                case Buttons.DeleteCharacterButton:
                    break;
            }
        }

        public override void ActivateByHREF(string href)
        {
            if (href.Length > 5 && href.StartsWith("CHAR="))
            {
                int charIndex = int.Parse(href.Substring(5));
                OnLoginWithCharacter(charIndex);
            }
        }
    }
}
