using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
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

        Slider[] sliderAttributes; TextLabelAscii[] lblAttributes;
        Slider[] sliderSkills; TextLabelAscii[] lblSkills; DropDownList[] listSkills;

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
            _renderFullScreen = false;
            // backdrop
            AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            AddGumpling(new GumpPic(this, 0, 0, 0, 5500, 0));
            // center menu with fancy top
            AddGumpling(new ResizePic(this, 1, 100, 80, 2600, 470, 372));
            AddGumpling(new GumpPic(this, 1, 291, 42, 1417, 0));
            AddGumpling(new GumpPic(this, 1, 214, 58, 1419, 0));
            AddGumpling(new GumpPic(this, 1, 300, 51, 5545, 0));
            // title text
            AddGumpling(new TextLabelAscii(this, 1, 148, 132, 841, 2, Data.StringList.Entry(3000326)));

            // strength, dexterity, intelligence
            AddGumpling(new TextLabelAscii(this, 1, 158, 170, 2430, 1, Data.StringList.Entry(3000111)));
            AddGumpling(new TextLabelAscii(this, 1, 158, 250, 2430, 1, Data.StringList.Entry(3000112)));
            AddGumpling(new TextLabelAscii(this, 1, 158, 330, 2430, 1, Data.StringList.Entry(3000113)));
            // sliders for attributes
            sliderAttributes = new Slider[3];
            sliderAttributes[0] = new Slider(this, 1, 164, 196, 93, 10, 60, 60);
            sliderAttributes[1] = new Slider(this, 1, 164, 276, 93, 10, 60, 10);
            sliderAttributes[2] = new Slider(this, 1, 164, 356, 93, 10, 60, 10);
            // labels for attributes
            lblAttributes = new TextLabelAscii[3];
            lblAttributes[0] = new TextLabelAscii(this, 1, 284, 170, 2430, 1, string.Empty);
            lblAttributes[1] = new TextLabelAscii(this, 1, 284, 250, 2430, 1, string.Empty);
            lblAttributes[2] = new TextLabelAscii(this, 1, 284, 330, 2430, 1, string.Empty);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i != j)
                        sliderAttributes[i].PairSlider(sliderAttributes[j]);
                }
                AddGumpling(sliderAttributes[i]);
                AddGumpling(lblAttributes[i]);
            }

            // sliders for skills
            sliderSkills = new Slider[3];
            sliderSkills[0] = new Slider(this, 1, 344, 204, 93, 0, 50, 50);
            sliderSkills[1] = new Slider(this, 1, 344, 284, 93, 0, 50, 50);
            sliderSkills[2] = new Slider(this, 1, 344, 364, 93, 0, 50, 0);
            // labels for skills
            lblSkills = new TextLabelAscii[3];
            lblSkills[0] = new TextLabelAscii(this, 1, 494, 200, 2430, 1, string.Empty);
            lblSkills[1] = new TextLabelAscii(this, 1, 494, 280, 2430, 1, string.Empty);
            lblSkills[2] = new TextLabelAscii(this, 1, 494, 360, 2430, 1, string.Empty);
            // drop downs for skills
            listSkills = new DropDownList[3];
            string[] skillList = Data.Skills.ListNames;
            listSkills[0] = new DropDownList(this, 1, 344, 172, 182, -1, 8, skillList, true);
            listSkills[1] = new DropDownList(this, 1, 344, 252, 182, -1, 8, skillList, true);
            listSkills[2] = new DropDownList(this, 1, 344, 332, 182, -1, 8, skillList, true);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i != j)
                        sliderSkills[i].PairSlider(sliderSkills[j]);
                }
                AddGumpling(sliderSkills[i]);
                AddGumpling(lblSkills[i]);
                AddGumpling(listSkills[i]);
            }
            AddGumpling(new TextLabelAscii(this, 1, 158, 170, 2430, 1, Data.StringList.Entry(3000111)));
            AddGumpling(new TextLabelAscii(this, 1, 158, 250, 2430, 1, Data.StringList.Entry(3000112)));
            AddGumpling(new TextLabelAscii(this, 1, 158, 330, 2430, 1, Data.StringList.Entry(3000113)));

            // back button
            AddGumpling(new Button(this, 1, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)Buttons.BackButton));
            ((Button)this.LastGumpling).GumpOverID = 5538;
            // forward button
            AddGumpling(new Button(this, 1, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)Buttons.ForwardButton));
            ((Button)this.LastGumpling).GumpOverID = 5541;
            // quit button
            AddGumpling(new Button(this, 0, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)Buttons.QuitButton));
            ((Button)this.LastGumpling).GumpOverID = 5514;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
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
                    Quit();
                    break;
            }
        }
    }
}
