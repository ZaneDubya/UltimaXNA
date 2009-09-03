using System;

namespace UltimaXNA.Graphics.UI
{
    public class NodeEventArgs : EventArgs
    {
        readonly UINode _node;

        public UINode Node
        {
            get { return _node; }
        } 


        public NodeEventArgs(UINode node)
        {
            _node = node;
        }
    }
}
