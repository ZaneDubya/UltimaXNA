using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements
{
    public class MenuBar : AElement
    {
        private List<MenuElement> m_Elements;
        public int Height = 24;
        public new List<MenuElement> Children
        {
            get { return m_Elements; }
        }

        public MenuBar(List<MenuElement> children, AElement parent, int page)
            : base(parent, page)
        {
            if (children == null)
                m_Elements = new List<MenuElement>();
            else
                m_Elements = children;
            HandlesMouseInput = true;
        }

        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.MenuBarView(this, Manager);
        }

        protected override Patterns.MVC.AController CreateController()
        {
            return new Controllers.MenuBarController(this, Manager);
        }
    }

    public class MenuElement
    {
        public string Label { get; set; }
        public bool Enabled { get; set; }

        public MenuElement(string label, bool enabled = true, XGUIAction action = null, List<MenuElement> children = null)
        {
            Label = label;
            Enabled = enabled;
            if (action != null)
                Action = action;
            if (children != null)
                m_Children = children;
        }

        public bool HasChildren
        {
            get
            {
                return (m_Children == null) ? false : true;
            }
        }

        public bool HasAction
        {
            get
            {
                return (m_Action == null) ? false : true;
            }
        }

        public XGUIAction Action
        {
            get
            {
                if (HasAction)
                    return m_Action;
                return null;
            }
            set
            {
                if (HasChildren)
                    clearChildren();
                m_Action = value;
            }
        }

        public List<MenuElement> Children
        {
            get
            {
                if (HasChildren)
                    return m_Children;
                return null;
            }
        }

        public void AddChild(MenuElement element)
        {
            if (HasAction)
            {
                m_Action = null;
                m_Children = new List<MenuElement>();
            }
            m_Children.Add(element);
        }

        private List<MenuElement> m_Children = null;
        private XGUIAction m_Action = null;

        private void clearChildren()
        {
            for (int i = 0; i < m_Children.Count; i++)
                m_Children[i] = null;
            m_Children.Clear();
            m_Children = null;
        }
    }

    class MenuDivider : MenuElement
    {
        public MenuDivider()
            : base("|", false)
        {

        }
    }
}
