/***************************************************************************
 *   UserInterfaceService.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Input.Windows;
#endregion

namespace UltimaXNA.Core.UI
{
    public class UserInterfaceService
    {
        // ======================================================================
        // Private variables
        // ======================================================================
        // All open controls:
        List<AControl> m_Controls = null;

        // List of controls that the Cursor is over, with the control at index 0 being the frontmost control:
        AControl m_MouseOverControl = null;

        // Controls that the Cursor was over when a mouse button was clicked. We allow for five buttons:
        AControl[] m_MouseDownControl = new AControl[5];

        // private services
        private readonly InputManager m_Input;

        // ======================================================================
        // Public properties
        // ======================================================================

        /// <summary>
        /// An array of all open root controls in the user interface.
        /// </summary>
        public AControl[] Controls
        {
            get
            {
                return m_Controls.ToArray();
            }
        }

        // ======================================================================
        // Ctor, Dispose, Update, and Draw
        // ======================================================================

        public UserInterfaceService()
        {
            m_Input = ServiceRegistry.GetService<InputManager>();
            m_SpriteBatch = ServiceRegistry.GetService<SpriteBatchUI>();

            m_Controls = new List<AControl>();
        }

        public void Dispose()
        {
            Reset();
        }

        /// <summary>
        /// Disposes of all controls.
        /// </summary>
        public void Reset()
        {
            foreach (AControl c in m_Controls)
                c.Dispose();
            m_Controls.Clear();
        }

        SpriteBatchUI m_SpriteBatch;
        internal SpriteBatchUI SpriteBatch { get { return m_SpriteBatch; } }

        internal ICursor Cursor
        {
            get;
            set;
        }

        public int Width { get { return m_SpriteBatch.GraphicsDevice.Viewport.Width; } }
        public int Height { get { return m_SpriteBatch.GraphicsDevice.Viewport.Height; } }

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
                    foreach (AControl c in m_Controls)
                    {
                        if (!c.IsDisposed && c.IsVisible && 
                            c.IsEnabled && c.HandlesKeyboardFocus)
                        {
                            m_keyboardFocusControl = c.FindControlThatAcceptsKeyboardFocus();
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
                    if (c.MetaData.IsModal)
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
        /// <returns>If the control was added to the list of active controls, then returns the added control. If the control was not added, returns null.</returns>
        public AControl AddControl(AControl control, int x, int y)
        {
            if (control.IsDisposed)
            {
                return null;
            }

            control.Position = new Point(x, y);
            m_Controls.Insert(0, control);
            return control;
        }

        public void RemoveControl<T>(int? localID = null) where T : AControl
        {
            foreach (AControl c in m_Controls)
            {
                if (c.GetType() == typeof(T))
                {
                    if (!localID.HasValue || (c.GumpLocalID == localID))
                    {
                        if (!c.IsDisposed)
                            c.Dispose();
                    }
                }
            }
        }

        public AControl GetControl(int localID)
        {
            foreach (AControl c in m_Controls)
            {
                if (!c.IsDisposed && c.GumpLocalID == localID)
                    return c;
            }
            return null;
        }

        public T GetControl<T>(int? localID = null) where T : AControl
        {
            foreach (AControl c in m_Controls)
            {
                if (!c.IsDisposed && c.GetType() == typeof(T) && 
                    (!localID.HasValue || c.GumpLocalID == localID))
                    return (T)c;
            }
            return null;
        }

        public void Update(double totalMS, double frameMS)
        {
            OrderControlsBasedOnUILayerMetaData();

            foreach (AControl c in m_Controls)
            {
                if (!c.IsInitialized)
                    c.Initialize();
                c.Update(totalMS, frameMS);
            }

            for (int i = 0; i < m_Controls.Count; i++)
            {
                if (m_Controls[i].IsDisposed)
                {
                    m_Controls.RemoveAt(i);
                    i--;
                }     
            }

            if (Cursor != null)
                Cursor.Update();

            InternalHandleKeyboardInput();
            InternalHandleMouseInput();
        }

        public void Draw(double frameTime)
        {
            OrderControlsBasedOnUILayerMetaData();

            foreach (AControl c in m_Controls.Reverse<AControl>())
            {
                if (c.IsInitialized)
                    c.Draw(m_SpriteBatch, c.Position);
            }

            if (Cursor != null)
                Cursor.Draw(m_SpriteBatch, m_Input.MousePosition);

            m_SpriteBatch.Flush(false);
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

        private void OrderControlsBasedOnUILayerMetaData()
        {
            List<AControl> gumps = new List<AControl>();

            foreach (AControl c in m_Controls)
            {
                if (c.MetaData.Layer != UILayer.Default)
                    gumps.Add(c);
            }

            foreach (AControl c in gumps)
            {
                if (c.MetaData.Layer == UILayer.Under)
                {
                    for (int i = 0; i < m_Controls.Count; i++)
                    {
                        if (m_Controls[i] == c)
                        {
                            m_Controls.RemoveAt(i);
                            m_Controls.Insert(m_Controls.Count, c);
                        }
                    }
                }
                else if (c.MetaData.Layer == UILayer.Over)
                {
                    for (int i = 0; i < m_Controls.Count; i++)
                    {
                        if (m_Controls[i] == c)
                        {
                            m_Controls.RemoveAt(i);
                            m_Controls.Insert(0, c);
                        }
                    }
                }
            }
        }

        private void InternalHandleMouseInput()
        {
            // clip the mouse position
            Point clippedPosition = m_Input.MousePosition;
            ClipMouse(ref clippedPosition);

            // Get the topmost control that is under the mouse and handles mouse input.
            // If this control is different from the previously focused control,
            // send that previous control a MouseOut event.
            AControl focusedControl = InternalGetMouseOverControl(clippedPosition);
            if ((MouseOverControl != null) && (focusedControl != MouseOverControl))
            {
                MouseOverControl.MouseOut(clippedPosition);
                // Also let the parent control know we've been moused out (for gumps).
                if (MouseOverControl.RootParent != null)
                {
                    if (focusedControl == null || MouseOverControl.RootParent != focusedControl.RootParent)
                        MouseOverControl.RootParent.MouseOut(clippedPosition);
                }
            }

            if (focusedControl != null)
            {
                focusedControl.MouseOver(clippedPosition);
                if (m_MouseDownControl[0] == focusedControl)
                    AttemptDragControl(focusedControl, clippedPosition);
                if (IsDraggingControl)
                    DoDragControl(clippedPosition);
            }

            // Set the new MouseOverControl.
            m_MouseOverControl = focusedControl;

            // Send a MouseOver event to any control that was previously the target of a MouseDown event.
            for (int iButton = 0; iButton < 5; iButton++)
            {
                
                if ((m_MouseDownControl[iButton] != null) && (m_MouseDownControl[iButton] != focusedControl))
                {
                    m_MouseDownControl[iButton].MouseOver(clippedPosition);
                }
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
                        MakeTopMostGump(focusedControl);
                        focusedControl.MouseDown(clippedPosition, e.Button);
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

                    EndDragControl(e.Position);

                    if (focusedControl != null)
                    {
                        if (m_MouseDownControl[btn] != null && focusedControl == m_MouseDownControl[btn])
                        {
                            focusedControl.MouseClick(clippedPosition, e.Button);
                        }
                        focusedControl.MouseUp(clippedPosition, e.Button);
                        if (m_MouseDownControl[btn] != null && focusedControl != m_MouseDownControl[btn])
                        {
                            m_MouseDownControl[btn].MouseUp(clippedPosition, e.Button);
                        }
                    }
                    else
                    {
                        if (m_MouseDownControl[btn] != null)
                        {
                            m_MouseDownControl[btn].MouseUp(clippedPosition, e.Button);
                        }
                    }

                    m_MouseDownControl[btn] = null;
                }
            }
        }

        private void MakeTopMostGump(AControl control)
        {
            AControl c = control;
            while (c.Parent != null)
                c = c.Parent;

            for (int i = 0; i < m_Controls.Count; i++)
            {
                if (m_Controls[i] == c)
                {
                    AControl cm = m_Controls[i];
                    m_Controls.RemoveAt(i);
                    m_Controls.Insert(0, cm);
                }
            }
        }

        private AControl InternalGetMouseOverControl(Point atPosition)
        {
            if (IsDraggingControl)
            {
                return m_DraggingControl;
            }

            List<AControl> possibleControls;
            if (IsModalControlOpen)
            {
                possibleControls = new List<AControl>();
                foreach (AControl c in m_Controls)
                    if (c.MetaData.IsModal)
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
                AControl[] controls = c.HitTest(atPosition, false);
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

        // ======================================================================
        // Control dragging
        // ======================================================================

        private AControl m_DraggingControl = null;
        private bool m_IsDragging = false;
        private int m_DragOriginX, m_DragOriginY;

        public bool IsDraggingControl
        {
            get { return m_IsDragging; }
        }

        public void AttemptDragControl(AControl control, Point mousePosition, bool attemptAlwaysSuccessful = false)
        {
            if (IsDraggingControl)
                return;

            AControl dragTarget = control;
            if (!dragTarget.IsMoveable)
                return;

            while (dragTarget.Parent != null)
                dragTarget = dragTarget.Parent;

            if (dragTarget.IsMoveable)
            {
                if (attemptAlwaysSuccessful)
                {
                    m_DraggingControl = dragTarget;
                    m_DragOriginX = mousePosition.X;
                    m_DragOriginY = mousePosition.Y;
                }
                if (m_DraggingControl == dragTarget)
                {
                    int deltaX = mousePosition.X - m_DragOriginX;
                    int deltaY = mousePosition.Y - m_DragOriginY;
                    if (attemptAlwaysSuccessful || Math.Abs(deltaX) + Math.Abs(deltaY) > 4)
                    {
                        m_IsDragging = true;
                    }
                }
                else
                {
                    m_DraggingControl = dragTarget;
                    m_DragOriginX = mousePosition.X;
                    m_DragOriginY = mousePosition.Y;
                }
            }

            if (IsDraggingControl)
            {

                for (int i = 0; i < 5; i++)
                {
                    if (m_MouseDownControl[i] != null && m_MouseDownControl[i] != m_DraggingControl)
                    {
                        m_MouseDownControl[i].MouseUp(mousePosition, (MouseButton)i);
                        m_MouseDownControl[i] = null;
                    }
                }
            }
        }

        private void DoDragControl(Point mousePosition)
        {
            if (m_DraggingControl == null)
                return;

            int deltaX = mousePosition.X - m_DragOriginX;
            int deltaY = mousePosition.Y - m_DragOriginY;
            m_DraggingControl.Position = new Point(m_DraggingControl.X + deltaX, m_DraggingControl.Y + deltaY);
            m_DragOriginX = mousePosition.X;
            m_DragOriginY = mousePosition.Y;
        }

        private void EndDragControl(Point mousePosition)
        {
            if (m_IsDragging)
                DoDragControl(mousePosition);
            m_DraggingControl = null;
            m_IsDragging = false;
        }

        private void ClipMouse(ref Point position)
        {
            if (position.X < -8)
                position.X = -8;
            if (position.Y < -8)
                position.Y = -8;
            if (position.Y >= Width + 8)
                position.X = Width + 8;
            if (position.Y >= Height + 8)
                position.Y = Height + 8;
        }
    }
}
