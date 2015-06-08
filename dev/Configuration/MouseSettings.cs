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
using UltimaXNA.Core.Configuration;
#endregion

namespace UltimaXNA.Configuration
{
    public class MouseSettings : NotifyPropertyChangedBase
    {
        private MouseButton m_InteractionButton;
        private MouseButton m_movementButton;
        private bool m_isEnabled;
        private float m_ClickAndPickUpMS;
        private float m_DoubleClickMS;

        public MouseSettings(MouseButton interaction, MouseButton movement)
        {
            InteractionButton = interaction;
            MovementButton = movement;
            IsEnabled = true;
            m_ClickAndPickUpMS = 800f; // this is close to what the legacy client uses.
            m_DoubleClickMS = 400;
        }

        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set { SetProperty(ref m_isEnabled, value); }
        }

        public MouseButton MovementButton
        {
            get { return m_movementButton; }
            set { SetProperty(ref m_movementButton, value); }
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