using System;
using System.Collections.Generic;

namespace UltimaXNA.Graphics.UI
{
    public class UIContainer
    {
        List<UINode> _nodes;
        UINode _owner;
        bool _sortable;

        public event EventHandler<NodeEventArgs> Added;
        public event EventHandler<NodeEventArgs> Removed;

        public bool Sortable
        {
            get { return _sortable; }
            set { _sortable = value; }
        }

        public UINode Owner
        {
            get { return _owner; }
        }

        public int Count
        {
            get { return _nodes.Count; }
        }

        public UINode this[int index]
        {
            get { return _nodes[index]; }
            set { _nodes[index] = value; }
        }

        public UIContainer(UINode owner)
            : this(owner, false) { }

        public UIContainer(UINode owner, bool sortable)
        {
            _owner = owner;
            _sortable = sortable;
            _nodes = new List<UINode>();
        }

        public void Add(UINode node)
        {
            if (!_nodes.Contains(node))
            {
                node.Parent = _owner;
                node.Index = _nodes.Count;

                _nodes.Add(node);

                OnAdded(this, new NodeEventArgs(node));
            }
        }

        public bool Remove(UINode node)
        {
            bool removed = _nodes.Remove(node);

            if (removed)
            {
                OnRemoved(this, new NodeEventArgs(node));

                node.Parent = null;
            }

            return removed;
        }

        public bool Contains(UINode node)
        {
            if (_nodes.Count == 0)
            {
                return false;
            }

            return _nodes.Contains(node);
        }

        public void Sort()
        {
            if (_sortable)
            {
                _nodes.Sort(new Comparison<UINode>(delegate(UINode a, UINode b)
                {
                    return a.Index.CompareTo(b.Index);
                }));
            }
        }

        public void SendToBack(UINode node)
        {
            if (_sortable)
            {
                if (_nodes.Contains(node) && node.Index > 0)
                {
                    for (int i = 0; i < _nodes.Count; i++)
                    {
                        _nodes[i].Index++;
                    }

                    node.Index = 0;
                }
            }
        }

        public void BringToFront(UINode node)
        {
            if (_sortable)
            {
                if (_nodes.Contains(node) && node.Index < _nodes.Count - 1)
                {
                    for (int i = node.Index; i < _nodes.Count; i++)
                    {
                        _nodes[i].Index--;
                    }

                    node.Index = _nodes.Count - 1;
                }
            }
        }

        public UINode FindFistNodeByType<T>()
        {
            return FindFistNodeByType<T>(false);
        }

        public UINode[] FindNodesByType<T>()
        {
            return FindNodesByType<T>(false);
        }

        public UINode FindFistNodeByType<T>(bool recursive)
        {
            UINode node = null;

            for (int i = 0; i < _nodes.Count && node == null; i++)
            {
                if (_nodes[i] is T)
                {
                    node = _nodes[i];
                }
                else if (recursive)
                {
                    IUIContainer host = _nodes[i] as IUIContainer;

                    if (host != null)
                    {
                        node = host.Children.FindFistNodeByType<T>(recursive);
                    }
                }
            }

            return node;
        }

        public UINode[] FindNodesByType<T>(bool recursive)
        {
            List<UINode> found = null;

            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i] is T)
                {
                    found.Add(_nodes[i]);
                }

                if (recursive)
                {
                    IUIContainer host = _nodes[i] as IUIContainer;

                    if (host != null)
                    {
                        found.AddRange(host.Children.FindNodesByType<T>(recursive));
                    }
                }
            }

            return found.ToArray();
        }

        public UINode FindNodeByName(string name)
        {
            return FindNodeByName(name, false);
        }

        public UINode FindNodeByName(string name, bool recursive)
        {
            UINode node = null;

            for (int i = 0; i < _nodes.Count && node == null; i++)
            {
                if (_nodes[i].Name == name)
                {
                    node = _nodes[i];
                }
                else if (recursive)
                {
                    IUIContainer host = _nodes[i] as IUIContainer;

                    if (host != null)
                    {
                        node = host.Children.FindNodeByName(name, recursive);
                    }
                }
            }

            return node;
        }

        public T FindNodeByName<T>(string name) where T : UINode
        {
            return FindNodeByName<T>(name, false);
        }

        public T FindNodeByName<T>(string name, bool recursive) where T : UINode
        {
            T node = default(T);

            for (int i = 0; i < _nodes.Count && node == null; i++)
            {
                if (_nodes[i].Name == name)
                {
                    node = (T)_nodes[i];
                }
                else if (recursive)
                {
                    IUIContainer host = _nodes[i] as IUIContainer;

                    if (host != null)
                    {
                        UINode n = host.Children.FindNodeByName(name, recursive);

                        if (n != null)
                        {
                            node = (T)n;
                        }
                    }
                }
            }

            return node;
        }

        protected virtual void OnAdded(object sender, NodeEventArgs e)
        {
            if (Added != null)
            {
                Added(sender, e);
            }
        }

        protected virtual void OnRemoved(object sender, NodeEventArgs e)
        {
            if (Removed != null)
            {
                Removed(sender, e);
            }
        }
    }
}
