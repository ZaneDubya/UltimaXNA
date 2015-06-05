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

        public MouseSettings()
        {
            InteractionButton = MouseButton.Left;
            MovementButton = MouseButton.Right;
            IsEnabled = true;
        }

        public MouseSettings(MouseButton interaction, MouseButton movement)
        {
            InteractionButton = interaction;
            MovementButton = movement;
            IsEnabled = true;
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
    }
}