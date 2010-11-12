using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Input;
using UltimaXNA.Entities;

namespace UltimaXNA.UILegacy
{
    class DropWidget
    {
        ControlEvent _onItemDrop;
        ControlEvent _onItemOver;
        UIManager _manager;

        public DropWidget(UIManager manager, ControlEvent notifyOnDrop, ControlEvent notifyOnOver)
        {
            _manager = manager;
            _onItemDrop = notifyOnDrop;
            _onItemOver = notifyOnOver;
        }

        public void AddDropTarget(Control target)
        {
            target.OnMouseUp += mouseUp;
            target.OnMouseOver += mouseOver;
        }

        public void ClearDropTargets()
        {
            // Since our targets notify this widget, we don't need to do anything.
            // Targets will remove the widget's references during disposal.
        }

        void mouseUp(int x, int y, MouseButton button)
        {
            if (_manager.Cursor.IsHolding)
            {
                _onItemDrop();
            }
        }

        void mouseOver(int x, int y)
        {
            if (_manager.Cursor.IsHolding)
            {
                _onItemOver();
            }
        }
    }
}
