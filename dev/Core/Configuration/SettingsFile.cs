/***************************************************************************
 *   SettingsFile.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using UltimaXNA.Core.Diagnostics.Tracing;
#endregion

namespace UltimaXNA.Core.Configuration
{
    public class SettingsFile
    {
        private readonly Dictionary<string, ASettingsSection> m_SectionCache;
        private static readonly object m_SyncRoot = new object();

        private readonly string m_Filename;
        private readonly Timer m_SaveTimer;

        public SettingsFile(string filename)
        {
            m_SectionCache = new Dictionary<string, ASettingsSection>();
            m_SaveTimer = new Timer
            {
                Interval = 1000, // save settings every 1 second
                AutoReset = true
            };
            m_SaveTimer.Elapsed += OnTimerElapsed;
            m_Filename = filename;
        }

        public bool Exists
        {
            get { return File.Exists(m_Filename); }
        }
        
        public void Save()
        {
            try
            {
                lock(m_SyncRoot)
                {
                    TextSettingsFileWriter serializer = new TextSettingsFileWriter(m_Filename);
                    serializer.Serialize(m_SectionCache);
                }
            }
            catch(Exception e)
            {
                Tracer.Error(e);
            }
        }

        internal void InvalidateDirty()
        {
            // possibly save the file here?
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Lock the timer so we dont start it till its done save.
            lock(m_SaveTimer)
            {
                Save();
            }
        }

        public void Load()
        {
            try
            {
                lock (m_SyncRoot)
                {
                    if (LoadFromFile(m_Filename))
                    {
                        Tracer.Debug("Read settings from settings file.");
                    }
                    else
                    {
                        if (File.Exists(m_Filename + ".bak"))
                        {
                            Tracer.Error("Unable to read settings file.  Trying backup file");
                            if (LoadFromFile(m_Filename + ".bak"))
                            {
                                Tracer.Debug("Read settings from backup settings file.", null);
                            }
                            else
                            {
                                Tracer.Error("Unable to read backup settings file. All settings are set to default values.");
                            }
                        }
                        else
                        {
                            Tracer.Error("Unable to read settings file. All settings are set to default values.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }

            m_SaveTimer.Enabled = true;
        }

        private bool LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                TextSettingsFileWriter serializer = new TextSettingsFileWriter(fileName);
                serializer.Deserialize(m_SectionCache);
                return true;
            }
            catch (Exception e)
            {
                Tracer.Error(e);
                return false;
            }
        }

        internal T CreateOrOpenSection<T>(string sectionName)
            where T : ASettingsSection, new()
        {
            ASettingsSection section;

            // If we've already deserialized the section, just return it.
            if(m_SectionCache.TryGetValue(sectionName, out section))
            {
                return (T)section;
            }

            section = new T();
            InvalidateDirty();
            m_SectionCache[sectionName] = section;
            return (T)section;
        }
    }
}