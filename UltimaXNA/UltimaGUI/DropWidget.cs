using UltimaXNA.GUI;
/***************************************************************************
 *   DropWidget.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Input;

namespace UltimaXNA.UltimaGUI
{
    class DropWidget
    {
        ControlEvent _onItemDrop;
        ControlEvent _onItemOver;

        public DropWidget(ControlEvent notifyOnDrop, ControlEvent notifyOnOver)
        {
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
            Control that = ((Control)_onItemDrop.Target);
            if (UltimaEngine.UltimaUI.Cursor.IsHolding && 
                (that.Area.Contains(x, y)))
            {
                _onItemDrop();
            }
        }

        void mouseOver(int x, int y)
        {
            if (UltimaEngine.UltimaUI.Cursor.IsHolding)
            {
                _onItemOver();
            }
        }
    }
}
