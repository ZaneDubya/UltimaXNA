using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements.Controllers
{
    class ListBoxController<T> : AElementController where T : class
    {
        new ListBox<T> Model
        {
            get
            {
                return (ListBox<T>)base.Model;
            }
        }

        public ListBoxController(AElement model, GUIManager manager)
            : base(model, manager)
        {

        }

        protected override bool OnMouseClick(InputEventMouse e)
        {
            if (e.Button == MouseButton.Left)
            {
                int index = Model.ItemAtPosition(new Point(e.Position.X, e.Position.Y));
                if (index >= 0 && index < Model.Items.Count)
                    Model.SelectedIndex = index;
            }
            return true;
        }
    }
}
