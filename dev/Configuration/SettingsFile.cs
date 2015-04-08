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
        private Dictionary<string, JContainer> _sections;

        public SettingsFile(string filename)
        {
            _saveTimer = new Timer();
            _saveTimer.Interval = 300; // 0.3 Seconds
            _saveTimer.AutoReset = true;
            _saveTimer.Elapsed += OnTimerElapsed;

            _filename = filename;
            _sections = new Dictionary<string, JContainer>();

            Reload();
        }

        public bool Exists
        {
            get { return File.Exists(_filename); }
        }

        public void SetValue<T>(string section, string key, T value)
        {
            JContainer container;

            if(!_sections.TryGetValue(section, out container))
            {
                container = JToken.Parse("{}") as JContainer;
                _sections.Add(section, container);
            }

            var v = JToken.FromObject(value);

            if(container[key] != v)
            {
                container[key] = v;
                InvalidateDirty();
            }
        }

        public T GetValue<T>(string section, string key, T defaultValue = default(T))
        {
            if(!_sections.ContainsKey(section))
            {
                return defaultValue;
            }

            var container = _sections[section];
            var value = container[key];

            return value == null ? defaultValue : value.ToObject<T>();
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

                    _sections = JsonConvert.DeserializeObject<Dictionary<string, JContainer>>(contents, settings);
                }
            }
            catch(Exception e)
            {
                Tracer.Error(e);
            }
        }
    }
}