using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;
using Microsoft.Xna.Framework;

namespace UltimaXNA.UILegacy.Clientside
{
    class CreateCharAppearanceGump : Gump
    {
        public delegate void EventNoParams();

        enum Buttons
        {
            BackButton,
            ForwardButton
        }

        public EventNoParams OnForward;
        public EventNoParams OnBackward;

        public CreateCharAppearanceGump()
            : base(0, 0)
        {
            _renderFullScreen = false;
            // backdrop
            AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            AddGumpling(new GumpPic(this, 0, 0, 0, 5500, 0));
            // character name 
            AddGumpling(new GumpPic(this, 0, 280, 53, 1801, 0));
            // character window
            AddGumpling(new GumpPic(this, 0, 238, 98, 1800, 0));
            // left option window
            AddGumpling(new ResizePic(this, 0, 82, 125, 3600, 151, 310));
            // hair (male)
            AddGumpling(new TextLabelAscii(this, 0, 100, 141, 2037, 9, Data.StringList.Entry(3000121)));
            AddGumpling(new DropDownList(this, 0, 97, 154, 122, 0, 6, Data.HairStyles.Male, false));
            // facial hair (male)
            AddGumpling(new TextLabelAscii(this, 0, 100, 186, 2037, 9, Data.StringList.Entry(3000122)));
            AddGumpling(new DropDownList(this, 0, 97, 199, 122, 0, 6, Data.HairStyles.FacialHair, false));

            // right option window
            AddGumpling(new ResizePic(this, 0, 475, 125, 3600, 151, 310));
            // skin tone
            AddGumpling(new TextLabelAscii(this, 0, 489, 141, 2037, 9, Data.StringList.Entry(3000183)));
            AddGumpling(new ColorPicker(this, 0, new Rectangle(490, 154, 120, 24), new Rectangle(490, 140, 120, 280), 7, 8, Data.Hues.SkinTones));
            // hair color
            AddGumpling(new TextLabelAscii(this, 0, 489, 186, 2037, 9, Data.StringList.Entry(3000184)));
            AddGumpling(new ColorPicker(this, 0, new Rectangle(490, 199, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, Data.Hues.HairTones));
            // facial hair color (male)
            AddGumpling(new TextLabelAscii(this, 0, 489, 231, 2037, 9, Data.StringList.Entry(3000185)));
            AddGumpling(new ColorPicker(this, 0, new Rectangle(490, 244, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, Data.Hues.HairTones));

            // back button
            AddGumpling(new Button(this, 1, 586, 435, 5537, 5539, 1, 0, (int)Buttons.BackButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5538;
            // forward button
            AddGumpling(new Button(this, 1, 610, 435, 5540, 5542, 1, 0, (int)Buttons.ForwardButton));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5541;
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.BackButton:
                    OnBackward();
                    break;
                case Buttons.ForwardButton:
                    OnForward();
                    break;
            }
        }
    }
}
