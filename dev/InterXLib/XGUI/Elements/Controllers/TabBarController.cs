using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using InterXLib.Input.Windows;

namespace InterXLib.XGUI.Elements.Controllers
{
    public class TabBarController : AElementController
    {
        new TabBar Model
        {
            get
            {
                return (TabBar)base.Model;
            }
        }

        Views.TabBarView View
        {
            get
            {
                return (Views.TabBarView)Model.GetView();
            }
        }

        public TabBarController(AElement model, GUIManager manager)
            : base(model, manager)
        {

        }

        protected override bool OnMouseMove(InputEventMouse e)
        {
            int index = 0;

            if (View.TabAreas == null)
                return false; // must initialize tabs before we can receive mouse input.

            foreach (Rectangle area in View.TabAreas)
            {
                if (area.Contains(e.Position))
                {
                    Model.HoverTab = index;
                    return true;
                }
                index++;
            }

            Model.HoverTab = -1;
            return false;
        }

        protected override bool OnMouseDown(InputEventMouse e)
        {
            if (Model.HoverTab != -1)
            {
                Model.SelectedTab = Model.HoverTab;
                return true;
            }

            return false;
        }

        public override void OnMouseExit()
        {
            Model.HoverTab = -1;
        }
    }
}
