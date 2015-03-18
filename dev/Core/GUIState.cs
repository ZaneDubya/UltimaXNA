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
        Control[] m_MouseOverControl = null;
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
                    return m_MouseOverControl[0];
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
                    if (c.GetType() == gump.GetType() && c.Equals(gump))
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
                    if (c.GetType() == gump.GetType() && c.Equals(gump))
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

            updateInput();
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

        void updateInput()
        {
            Control[] focusedControls = null;
            List<Control> workingControls;
            if (IsModalControlOpen)
            {
                workingControls = new List<Control>();
                foreach (Control c in m_Controls)
                    if (c.IsModal)
                        workingControls.Add(c);
            }
            else
            {
                workingControls = m_Controls;
            }

            // Get the list of controls under the mouse cursor
            foreach (Control c in workingControls)
            {
                Control[] mouseOverControls = c.HitTest(UltimaEngine.Input.MousePosition, false);
                if (mouseOverControls != null)
                {
                    focusedControls = mouseOverControls;
                }
            }

            // MouseOver event.
            Control controlGivenMouseOver = null;
            if (focusedControls != null)
            {
                for (int iControl = 0; iControl < focusedControls.Length; iControl++)
                {
                    if (focusedControls[iControl].HandlesMouseInput)
                    {
                        // mouse over for the moused over control
                        focusedControls[iControl].MouseOver(UltimaEngine.Input.MousePosition);
                        controlGivenMouseOver = focusedControls[iControl];
                        if (MouseOverControl != null && controlGivenMouseOver != MouseOverControl)
                            MouseOverControl.MouseOut(UltimaEngine.Input.MousePosition);
                        break;
                    }
                }
            }

            // mouse over for any controls with mouse focus
            for (int iButton = 0; iButton < 5; iButton++)
            {
                if (m_MouseDownControl[iButton] != null && m_MouseDownControl[iButton] != controlGivenMouseOver)
                    m_MouseDownControl[iButton].MouseOver(UltimaEngine.Input.MousePosition);
            }

            if (!Cursor.BlockingUIMouseEvents)
            {
                List<InputEventMouse> events = UltimaEngine.Input.GetMouseEvents();
                foreach (InputEventMouse e in events)
                {
                    if (e.Handled)
                        continue;

                    if (focusedControls != null)
                        e.Handled = true;

                    // MouseDown event.
                    if (e.EventType == MouseEvent.Down)
                    {
                        if (focusedControls != null)
                        {
                            for (int iControl = 0; iControl < focusedControls.Length; iControl++)
                            {
                                if (focusedControls[iControl].HandlesMouseInput)
                                {
                                    focusedControls[iControl].MouseDown(UltimaEngine.Input.MousePosition, e.Button);
                                    // if we're over a keyboard-handling control and press lmb, then give focus to the control.
                                    if (focusedControls[iControl].HandlesKeyboardFocus)
                                        m_keyboardFocusControl = focusedControls[iControl];
                                    m_MouseDownControl[(int)e.Button] = focusedControls[iControl];
                                    break;
                                }
                            }
                        }
                    }

                    // MouseUp and MouseClick events
                    if (e.EventType == MouseEvent.Up)
                    {
                        if (focusedControls != null)
                        {
                            if (m_MouseDownControl[(int)e.Button] != null && focusedControls[0] == m_MouseDownControl[(int)e.Button])
                            {
                                focusedControls[0].MouseClick(UltimaEngine.Input.MousePosition, e.Button);
                            }
                            focusedControls[0].MouseUp(UltimaEngine.Input.MousePosition, e.Button);
                            if (m_MouseDownControl[(int)e.Button] != null && focusedControls[0] != m_MouseDownControl[(int)e.Button])
                            {
                                m_MouseDownControl[(int)e.Button].MouseUp(UltimaEngine.Input.MousePosition, e.Button);
                            }
                        }
                        else
                        {
                            if (m_MouseDownControl[(int)e.Button] != null)
                            {
                                m_MouseDownControl[(int)e.Button].MouseUp(UltimaEngine.Input.MousePosition, e.Button);
                            }
                        }

                        m_MouseDownControl[(int)e.Button] = null;
                    }
                }
            }

            m_MouseOverControl = focusedControls;

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
    }
}
