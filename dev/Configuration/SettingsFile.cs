using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UltimaXNA.Diagnostics.Tracing;

namespace UltimaXNA.Data
{
    public class SettingsFile
    {
        private static readonly object _syncRoot = new object();
        private readonly string _filename;
        private readonly Timer _saveTimer;
        private Dictionary<string, SettingsContainer> _sections;

        public SettingsFile(string filename)
        {
            _saveTimer = new Timer 
            {
                Interval = 300, 
                AutoReset = true
            };

            _saveTimer.Elapsed += OnTimerElapsed;

            _filename = filename;
            _sections = new Dictionary<string, SettingsContainer>();

            Reload();
        }

        public bool Exists
        {
            get { return File.Exists(_filename); }
        }

        public void SetValue<T>(string section, string key, T value, string sectionComments = null, string valueComments = null)
        {
            SettingsContainer container;

            if(!_sections.TryGetValue(section, out container))
            {
                container = new SettingsContainer(sectionComments);
                _sections.Add(section, container);
            }

            container.Comments = sectionComments;

            if(typeof(T) == typeof(string) && string.IsNullOrWhiteSpace(value as string))
            {
                value = (T)(object)"";
            }

            SettingsToken token;
            var v = JToken.FromObject(value);

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
            if(!_sections.ContainsKey(section))
            {
                return defaultValue;
            }

            SettingsToken token;

            var container = _sections[section];

            return !container.TryGetValue(key, out token) ? defaultValue : token.Value.ToObject<T>();
        }

        public void Save()
        {
            try
            {
                lock(_syncRoot)
                {
                    var settings = new JsonSerializerSettings
                                   {
                                       NullValueHandling = NullValueHandling.Ignore,
                                       DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                       DateParseHandling = DateParseHandling.DateTimeOffset,
                                       DateTimeZoneHandling = DateTimeZoneHandling.Local,
                                       Converters = new List<JsonConverter> {new CommentJsonConverter()}
                                   };

                    var result = JsonConvert.SerializeObject(_sections, Formatting.Indented, settings);

                    File.WriteAllText(_filename, result);
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
            lock(_saveTimer)
            {
                _saveTimer.Stop();
                _saveTimer.Start();
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Lock the timer so we dont start it till its done save.
            lock(_saveTimer)
            {
                Save();
            }
        }

        private void Reload()
        {
            if(File.Exists(_filename))
            {
                Load();
            }
        }

        private void Load()
        {
            _sections.Clear();

            try
            {
                lock(_syncRoot)
                {
                    if(!File.Exists(_filename))
                    {
                        return;
                    }

                    var contents = File.ReadAllText(_filename);
                    var settings = new JsonSerializerSettings
                                   {
                                       NullValueHandling = NullValueHandling.Ignore,
                                   };

                    settings.Converters.Add(new IsoDateTimeConverter());

                    _sections = JsonConvert.DeserializeObject<Dictionary<string, SettingsContainer>>(contents, settings);
                }
            }
            catch(Exception e)
            {
                Tracer.Error(e);
            }
        }
    }
}