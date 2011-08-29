/***************************************************************************
 *   DebugGump.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
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
            AddControl(new Gumplings.ResizePic(this, 2, 0, 0, 0x2486, width, 16));
            AddControl(new Gumplings.Button(this, 2, width - 18, 0, 2117, 2118, 0, 1, 0));
            // maximized view
            AddControl(new Gumplings.ResizePic(this, 1, 0, 0, 0x2486, width, 256));
            AddControl(new Gumplings.Button(this, 1, width - 18, 0, 2117, 2118, 0, 2, 0));
            AddControl(new Gumplings.TextLabel(this, 1, 4, 2, 0, "Debug Gump"));
            // AddGumpling(new Gumplings.Button(this, 2, 2, 18, 2117, 2118, ButtonTypes.Activate, 0, 0));
            _html = (Gumplings.HtmlGump)AddControl(new Gumplings.HtmlGump(this, 1, 4, 16, width - 8, 230, 0, 0, ""));
        }

        public override void ActivateByButton(int buttonID)
        {
            // Data.AnimEncode.SaveData(59, "animdata");
            // Data.AnimEncode.TransformData(59, "animdata");
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _html.Text = ClientVars.DebugVars.DebugMessage;
            base.Update(gameTime);
        }
    }
}
