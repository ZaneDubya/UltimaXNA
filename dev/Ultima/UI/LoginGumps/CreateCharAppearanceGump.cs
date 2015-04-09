/***************************************************************************
 *   CreateCharAppearanceGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using UltimaXNA.Configuration;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.ClientVars;

namespace UltimaXNA.Ultima.UI.LoginGumps
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

        public string Name { get { return m_Name.Text; } set { m_Name.Text = value; } }
        public int Gender { get { return m_Gender.Index; } set { } }
        public int Race { get { return 1; } set { } } // hard coded to human
        public int HairID
        { 
            get { return (Gender == 0) ? IO.HairStyles.MaleIDs[m_HairMale.Index] : IO.HairStyles.FemaleIDs[m_HairFemale.Index]; }
            set
            {
                for (int i = 0; i < 10; i++)
                {
                    if (value == ((Gender == 0) ? IO.HairStyles.MaleIDs[i] : IO.HairStyles.FemaleIDs[i]))
                    {
                        m_HairMale.Index = i;
                        m_HairFemale.Index = i;
                        break;
                    }
                }
            }
        }
        public int FacialHairID
        {
            get { return (Gender == 0) ? IO.HairStyles.FacialHairIDs[m_FacialHairMale.Index] : 0; }
            set
            {
                for (int i = 0; i < 8; i++)
                {
                    if (value == IO.HairStyles.FacialHairIDs[i])
                    {
                        m_FacialHairMale.Index = i;
                        break;
                    }
                }
            }
        }
        public int SkinHue
        {
            get { return m_SkinHue.HueValue; }
            set { m_SkinHue.HueValue = value; }
        }
        public int HairHue
        {
            get { return m_HairHue.HueValue; }
            set { m_HairHue.HueValue = value; }
        }
        public int FacialHairHue
        {
            get { return (Gender == 0) ? m_FacialHairHue.HueValue : 0; }
            set { m_FacialHairHue.HueValue = value; }
        }

        TextEntry m_Name;
        DropDownList m_Gender;
        DropDownList m_HairMale;
        DropDownList m_FacialHairMale;
        DropDownList m_HairFemale;
        ColorPicker m_SkinHue;
        ColorPicker m_HairHue;
        ColorPicker m_FacialHairHue;
        PaperdollLargeUninteractable m_paperdoll;

        public CreateCharAppearanceGump()
            : base(0, 0)
        {
            m_renderFullScreen = false;
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 0, Settings.Game.Resolution.Width, Settings.Game.Resolution.Height, 9274));
            AddControl(new GumpPic(this, 0, 0, 0, 5500, 0));
            // character name 
            AddControl(new GumpPic(this, 0, 280, 53, 1801, 0));
            m_Name = new TextEntry(this, 0, 238, 70, 234, 20, 0, 0, 29, string.Empty);
            AddControl(new ResizePic(this, m_Name));
            AddControl(m_Name);
            // character window
            AddControl(new GumpPic(this, 0, 238, 98, 1800, 0));
            // paperdoll
            m_paperdoll = new PaperdollLargeUninteractable(this, 0, 237, 97);
            m_paperdoll.IsCharacterCreation = true;
            AddControl(m_paperdoll);

            // left option window
            AddControl(new ResizePic(this, 0, 82, 125, 3600, 151, 310));
            // this is the place where you would put the race selector.
            // if you do add it, move everything else in this left window down by 45 pixels
            // gender
            AddControl(new TextLabelAscii(this, 1, 100, 141, 2037, 9, IO.StringData.Entry(3000120)));
            m_Gender = new DropDownList(this, 0, 97, 154, 122, 0, 2, new string[] { IO.StringData.Entry(3000118), IO.StringData.Entry(3000119) }, false);
            AddControl(m_Gender);
            // hair (male)
            AddControl(new TextLabelAscii(this, 1, 100, 186, 2037, 9, IO.StringData.Entry(3000121)));
            m_HairMale = new DropDownList(this, 1, 97, 199, 122, 0, 6, IO.HairStyles.MaleHairNames, false);
            AddControl(m_HairMale);
            // facial hair (male)
            AddControl(new TextLabelAscii(this, 1, 100, 231, 2037, 9, IO.StringData.Entry(3000122)));
            m_FacialHairMale = new DropDownList(this, 1, 97, 244, 122, 0, 6, IO.HairStyles.FacialHair, false);
            AddControl(m_FacialHairMale);
            // hair (female)
            AddControl(new TextLabelAscii(this, 2, 100, 186, 2037, 9, IO.StringData.Entry(3000121)));
            m_HairFemale = new DropDownList(this, 2, 97, 199, 122, 0, 6, IO.HairStyles.FemaleHairNames, false);
            AddControl(m_HairFemale);

            // right option window
            AddControl(new ResizePic(this, 0, 475, 125, 3600, 151, 310));
            // skin tone
            AddControl(new TextLabelAscii(this, 0, 489, 141, 2037, 9, IO.StringData.Entry(3000183)));
            m_SkinHue = new ColorPicker(this, 0, new Rectangle(490, 154, 120, 24), new Rectangle(490, 140, 120, 280), 7, 8, IO.HueData.SkinTones);
            AddControl(m_SkinHue);
            // hair color
            AddControl(new TextLabelAscii(this, 0, 489, 186, 2037, 9, IO.StringData.Entry(3000184)));
            m_HairHue = new ColorPicker(this, 0, new Rectangle(490, 199, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, IO.HueData.HairTones);
            AddControl(m_HairHue);
            // facial hair color (male)
            AddControl(new TextLabelAscii(this, 1, 489, 231, 2037, 9, IO.StringData.Entry(3000185)));
            m_FacialHairHue = new ColorPicker(this, 1, new Rectangle(490, 244, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, IO.HueData.HairTones);
            AddControl(m_FacialHairHue);

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

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            
            // show different controls based on what gender we're looking at.
            // Also copy over the hair id to facilitate easy switching between male and female appearances.
            if (m_Gender.Index == 0)
            {
                ActivePage = 1;
                m_HairFemale.Index = m_HairMale.Index;
            }
            else
            {
                ActivePage = 2;
                m_HairMale.Index = m_HairFemale.Index;
            }
            // update the paperdoll
            m_paperdoll.Gender = m_Gender.Index;
            m_paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.Body, SkinHue);
            m_paperdoll.SetSlotEquipment(PaperdollLargeUninteractable.EquipSlots.Hair, HairID);
            m_paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.Hair, HairHue);
            m_paperdoll.SetSlotEquipment(PaperdollLargeUninteractable.EquipSlots.FacialHair, FacialHairID);
            m_paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.FacialHair, FacialHairHue);
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
                    EngineVars.EngineRunning = false;
                    break;
            }
        }
    }
}
