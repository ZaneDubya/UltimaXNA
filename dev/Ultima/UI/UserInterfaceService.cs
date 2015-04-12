/***************************************************************************
 *   GUIManager.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Network.Client;
#endregion

namespace UltimaXNA.Ultima.UI
{
    public class UserInterfaceService
    {
        private readonly INetworkClient m_Network;
        private readonly InputManager m_Input;

        public UserInterfaceService()
        {
            m_Network = UltimaServices.GetService<INetworkClient>();
            m_Input = UltimaServices.GetService<InputManager>();
            m_SpriteBatch = UltimaServices.GetService<SpriteBatchUI>();

            m_Controls = new List<AControl>();
            m_DisposedControls = new List<AControl>();
        }

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

        /// <summary>
        /// Opens a modal message box with either 'OK' or 'OK and Cancel' buttons.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        /// <returns>The created message box.</returns>
        public MsgBox MsgBox(string msg, MsgBoxTypes type)
        {
            // pop up an error message, modal.
            MsgBox msgbox = new MsgBox(msg, type);
            AddControl(msgbox, 0, 0);
            return msgbox;
        }

        /// <summary>
        /// Informs the server that we have activated a control.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="gumpId"></param>
        /// <param name="buttonId"></param>
        /// <param name="switchIds"></param>
        /// <param name="textEntries"></param>
        public void GumpMenuSelect(int id, int gumpId, int buttonId, int[] switchIds, Tuple<short, string>[] textEntries)
        {
            m_Network.Send(new GumpMenuSelectPacket(id, gumpId, buttonId, switchIds, textEntries));
        }

        // All open controls:
        List<AControl> m_Controls = null;
        List<AControl> m_DisposedControls = null;
        // List of controls that the Cursor is over, with the control at index 0 being the frontmost control:
        AControl m_MouseOverControl = null;
        // Controls that the Cursor was over when a mouse button was clicked. We allow for five buttons:
        AControl[] m_MouseDownControl = new AControl[5];
        /// <summary>
        /// Returns the control directly under the Cursor.
        /// </summary>
        public AControl MouseOverControl
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

        private AControl m_keyboardFocusControl;
        public AControl KeyboardFocusControl
        {
            get
            {
                if (IsModalControlOpen)
                    return null;
                if (m_keyboardFocusControl == null)
                {
                    for (int i = m_Controls.Count - 1; i >= 0; i--)
                    {
                        AControl c = m_Controls[i];
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

        public bool IsModalControlOpen
        {
            get
            {
                foreach (AControl c in m_Controls)
                    if (c.IsModal)
                        return true;
                return false;
            }
        }

        
        /// <summary>
        /// Adds or toggles the passed control to the list of active controls.
        /// </summary>
        /// <param name="control">The control to be opened or toggled.</param>
        /// <param name="x">C coordinate where new control should be placed.</param>
        /// <param name="y">Y coordinate where new control should be placed.</param>
        /// <param name="addType">By default, always adds the control.
        /// If OnlyAllowOne, then any controls of the same type that are active are disposed of, and the passed control is added.
        /// If Toggle, then only adds the control is another control of the same type is not active; else, disposes of all controls of the passed type, including the passed control.</param>
        /// <returns>If the control was added to the list of active controls, then returns the added control. If the control was not added, returns null.</returns>
        public AControl AddControl(AControl control, int x, int y, AddControlType addType = AddControlType.Always)
        {
            bool addControl = false;

            if (addType == AddControlType.Always)
            {
                addControl = true;
            }
            else if (addType == AddControlType.Toggle)
            {
                bool alreadyActive = false;
                foreach (AControl c in m_Controls)
                {
                    if (c.Equals(control) && control.Equals(c))
                    {
                        alreadyActive = true;
                        c.Dispose();
                    }
                }

                addControl = !alreadyActive;
            }
            else if (addType == AddControlType.OnlyAllowOne)
            {
                foreach (AControl c in m_Controls)
                {
                    if (c.Equals(control) && control.Equals(c))
                    {
                        c.Dispose();
                    }
                }

                addControl = true;
            }

            if (addControl)
            {
                control.Position = new Point(x, y);
                m_Controls.Add(control);
                return control;
            }
            else
            {
                control.Dispose();
                return null;
            }
        }

        public enum AddControlType
        {
            Always = 0,
            OnlyAllowOne = 1,
            Toggle = 2
        }

        public AControl GetControl(int serial)
        {
            foreach (AControl c in m_Controls)
            {
                if (c.Serial == serial)
                    return c;
            }
            return null;
        }

        public T GetControl<T>(int serial) where T : AControl
        {
            foreach (AControl c in m_Controls)
            {
                if (c.Serial == serial)
                    if (c.GetType() == typeof(T))
                        return (T)c;
            }
            return null;
        }

        public void Update(double totalMS, double frameMS)
        {
            foreach (AControl c in m_Controls)
            {
                if (!c.IsInitialized)
                    c.ControlInitialize();
                c.Update(totalMS, frameMS);
            }

            foreach (AControl c in m_Controls)
                if (c.IsDisposed)
                    m_DisposedControls.Add(c);

            foreach (AControl c in m_DisposedControls)
                m_Controls.Remove(c);
            m_DisposedControls.Clear();

            if (Cursor != null)
                Cursor.Update();

            InternalHandleKeyboardInput();
            InternalHandleMouseInput();
        }

        public void Draw(double frameTime)
        {
            m_SpriteBatch.Prepare();

            foreach (AControl c in m_Controls)
            {
                if (c.IsInitialized)
                    c.Draw(m_SpriteBatch);
            }

            if (Cursor != null)
                Cursor.Draw(m_SpriteBatch, m_Input.MousePosition);

            m_SpriteBatch.Flush();
        }

        /// <summary>
        /// Disposes of all controls.
        /// </summary>
        public void Reset()
        {
            foreach (AControl c in m_Controls)
                c.Dispose();
        }

        private void InternalHandleKeyboardInput()
        {
            if (KeyboardFocusControl != null)
            {
                if (m_keyboardFocusControl.IsDisposed)
                {
                    m_keyboardFocusControl = null;
                }
                else
                {
                    List<InputEventKeyboard> k_events = m_Input.GetKeyboardEvents();
                    foreach (InputEventKeyboard e in k_events)
                    {
                        if (e.EventType == KeyboardEventType.Press)
                            m_keyboardFocusControl.KeyboardInput(e);
                    }
                }
            }
        }

        private void InternalHandleMouseInput()
        {
            // Get the topmost control that is under the mouse and handles mouse input.
            // If this control is different from the previously focused control,
            // send that previous control a MouseOut event.
            AControl focusedControl = InternalGetMouseOverControl();
            if ((MouseOverControl != null) && (focusedControl != MouseOverControl))
                MouseOverControl.MouseOut(m_Input.MousePosition);
            if (focusedControl != null)
                focusedControl.MouseOver(m_Input.MousePosition);

            // Set the new MouseOverControl.
            m_MouseOverControl = focusedControl;

            // Send a MouseOver event to any control that was previously the target of a MouseDown event.
            for (int iButton = 0; iButton < 5; iButton++)
            {
                if ((m_MouseDownControl[iButton] != null) && (m_MouseDownControl[iButton] != focusedControl))
                    m_MouseDownControl[iButton].MouseOver(m_Input.MousePosition);
            }

            // The cursor and world input objects occasionally must block input events from reaching the UI:
            // e.g. when the cursor is carrying an object.
            if (!IsModalControlOpen && ObjectsBlockingInput)
                return;

            List<InputEventMouse> events = m_Input.GetMouseEvents();
            foreach (InputEventMouse e in events)
            {
                // MouseDown event: the currently focused control gets a MouseDown event, and if
                // it handles Keyboard input, gets Keyboard focus as well.
                if (e.EventType == MouseEvent.Down)
                {
                    if (focusedControl != null)
                    {
                        focusedControl.MouseDown(m_Input.MousePosition, e.Button);
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
                            focusedControl.MouseClick(m_Input.MousePosition, e.Button);
                        }
                        focusedControl.MouseUp(m_Input.MousePosition, e.Button);
                        if (m_MouseDownControl[btn] != null && focusedControl != m_MouseDownControl[btn])
                        {
                            m_MouseDownControl[btn].MouseUp(m_Input.MousePosition, e.Button);
                        }
                    }
                    else
                    {
                        if (m_MouseDownControl[btn] != null)
                        {
                            m_MouseDownControl[btn].MouseUp(m_Input.MousePosition, e.Button);
                        }
                    }

                    m_MouseDownControl[btn] = null;
                }
            }
        }

        private AControl InternalGetMouseOverControl()
        {
            List<AControl> possibleControls;
            if (IsModalControlOpen)
            {
                possibleControls = new List<AControl>();
                foreach (AControl c in m_Controls)
                    if (c.IsModal)
                        possibleControls.Add(c);
            }
            else
            {
                possibleControls = m_Controls;
            }

            AControl[] mouseOverControls = null;
            // Get the list of controls under the mouse cursor
            foreach (AControl c in possibleControls)
            {
                AControl[] controls = c.HitTest(m_Input.MousePosition, false);
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

        // ======================================================================
        // Input blocking objects
        // ======================================================================

        private List<object> m_InputBlockingObjects = new List<object>();

        /// <summary>
        /// Returns true if there are any active objects blocking input.
        /// </summary>
        protected bool ObjectsBlockingInput
        {
            get
            {
                return (m_InputBlockingObjects.Count > 0);
            }
        }

        /// <summary>
        /// Add an input blocking object. Until RemoveInputBlocker is called with this same parameter,
        /// GUIState will not process any MouseDown, MouseUp, or MouseClick events, or any keyboard events.
        /// </summary>
        /// <param name="obj"></param>
        public void AddInputBlocker(object obj)
        {
            if (!m_InputBlockingObjects.Contains(obj))
                m_InputBlockingObjects.Add(obj);
        }

        /// <summary>
        /// Removes an input blocking object. Only when there are no input blocking objects will GUIState
        /// process MouseDown, MouseUp, MouseClick, and all keyboard events.
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveInputBlocker(object obj)
        {
            if (m_InputBlockingObjects.Contains(obj))
                m_InputBlockingObjects.Remove(obj);
        }
    }
}
