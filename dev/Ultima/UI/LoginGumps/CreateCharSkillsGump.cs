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
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Core.Resources;
#endregion

namespace UltimaXNA.Ultima.UI.LoginGumps
{
    class CreateCharSkillsGump : Gump
    {
        public delegate void EventNoParams();

        enum Buttons
        {
            BackButton,
            ForwardButton,
            QuitButton
        }

        public EventNoParams OnForward;
        public EventNoParams OnBackward;

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

        public CreateCharSkillsGump()
            : base(0, 0)
        {
            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 9274));
            AddControl(new GumpPic(this, 0, 0, 5500, 0));
            // center menu with fancy top
            AddControl(new ResizePic(this, 100, 80, 2600, 470, 372), 1);
            AddControl(new GumpPic(this, 291, 42, 1417, 0), 1);
            AddControl(new GumpPic(this, 214, 58, 1419, 0), 1);
            AddControl(new GumpPic(this, 300, 51, 5545, 0), 1);
            // title text
            AddControl(new TextLabelAscii(this, 148, 132, 841, 2, provider.GetString(3000326)), 1);

            // strength, dexterity, intelligence
            AddControl(new TextLabelAscii(this, 158, 170, 2430, 1, provider.GetString(3000111)), 1);
            AddControl(new TextLabelAscii(this, 158, 250, 2430, 1, provider.GetString(3000112)), 1);
            AddControl(new TextLabelAscii(this, 158, 330, 2430, 1, provider.GetString(3000113)), 1);
            // sliders for attributes
            sliderAttributes = new HSliderBar[3];
            sliderAttributes[0] = new HSliderBar(this, 164, 196, 93, 10, 60, 60, HSliderBarStyle.MetalWidgetRecessedBar);
            sliderAttributes[1] = new HSliderBar(this, 164, 276, 93, 10, 60, 10, HSliderBarStyle.MetalWidgetRecessedBar);
            sliderAttributes[2] = new HSliderBar(this, 164, 356, 93, 10, 60, 10, HSliderBarStyle.MetalWidgetRecessedBar);
            // labels for attributes
            lblAttributes = new TextLabelAscii[3];
            lblAttributes[0] = new TextLabelAscii(this, 284, 170, 2430, 1, string.Empty);
            lblAttributes[1] = new TextLabelAscii(this, 284, 250, 2430, 1, string.Empty);
            lblAttributes[2] = new TextLabelAscii(this, 284, 330, 2430, 1, string.Empty);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
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
            lblSkills[0] = new TextLabelAscii(this, 494, 200, 2430, 1, string.Empty);
            lblSkills[1] = new TextLabelAscii(this, 494, 280, 2430, 1, string.Empty);
            lblSkills[2] = new TextLabelAscii(this, 494, 360, 2430, 1, string.Empty);
            // drop downs for skills
            listSkills = new DropDownList[3];
            string[] skillList = SkillsData.ListNames;
            listSkills[0] = new DropDownList(this, 344, 172, 182, skillList, 8, -1, true);
            listSkills[1] = new DropDownList(this, 344, 252, 182, skillList, 8, -1, true);
            listSkills[2] = new DropDownList(this, 344, 332, 182, skillList, 8, -1, true);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i != j)
                        sliderSkills[i].PairSlider(sliderSkills[j]);
                }
                AddControl(sliderSkills[i], 1);
                AddControl(lblSkills[i], 1);
                AddControl(listSkills[i], 1);
            }
            AddControl(new TextLabelAscii(this, 158, 170, 2430, 1, provider.GetString(3000111)), 1);
            AddControl(new TextLabelAscii(this, 158, 250, 2430, 1, provider.GetString(3000112)), 1);
            AddControl(new TextLabelAscii(this, 158, 330, 2430, 1, provider.GetString(3000113)), 1);

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

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            for (int i = 0; i < 3; i++)
            {
                lblAttributes[i].Text = sliderAttributes[i].Value.ToString();
                lblSkills[i].Text = sliderSkills[i].Value.ToString();
            }
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
                    UltimaGame.IsRunning = false;
                    break;
            }
        }
    }
}
