using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Data;

namespace UltimaXNA.UILegacy.Gumplings
{
    class ListGumpling : HtmlGump
    {
        public ListGumpling(Control owner, int page, int x, int y, int width, int height)
            : base(owner, page, x, y, width, height, 0, 1, "")
        {
            Background = true;

            StringBuilder str = new StringBuilder();

            Skill[] skills = Data.Skills.List;
            foreach (Skill skill in skills)
            {
                if (skill.UseButton)
                {
                    str.Append("<a href='activateskill=" +
                        skill.Name + "' color='5b4f29' hovercolor='857951' activecolor='402708' text-decoration=none>" + 
                        "<gumpimg src='2103' hoversrc='2104' activesrc='2103'/><span width='2'/>" + skill.Name + "</a><br/>");
                }
                else
                {
                    str.Append("<span width='14'/><medium color=50422D>" + skill.Name + "</medium><br/>");
                }
                
            }
            Text = str.ToString();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(Graphics.ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
