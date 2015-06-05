/***************************************************************************
 *   UltimaOnlineSettings.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using UltimaXNA.Core.Configuration;
#endregion

namespace UltimaXNA.Configuration
{
    public sealed class UltimaOnlineSettings : ASettingsSection
    {
        public const string SectionName = "ultimaOnline";

        private string m_DataDirectory;
        private byte[] m_ClientVersion;

        public UltimaOnlineSettings()
        {
            ClientVersion = new byte[] {6, 0, 6, 2};
        }

        public byte[] ClientVersion
        {
            get { return m_ClientVersion; }
            set { SetProperty(ref m_ClientVersion, value); }
        }
        
        public string DataDirectory
        {
            get { return m_DataDirectory; }
            set { SetProperty(ref m_DataDirectory, value); }
        }
    }
}