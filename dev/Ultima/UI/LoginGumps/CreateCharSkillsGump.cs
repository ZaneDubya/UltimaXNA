/***************************************************************************
 *   CreateCharSkillsGump.cs
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
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.Login.Data;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.LoginGumps {
    class CreateCharSkillsGump : Gump {
        enum Buttons {
            BackButton,
            ForwardButton,
            QuitButton
        }

        event Action m_OnForward;
        event Action m_OnBackward;

        HSliderBar[] sliderAttributes; TextLabelAscii[] lblAttributes;
        HSliderBar[] sliderSkills; TextLabelAscii[] lblSkills; DropDownList[] listSkills;

        public int Strength { get { return sliderAttributes[0].Value; } set { sliderAttributes[0].Value = value; } }
        public int Dexterity { get { return sliderAttributes[1].Value; } set { sliderAttributes[1].Value = value; } }
        public int Intelligence { get { return sliderAttributes[2].Value; } set { sliderAttributes[2].Value = value; } }
        public int SkillIndex0 { get { return listSkills[0].Index; } set { listSkills[0].Index = value; } }
        public int SkillIndex1 { get { return listSkills[1].Index; } set { listSkills[1].Index = value; } }
        public int SkillIndex2 { get { return listSkills[2].Index; } set { listSkills[2].Index = value; } }
        public int SkillPoints0 { get { return sliderSkills[0].Value; } set { sliderSkills[0].Value = value; } }
        public int SkillPoints1 { get { return sliderSkills[1].Value; } set { sliderSkills[1].Value = value; } }
        public int SkillPoints2 { get { return sliderSkills[2].Value; } set { sliderSkills[2].Value = value; } }

        public CreateCharSkillsGump(Action onForward, Action onBackward)
            : base(0, 0) {
            m_OnForward = onForward;
            m_OnBackward = onBackward;

            // get the resource provider
            IResourceProvider provider = Services.Get<IResourceProvider>();

            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 9274));
            AddControl(new GumpPic(this, 0, 0, 5500, 0));
            // center menu with fancy top
            AddControl(new ResizePic(this, 100, 80, 2600, 470, 372), 1);
            AddControl(new GumpPic(this, 291, 42, 1417, 0), 1);
            AddControl(new GumpPic(this, 214, 58, 1419, 0), 1);
            AddControl(new GumpPic(this, 300, 51, 5545, 0), 1);
            // title text
            AddControl(new TextLabelAscii(this, 148, 132, 2, 841, provider.GetString(3000326)), 1);

            // strength, dexterity, intelligence
            AddControl(new TextLabelAscii(this, 158, 170, 1, 2430, provider.GetString(3000111)), 1);
            AddControl(new TextLabelAscii(this, 158, 250, 1, 2430, provider.GetString(3000112)), 1);
            AddControl(new TextLabelAscii(this, 158, 330, 1, 2430, provider.GetString(3000113)), 1);
            // sliders for attributes
            sliderAttributes = new HSliderBar[3];
            sliderAttributes[0] = new HSliderBar(this, 164, 196, 93, 10, 60, 60, HSliderBarStyle.MetalWidgetRecessedBar);
            sliderAttributes[1] = new HSliderBar(this, 164, 276, 93, 10, 60, 10, HSliderBarStyle.MetalWidgetRecessedBar);
            sliderAttributes[2] = new HSliderBar(this, 164, 356, 93, 10, 60, 10, HSliderBarStyle.MetalWidgetRecessedBar);
            // labels for attributes
            lblAttributes = new TextLabelAscii[3];
            lblAttributes[0] = new TextLabelAscii(this, 284, 170, 1, 2430, string.Empty);
            lblAttributes[1] = new TextLabelAscii(this, 284, 250, 1, 2430, string.Empty);
            lblAttributes[2] = new TextLabelAscii(this, 284, 330, 1, 2430, string.Empty);
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (i != j)
                        sliderAttributes[i].PairSlider(sliderAttributes[j]);
                }
                AddControl(sliderAttributes[i], 1);
                AddControl(lblAttributes[i], 1);
            }

            // sliders for skills
            sliderSkills = new HSliderBar[3];
            sliderSkills[0] = new HSliderBar(this, 344, 204, 93, 0, 50, 50, HSliderBarStyle.MetalWidgetRecessedBar);
            sliderSkills[1] = new HSliderBar(this, 344, 284, 93, 0, 50, 50, HSliderBarStyle.MetalWidgetRecessedBar);
            sliderSkills[2] = new HSliderBar(this, 344, 364, 93, 0, 50, 0, HSliderBarStyle.MetalWidgetRecessedBar);
            // labels for skills
            lblSkills = new TextLabelAscii[3];
            lblSkills[0] = new TextLabelAscii(this, 494, 200, 1, 2430, string.Empty);
            lblSkills[1] = new TextLabelAscii(this, 494, 280, 1, 2430, string.Empty);
            lblSkills[2] = new TextLabelAscii(this, 494, 360, 1, 2430, string.Empty);
            // drop downs for skills
            listSkills = new DropDownList[3];
            string[] skillList = SkillsData.ListNames;
            listSkills[0] = new DropDownList(this, 344, 172, 182, skillList, 8, -1, true);
            listSkills[1] = new DropDownList(this, 344, 252, 182, skillList, 8, -1, true);
            listSkills[2] = new DropDownList(this, 344, 332, 182, skillList, 8, -1, true);
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (i != j)
                        sliderSkills[i].PairSlider(sliderSkills[j]);
                }
                AddControl(sliderSkills[i], 1);
                AddControl(lblSkills[i], 1);
                AddControl(listSkills[i], 1);
            }
            AddControl(new TextLabelAscii(this, 158, 170, 1, 2430, provider.GetString(3000111)), 1);
            AddControl(new TextLabelAscii(this, 158, 250, 1, 2430, provider.GetString(3000112)), 1);
            AddControl(new TextLabelAscii(this, 158, 330, 1, 2430, provider.GetString(3000113)), 1);

            // back button
            AddControl(new Button(this, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)Buttons.BackButton), 1);
            ((Button)LastControl).GumpOverID = 5538;
            // forward button
            AddControl(new Button(this, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)Buttons.ForwardButton), 1);
            ((Button)LastControl).GumpOverID = 5541;
            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)Buttons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;

            IsUncloseableWithRMB = true;
        }

        internal void SaveData(CreateCharacterData data) {
            // save the values;
            data.Attributes[0] = Strength;
            data.Attributes[1] = Dexterity;
            data.Attributes[2] = Intelligence;
            data.SkillIndexes[0] = SkillIndex0;
            data.SkillIndexes[1] = SkillIndex1;
            data.SkillIndexes[2] = SkillIndex2;
            data.SkillValues[0] = SkillPoints0;
            data.SkillValues[1] = SkillPoints1;
            data.SkillValues[2] = SkillPoints2;
            data.HasSkillData = true;
        }

        internal void RestoreData(CreateCharacterData data) {
            Strength = data.Attributes[0];
            Dexterity = data.Attributes[1];
            Intelligence = data.Attributes[2];
            SkillIndex0 = data.SkillIndexes[0];
            SkillIndex1 = data.SkillIndexes[1];
            SkillIndex2 = data.SkillIndexes[2];
            SkillPoints0 = data.SkillValues[0];
            SkillPoints1 = data.SkillValues[1];
            SkillPoints2 = data.SkillValues[2];
        }

        public override void Update(double totalMS, double frameMS) {
            base.Update(totalMS, frameMS);
            for (int i = 0; i < 3; i++) {
                lblAttributes[i].Text = sliderAttributes[i].Value.ToString();
                lblSkills[i].Text = sliderSkills[i].Value.ToString();
            }
        }

        public override void OnButtonClick(int buttonID) {
            switch ((Buttons)buttonID) {
                case Buttons.BackButton:
                    m_OnBackward();
                    break;
                case Buttons.ForwardButton:
                    m_OnForward();
                    break;
                case Buttons.QuitButton:
                    Services.Get<UltimaGame>().Quit();
                    break;
            }
        }
    }
}
