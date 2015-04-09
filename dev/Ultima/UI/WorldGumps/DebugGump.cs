#region Usings

/***************************************************************************
 *   DebugGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Data;
using UltimaXNA.UltimaGUI.Controls;

#endregion

namespace UltimaXNA.UltimaGUI.WorldGumps
{
    public class DebugGump : Gump
    {
        private readonly HtmlGump m_html;

        public DebugGump()
            : base(0, 0)
        {
            int width = 200;
            // minimized view
            IsMovable = true;
            AddControl(new ResizePic(this, 2, 0, 0, 0x2486, width, 16));
            AddControl(new Button(this, 2, width - 18, 0, 2117, 2118, 0, 1, 0));
            // maximized view
            AddControl(new ResizePic(this, 1, 0, 0, 0x2486, width, 256));
            AddControl(new Button(this, 1, width - 18, 0, 2117, 2118, 0, 2, 0));
            AddControl(new TextLabel(this, 1, 4, 2, 0, "Debug Gump"));
            // AddGumpling(new Controls.Button(this, 2, 2, 18, 2117, 2118, ButtonTypes.Activate, 0, 0));
            m_html = (HtmlGump)AddControl(new HtmlGump(this, 1, 4, 16, width - 8, 230, 0, 0, ""));
        }

        public override void ActivateByButton(int buttonID)
        {
            // UltimaData.AnimEncode.SaveData(59, "animdata");
            // UltimaData.AnimEncode.TransformData(59, "animdata");
        }

        public override void Update(double totalMS, double frameMS)
        {
            string debugMessage = string.Empty;

            if(Settings.Debug.ShowDataRead)
            {
                if(Settings.Debug.ShowDataReadBreakdown)
                {
                    debugMessage += Metrics.DataReadBreakdown;
                }
                else
                {
                    debugMessage += string.Format("\nData Read: {0}", Metrics.TotalDataRead);
                }
            }

            m_html.Text = debugMessage;

            base.Update(totalMS, frameMS);
        }
    }
}