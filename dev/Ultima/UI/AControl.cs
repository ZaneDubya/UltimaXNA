/***************************************************************************
 *   Control.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
#endregion

namespace UltimaXNA.Ultima.UI
{
    /// <summary>
    /// The base class used by all GUI objects.
    /// NOTE: Gumps MUST NOT inherit from Control. They must inherit from Gump instead.
    /// </summary>
    public abstract class AControl
    {
        internal Action<int, int, MouseButton> MouseClickEvent;
        internal Action<int, int, MouseButton> MouseDoubleClickEvent;
        internal Action<int, int, MouseButton> MouseDownEvent;
        internal Action<int, int, MouseButton> MouseUpEvent;
        internal Action<int, int> MouseOverEvent;
        internal Action<int, int> MouseOutEvent;

        /// <summary>
        /// An identifier for this control.
        /// </summary>
        public int Serial
        {
            get;
            protected set;
        }

        /// <summary>
        /// Indicates that the control has been disposed, and will be removed on the next Update() of the UserInterface object.
        /// </summary>
        public bool IsDisposed
        {
            get;
            protected set;
        }

        /// <summary>
        /// Controls that are not enabled cannot receive keyboard and mouse input, but still Draw.
        /// </summary>
        public bool IsEnabled
        {
            get;
            protected set;
        }

        /// <summary>
        /// Indicates whether the control has been Initialized by the UserInterface object, which happens every time the UserInterface updates.
        /// Controls that are not initialized do not update and do not draw.
        /// </summary>
        public bool IsInitialized
        {
            get;
            protected set;
        }

        /// <summary>
        /// If controls with IsModal are active, they appear on top of all other controls and block input to all other controls and the world.
        /// </summary>
        public bool IsModal
        {
            get;
            protected set;
        }

        /// <summary>
        /// If true, control can be moved by click-dragging with left mouse button.
        /// A child control can be made a dragger for a parent control with MakeDragger().
        /// </summary>
        public virtual bool IsMovable
        {
            get;
            set;
        }

        /// <summary>
        /// If true, gump cannot be closed with right-click.
        /// </summary>
        public bool IsUncloseableWithRMB
        {
            get;
            protected set;
        }

        /// <summary>
        /// If true, gump does not close when the player hits the Escape key. This behavior is currently unimplemented.
        /// </summary>
        public bool IsUncloseableWithEsc
        {
            get;
            protected set;
        }

        /// <summary>
        /// If true, the gump will draw. Not visible gumps still update and receive mouse input (but not keyboard input).
        /// </summary>
        public bool IsVisible
        {
            get;
            protected set;
        }

        int m_Page = 0;
        public int Page
        {
            get
            {
                return m_Page;
            }
        }

        int m_ActivePage = 0; // we always draw m_activePage and Page 0.
        public int ActivePage
        {
            get { return m_ActivePage; }
            set
            {
                m_ActivePage = value;
                // If we own the current KeyboardFocusControl, then we should clear it.
                // UNLESS page = 0; in which case it still exists and should maintain focus.
                // Clear the current keyboardfocus if we own it and it's page != 0
                // If the page = 0, then it will still exist so it should maintain focus.
                if (m_UserInterface.KeyboardFocusControl != null)
                {
                    if (Children.Contains(m_UserInterface.KeyboardFocusControl))
                    {
                        if (m_UserInterface.KeyboardFocusControl.Page != 0)
                            m_UserInterface.KeyboardFocusControl = null;
                    }
                }
                // When ActivePage changes, check to see if there are new text input boxes
                // that we should redirect text input to.
                foreach (AControl c in Children)
                {
                    if (c.HandlesKeyboardFocus && (c.Page == m_ActivePage))
                    {
                        m_UserInterface.KeyboardFocusControl = c;
                        break;
                    }
                }
            }
        }

        Rectangle m_Area = Rectangle.Empty;
        Point m_Position;
        protected int OwnerX
        {
            get
            {
                if (m_Owner != null)
                    return m_Owner.X + m_Owner.OwnerX;
                else
                    return 0;
            }
        }
        protected int OwnerY
        {
            get
            {
                if (m_Owner != null)
                    return m_Owner.Y + m_Owner.OwnerY;
                else
                    return 0;
            }
        }
        public int X { get { return m_Position.X; } set { m_Position.X = value; } }
        public int Y { get { return m_Position.Y; } set { m_Position.Y = value; } }

        public virtual int Width
        {
            get { return m_Area.Width; }
            set
            {
                m_Area.Width = value;
            }
        }

        public virtual int Height
        {
            get { return m_Area.Height; }
            set
            {
                m_Area.Height = value;
            }
        }

        public Point Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }
        public Point Size
        {
            get { return new Point(m_Area.Width, m_Area.Height); }
            set
            {
                m_Area.Width = value.X;
                m_Area.Height = value.Y;
            }
        }
        public Rectangle Area
        {
            get { return m_Area; }
        }

        protected AControl m_Owner = null;
        public AControl Owner { get { return m_Owner; } }

        private List<AControl> m_Children = null;
        public List<AControl> Children
        {
            get
            {
                if (m_Children == null)
                    m_Children = new List<AControl>();
                return m_Children;
            }
        }

        public void Center()
        {
            Position = new Point(
                (m_UserInterface.Width - Width) / 2,
                (m_UserInterface.Height - Height) / 2);
        }

        GUIManager m_UserInterface;

        public AControl(AControl owner, int page)
        {
            m_Owner = owner;
            m_Page = page;

            m_UserInterface = UltimaServices.GetService<GUIManager>();
        }

        public void Initialize()
        {
            IsDisposed = false;
            IsEnabled = true;
            IsInitialized = true;
            IsVisible = true;
            OnInitialize();
        }

        public virtual void Dispose()
        {
            ClearControls();
            IsDisposed = true;
        }

        virtual public void Update(double totalMS, double frameMS)
        {
            if (!IsInitialized || IsDisposed)
                return;

            // update our area X and Y to reflect any movement.
            m_Area.X = X;
            m_Area.Y = Y;

            foreach (AControl c in Children)
            {
                if (!c.IsInitialized)
                    c.Initialize();
                c.Update(totalMS, frameMS);
            }

            List<AControl> disposedControls = new List<AControl>();
            foreach (AControl c in Children)
            {
                if (c.IsDisposed)
                    disposedControls.Add(c);
            }
            foreach (AControl c in disposedControls)
            {
                Children.Remove(c);
            }
        }

        virtual public void Draw(SpriteBatchUI spriteBatch)
        {
            if (!IsInitialized || !IsVisible)
                return;

            if (Settings.Debug.ShowUIOutlines)
                DebugDrawBounds(spriteBatch, Color.White);

            foreach (AControl c in Children)
            {
                if ((c.Page == 0) || (c.Page == ActivePage))
                {
                    if (c.IsInitialized)
                    {
                        Point saved = c.Position;
                        c.Position = new Point(c.Position.X + Position.X, c.Position.Y + Position.Y);
                        c.Draw(spriteBatch);
                        c.Position = saved;
                    }
                }
            }
        }



        public AControl AddControl(AControl c)
        {
            Children.Add(c);
            return LastControl;
        }

        public AControl LastControl
        {
            get { return Children[Children.Count - 1]; }
        }

        public void ClearControls()
        {
            if (Children != null)
                foreach (AControl c in Children)
                    c.Dispose();
        }



        DragWidget m_Dragger;
        public void MakeThisADragger()
        {
            HandlesMouseInput = true;
            AControl dragTarget = this;
            while (dragTarget.m_Owner != null)
            {
                dragTarget = dragTarget.m_Owner;
            }

            m_Dragger = new DragWidget(this, dragTarget);
        }



        public virtual void ActivateByButton(int buttonID)
        {
            if (m_Owner != null)
                m_Owner.ActivateByButton(buttonID);
        }

        public virtual void ActivateByHREF(string href)
        {
            if (m_Owner != null)
                m_Owner.ActivateByHREF(href);
        }

        public virtual void ActivateByKeyboardReturn(int textID, string text)
        {
            if (m_Owner != null)
                m_Owner.ActivateByKeyboardReturn(textID, text);
        }

        public virtual void ChangePage(int pageIndex)
        {
            if (m_Owner != null)
                m_Owner.ChangePage(pageIndex);
        }

        // ================================================================================
        // Overrideable methods
        // ================================================================================
        #region OverrideableMethods
        protected virtual void OnMouseDown(int x, int y, MouseButton button)
        {

        }

        protected virtual void OnMouseUp(int x, int y, MouseButton button)
        {

        }

        protected virtual void OnMouseOver(int x, int y)
        {

        }

        protected virtual void OnMouseOut(int x, int y)
        {

        }

        protected virtual void OnMouseClick(int x, int y, MouseButton button)
        {

        }

        protected virtual void OnMouseDoubleClick(int x, int y, MouseButton button)
        {

        }

        protected virtual void OnKeyboardInput(InputEventKeyboard e)
        {

        }

        protected virtual void OnInitialize()
        {

        }

        protected virtual bool InternalHitTest(int x, int y)
        {
            return true;
        }
        #endregion

        // ================================================================================
        // Tooltip handling code - shows text when the player mouses over this control.
        // ================================================================================
        #region Tooltip

        private string m_Tooltip = null;

        public string Tooltip
        {
            get { return m_Tooltip; }
        }

        public bool HasTooltip
        {
            get
            {
                return (m_Tooltip != null);
            }
        }

        public void SetTooltip(string caption)
        {
            if (caption == null)
                ClearTooltip();
            else
            {
                m_Tooltip = caption;
            }
        }

        public void ClearTooltip()
        {
            m_Tooltip = null;
        }

        #endregion

        // ================================================================================
        // Mouse handling code
        // ================================================================================
        #region MouseInput

        // private variables

        private bool m_HandlesMouseInput = false;
        private float m_MaxTimeForDoubleClick = 0f;
        private Point m_LastClickPosition;

        // public methods

        public bool IsMouseOver
        {
            get
            {
                if (m_UserInterface.MouseOverControl == this)
                    return true;
                return false;
            }
        }

        public bool HandlesMouseInput
        {
            get
            {
                return (IsEnabled && IsInitialized && !IsDisposed && m_HandlesMouseInput);
            }
            set
            {
                m_HandlesMouseInput = value;
            }
        }

        public void MouseDown(Point position, MouseButton button)
        {
            m_LastClickPosition = position;
            int x = (int)position.X - X - OwnerX;
            int y = (int)position.Y - Y - OwnerY;
            OnMouseDown(x, y, button);
            if (MouseDownEvent != null)
                MouseDownEvent(x, y, button);
        }

        public void MouseUp(Point position, MouseButton button)
        {
            int x = (int)position.X - X - OwnerX;
            int y = (int)position.Y - Y - OwnerY;
            OnMouseUp(x, y, button);
            if (MouseUpEvent != null)
                MouseUpEvent(x, y, button);
        }

        public void MouseOver(Point position)
        {
            // Does not double-click if you move your mouse more than x pixels from where you first clicked.
            if (Math.Abs(m_LastClickPosition.X - position.X) + Math.Abs(m_LastClickPosition.Y - position.Y) > 3)
                m_MaxTimeForDoubleClick = 0.0f;

            int x = (int)position.X - X - OwnerX;
            int y = (int)position.Y - Y - OwnerY;
            OnMouseOver(x, y);
            if (MouseOverEvent != null)
                MouseOverEvent(x, y);
        }

        public void MouseOut(Point position)
        {
            int x = (int)position.X - X - OwnerX;
            int y = (int)position.Y - Y - OwnerY;
            OnMouseOut(x, y);
            if (MouseOutEvent != null)
                MouseOutEvent(x, y);
        }

        public void MouseClick(Point position, MouseButton button)
        {
            int x = (int)position.X - X - OwnerX;
            int y = (int)position.Y - Y - OwnerY;

            bool doubleClick = false;
            if (m_MaxTimeForDoubleClick != 0f)
            {
                if (UltimaEngine.TotalMS <= m_MaxTimeForDoubleClick)
                {
                    m_MaxTimeForDoubleClick = 0f;
                    doubleClick = true;
                }
            }
            else
            {
                m_MaxTimeForDoubleClick = (float)UltimaEngine.TotalMS + EngineVars.DoubleClickMS;
            }

            OnMouseClick(x, y, button);
            if (MouseClickEvent != null)
                MouseClickEvent(x, y, button);

            if (doubleClick)
            {
                OnMouseDoubleClick(x, y, button);
                if (MouseDoubleClickEvent != null)
                    MouseDoubleClickEvent(x, y, button);
            }

            if (button == MouseButton.Right)
            {
                CloseWithRightMouseButton();
            }
        }

        private void CloseWithRightMouseButton()
        {
            if (IsUncloseableWithRMB)
                return;
            AControl parent = m_Owner;
            while (parent != null)
            {
                if (parent.IsUncloseableWithRMB)
                    return;
                parent = parent.m_Owner;
            }

            // send cancel message for server gump
            if (Serial != 0)
                ActivateByButton(0);

            // dispose of this, or owner, if it has one, which will close this as a child.
            if (m_Owner == null)
                Dispose();
            else
                m_Owner.CloseWithRightMouseButton();
        }

        public AControl[] HitTest(Point position, bool alwaysHandleMouseInput)
        {
            List<AControl> focusedControls = new List<AControl>();

            bool inBounds = Area.Contains((int)position.X - OwnerX, (int)position.Y - OwnerY);
            if (inBounds)
            {
                if (InternalHitTest((int)position.X - X - OwnerX, (int)position.Y - Y - OwnerY))
                {
                    if (alwaysHandleMouseInput || HandlesMouseInput)
                        focusedControls.Insert(0, this);
                    foreach (AControl c in Children)
                    {
                        if ((c.Page == 0) || (c.Page == ActivePage))
                        {
                            AControl[] c1 = c.HitTest(position, false);
                            if (c1 != null)
                            {
                                for (int i = c1.Length - 1; i >= 0; i--)
                                {
                                    focusedControls.Insert(0, c1[i]);
                                }
                            }
                        }
                    }
                }
            }

            if (focusedControls.Count == 0)
                return null;
            else
                return focusedControls.ToArray();
        }
        #endregion

        // ================================================================================
        // Keyboard handling code
        // ================================================================================
        #region KeyboardInput

        // private variables

        private bool m_HandlesKeyboardFocus = false;

        // public methods

        public bool HandlesKeyboardFocus
        {
            get
            {
                if (!IsEnabled || !IsInitialized || IsDisposed || !IsVisible)
                    return false;

                if (m_HandlesKeyboardFocus)
                    return true;

                if (m_Children == null)
                    return false;

                foreach (AControl c in m_Children)
                    if (c.HandlesKeyboardFocus)
                        return true;

                return false;
            }
            set
            {
                m_HandlesKeyboardFocus = value;
            }
        }

        public void KeyboardInput(InputEventKeyboard e)
        {
            OnKeyboardInput(e);
        }

        /// <summary>
        /// Called when the Control that current has keyboard focus releases that focus; for example, when Tab is pressed.
        /// </summary>
        /// <param name="c">The control that is releasing focus.</param>
        internal void KeyboardTabToNextFocus(AControl c)
        {
            int startIndex = Children.IndexOf(c);
            for (int i = startIndex + 1; i < Children.Count; i++)
            {
                if (Children[i].HandlesKeyboardFocus)
                {
                    m_UserInterface.KeyboardFocusControl = Children[i];
                    return;
                }
            }
            for (int i = 0; i < startIndex; i++)
            {
                if (Children[i].HandlesKeyboardFocus)
                {
                    m_UserInterface.KeyboardFocusControl = Children[i];
                    return;
                }
            }
        }

        public AControl FindControlThatAcceptsKeyboardFocus()
        {
            if (m_HandlesKeyboardFocus)
                return this;
            if (m_Children == null)
                return null;
            foreach (AControl c in m_Children)
                if (c.HandlesKeyboardFocus)
                    return c.FindControlThatAcceptsKeyboardFocus();
            return null;
        }

        #endregion

        // ================================================================================
        // Debug control boundary drawing code
        // ================================================================================
        #region DebugBoundaryDrawing
#if DEBUG
        static Texture2D s_BoundsTexture;
#endif

        protected void DebugDrawBounds(SpriteBatchUI spriteBatch, Color color)
        {
#if DEBUG
            int hue = IO.HuesXNA.GetWebSafeHue(color);

            Rectangle drawArea = m_Area;
            if (m_Owner == null)
            {
                m_Area.X -= X;
                m_Area.Y -= Y;
            }

            if (s_BoundsTexture == null)
            {
                s_BoundsTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                s_BoundsTexture.SetData<Color>(new Color[] { Color.White });
            }

            spriteBatch.Draw2D(s_BoundsTexture, new Rectangle(X, Y, Width, 1), hue, false, false);
            spriteBatch.Draw2D(s_BoundsTexture, new Rectangle(X, Y + Height - 1, Width, 1), hue, false, false);
            spriteBatch.Draw2D(s_BoundsTexture, new Rectangle(X, Y, 1, Height), hue, false, false);
            spriteBatch.Draw2D(s_BoundsTexture, new Rectangle(X + Width - 1, Y, 1, Height), hue, false, false);
#endif
        #endregion
        }
    }
}