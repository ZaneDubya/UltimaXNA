/***************************************************************************
 *   MouseSettings.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.ComponentModel;
using UltimaXNA.Core.Input.Windows;
#endregion

namespace UltimaXNA.Configuration
{
    public class MouseConfig : NotifyPropertyChangedBase
    {
        private MouseButton m_InteractionButton = MouseButton.Left;
        private MouseButton m_MovementButton = MouseButton.Right;
        private bool m_IsEnabled = true;
        private float m_ClickAndPickUpMS = 800f; // this is close to what the legacy client uses.
        private float m_DoubleClickMS = 400f;

        public MouseConfig()
        {

        }

        public bool IsEnabled
        {
            get { return m_IsEnabled; }
            set { SetProperty(ref m_IsEnabled, value); }
        }

        public MouseButton MovementButton
        {
            get { return m_MovementButton; }
            set { SetProperty(ref m_MovementButton, value); }
        }

        public MouseButton InteractionButton
        {
            get { return m_InteractionButton; }
            set { SetProperty(ref m_InteractionButton, value); }
        }

        public float ClickAndPickupMS
        {
            get { return m_ClickAndPickUpMS; }
            set { SetProperty(ref m_ClickAndPickUpMS, value); }
        }

        public float DoubleClickMS
        {
            get { return m_DoubleClickMS; }
            set { SetProperty(ref m_DoubleClickMS, value); }
        }

        
    }
}