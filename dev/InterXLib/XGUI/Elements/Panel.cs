using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.XGUI.Elements
{
    public class Panel : AElement
    {
        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.PanelView(this, Manager);
        }

        public bool DrawHeader
        {
            get { return m_DrawHeader; }
            set { m_DrawHeader = value; }
        }

        public object HeaderIcon
        {
            get { return null; }
        }

        public bool DrawAsRecessed
        {
            get { return m_DrawAsRecessed; }
            set { m_DrawAsRecessed = value; }
        }

        private bool m_DrawHeader = false;
        private bool m_DrawAsRecessed = false;

        public Panel(AElement parent, int page)
            : base(parent, page)
        {

        }

        public string Caption = null;
    }
}
