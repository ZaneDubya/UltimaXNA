using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.XGUI.Elements
{
    public class TabBar : AElement
    {
        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.TabBarView(this, Manager);
        }

        protected override Patterns.MVC.AController CreateController()
        {
            return new Controllers.TabBarController(this, Manager);
        }

        public TabBar(AElement parent, int page, string[] items = null)
            : base(parent, page)
        {
            HandlesMouseInput = true;

            if (items != null)
                foreach (string item in items)
                    AddTab(item);
        }

        public string Caption = null;

        private int m_HoverTab = -1, m_SelectedTab = -1;
        public int SelectedTab
        {
            get { return m_SelectedTab; }
            set
            {
                if (value < 0 || value > TabCount || m_SelectedTab == value)
                    return;
                m_SelectedTab = value;
                if (OnTabChanged != null)
                    OnTabChanged();
            }
        }

        public int HoverTab
        {
            get { return m_HoverTab; }
            set { m_HoverTab = value; }
        }

        private List<string> m_Tabs = null;
        public int TabCount
        {
            get
            {
                if (m_Tabs == null)
                    return 0;
                return m_Tabs.Count;
            }
        }

        public string GetTabCaption(int index)
        {
            if (m_Tabs == null)
                return null;
            if (index < 0 || index >= m_Tabs.Count)
                return null;
            return m_Tabs[index];
        }

        private void AddTab(string item, int? index = null)
        {
            if (m_Tabs == null)
                m_Tabs = new List<string>();
            if (index == null || index.Value == 0)
                m_Tabs.Add(item);
            else
                m_Tabs.Insert(index.Value, item);
        }

        public XGUIAction OnTabChanged;
    }
}
