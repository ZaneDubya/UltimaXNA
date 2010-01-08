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
    public delegate void DeleteCharacterEvent(int index);
    public delegate void NewCharacterEvent();
    public delegate void BackToSelectServerEvent();
    public delegate void LoginWithCharacterEvent(int index);

    class CharacterListGump : Gump
    {
        public DeleteCharacterEvent OnDeleteCharacter;
        public NewCharacterEvent OnNewCharacter;
        public BackToSelectServerEvent OnBackToSelectServer;
        public LoginWithCharacterEvent OnLoginWithCharacter;

        enum CharacterListGumpButtons
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
            this.AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            this.AddGumpling(new GumpPic(this, 0, 0, 0, 9003, 0));
            // quit button
            this.AddGumpling(new Button(this, 0, 554, 2, 5513, 5515, 1, 0, (int)CharacterListGumpButtons.QuitButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5514;

            // Page 1 - select a character
            // back button
            this.AddGumpling(new Button(this, 1, 586, 435, 5537, 5539, 1, 0, (int)CharacterListGumpButtons.BackButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5538;
            // forward button
            this.AddGumpling(new Button(this, 1, 610, 435, 5540, 5542, 1, 0, (int)CharacterListGumpButtons.ForwardButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5541;
            // center message window backdrop
            this.AddGumpling(new ResizePic(this, 1, 160, 70, 2600, 408, 390));
            AddGumpling(new TextLabelAscii(this, 1, 266, 112, 2017, 2, Data.StringList.Table[3000050].ToString()));
            // display the character list.
            int entryIndex = 0;
            foreach (CharacterListEntry e in UltimaClient.CharacterListPacket.Characters)
            {
                if (e.Name != string.Empty)
                {
                    Control c = new HtmlGump(this, 1, 228, 154, 272, 22, 0, 0, "<center><big><basefont color=#000000><a href=\"CHAR=" + entryIndex + "\">" + e.Name + "</a></big></center>");
                    AddGumpling(new ResizePic(this, c));
                    AddGumpling(c);
                }
                entryIndex++;
            }
            // delete button
            this.AddGumpling(new Button(this, 1, 224, 398, 5530, 5532, 1, 0, (int)CharacterListGumpButtons.DeleteCharacterButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5531;
            // new button
            this.AddGumpling(new Button(this, 1, 442, 398, 5533, 5535, 1, 0, (int)CharacterListGumpButtons.NewCharacterButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5534;

            // Page 2 - logging in to server
            // center message window backdrop
            AddGumpling(new ResizePic(this, 2, 116, 95, 2600, 408, 288));
            AddGumpling(new TextLabelAscii(this, 2, 166, 143, 2017, 2, Data.StringList.Table[3000001].ToString()));
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((CharacterListGumpButtons)buttonID)
            {
                case CharacterListGumpButtons.QuitButton:
                    Quit();
                    break;
                case CharacterListGumpButtons.BackButton:
                    OnBackToSelectServer();
                    break;
                case CharacterListGumpButtons.ForwardButton:
                    OnLoginWithCharacter(0);
                    break;
                case CharacterListGumpButtons.NewCharacterButton:
                    OnNewCharacter();
                    break;
                case CharacterListGumpButtons.DeleteCharacterButton:
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
