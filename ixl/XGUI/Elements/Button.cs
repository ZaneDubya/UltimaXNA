using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.XGUI.Elements
{
    public class Button : AElement
    {
        public string Caption = null;
        public bool DrawIcon = false;

        public XGUIAction OnClickEvent = null;

        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.ButtonView(this, Manager);
        }

        protected override Patterns.MVC.AController CreateController()
        {
            return new Controllers.ButtonController(this, Manager);
        }

        public Button(AElement parent, int page)
            : base(parent, page)
        {
            HandlesMouseInput = true;
        }
    }
}
