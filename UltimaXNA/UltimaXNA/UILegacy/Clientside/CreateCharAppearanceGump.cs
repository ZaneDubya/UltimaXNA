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
        public EventNoParams OnForward;
        public EventNoParams OnBackward;

        enum Buttons
        {
            BackButton,
            ForwardButton
        }

        public string Name { get { return _Name.Text; } set { _Name.Text = value; } }
        public int Gender { get { return 0; } set { } }
        public int Race { get { return 1; } set { } } // hard coded to human
        public int HairID
        { 
            get { return (Gender == 0) ? Data.HairStyles.MaleIDs[_HairMale.Index] : Data.HairStyles.FemaleIDs[_HairFemale.Index]; }
            set
            {
                for (int i = 0; i < 10; i++)
                {
                    if (value == ((Gender == 0) ? Data.HairStyles.MaleIDs[i] : Data.HairStyles.FemaleIDs[i]))
                    {
                        _HairMale.Index = i;
                        _HairFemale.Index = i;
                        break;
                    }
                }
            }
        }
        public int FacialHairID
        {
            get { return (Gender == 0) ? Data.HairStyles.MaleIDs[_FacialHairMale.Index] : 0; }
            set
            {
                for (int i = 0; i < 8; i++)
                {
                    if (value == Data.HairStyles.FacialIDs[i])
                    {
                        _FacialHairMale.Index = i;
                        break;
                    }
                }
            }
        }
        public int SkinHue
        {
            get { return _SkinHue.HueValue; }
            set { _SkinHue.HueValue = value; }
        }
        public int HairHue
        {
            get { return _HairHue.HueValue; }
            set { _HairHue.HueValue = value; }
        }
        public int FacialHairHue
        {
            get { return (Gender == 0) ? _FacialHairHue.HueValue : 0; }
            set { _FacialHairHue.HueValue = value; }
        }

        TextEntry _Name;
        DropDownList _HairMale;
        DropDownList _FacialHairMale;
        DropDownList _HairFemale;
        ColorPicker _SkinHue;
        ColorPicker _HairHue;
        ColorPicker _FacialHairHue;

        public CreateCharAppearanceGump()
            : base(0, 0)
        {
            _renderFullScreen = false;
            // backdrop
            AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            AddGumpling(new GumpPic(this, 0, 0, 0, 5500, 0));
            // character name 
            AddGumpling(new GumpPic(this, 0, 280, 53, 1801, 0));
            _Name = new TextEntry(this, 0, 238, 70, 234, 20, 0, 0, 29, string.Empty);
            AddGumpling(new ResizePic(this, _Name));
            AddGumpling(_Name);
            // character window
            AddGumpling(new GumpPic(this, 0, 238, 98, 1800, 0));
            // left option window
            AddGumpling(new ResizePic(this, 0, 82, 125, 3600, 151, 310));
            // hair (male)
            AddGumpling(new TextLabelAscii(this, 1, 100, 141, 2037, 9, Data.StringList.Entry(3000121)));
            _HairMale = new DropDownList(this, 1, 97, 154, 122, 0, 6, Data.HairStyles.Male, false);
            AddGumpling(_HairMale);
            // facial hair (male)
            AddGumpling(new TextLabelAscii(this, 1, 100, 186, 2037, 9, Data.StringList.Entry(3000122)));
            _FacialHairMale = new DropDownList(this, 1, 97, 199, 122, 0, 6, Data.HairStyles.FacialHair, false);
            AddGumpling(_FacialHairMale);
            // hair (female)
            AddGumpling(new TextLabelAscii(this, 2, 100, 141, 2037, 9, Data.StringList.Entry(3000121)));
            _HairFemale = new DropDownList(this, 2, 97, 154, 122, 0, 6, Data.HairStyles.Female, false);
            AddGumpling(_HairFemale);

            // right option window
            AddGumpling(new ResizePic(this, 0, 475, 125, 3600, 151, 310));
            // skin tone
            AddGumpling(new TextLabelAscii(this, 0, 489, 141, 2037, 9, Data.StringList.Entry(3000183)));
            _SkinHue = new ColorPicker(this, 0, new Rectangle(490, 154, 120, 24), new Rectangle(490, 140, 120, 280), 7, 8, Data.Hues.SkinTones);
            AddGumpling(_SkinHue);
            // hair color
            AddGumpling(new TextLabelAscii(this, 0, 489, 186, 2037, 9, Data.StringList.Entry(3000184)));
            _HairHue = new ColorPicker(this, 0, new Rectangle(490, 199, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, Data.Hues.HairTones);
            AddGumpling(_HairHue);
            // facial hair color (male)
            AddGumpling(new TextLabelAscii(this, 1, 489, 231, 2037, 9, Data.StringList.Entry(3000185)));
            _FacialHairHue = new ColorPicker(this, 1, new Rectangle(490, 244, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, Data.Hues.HairTones);
            AddGumpling(_FacialHairHue);

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
