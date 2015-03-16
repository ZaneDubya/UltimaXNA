using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.XGUI.Elements
{
    public class CheckBox : AElement
    {
        public string Caption = null;
        public bool DrawIcon = false;

        public XGUIAction OnClickEvent = null;

        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.CheckBoxView(this, Manager);
        }

        protected override Patterns.MVC.AController CreateController()
        {
            return new Controllers.CheckBoxController(this, Manager);
        }

        public CheckBox(AElement parent, int page)
            : base(parent, page)
        {
            HandlesMouseInput = true;
            FontSize = 14f;
        }

        public bool IsChecked = false;
    }
}
