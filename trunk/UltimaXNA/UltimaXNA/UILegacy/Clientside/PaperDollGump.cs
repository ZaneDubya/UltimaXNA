using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
{
    class PaperDollGump: Gump
    {
        public PaperDollGump()
            : base(0, 0)
        {
            IsMovable = true;
            AddGumpling(new GumpPic(this, 0, 0, 0, 0x07d0, 0));
            this.LastGumpling.MakeADragger(this);

            // HELP
            AddGumpling(new Button(this, 0, 186, 45, 0x07ef, 0x07f0, ButtonTypes.Activate, 0, 0));

            // OPTIONS
            AddGumpling(new Button(this, 0, 186, 45 + 27 * 1, 0x07d6, 0x07d7, ButtonTypes.Activate, 0, 0));

            // LOG OUT
            AddGumpling(new Button(this, 0, 186, 45 + 27 * 2, 0x07d9, 0x07da, ButtonTypes.Activate, 0, 0));

            // QUESTS
            AddGumpling(new Button(this, 0, 186, 45 + 27 * 3, 0x57b5, 0x57b6, ButtonTypes.Activate, 0, 0));

            // SKILLS
            AddGumpling(new Button(this, 0, 186, 45 + 27 * 4, 0x07df, 0x07e0, ButtonTypes.Activate, 0, 0));

            // GUILD
            AddGumpling(new Button(this, 0, 186, 45 + 27 * 5, 0x57b2, 0x57b3, ButtonTypes.Activate, 0, 0));

            // PEACE / WAR
            AddGumpling(new Button(this, 0, 186, 45 + 27 * 6, 0x07e5, 0x07e6, ButtonTypes.Activate, 0, 0));

            // STATUS
            AddGumpling(new Button(this, 0, 186, 45 + 27 * 7, 0x07eb, 0x07ec, ButtonTypes.Activate, 0, 0));
            // ((Button)LastGumpling.
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        void onClickMap(int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.RightButton)
            {
                Dispose();
            }
        }
    }
}
