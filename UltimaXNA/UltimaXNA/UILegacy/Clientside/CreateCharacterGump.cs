using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
{
    class CreateCharacterGump : Gump
    {
        Slider[] sliderAttributes;
        TextLabelAscii[] lblAttributes;

        Slider[] sliderSkills;
        TextLabelAscii[] lblSkills;

        public CreateCharacterGump()
            : base(0, 0)
        {
            _renderFullScreen = false;
            // backdrop
            this.AddGumpling(new GumpPicTiled(this, 0, 0, 0, 640, 480, 9274));
            this.AddGumpling(new GumpPic(this, 0, 0, 0, 9003, 0));
            // center menu with fancy top
            AddGumpling(new ResizePic(this, 1, 100, 80, 2600, 470, 372));
            AddGumpling(new GumpPic(this, 1, 291, 42, 1417, 0));
            AddGumpling(new GumpPic(this, 1, 214, 58, 1419, 0));
            AddGumpling(new GumpPic(this, 1, 300, 51, 5545, 0));
            // title text
            AddGumpling(new TextLabelAscii(this, 1, 148, 132, 841, 2, Data.StringList.Table[3000326].ToString()));
            
            // sliders for attributes
            sliderAttributes = new Slider[3];
            sliderAttributes[0] = new Slider(this, 1, 164, 196, 93, 10, 60, 60);
            sliderAttributes[1] = new Slider(this, 1, 164, 276, 93, 10, 60, 10);
            sliderAttributes[2] = new Slider(this, 1, 164, 356, 93, 10, 60, 10);
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
            AddGumpling(new TextLabelAscii(this, 1, 158, 170, 2430, 1, Data.StringList.Table[3000111].ToString()));
            AddGumpling(new TextLabelAscii(this, 1, 158, 250, 2430, 1, Data.StringList.Table[3000112].ToString()));
            AddGumpling(new TextLabelAscii(this, 1, 158, 330, 2430, 1, Data.StringList.Table[3000113].ToString()));

            // sliders for skills
            sliderSkills = new Slider[3];
            sliderSkills[0] = new Slider(this, 1, 344, 204, 93, 0, 50, 50);
            sliderSkills[1] = new Slider(this, 1, 344, 284, 93, 0, 50, 50);
            sliderSkills[2] = new Slider(this, 1, 344, 364, 93, 0, 50, 0);
            lblSkills = new TextLabelAscii[3];
            lblSkills[0] = new TextLabelAscii(this, 1, 494, 200, 2430, 1, string.Empty);
            lblSkills[1] = new TextLabelAscii(this, 1, 494, 280, 2430, 1, string.Empty);
            lblSkills[2] = new TextLabelAscii(this, 1, 494, 360, 2430, 1, string.Empty);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i != j)
                        sliderSkills[i].PairSlider(sliderSkills[j]);
                }
                AddGumpling(sliderSkills[i]);
                AddGumpling(lblSkills[i]);
            }
            AddGumpling(new TextLabelAscii(this, 1, 158, 170, 2430, 1, Data.StringList.Table[3000111].ToString()));
            AddGumpling(new TextLabelAscii(this, 1, 158, 250, 2430, 1, Data.StringList.Table[3000112].ToString()));
            AddGumpling(new TextLabelAscii(this, 1, 158, 330, 2430, 1, Data.StringList.Table[3000113].ToString()));

            string[] skillList = Data.Skills.ListNames;

            AddGumpling(new DropDownList(this, 1, 344, 172, 182, -1, 8, skillList));
            AddGumpling(new DropDownList(this, 1, 344, 252, 182, -1, 8, skillList));
            AddGumpling(new DropDownList(this, 1, 344, 332, 182, -1, 8, skillList));
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
    }
}
