/***************************************************************************
 *   DropWidget.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.UltimaGUI;
using UltimaXNA.Input.Windows;
#endregion

namespace UltimaXNA.UltimaGUI
{
    class DropWidget_Unused
    {
        ControlEvent m_onItemDrop;
        ControlEvent m_onItemOver;

        public DropWidget_Unused(ControlEvent notifyOnDrop, ControlEvent notifyOnOver)
        {
            m_onItemDrop = notifyOnDrop;
            m_onItemOver = notifyOnOver;
        }

        public void AddDropTarget(AControl target)
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
            /*Control that = ((Control)m_onItemDrop.Target);
            if (WorldInteraction.Cursor.IsHolding && 
                (that.Area.Contains(x, y)))
            {
                m_onItemDrop();
            }*/
        }

        void mouseOver(int x, int y)
        {
            /*if (WorldInteraction.Cursor.IsHolding)
            {
                m_onItemOver();
            }*/
        }
    }
}
