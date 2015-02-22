using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Input.Windows;

namespace InterXLib.XGUI.Elements.Controllers
{
    class CheckBoxController : AElementController
    {
        new CheckBox Model
        {
            get
            {
                return (CheckBox)base.Model;
            }
        }

        public CheckBoxController(AElement model, GUIManager manager)
            : base(model, manager)
        {

        }

        protected override bool OnMouseClick(InputEventMouse e)
        {
            Model.IsChecked = !Model.IsChecked;
            if (Model.OnClickEvent != null)
                Model.OnClickEvent();
            return true;
        }
    }
}
