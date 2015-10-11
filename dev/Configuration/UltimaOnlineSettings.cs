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
            // NOTE FROM ZaneDubya: DO NOT CHANGE ClientVersion from 6.0.6.2.
            // We are focusing our efforts on getting a specific version of the client working.
            // Once we have this version working, we will attempt to support additional versions.
            // We will not support any issues you experience after changing this value.
            ClientVersion = new byte[] {6, 0, 6, 2};
        }

        /// <summary>
        /// The patch version which is sent to the server. Hardcoded to 6.0.6.2.
        /// RunUO (and possibly other server software) rely on the client's reported
        /// patch version to enable/disable certain packets and features. You WILL have
        /// issues if you change this out of a given range of supported values.
        /// </summary>
        public byte[] ClientVersion
        {
            get { return m_ClientVersion; }
            set
            {
                if (value == null || value.Length != 4)
                    return;
                // Do not remove this check. See above.
                if (value != new byte[] { 6, 0, 6, 2 })
                    return;
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
    }
}