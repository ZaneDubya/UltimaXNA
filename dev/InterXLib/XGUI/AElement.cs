using System.Collections.Generic;
using InterXLib.Display;
using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using InterXLib.Patterns.MVC;

namespace InterXLib.XGUI
{
    public abstract class AElement : AModel
    {
        public string SkinName { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }

        protected GUIManager Manager;
        public AElement Parent { get; private set; }
        public List<AElement> Children { get; private set; }

        protected virtual void OnInitialize() { }
        protected virtual void OnDispose() { }
        protected virtual void OnUpdate(double totalTime, double frameTime) { }

        public bool IsMouseOver = false;
        public bool IsMouseDown = false;
        public Point MousePosition = Point.Zero;

        public bool IsActive
        {
            get
            {
                return !this.IsDisposed && this.IsInitialized && this.IsVisible &&
                    (Parent == null || Parent.IsPageActive(this.Page));
            }
        }

        public bool HasKeyboardFocus
        {
            get
            {
                return (Manager.KeyboardFocusControl == this);
            }
        }

        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }

        public bool IsDisposed { get; protected set; }
        public bool IsInitialized { get; protected set; }
        public bool IsModal { get; protected set; }
        public bool IsMovable { get; protected set; }

        bool m_HandlesKeyboardInput = false, m_HandlesMouseInput = false;
        public bool HandlesMouseInput
        {
            get
            {
                if (!IsEnabled || !IsActive)
                    return false;
                if (m_HandlesMouseInput)
                    return true;
                if (Children == null)
                    return false;
                foreach (AElement c in Children)
                    if (c.HandlesMouseInput)
                        return true;
                return false;
            }
            protected set
            {
                m_HandlesMouseInput = value;
            }
        }
        
        public bool HandlesKeyboardInput
        {
            get
            {
                if (m_HandlesKeyboardInput)
                    return true;
                if (Children == null)
                    return false;
                foreach (AElement c in Children)
                    if (c.HandlesKeyboardInput)
                        return true;
                return false;
            }
            protected set
            {
                if (m_HandlesKeyboardInput != value)
                {
                    m_HandlesKeyboardInput = value;
                    // if this element previously was handling keyboard input and now it doesn't,
                    // announce to the Manager that it no longer handles input.
                    if (m_HandlesKeyboardInput == false && Manager.KeyboardFocusControl == this)
                    {
                        Manager.AnnounceElementGivesUpKeyboardInput(this);
                    }
                    // if this element now handles keyboard input and it's not disposed, disabled, or invisible,
                    // and it's on the right active page for it's parent, then announce that it wants input.
                    else if (m_HandlesKeyboardInput == true && this.IsActive)
                    {
                        Manager.AnnounceElementWantsKeyboardInput(this);
                    }
                }
            }
        }

        protected override AController CreateController()
        {
            // Many gui elements don't need a Controller, so we just return a null controller here.
            // If a gui element needs a controller, it can override this routine.
            return null;
        }

        // when drawing Children, we always draw those with OnPage = 0 and OnPage = ActiveChildrenPage.
        public int Page { get; protected set; }

        private int m_ActiveChildrenPage = 0; 
        public int ActiveChildrenPage
        {
            get
            {
                return m_ActiveChildrenPage;
            }
            protected set
            {
                if (m_ActiveChildrenPage != value)
                {
                    // If the active keyboard handling control is (1) in this.Children & (2) on the current
                    // ActiveChildrenPage, then when ActiveChildrenPage changes, that control will no longer
                    // be able to take input. We need to announce this to the manager.
                    if (Children.Contains(Manager.KeyboardFocusControl) &&
                        Manager.KeyboardFocusControl.Page == m_ActiveChildrenPage)
                    {
                        Manager.AnnounceElementGivesUpKeyboardInput(Manager.KeyboardFocusControl);
                    }
                    m_ActiveChildrenPage = value;
                    // Since the active page has changed, we should see if there are any new keyboard handling
                    // controls that want input. Note that IsActive checks if e.OnPage == (0 or ActiveChildrenPage).
                    foreach (AElement e in Children)
                    {
                        if (e.HandlesKeyboardInput && e.IsActive)
                        {
                            Manager.AnnounceElementWantsKeyboardInput(e);
                            break; // only announce the first one on the page.
                        }
                    }
                }
            }
        }

        public bool IsPageActive(int page)
        {
            return (page == 0) || (page == ActiveChildrenPage);
        }

