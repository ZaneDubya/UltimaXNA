using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.XGUI.Elements
{
    public class ButtonAsIcon : Button
    {
        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.ButtonView(this, Manager, true, m_IconNormal, m_IconDown, m_IconHover);
        }

        private string m_IconNormal, m_IconDown, m_IconHover;

        public ButtonAsIcon(AElement parent, int page, string icon_normal = null, string icon_down = null, string icon_hover = null)
            : base(parent, page)
        {
            m_IconNormal = icon_normal;
            m_IconDown = icon_down;
            m_IconHover = icon_hover;
        }
    }
}
