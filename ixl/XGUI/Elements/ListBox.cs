using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using InterXLib.Input.Windows;

namespace InterXLib.XGUI.Elements
{
    public class ListBox<T> : AElement where T : class
    {
        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.ListBoxView<T>(this, Manager);
        }

        protected override Patterns.MVC.AController CreateController()
        {
            return new Controllers.ListBoxController<T>(this, Manager);
        }

        public ListBox(AElement parent, int page, List<T> items = null)
            :base(parent, page)
        {
            if (items != null)
                SetItems(items);
            HandlesMouseInput = true;
            FontSize = 14f;
        }

        private int? m_SelectedIndex;
        private List<T> m_Items = new List<T>();
        // Core.GUI.Content.ScrollBars m_Scroll; !!!

        /// <summary>
        /// Get the item currently selected. If no item has been selected return null.
        /// </summary>
        public T SelectedItem
        {
            get
            {
                if (m_Items.Count == 0)
                    return null;
                return m_SelectedIndex.HasValue ? (T)m_Items[m_SelectedIndex.Value] : null;
            }
        }

        /// <summary>
        /// The index of the item currently selected. Setting this will call OnSelectionChanged.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return m_SelectedIndex.HasValue ? (int)m_SelectedIndex : -1;
            }
            set
            {
                bool bypass_check_new = false;
                if (value < 0)
                {
                    value = 0;
                    bypass_check_new = true;
                }
                if (bypass_check_new || value >= 0 && value < m_Items.Count)
                {
                    if (bypass_check_new || !m_SelectedIndex.HasValue || value != m_SelectedIndex.Value)
                    {
                        m_SelectedIndex = value;
                        if (OnSelectionChanged != null)
                            OnSelectionChanged();
                    }
                }
            }
        }

        public int ItemAtPosition(Point position)
        {
            Views.ListBoxView<T> view = (Views.ListBoxView<T>)CreateView();

            int first_item = (ScrollPosition / view.LineSpacing);
            int item_offset = (ScrollPosition % view.LineSpacing);

            int y = position.Y - view.SafeArea.Y + item_offset;
            int index = (y / view.LineSpacing) + first_item;
            return index;
        }

        public void SetSelectionIndexWithoutOnSelectionChanged(int value)
        {
            XGUIAction e = OnSelectionChanged;
            OnSelectionChanged = null;
            SelectedIndex = value;
            OnSelectionChanged = e;
        }

        public int ScrollPosition
        {
            get { return 0; }
            set { }
            // get { return m_Scroll.ScrollPosition.Y; }
            // set { m_Scroll.ScrollPosition = new Point(0, value); }
        }

        /// <summary>
        /// Event fired when ever the selection is changed.
        /// </summary>
        public XGUIAction OnSelectionChanged { get; set; }

        private void UpdateScrollArea()
        {
            Views.ListBoxView<T> view = (Views.ListBoxView<T>)CreateView();

            /*if (m_Scroll != null)
            {
                m_Scroll.OverrideScrollArea(new Rectangle(0, 0, 1, view.LineSpacing * Items.Count));
                if (ScrollPosition + LocalArea.Height > view.LineSpacing * Items.Count)
                    ScrollPosition = view.LineSpacing * Items.Count;
            }*/
        }

        protected override void OnUpdate(double totalTime, double frameTime)
        {
            if (Items == null ||Items.Count == 0)
                m_SelectedIndex = null;
            else
            {
                if (m_SelectedIndex == null)
                    SelectedIndex = 0;
            }
            /*if (m_Scroll == null)
            {
                // AddChild(m_Scroll = new Core.GUI.Content.ScrollBars());
                UpdateScrollArea();
            }*/
        }

        // ================================================================================
        // Item Management
        // ================================================================================

        /// <summary>
        /// Return the ListBox's Items
        /// </summary>        
        public List<T> Items
        {
            get { return m_Items; }
            set
            {
                if (m_Items != value)
                {
                    m_Items = value;
                    SelectedIndex = -1;
                }
            }
        }

        public void Add(T item)
        {
            if (m_Items == null)
                m_Items = new List<T>();
            m_Items.Add(item);
            UpdateScrollArea();
        }

        public void RemoveAt(int index)
        {
            if (m_Items == null)
                return;
            if (index < 0 || index >= m_Items.Count)
                return;
            m_Items.RemoveAt(index);
            UpdateScrollArea();
        }

        public void Remove(T item)
        {
            if (m_Items == null)
                m_Items = new List<T>();
            m_Items.Remove(item);
            UpdateScrollArea();
        }

        public void RemoveLast()
        {
            if (m_Items == null)
                return;
            RemoveAt(m_Items.Count - 1);
            UpdateScrollArea();
        }

        /// <summary>
        /// Set the items for the list box.
        /// </summary>
        public void SetItems(IEnumerable<T> items)
        {
            if (m_Items == null)
                m_Items = new List<T>();
            m_Items.Clear();

            IEnumerator<T> enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
                m_Items.Add(enumerator.Current);
            UpdateScrollArea();
        }
    }
}
