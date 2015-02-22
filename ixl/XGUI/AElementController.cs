using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using InterXLib.Input.Windows;

namespace InterXLib.XGUI
{
    public class AElementController : Patterns.MVC.AController
    {
        // ### Overrideable Methods allowing for Input ###

        protected virtual bool OnKeyboardKeyDown(InputEventKeyboard e) { return false; }
        protected virtual bool OnKeyboardKeyUp(InputEventKeyboard e) { return false; }
        protected virtual bool OnKeyboardKeyPress(InputEventKeyboard e) { return false; }

        protected virtual bool OnMouseMove(InputEventMouse e) { return false; }
        protected virtual bool OnMouseDown(InputEventMouse e) { return false; }
        protected virtual bool OnMouseUp(InputEventMouse e) { return false; }
        protected virtual bool OnWheelScroll(InputEventMouse e) { return false; }
        protected virtual bool OnMouseDragBegin(InputEventMouse e) { return false; }
        protected virtual bool OnMouseDragEnd(InputEventMouse e) { return false; }
        protected virtual bool OnMouseClick(InputEventMouse e) { return false; }
        protected virtual bool OnMouseDoubleClick(InputEventMouse e) { return false; }

        public virtual void OnMouseEnter() { }
        public virtual void OnMouseExit() { }

        // ### End Overrideable Methods ###

        new AElement Model
        {
            get
            {
                return (AElement)base.Model;
            }
        }

        protected GUIManager Manager;

        public AElementController(AElement model, GUIManager manager)
            : base(model)
        {
            Manager = manager;
        }

        public override void ReceiveKeyboardInput(List<InputEventKeyboard> events)
        {
            foreach (InputEventKeyboard e in events)
            {
                if (e.Handled)
                    continue;

                switch (e.EventType)
                {
                    case KeyboardEventType.Down:
                        if (OnKeyboardKeyDown(e))
                            e.Handled = true;
                        break;
                    case KeyboardEventType.Press:
                        if (OnKeyboardKeyPress(e))
                            e.Handled = true;
                        break;
                    case KeyboardEventType.Up:
                        if (OnKeyboardKeyUp(e))
                            e.Handled = true;
                        break;
                }
            }
        }

        public override void ReceiveMouseInput(Point MousePosition, List<InputEventMouse> events)
        {
            Model.MousePosition = MousePosition;
            if (events == null)
                return;
            foreach (InputEventMouse e in events)
            {
                if (e.Handled)
                    continue;

                switch (e.EventType)
                {
                    case MouseEvent.Move:
                        if (OnMouseMove(e))
                            e.Handled = true;
                        break;
                    case MouseEvent.Down:
                        Model.IsMouseDown = true;
                        Manager.AnnounceElementOnMouseDown(Model, e.Button);
                        if (OnMouseDown(e))
                            e.Handled = true;
                        break;
                    case MouseEvent.Up:
                        if (OnMouseUp(e))
                            e.Handled = true;
                        if (Manager.AnnounceElementOnMouseUp(Model, e.Button))
                            Model.IsMouseDown = false;
                        break;
                    case MouseEvent.WheelScroll:
                        if (OnWheelScroll(e))
                            e.Handled = true;
                        break;
                    case MouseEvent.DragBegin:
                        if (OnMouseDragBegin(e))
                            e.Handled = true;
                        break;
                    case MouseEvent.DragEnd:
                        if (OnMouseDragEnd(e))
                            e.Handled = true;
                        break;
                    case MouseEvent.Click:
                        if (OnMouseClick(e))
                            e.Handled = true;
                        break;
                    case MouseEvent.DoubleClick:
                        if (OnMouseDoubleClick(e))
                            e.Handled = true;
                        break;
                }
            }
        }
    }
}
