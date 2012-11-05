﻿/***************************************************************************
 *   CreateCharAppearanceGump.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaGUI.Gumplings;

namespace UltimaXNA.UltimaGUI.ClientsideGumps
{
    class CreateCharAppearanceGump : Gump
    {
        public delegate void EventNoParams();
        public EventNoParams OnForward;
        public EventNoParams OnBackward;

        enum Buttons
        {
            BackButton,
            ForwardButton,
            QuitButton
        }

        public string Name { get { return _Name.Text; } set { _Name.Text = value; } }
        public int Gender { get { return _Gender.Index; } set { } }
        public int Race { get { return 1; } set { } } // hard coded to human
        public int HairID
        { 
            get { return (Gender == 0) ? UltimaData.HairStyles.MaleIDs[_HairMale.Index] : UltimaData.HairStyles.FemaleIDs[_HairFemale.Index]; }
            set
            {
                for (int i = 0; i < 10; i++)
                {
                    if (value == ((Gender == 0) ? UltimaData.HairStyles.MaleIDs[i] : UltimaData.HairStyles.FemaleIDs[i]))
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
            get { return (Gender == 0) ? UltimaData.HairStyles.FacialHairIDs[_FacialHairMale.Index] : 0; }
            set
            {
                for (int i = 0; i < 8; i++)
                {
                    if (value == UltimaData.HairStyles.FacialHairIDs[i])
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
        DropDownList _Gender;
        DropDownList _HairMale;
        DropDownList _FacialHairMale;
        DropDownList _HairFemale;
        ColorPicker _SkinHue;
        ColorPicker _HairHue;
        ColorPicker _FacialHairHue;
        PaperdollLargeUninteractable _paperdoll;

        public CreateCharAppearanceGump()
            : base(0, 0)
        {
            _renderFullScreen = false;
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            AddControl(new GumpPic(this, 0, 0, 0, 5500, 0));
            // character name 
            AddControl(new GumpPic(this, 0, 280, 53, 1801, 0));
            _Name = new TextEntry(this, 0, 238, 70, 234, 20, 0, 0, 29, string.Empty);
            AddControl(new ResizePic(this, _Name));
            AddControl(_Name);
            // character window
            AddControl(new GumpPic(this, 0, 238, 98, 1800, 0));
            // paperdoll
            _paperdoll = new PaperdollLargeUninteractable(this, 0, 237, 97);
            _paperdoll.IsCharacterCreation = true;
            AddControl(_paperdoll);

            // left option window
            AddControl(new ResizePic(this, 0, 82, 125, 3600, 151, 310));
            // this is the place where you would put the race selector.
            // if you do add it, move everything else in this left window down by 45 pixels
            // gender
            AddControl(new TextLabelAscii(this, 1, 100, 141, 2037, 9, UltimaData.StringList.Entry(3000120)));
            _Gender = new DropDownList(this, 0, 97, 154, 122, 0, 2, new string[] { UltimaData.StringList.Entry(3000118), UltimaData.StringList.Entry(3000119) }, false);
            AddControl(_Gender);
            // hair (male)
            AddControl(new TextLabelAscii(this, 1, 100, 186, 2037, 9, UltimaData.StringList.Entry(3000121)));
            _HairMale = new DropDownList(this, 1, 97, 199, 122, 0, 6, UltimaData.HairStyles.MaleHairNames, false);
            AddControl(_HairMale);
            // facial hair (male)
            AddControl(new TextLabelAscii(this, 1, 100, 231, 2037, 9, UltimaData.StringList.Entry(3000122)));
            _FacialHairMale = new DropDownList(this, 1, 97, 244, 122, 0, 6, UltimaData.HairStyles.FacialHair, false);
            AddControl(_FacialHairMale);
            // hair (female)
            AddControl(new TextLabelAscii(this, 2, 100, 186, 2037, 9, UltimaData.StringList.Entry(3000121)));
            _HairFemale = new DropDownList(this, 2, 97, 199, 122, 0, 6, UltimaData.HairStyles.FemaleHairNames, false);
            AddControl(_HairFemale);

            // right option window
            AddControl(new ResizePic(this, 0, 475, 125, 3600, 151, 310));
            // skin tone
            AddControl(new TextLabelAscii(this, 0, 489, 141, 2037, 9, UltimaData.StringList.Entry(3000183)));
            _SkinHue = new ColorPicker(this, 0, new Rectangle(490, 154, 120, 24), new Rectangle(490, 140, 120, 280), 7, 8, UltimaData.Hues.SkinTones);
            AddControl(_SkinHue);
            // hair color
            AddControl(new TextLabelAscii(this, 0, 489, 186, 2037, 9, UltimaData.StringList.Entry(3000184)));
            _HairHue = new ColorPicker(this, 0, new Rectangle(490, 199, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, UltimaData.Hues.HairTones);
            AddControl(_HairHue);
            // facial hair color (male)
            AddControl(new TextLabelAscii(this, 1, 489, 231, 2037, 9, UltimaData.StringList.Entry(3000185)));
            _FacialHairHue = new ColorPicker(this, 1, new Rectangle(490, 244, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, UltimaData.Hues.HairTones);
            AddControl(_FacialHairHue);

            // back button
            AddControl(new Button(this, 1, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)Buttons.BackButton));
            ((Button)LastControl).GumpOverID = 5538;
            // forward button
            AddControl(new Button(this, 1, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)Buttons.ForwardButton));
            ((Button)LastControl).GumpOverID = 5541;
            // quit button
            AddControl(new Button(this, 0, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)Buttons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            // show different controls based on what gender we're looking at.
            // Also copy over the hair id to facilitate easy switching between male and female appearances.
            if (_Gender.Index == 0)
            {
                ActivePage = 1;
                _HairFemale.Index = _HairMale.Index;
            }
            else
            {
                ActivePage = 2;
                _HairMale.Index = _HairFemale.Index;
            }
            // update the paperdoll
            _paperdoll.Gender = _Gender.Index;
            _paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.Body, SkinHue);
            _paperdoll.SetSlotEquipment(PaperdollLargeUninteractable.EquipSlots.Hair, HairID);
            _paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.Hair, HairHue);
            _paperdoll.SetSlotEquipment(PaperdollLargeUninteractable.EquipSlots.FacialHair, FacialHairID);
            _paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.FacialHair, FacialHairHue);
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
                case Buttons.QuitButton:
                    Quit();
                    break;
            }
        }
    }
}
