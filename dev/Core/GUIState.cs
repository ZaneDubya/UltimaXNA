/***************************************************************************
 *   UIManager.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaGUI;
#endregion

namespace UltimaXNA
{
    public class GUIState
    {
        SpriteBatchUI m_SpriteBatch;
        internal SpriteBatchUI SpriteBatch { get { return m_SpriteBatch; } }

        private UltimaCursor m_Cursor;
        internal UltimaCursor Cursor
        {
            get { return m_Cursor; }
            set { m_Cursor = value; }
        }

        public int Width { get { return m_SpriteBatch.GraphicsDevice.Viewport.Width; } }
        public int Height { get { return m_SpriteBatch.GraphicsDevice.Viewport.Height; } }

        // All open controls:
        List<Control> m_Controls = null;
        List<Control> m_DisposedControls = null;
        // List of controls that the Cursor is over, with the control at index 0 being the frontmost control:
        Control m_MouseOverControl = null;
        // Controls that the Cursor was over when a mouse button was clicked. We allow for five buttons:
        Control[] m_MouseDownControl = new Control[5];
        /// <summary>
        /// Returns the control directly under the Cursor.
        /// </summary>
        public Control MouseOverControl
        {
            get
            {
                if (m_MouseOverControl == null)
                    return null;
                else
                    return m_MouseOverControl;
            }
        }
        /// <summary>
        /// Returns True if the Cursor is over the UserInterface.
        /// </summary>
        public bool IsMouseOverUI
        {
            get
            {
                if (MouseOverControl == null)
                    return false;
                else
                    return true;
            }
        }

        Control m_keyboardFocusControl;
        public Control KeyboardFocusControl
        {
            get
            {
                if (IsModalControlOpen)
                    return null;
                if (m_keyboardFocusControl == null)
                {
                    for (int i = m_Controls.Count - 1; i >= 0; i--)
                    {
                        Control c = m_Controls[i];
                        if (!c.IsDisposed && c.Visible && c.Enabled && c.HandlesKeyboardFocus)
                        {
                            m_keyboardFocusControl = c.KeyboardFocusControl;
                            if (m_keyboardFocusControl != null)
                                break;
                        }
                    }
                }
                return m_keyboardFocusControl;
            }
            set
            {
                m_keyboardFocusControl = value;
            }
        }

        

        public void Initialize(Game game)
        {
            Control.UserInterface = this;
            m_SpriteBatch = new SpriteBatchUI(game);
            m_Controls = new List<Control>();
            m_DisposedControls = new List<Control>();
            
        }

        public bool IsModalControlOpen
        {
            get
            {
                foreach (Control c in m_Controls)
                    if (c.IsModal)
                        return true;
                return false;
            }
        }

        
        /// <summary>
        /// Adds or toggles the passed gump to the list of active gumps.
        /// </summary>
        /// <param name="gump">The gump to be opened or toggled.</param>
        /// <param name="x">C coordinate where new gump should be placed.</param>
        /// <param name="y">Y coordinate where new gump should be placed.</param>
        /// <param name="addType">By default, always adds the gump.
        /// If OnlyAllowOne, then any gumps of the same type that are active are disposed of, and the passed gump is added.
        /// If Toggle, then only adds the gump is another gump of the same type is not active; else, disposes of all gumps of the passed type, including the passed gump.</param>
        /// <returns>If the gump was added to the list of active gumps, then returns the added gump. If the gump was not added, returns null.</returns>
        public Control AddControl(Control gump, int x, int y, AddGumpType addType = AddGumpType.Always)
        {
            bool addGump = false;

            if (addType == AddGumpType.Always)
            {
                addGump = true;
            }
            else if (addType == AddGumpType.Toggle)
            {
                bool alreadyActive = false;
                foreach (Control c in m_Controls)
                {
                    if (c.Equals(gump) && gump.Equals(c))
                    {
                        alreadyActive = true;
                        c.Dispose();
                    }
                }

                addGump = !alreadyActive;
            }
            else if (addType == AddGumpType.OnlyAllowOne)
            {
                foreach (Control c in m_Controls)
                {
                    if (c.Equals(gump) && gump.Equals(c))
                    {
                        c.Dispose();
                    }
                }

                addGump = true;
            }

            if (addGump)
            {
                gump.Position = new Point(x, y);
                m_Controls.Add(gump);
                return gump;
            }
            else
            {
                gump.Dispose();
                return null;
            }
        }

        public enum AddGumpType
        {
            Always = 0,
            OnlyAllowOne = 1,
            Toggle = 2
        }

        public Control GetControl(int serial)
        {
            foreach (Control c in m_Controls)
            {
                if (c.Serial == serial)
                    return c;
            }
            return null;
        }

        public T GetControl<T>(int serial) where T : Control
        {
            foreach (Control c in m_Controls)
            {
                if (c.Serial == serial)
                    if (c.GetType() == typeof(T))
                        return (T)c;
            }
            return null;
        }

        public void Update(GameTime gameTime)
        {
            foreach (Control c in m_Controls)
            {
                if (!c.IsInitialized)
                    c.ControlInitialize();
                c.Update(gameTime);
            }

            foreach (Control c in m_Controls)
                if (c.IsDisposed)
                    m_DisposedControls.Add(c);

            foreach (Control c in m_DisposedControls)
                m_Controls.Remove(c);
            m_DisposedControls.Clear();

            if (Cursor != null)
                Cursor.Update();

            InternalHandleInput();
        }

        public void Draw(double frameTime)
        {
            m_SpriteBatch.Prepare();

            foreach (Control c in m_Controls)
            {
                if (c.IsInitialized)
                    c.Draw(m_SpriteBatch);
            }

            if (Cursor != null)
                Cursor.Draw(m_SpriteBatch, UltimaEngine.Input.MousePosition);

            m_SpriteBatch.Flush();
        }

        public void Reset()
        {
            foreach (Control c in m_Controls)
                c.Dispose();
        }

        private void InternalHandleInput()
        {
            // Get the topmost control that is under the mouse and handles mouse input.
            // If this control is different from the previously focused control,
            // send that previous control a MouseOut event.
            Control focusedControl = InternalHandleInput_GetMouseOverControl();
            if (focusedControl != null)
                focusedControl.MouseOver(UltimaEngine.Input.MousePosition);
            if ((MouseOverControl != null) && (focusedControl != MouseOverControl))
                MouseOverControl.MouseOut(UltimaEngine.Input.MousePosition);

            // Set the new MouseOverControl.
            m_MouseOverControl = focusedControl;

            // Send a MouseOver event to any control that was previously the target of a MouseDown event.
            for (int iButton = 0; iButton < 5; iButton++)
            {
                if ((m_MouseDownControl[iButton] != null) && (m_MouseDownControl[iButton] != focusedControl))
                    m_MouseDownControl[iButton].MouseOver(UltimaEngine.Input.MousePosition);
            }

            // The cursor occasionally must block input events from reaching the UI - for example, when it is carrying an object.
            if (Cursor.BlockingUIMouseEvents)
                return;

            List<InputEventMouse> events = UltimaEngine.Input.GetMouseEvents();
            foreach (InputEventMouse e in events)
            {
                // MouseDown event: the currently focused control gets a MouseDown event, and if
                // it handles Keyboard input, gets Keyboard focus as well.
                if (e.EventType == MouseEvent.Down)
                {
                    if (focusedControl != null)
                    {
                        focusedControl.MouseDown(UltimaEngine.Input.MousePosition, e.Button);
                        if (focusedControl.HandlesKeyboardFocus)
                            m_keyboardFocusControl = focusedControl;
                        m_MouseDownControl[(int)e.Button] = focusedControl;
                    }
                }

                // MouseUp and MouseClick events
                if (e.EventType == MouseEvent.Up)
                {
                    int btn = (int)e.Button;

                    // If there is a currently focused control:
                    // 1.   If the currently focused control is the same control that was MouseDowned on with this button,
                    //      then send that control a MouseClick event.
                    // 2.   Send the currently focused control a MouseUp event.
                    // 3.   If the currently focused control is NOT the same control that was MouseDowned on with this button,
                    //      send that MouseDowned control a MouseUp event (but it does not receive MouseClick).
                    // If there is NOT a currently focused control, then simply inform the control that was MouseDowned on
                    // with this button that the button has been released, by sending it a MouseUp event.

                    if (focusedControl != null)
                    {
                        if (m_MouseDownControl[btn] != null && focusedControl == m_MouseDownControl[btn])
                        {
                            focusedControl.MouseClick(UltimaEngine.Input.MousePosition, e.Button);
                        }
                        focusedControl.MouseUp(UltimaEngine.Input.MousePosition, e.Button);
                        if (m_MouseDownControl[btn] != null && focusedControl != m_MouseDownControl[btn])
                        {
                            m_MouseDownControl[btn].MouseUp(UltimaEngine.Input.MousePosition, e.Button);
                        }
                    }
                    else
                    {
                        if (m_MouseDownControl[btn] != null)
                        {
                            m_MouseDownControl[btn].MouseUp(UltimaEngine.Input.MousePosition, e.Button);
                        }
                    }

                    m_MouseDownControl[btn] = null;
                }
            }

            if (KeyboardFocusControl != null)
            {
                if (m_keyboardFocusControl.IsDisposed)
                {
                    m_keyboardFocusControl = null;
                }
                else
                {
                    List<InputEventKeyboard> k_events = UltimaEngine.Input.GetKeyboardEvents();
                    foreach (InputEventKeyboard e in k_events)
                    {
                        if (e.EventType == KeyboardEventType.Press)
                            m_keyboardFocusControl.KeyboardInput(e);
                    }
                }
            }
        }

        private Control InternalHandleInput_GetMouseOverControl()
        {
            List<Control> possibleControls;
            if (IsModalControlOpen)
            {
                possibleControls = new List<Control>();
                foreach (Control c in m_Controls)
                    if (c.IsModal)
                        possibleControls.Add(c);
            }
            else
            {
                possibleControls = m_Controls;
            }

            Control[] mouseOverControls = null;
            // Get the list of controls under the mouse cursor
            foreach (Control c in possibleControls)
            {
                Control[] controls = c.HitTest(UltimaEngine.Input.MousePosition, false);
                if (controls != null)
                {
                    mouseOverControls = controls;
                    break;
                }
            }

            if (mouseOverControls == null)
                return null;

            // Get the topmost control that is under the mouse and handles mouse input.
            // If this control is different from the previously focused control,
            // send that previous control a MouseOut event.
            if (mouseOverControls != null)
            {
                for (int i = 0; i < mouseOverControls.Length; i++)
                {
                    if (mouseOverControls[i].HandlesMouseInput)
                    {
                        return mouseOverControls[i];
                    }
                }
            }

            return null;
        }
    }
}