        /// <summary>
        /// This control's local area within its Parent.
        /// </summary>
        public Rectangle LocalArea
        {
            get { return m_LocalArea; }
            set { m_LocalArea = value; }
        }
        private Rectangle m_LocalArea;
        public int LocalX
        {
            get { return m_LocalArea.X; }
            set { m_LocalArea.X = value; }
        }
        public int LocalWidth
        {
            get { return m_LocalArea.Width; }
            set { m_LocalArea.Width = value; }
        }
        public int LocalY
        {
            get { return m_LocalArea.Y; }
            set { m_LocalArea.Y = value; }
        }
        public int LocalHeight
        {
            get { return m_LocalArea.Height; }
            set { m_LocalArea.Height = value; }
        }



        /// <summary>
        /// This control's area on the screen.
        /// </summary>
        public Rectangle ScreenArea
        {
            get
            {
                if (Parent == null)
                    return LocalArea;
                else
                {
                    Rectangle p = Parent.ScreenArea;
                    return new Rectangle(
                        LocalArea.X + p.X, LocalArea.Y + p.Y,
                        LocalArea.Width, LocalArea.Height);
                }
            }
        }

        public Rectangle ChildArea
        {
            get { return ((AElementView)GetView()).BuildChildArea(new Point(LocalArea.Width, LocalArea.Height)); }
        }

        public AElement(AElement parent, int on_page)
        {
            Parent = parent;
            Page = on_page;

            IsDisposed = false;
            IsEnabled = true;
            IsModal = false;
            IsMovable = false;
            IsVisible = true;
        }

        public void Initialize(GUIManager manager)
        {
            Manager = manager;
            if (SkinName == null) { SkinName = Manager.DefaultSkinName; }
            if (FontName == null) { FontName = Manager.DefaultFontName; }
            if (FontSize == 0f) { FontSize = 16f; }

            IsInitialized = true;

            OnInitialize();
        }

        public AElement AddChild(AElement c, bool behind_all = false)
        {
            if (Children == null)
                Children = new List<AElement>();

            if (behind_all)
                Children.Insert(0, c);
            else
                Children.Add(c);

            if (IsInitialized)
                c.Initialize(Manager);

            return c;
        }

        public void RemoveChild(AElement c)
        {
            if (Children == null)
                return;
            if (Children.Contains(c))
            {
                if (!c.IsDisposed)
                    c.Dispose();
                Children.Remove(c);
                if (Children.Count == 0)
                    Children = null;
            }
        }

        public AElement LastChild
        {
            get
            {
                if (Children == null)
                    return null;
                return Children[Children.Count - 1];
            }
        }

        public void ClearChildren()
        {
            if (Children != null)
                foreach (AElement c in Children)
                    c.Dispose();
            Children = null;
        }

        public virtual void Dispose()
        {
            ClearChildren();
            OnDispose();
            IsDisposed = true;
        }

        /// <summary>
        /// Returns controls that are under a point in screen space.
        /// </summary>
        /// <param name="point">The point in screen space.</param>
        /// <param name="returnAllControls">If true, return all controls, not just those that have HandlesMouseInput = true.</param>
        /// <returns>True if any elements, False if no elements</returns>
        public bool GetElementsAtPoint(Point point, bool onlyMouseHandlingElements, out List<AElement> controls)
        {
            controls = null;
            InternalElementsAtPoint(point, ref controls, onlyMouseHandlingElements);
            return (controls == null) ? false : true;
        }

        protected void InternalElementsAtPoint(Point point, ref List<AElement> controls, bool onlyMouseHandlingElements)
        {
            if (!onlyMouseHandlingElements || this.HandlesMouseInput)
            {
                bool inBounds = ScreenArea.Contains(point);
                if (inBounds)
                {
                    if (!onlyMouseHandlingElements || this.m_HandlesMouseInput)
                    {
                        if (controls == null)
                            controls = new List<AElement>();
                        controls.Insert(0, this);
                    }
                    if (Children != null)
                    {
                        foreach (AElement e in Children)
                        {
                            if (e.IsActive)
                                e.InternalElementsAtPoint(point, ref controls, onlyMouseHandlingElements);
                        }
                    }
                }
            }
        }

        public override void Update(double totalTime, double frameTime)
        {
            if (!IsInitialized || IsDisposed)
                return;

            OnUpdate(totalTime, frameTime);

            if (Children != null)
            {
                foreach (AElement e in Children)
                {
                    if (!e.IsInitialized)
                        e.Initialize(Manager);
                    e.Update(totalTime, frameTime);
                }

                List<AElement> disposedControls = null;
                foreach (AElement e in Children)
                {
                    if (e.IsDisposed)
                    {
                        if (disposedControls == null)
                            disposedControls = new List<AElement>();
                        disposedControls.Add(e);
                    }
                }

                if (disposedControls != null)
                {
                    foreach (AElement e in disposedControls)
                    {
                        RemoveChild(e);
                    }
                }
            }
        }
    }
}