/***************************************************************************
 *   AControl.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
#endregion

namespace UltimaXNA.Core.UI
{
    /// <summary>
    /// The base class that all UI controls should inherit from.
    /// </summary>
    public abstract class AControl
    {
        // ================================================================================
        // Private variables
        // ================================================================================
        private Rectangle m_Area = new Rectangle();
        private ControlMetaData m_MetaData = null;
        private List<AControl> m_Children = null;

        // ================================================================================
        // Private services
        // ================================================================================
        protected UserInterfaceService UserInterface
        {
            get;
            private set;
        }

        // ================================================================================
        // Public properties
        // ================================================================================
        #region Public properties
        /// <summary>
        /// An identifier for this control. Can be used to differentiate controls of the same type. Used by UO as a 'Serial'
        /// </summary>
        public int GumpLocalID
        {
            get;
            set;
        }

        /// <summary>
        /// Information used by UserInterfaceService to display and update this control.
        /// </summary>
        public ControlMetaData MetaData
        {
            get
            {
                if (m_MetaData == null)
                    m_MetaData = new ControlMetaData(this);
                return m_MetaData;
            }
        }

        /// <summary>
        /// Indicates that the control has been disposed, and will be removed on the next Update() of the UserInterface object.
        /// </summary>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Controls that are not enabled cannot receive keyboard and mouse input, but still Draw.
        /// </summary>
        public bool IsEnabled
        {
            get;
            private set;
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
        /// If true, control can be moved by click-dragging with left mouse button.
        /// A child control can be made a dragger for a parent control with MakeDragger().
        /// </summary>
        public virtual bool IsMoveable
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
            set;
        }

        /// <summary>
        /// If true, gump does not close when the player hits the Escape key. This behavior is currently unimplemented.
        /// </summary>
        public bool IsUncloseableWithEsc
        {
            get;
            set;
        }

        /// <summary>
        /// If true, the gump will draw. Not visible gumps still update and receive mouse input (but not keyboard input).
        /// </summary>
        public bool IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// A list of all the child controls that this control owns.
        /// </summary>
        public List<AControl> Children
        {
            get
            {
                if (m_Children == null)
                    m_Children = new List<AControl>();
                return m_Children;
            }
        }
        #endregion

        #region Position and Area properties
        public int X { get { return m_Area.X; } }
        public int Y { get { return m_Area.Y; } }

        public int ScreenX
        {
            get
            {
                return ParentX + X;
            }
        }

        public int ScreenY
        {
            get
            {
                return ParentY + Y;
            }
        }

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
                return new Point(m_Area.X, m_Area.Y);
            }
            set
            {
                if (value.X != m_Area.X || value.Y != m_Area.Y)
                {
                    m_Area.X = value.X;
                    m_Area.Y = value.Y;
                    OnMove();
                }
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
        #endregion

        #region Page
        /// <summary>
        /// This's control's drawing/input page index. On Update() and Draw(), only those controls with Page == 0 or
        /// Page == Parent.ActivePage will accept input and be drawn.
        /// </summary>
        public int Page
        {
            get;
            set;
        }

        int m_ActivePage = 0; // we always draw m_activePage and Page 0.
        /// <summary>
        /// This control's active page index. On Update and Draw(), this control will send update to and draw all children with Page == 0 or
        /// Page == this.Page.
        /// </summary>
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
                if (UserInterface.KeyboardFocusControl != null)
                {
                    if (Children.Contains(UserInterface.KeyboardFocusControl))
                    {
                        if (UserInterface.KeyboardFocusControl.Page != 0)
                            UserInterface.KeyboardFocusControl = null;
                    }
                }
                // When ActivePage changes, check to see if there are new text input boxes
                // that we should redirect text input to.
                if (UserInterface.KeyboardFocusControl == null)
                {
                    foreach (AControl c in Children)
                    {
                        if (c.HandlesKeyboardFocus && (c.Page == m_ActivePage))
                        {
                            UserInterface.KeyboardFocusControl = c;
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// An event that other objects can use to be notified when this control is clicked.
        /// </summary>
        internal event Action<AControl, int, int, MouseButton> MouseClickEvent;
        /// <summary>
        /// An event that other objects can use to be notified when this control is double-clicked.
        /// </summary>
        internal event Action<AControl, int, int, MouseButton> MouseDoubleClickEvent;
        /// <summary>
        /// An event that other objects can use to be notified when this control receives a mouse down event.
        /// </summary>
        internal event Action<AControl, int, int, MouseButton> MouseDownEvent;
        /// <summary>
        /// An event that other objects can use to be notified when this control receives a mouse up event.
        /// </summary>
        internal event Action<AControl, int, int, MouseButton> MouseUpEvent;
        /// <summary>
        /// An event that other objects can use to be notified when this control receives a mouse over event.
        /// </summary>
        internal event Action<AControl, int, int> MouseOverEvent;
        /// <summary>
        /// An event that other objects can use to be notified when this control receives a mouse out event.
        /// </summary>
        internal event Action<AControl, int, int> MouseOutEvent;
        #endregion

        #region Parent control variables
        public AControl Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the root (topmost, or final) parent of this control.
        /// </summary>
        public AControl RootParent
        {
            get
            {
                if (Parent == null)
                    return null;
                AControl parent = Parent;
                while (parent.Parent != null)
                    parent = parent.Parent;
                return parent;
            }
        }

        private int ParentX
        {
            get
            {
                if (Parent != null)
                    return Parent.X + Parent.ParentX;
                else
                    return 0;
            }
        }

        private int ParentY
        {
            get
            {
                if (Parent != null)
                    return Parent.Y + Parent.ParentY;
                else
                    return 0;
            }
        }
        #endregion

        // ================================================================================
        // Ctor, Init, Dispose, Update, and Draw
        // ================================================================================
        public AControl(AControl parent)
        {
            Parent = parent;
            Page = 0;
            UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public void Initialize()
        {
            IsDisposed = false;
            IsEnabled = true;
            IsInitialized = true;
            IsVisible = true;
            InitializeControls();
            OnInitialize();
        }

        public virtual void Dispose()
        {
            ClearControls();
            IsDisposed = true;
        }

        public virtual void Update(double totalMS, double frameMS)
        {
            if (!IsInitialized || IsDisposed)
                return;

            InitializeControls();
            UpdateControls(totalMS, frameMS);
            ExpandToFitControls();
        }

        virtual public void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            if (!IsInitialized || !IsVisible)
                return;

            if (Settings.Debug.ShowUIOutlines)
                DebugDrawBounds(spriteBatch, position, Color.White);

            foreach (AControl c in Children)
            {
                if ((c.Page == 0) || (c.Page == ActivePage))
                {
                    if (c.IsInitialized && c.IsVisible)
                    {
                        Point offset = new Point(c.Position.X + position.X, c.Position.Y + position.Y);
                        c.Draw(spriteBatch, offset);
                    }
                }
            }
        }

        // ================================================================================
        // Child control methods
        // ================================================================================
        public AControl AddControl(AControl c, int page = 0)
        {
            c.Page = page;
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

        private void InitializeControls()
        {
            bool newlyInitializedChildReceivedKeyboardFocus = false;

            foreach (AControl c in Children)
            {
                if (!c.IsInitialized)
                {
                    c.Initialize();
                    if (!newlyInitializedChildReceivedKeyboardFocus && c.HandlesKeyboardFocus)
                    {
                        UserInterface.KeyboardFocusControl = c;
                        newlyInitializedChildReceivedKeyboardFocus = true;
                    }
                }
            }
        }

        private void UpdateControls(double totalMS, double frameMS)
        {
            foreach (AControl c in Children)
            {
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

        private bool ExpandToFitControls()
        {
            bool changedDimensions = false;
            if (Children.Count > 0)
            {
                int w = 0, h = 0;
                foreach (AControl c in Children)
                {
                    if (c.Page == 0 || c.Page == ActivePage)
                    {
                        if (w < c.X + c.Width)
                        {
                            w = c.X + c.Width;
                        }
                        if (h < c.Y + c.Height)
                        {
                            h = c.Y + c.Height;
                        }
                    }
                }

                if (w != Width || h != Height)
                {
                    Width = w;
                    Height = h;
                    changedDimensions = true;
                }
            }
            return changedDimensions;
        }

        // ================================================================================
        // Miscellaneous methods
        // ================================================================================
        public void CenterThisControlOnScreen()
        {
            Position = new Point(
                (UserInterface.Width - Width) / 2,
                (UserInterface.Height - Height) / 2);
        }

        /// <summary>
        /// Convenience method: Sets this control to (1) handle mouse input and (2) make it moveable (which makes the parent control moveable).
        /// </summary>
        public void MakeThisADragger()
        {
            HandlesMouseInput = true;
            IsMoveable = true;
        }

        public virtual void ActivateByButton(int buttonID)
        {
            if (Parent != null)
                Parent.ActivateByButton(buttonID);
        }

        public virtual void ActivateByHREF(string href)
        {
            if (Parent != null)
                Parent.ActivateByHREF(href);
        }

        public virtual void ActivateByKeyboardReturn(int textID, string text)
        {
            if (Parent != null)
                Parent.ActivateByKeyboardReturn(textID, text);
        }

        public virtual void ChangePage(int pageIndex)
        {
            if (Parent != null)
                Parent.ChangePage(pageIndex);
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

        protected virtual void OnMove()
        {

        }

        protected virtual bool IsPointWithinControl(int x, int y)
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
                if (UserInterface.MouseOverControl == this)
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
            int x = (int)position.X - X - ParentX;
            int y = (int)position.Y - Y - ParentY;
            OnMouseDown(x, y, button);
            if (MouseDownEvent != null)
                MouseDownEvent(this, x, y, button);
        }

        public void MouseUp(Point position, MouseButton button)
        {
            int x = (int)position.X - X - ParentX;
            int y = (int)position.Y - Y - ParentY;
            OnMouseUp(x, y, button);
            if (MouseUpEvent != null)
                MouseUpEvent(this, x, y, button);
        }

        public void MouseOver(Point position)
        {
            // Does not double-click if you move your mouse more than x pixels from where you first clicked.
            if (Math.Abs(m_LastClickPosition.X - position.X) + Math.Abs(m_LastClickPosition.Y - position.Y) > 3)
                m_MaxTimeForDoubleClick = 0.0f;

            int x = (int)position.X - X - ParentX;
            int y = (int)position.Y - Y - ParentY;
            OnMouseOver(x, y);
            if (MouseOverEvent != null)
                MouseOverEvent(this, x, y);
        }

        public void MouseOut(Point position)
        {
            int x = (int)position.X - X - ParentX;
            int y = (int)position.Y - Y - ParentY;
            OnMouseOut(x, y);
            if (MouseOutEvent != null)
                MouseOutEvent(this, x, y);
        }

        public void MouseClick(Point position, MouseButton button)
        {
            int x = (int)position.X - X - ParentX;
            int y = (int)position.Y - Y - ParentY;

            bool doubleClick = false;
            if (m_MaxTimeForDoubleClick != 0f)
            {
                if (UltimaGame.TotalMS <= m_MaxTimeForDoubleClick)
                {
                    m_MaxTimeForDoubleClick = 0f;
                    doubleClick = true;
                }
            }
            else
            {
                m_MaxTimeForDoubleClick = (float)UltimaGame.TotalMS + Settings.World.Mouse.DoubleClickMS;
            }

            if (button == MouseButton.Right && !IsUncloseableWithRMB)
            {
                CloseWithRightMouseButton();
                return;
            }

            OnMouseClick(x, y, button);
            if (MouseClickEvent != null)
                MouseClickEvent(this, x, y, button);

            if (doubleClick)
            {
                OnMouseDoubleClick(x, y, button);
                if (MouseDoubleClickEvent != null)
                    MouseDoubleClickEvent(this, x, y, button);
            }
        }

        protected virtual void CloseWithRightMouseButton()
        {
            if (IsUncloseableWithRMB)
                return;
            AControl parent = Parent;
            while (parent != null)
            {
                if (parent.IsUncloseableWithRMB)
                    return;
                parent = parent.Parent;
            }

            // dispose of this, or the parent if it has one, which will close this as a child.
            if (Parent == null)
                Dispose();
            else
                Parent.CloseWithRightMouseButton();
        }

        public AControl[] HitTest(Point position, bool alwaysHandleMouseInput)
        {
            List<AControl> focusedControls = new List<AControl>();

            bool inBounds = m_Area.Contains((int)position.X - ParentX, (int)position.Y - ParentY);
            if (inBounds)
            {
                if (IsPointWithinControl((int)position.X - X - ParentX, (int)position.Y - Y - ParentY))
                {
                    if (alwaysHandleMouseInput || HandlesMouseInput)
                        focusedControls.Insert(0, this);
                    for (int i = 0; i < Children.Count; i++)
                    {
                        AControl c = Children[i];
                        if ((c.Page == 0) || (c.Page == ActivePage))
                        {
                            AControl[] c1 = c.HitTest(position, false);
                            if (c1 != null)
                            {
                                for (int j = c1.Length - 1; j >= 0; j--)
                                {
                                    focusedControls.Insert(0, c1[j]);
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
                    UserInterface.KeyboardFocusControl = Children[i];
                    return;
                }
            }
            for (int i = 0; i < startIndex; i++)
            {
                if (Children[i].HandlesKeyboardFocus)
                {
                    UserInterface.KeyboardFocusControl = Children[i];
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
        private Texture2D m_BoundsTexture;

        protected void DebugDrawBounds(SpriteBatchUI spriteBatch, Point position, Color color)
        {
            if (m_BoundsTexture == null)
            {
                m_BoundsTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                m_BoundsTexture.SetData<Color>(new Color[] { Color.White });
            }

            Rectangle drawArea = new Rectangle(ScreenX, ScreenY, Width, Height);
            spriteBatch.Draw2D(m_BoundsTexture, new Rectangle(position.X, position.Y, Width, 1), Vector3.Zero);
            spriteBatch.Draw2D(m_BoundsTexture, new Rectangle(position.X, position.Y + Height - 1, Width, 1), Vector3.Zero);
            spriteBatch.Draw2D(m_BoundsTexture, new Rectangle(position.X, position.Y, 1, Height), Vector3.Zero);
            spriteBatch.Draw2D(m_BoundsTexture, new Rectangle(position.X + Width - 1, position.Y, 1, Height), Vector3.Zero);
        #endregion
        }
    }
}