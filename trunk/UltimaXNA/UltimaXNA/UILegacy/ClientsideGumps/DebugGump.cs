using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.ClientsideGumps
{
    public class DebugGump : Gump 
    {
        HtmlGump _html;
        public DebugGump()
            : base(0, 0)
        {
            int width = 200;
            // minimized view
            IsMovable = true;
            AddGumpling(new Gumplings.ResizePic(this, 1, 0, 0, 0x2486, width, 16));
            AddGumpling(new Gumplings.Button(this, 1, width - 18, 0, 2117, 2118, 0, 2, 0));
            // maximized view
            AddGumpling(new Gumplings.ResizePic(this, 2, 0, 0, 0x2486, width, 256));
            AddGumpling(new Gumplings.Button(this, 2, width - 18, 0, 2117, 2118, 0, 1, 0));
            AddGumpling(new Gumplings.TextLabel(this, 2, 4, 2, 0, "Debug Gump"));
            // AddGumpling(new Gumplings.Button(this, 2, 2, 18, 2117, 2118, ButtonTypes.Activate, 0, 0));
            _html = (Gumplings.HtmlGump)AddGumpling(new Gumplings.HtmlGump(this, 2, 4, 16, width - 8, 230, 0, 0, ""));
        }

        public override void ActivateByButton(int buttonID)
        {
            // Data.AnimEncode.SaveData(59, "animdata");
            // Data.AnimEncode.TransformData(59, "animdata");
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _html.Text = ClientVars.DebugMessage;
            base.Update(gameTime);
        }
    }
}
