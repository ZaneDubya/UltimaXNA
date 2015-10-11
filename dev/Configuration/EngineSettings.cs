/***************************************************************************
 *   GameSettings.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Configuration;
#endregion

namespace UltimaXNA.Configuration
{
    public sealed class EngineSettings : ASettingsSection
    {
        public const string SectionName = "engine";

        private bool m_IsVSyncEnabled;
        private bool m_IsFixedTimeStep;

        public EngineSettings()
        {
            IsFixedTimeStep = true;
            IsVSyncEnabled = false;
        }

        public bool IsFixedTimeStep
        {
            get { return m_IsFixedTimeStep; }
            set { SetProperty(ref m_IsFixedTimeStep, value); }
        }

        public bool IsVSyncEnabled
        {
            get { return m_IsVSyncEnabled; }
            set { SetProperty(ref m_IsVSyncEnabled, value); }
        }
    }
}