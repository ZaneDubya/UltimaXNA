using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.XGUI.Elements.Views
{
    class RadioButtonView : ButtonView
    {
        new RadioButton Model
        {
            get
            {
                return (RadioButton)base.Model;
            }
        }

        public RadioButtonView(Button button, GUIManager manager, bool as_icon = false, 
            string icon_normal = null, string icon_hover = null, string icon_down = null)
            : base(button, manager, as_icon, icon_normal, icon_hover, icon_down)
        {

        }

        private bool m_SaveMouseDownStatus = false;
        private bool m_SaveMouseOverStatus = false;

        protected override void InternalBeforeDraw()
        {
            if (Model.IsDown)
            {
                m_SaveMouseOverStatus = Model.IsMouseOver;
                m_SaveMouseDownStatus = Model.IsMouseDown;
                Model.IsMouseOver = true;
                Model.IsMouseDown = true;
            }
        }

        protected override void InternalAfterDraw()
        {
            if (Model.IsDown)
            {
                Model.IsMouseOver = m_SaveMouseOverStatus;
                Model.IsMouseDown = m_SaveMouseDownStatus;
            }
        }
    }
}
