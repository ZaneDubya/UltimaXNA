using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Newtonsoft.Json.Linq;
using UltimaXNA.Diagnostics.Tracing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UltimaXNA.Data
{
    public class SettingsFile
    {
        private static readonly object _syncRoot = new object();

        private readonly string _filename;
        private readonly Subject<bool> _isDirty;

        private Dictionary<string, JContainer> _sections;

        public SettingsFile(string filename)
        {
            _filename = filename;
            _sections = new Dictionary<string, JContainer>();

            _isDirty = new Subject<bool>();
            _isDirty.Throttle(TimeSpan.FromSeconds(1), Scheduler.Default);
            _isDirty.Subscribe(OnDirtyChanged);

            Reload();
        }
        
        public bool Exists
        {
            get { return File.Exists(_filename); }
        }

        public void SetValue<T>(string section, string key, T value)
        {
            JContainer container;

            if (!_sections.TryGetValue(section, out container))
            {
                container = JToken.Parse("{}") as JContainer;
                _sections.Add(section, container);
            }

            var v = JToken.FromObject(value);

            if (container[key] != v)
            {
                container[key] = v;
                InvalidateDirty();
            }
        }

        public T GetValue<T>(string section, string key, T defaultValue = default(T))
        {
            if (!_sections.ContainsKey(section))
            {
                return defaultValue;
            }

            var container = _sections[section];
            var value = container[key];

            return value == null ? defaultValue : value.ToObject<T>();
        }

        private void Reload()
        {
            if (File.Exists(_filename))
            {
                Load();
            }
        }

        private void OnDirtyChanged(bool isDirty)
        {
            lock (_isDirty)
            {
                Save();
            }
        }

        private void Load()
        {
            _sections.Clear();

            try
            {
                lock (_syncRoot)
                {
                    if (!File.Exists(_filename))
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
            catch (Exception e)
            {
                Tracer.Error(e);
            }
        }

        public void Save()
        {
            try
            {
                lock (_syncRoot)
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
            catch (Exception e)
            {
                Tracer.Error(e);
            }
        }

        internal void InvalidateDirty()
        {
            _isDirty.OnNext(true);
        }
    }
}