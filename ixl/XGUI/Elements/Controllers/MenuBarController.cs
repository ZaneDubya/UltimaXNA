using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements.Controllers
{
    class MenuBarController : AElementController
    {
        new MenuBar Model
        {
            get
            {
                return (MenuBar)base.Model;
            }
        }

        public MenuBarController(AElement model, GUIManager manager)
            : base(model, manager)
        {

        }

        private bool m_MouseHover = false, m_MouseDown = false;
        private Point m_MouseLocation;

        public bool IsMouseOver
        {
            get { return m_MouseHover; }
        }
        public bool IsMouseDown
        {
            get { return m_MouseDown; }
        }
        public Point MouseLocation
        {
            get { return m_MouseLocation; }
        }

        public override void OnMouseEnter()
        {
            m_MouseHover = true;
        }

        public override void OnMouseExit()
        {
            m_MouseHover = false;
            m_MouseDown = false;
        }

        protected override bool OnMouseDown(InputEventMouse e)
        {
            m_MouseDown = true;
            return true;
        }

        protected override bool OnMouseUp(InputEventMouse e)
        {
            m_MouseDown = false;
            return true;
        }

        protected override bool OnMouseClick(InputEventMouse e)
        {
            MenuElement element = ((Views.MenuBarView)Model.GetView()).GetMenuElementUnderMouse();
            if (element != null && element.Action != null)
                element.Action();
            return true;
        }

        protected override bool OnMouseMove(InputEventMouse e)
        {
            m_MouseLocation = e.Position;
            m_MouseLocation.X -= Model.ScreenArea.X;
            m_MouseLocation.Y -= Model.ScreenArea.Y;
            return true;
        }
    }
}
