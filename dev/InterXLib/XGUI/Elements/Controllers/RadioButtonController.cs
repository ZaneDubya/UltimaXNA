using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Input.Windows;

namespace InterXLib.XGUI.Elements.Controllers
{
    class RadioButtonController : ButtonController
    {
        new RadioButton Model
        {
            get
            {
                return (RadioButton)base.Model;
            }
        }

        public RadioButtonController(RadioButton model, GUIManager manager)
            : base(model, manager)
        {

        }

        protected override bool OnMouseClick(InputEventMouse e)
        {
            if (Model.Group != null)
                Model.Group.ButtonClicked(Model);
            return base.OnMouseClick(e);
        }
    }
}
