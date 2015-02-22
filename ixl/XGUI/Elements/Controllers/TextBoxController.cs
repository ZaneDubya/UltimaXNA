using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Input.Windows;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements.Controllers
{
    class TextBoxController : AElementController
    {
        new TextBox Model
        {
            get
            {
                return (TextBox)base.Model;
            }
        }

        public TextBoxController(AElement model, GUIManager manager)
            : base(model, manager)
        {

        }

        protected override bool OnMouseClick(InputEventMouse e)
        {
            // if (Model.OnClickEvent != null)
            //     Model.OnClickEvent();
            return true;
        }

        /*####################################################################*/
        /*                                Events                              */
        /*####################################################################*/

        protected override bool OnMouseDown(InputEventMouse e)
        {
            Model.SetSelection(new Point(
                e.X - Model.ScreenArea.X,
                e.Y - Model.ScreenArea.Y));
            Model.SetTextCursor(new Point(
                e.X - Model.ScreenArea.X,
                e.Y - Model.ScreenArea.Y));
            return true;
        }

        protected override bool OnMouseMove(InputEventMouse e)
        {
            if (Model.IsMouseDown)
            {
                Model.SetTextCursor(new Point(
                    e.X - Model.ScreenArea.X,
                    e.Y - Model.ScreenArea.Y));
            }
            return true;
        }

        protected override bool OnMouseUp(InputEventMouse e)
        {
            Model.SetTextCursor(new Point(
                e.X - Model.ScreenArea.X,
                e.Y - Model.ScreenArea.Y));
            return true;
        }

        protected override bool OnKeyboardKeyPress(InputEventKeyboard e)
        {
            switch (e.KeyCode)
            {
                case WinKeys.Back:
                    //Backspace => remove selected or remove before cursor
                    if (Model.HasSelected)
                    {
                        Model.RemoveSelected();
                    }
                    else
                    {
                        Model.BackSpace();
                    }
                    return true;
                case WinKeys.Enter:
                    // Enter / Return / Newlines currently not supported.
                    return true;
                case WinKeys.C:
                    if (e.Control)
                    {
                        // Ctrl + C => Copy                        
                        Clipboard.SetDataObject(Model.GetSelected(), true);
                        return true;
                    }
                    break;
                case WinKeys.V:
                    if (e.Control)
                    {
                        // Ctrl + P => Paste
                        if (Model.HasSelected)
                            Model.RemoveSelected();
                        var dataObject = Clipboard.GetDataObject();
                        if (dataObject != null)
                        {
                            string text = dataObject.GetData(DataFormats.Text).ToString();
                            Model.Insert(text);
                        }
                        return true;
                    }
                    break;
                case WinKeys.X:
                    if (e.Control)
                    {
                        // Ctrl + X => Cut
                        if (Model.HasSelected)
                        {
                            Clipboard.SetDataObject(Model.GetSelected(), true);
                            Model.RemoveSelected();
                        }
                        return true;
                    }
                    break;
            }

            if (Model.Length != 0)
            {
                switch (e.KeyCode)
                {
                    case WinKeys.Left:
                        Model.CursorPosition--;
                        Model.SelectCursorPosition = null;
                        return true;
                    case WinKeys.Right:
                        Model.CursorPosition++;
                        Model.SelectCursorPosition = null;
                        return true;
                    case WinKeys.Up:
                        Model.CursorPosition = 0;
                        Model.SelectCursorPosition = null;
                        return true;
                    case WinKeys.Down:
                        Model.CursorPosition = Model.Length;
                        Model.SelectCursorPosition = null;
                        return true;
                    case WinKeys.Delete:
                        Model.Delete();
                        Model.SelectCursorPosition = null;
                        return true;
                }
            }

            if (e.IsChar)
            {
                //Add the character
                if (Model.HasSelected)
                {
                    Model.RemoveSelected();
                    Model.SelectCursorPosition = null;
                }
                Model.Insert(e.KeyChar);
            }

            return false;
        }
    }
}
