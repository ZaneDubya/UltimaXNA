#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UltimaXNA.Core.Diagnostics.Tracing;

#endregion

namespace UltimaXNA.Configuration
{
    public class SettingsFile
    {
        private readonly Dictionary<string, SettingsSectionBase> m_SectionCache;
        private static readonly object m_SyncRoot = new object();

        private readonly string m_Filename;
        private readonly Timer m_SaveTimer;
        private Dictionary<string, JToken> m_TokenCache;

        public SettingsFile(string filename)
        {
            m_SectionCache = new Dictionary<string, SettingsSectionBase>();
            m_TokenCache = new Dictionary<string, JToken>();
            m_SaveTimer = new Timer
            {
                Interval = 300,
                AutoReset = true
            };
            m_SaveTimer.Elapsed += OnTimerElapsed;
            m_Filename = filename;

            if (File.Exists(m_Filename))
            {
                try
                {
                    Load();
                }
                catch (Exception e)
                {
                    Tracer.Error(e, "Unable to load settings file {0}", m_Filename);
                }
            }
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
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DateParseHandling = DateParseHandling.DateTimeOffset,
                        DateTimeZoneHandling = DateTimeZoneHandling.Local
                    };

                    string result = JsonConvert.SerializeObject(m_SectionCache, Formatting.Indented, settings);

                    File.Copy(m_Filename, m_Filename + ".bak", true);
                    File.WriteAllText(m_Filename, result);
                }
            }
            catch(Exception e)
            {
                Tracer.Error(e);
            }
        }

        internal void InvalidateDirty()
        {
            //Lock the timer so we dont start it while its saving
            lock(m_SaveTimer)
            {
                m_SaveTimer.Stop();
                m_SaveTimer.Start();
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Lock the timer so we dont start it till its done save.
            lock(m_SaveTimer)
            {
                Save();
            }
        }

        private void Load()
        {
            try
            {
                lock(m_SyncRoot)
                {
                    LoadFromFile(m_Filename);

                    if (m_TokenCache == null && File.Exists(m_Filename + ".bak"))
                    {
                        Tracer.Error("Unable to read settings file.  Trying backup file");
                        LoadFromFile(m_Filename + ".bak");

                        if (m_TokenCache == null)
                        {
                            Tracer.Error("Unable to read settings backup file.  Resettings all settings.");
                            m_TokenCache = new Dictionary<string, JToken>();
                        }
                    }
                    else
                    {
                        Tracer.Error("Unable to read settings file.  Resettings all settings.");
                        m_TokenCache = new Dictionary<string, JToken>();
                    }
                }
            }
            catch(Exception e)
            {
                Tracer.Error(e);
            }
        }

        private void LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }
            string contents = File.ReadAllText(fileName);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new IsoDateTimeConverter() }
            };

            m_TokenCache = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(contents, settings);
        }

        internal T CreateOrOpenSection<T>(string sectionName)
            where T : SettingsSectionBase, new()
        {
            JToken token;
            SettingsSectionBase section;

            // We've already deserialized the section, so just return it.
            if(m_SectionCache.TryGetValue(sectionName, out section))
            {
                return (T)section;
            }

            bool isCached = m_TokenCache.TryGetValue(sectionName, out token);
            
            if (isCached)
            {
                // We've haven't deserialized it but it exists, so read it in and save it to the local cache
                section = token.ToObject<T>();
                section.OnDeserialized();
            }
            else
            {
                // New section not saved in the file, create it and save it.
                section = new T();
                m_TokenCache[sectionName] = JToken.FromObject(section);
                InvalidateDirty();
            }
            
            m_SectionCache[sectionName] = section;

            return (T)section;
        }
    }
}