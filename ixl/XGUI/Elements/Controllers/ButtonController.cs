using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Input.Windows;

namespace InterXLib.XGUI.Elements.Controllers
{
    class ButtonController : AElementController
    {
        new Button Model
        {
            get
            {
                return (Button)base.Model;
            }
        }

        public ButtonController(AElement model, GUIManager manager)
            : base(model, manager)
        {

        }

        protected override bool OnMouseClick(InputEventMouse e)
        {
            if (Model.OnClickEvent != null)
                Model.OnClickEvent();
            return true;
        }
    }
}
