#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UltimaXNA.Diagnostics.Tracing;

#endregion

namespace UltimaXNA.Data
{
    public class SettingsFile
    {
        private static readonly object m_syncRoot = new object();
        private readonly string m_filename;
        private readonly Timer m_saveTimer;
        private Dictionary<string, SettingsContainer> m_sections;

        public SettingsFile(string filename)
        {
            m_saveTimer = new Timer
            {
                Interval = 300,
                AutoReset = true
            };

            m_saveTimer.Elapsed += OnTimerElapsed;

            m_filename = filename;
            m_sections = new Dictionary<string, SettingsContainer>();

            Reload();
        }

        public bool Exists
        {
            get { return File.Exists(m_filename); }
        }

        public void SetValue<T>(string section, string key, T value, string sectionComments = null, string valueComments = null)
        {
            SettingsContainer container;

            if(!m_sections.TryGetValue(section, out container))
            {
                container = new SettingsContainer(sectionComments);
                m_sections.Add(section, container);
            }

            container.Comments = sectionComments;

            if(typeof(T) == typeof(string) && string.IsNullOrWhiteSpace(value as string))
            {
                value = (T)(object)"";
            }

            SettingsToken token;
            JToken v = JToken.FromObject(value);

            if(!container.TryGetValue(key, out token))
            {
                token = new SettingsToken(v, valueComments);
                container[key] = token;
            }
            else
            {
                token.Value = v;
                token.Comments = valueComments;
            }
        }

        public T GetValue<T>(string section, string key, T defaultValue = default(T))
        {
            if(!m_sections.ContainsKey(section))
            {
                return defaultValue;
            }

            SettingsToken token;

            SettingsContainer container = m_sections[section];

            return !container.TryGetValue(key, out token) ? defaultValue : token.Value.ToObject<T>();
        }

        public void Save()
        {
            try
            {
                lock(m_syncRoot)
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DateParseHandling = DateParseHandling.DateTimeOffset,
                        DateTimeZoneHandling = DateTimeZoneHandling.Local,
                        Converters = new List<JsonConverter> {new CommentJsonConverter()}
                    };

                    string result = JsonConvert.SerializeObject(m_sections, Formatting.Indented, settings);

                    File.WriteAllText(m_filename, result);
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
            lock(m_saveTimer)
            {
                m_saveTimer.Stop();
                m_saveTimer.Start();
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Lock the timer so we dont start it till its done save.
            lock(m_saveTimer)
            {
                Save();
            }
        }

        private void Reload()
        {
            if(File.Exists(m_filename))
            {
                Load();
            }
        }

        private void Load()
        {
            m_sections.Clear();

            try
            {
                lock(m_syncRoot)
                {
                    if(!File.Exists(m_filename))
                    {
                        return;
                    }

                    string contents = File.ReadAllText(m_filename);
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    };

                    settings.Converters.Add(new IsoDateTimeConverter());

                    m_sections = JsonConvert.DeserializeObject<Dictionary<string, SettingsContainer>>(contents, settings);
                }
            }
            catch(Exception e)
            {
                Tracer.Error(e);
            }
        }
    }
}