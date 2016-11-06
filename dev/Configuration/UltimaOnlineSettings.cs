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
#region usings
using UltimaXNA.Core.Configuration;
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Configuration
{
    public sealed class UltimaOnlineSettings : ASettingsSection
    {
        public const string SectionName = "ultimaOnline";

        bool m_AllowCornerMovement = false;
        string m_DataDirectory;
        byte[] m_ClientVersion;

        public UltimaOnlineSettings()
        {
            PatchVersion = ClientVersion.DefaultVersion;
        }

        /// <summary>
        /// The patch version which is sent to the server. RunUO (and possibly other server software) rely on the
        /// client's reported patch version to enable/disable certain packets and features.
        /// </summary>
        public byte[] PatchVersion
        {
            get {
                if (m_ClientVersion == null || m_ClientVersion.Length != 4)
                    return ClientVersion.DefaultVersion;
                return m_ClientVersion;
            }
            set
            {
                if (value == null || value.Length != 4)
                    return;
                // Note from ZaneDubya: I will not support your client if you change or remove this line:
                if (!ClientVersion.EqualTo(value, ClientVersion.DefaultVersion)) return;
                SetProperty(ref m_ClientVersion, value);
            }
        }
        
        /// <summary>
        /// The directory where the Ultima Online resource files and executable are located.
        /// </summary>
        public string DataDirectory
        {
            get { return m_DataDirectory; }
            set { SetProperty(ref m_DataDirectory, value); }
        }

        public bool AllowCornerMovement
        {
            get { return m_AllowCornerMovement; }
            set { SetProperty(ref m_AllowCornerMovement, value); }
        }
    }
}