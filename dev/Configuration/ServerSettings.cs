/***************************************************************************
 *   ServerSettings.cs
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
    public sealed class ServerSettings : ASettingsSection
    {
        public const string SectionName = "server";

        private string m_ServerAddress;
        private int m_ServerPort;
        private string m_UserName;

        public ServerSettings()
        {
            ServerAddress = "127.0.0.1";
            ServerPort = 2593;
        }

        public string UserName
        {
            get { return m_UserName; }
            set { SetProperty(ref m_UserName, value); }
        }

        public int ServerPort
        {
            get { return m_ServerPort; }
            set { SetProperty(ref m_ServerPort, value); }
        }

        public string ServerAddress
        {
            get { return m_ServerAddress; }
            set { SetProperty(ref m_ServerAddress, value); }
        }
    }
}