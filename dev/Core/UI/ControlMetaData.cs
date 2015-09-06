/***************************************************************************
 *   ControlMetaData.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Core.UI
{
    public class ControlMetaData
    {
        public UILayer Layer
        {
            get;
            set;
        }

        /// <summary>
        /// Controls that are Modal appear on top of all other controls and block input to all other controls and the world.
        /// </summary>
        public bool IsModal
        {
            get;
            set;
        }

        /// <summary>
        /// If modal, and this is true, then a click outside the modal area will close the control.
        /// </summary>
        public bool ModalClickOutsideAreaClosesThisControl
        {
            get;
            set;
        }

        public AControl Control
        {
            get;
            private set;
        }

        public ControlMetaData(AControl control)
        {
            Control = control;
        }
    }
}
