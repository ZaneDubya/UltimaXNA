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
            _saveTimer = new Timer();
            _saveTimer.Interval = 300; // 0.3 Seconds
            _saveTimer.AutoReset = true;
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

            if(typeof(T) == typeof(string) && value == null)
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
                token.Token = v;
                token.Comments = valueComments;
            }
        }

        public T GetValue<T>(string section, string key, T defaultValue = default(T))
        {
            if(!_sections.ContainsKey(section))
            {
                return defaultValue;
            }

            var container = _sections[section];
            var token = default(SettingsToken);

            if(!container.TryGetValue(key, out token))
            {
                return defaultValue;
            }

            return token.Token.ToObject<T>();
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

        private sealed class CommentJsonConverter : JsonConverter
        {
            public override bool CanWrite
            {
                get { return true; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var containers = value as Dictionary<string, SettingsContainer>;

                if(containers == null)
                {
                    return;
                }

                writer.WriteStartObject();

                var textWriter = writer as JsonTextWriter;

                foreach(var kvp in containers)
                {
                    if(!string.IsNullOrWhiteSpace(kvp.Value.Comments))
                    {
                        if(textWriter != null)
                        {
                            writer.WriteWhitespace(Environment.NewLine);

                            for(int i = 0; i < textWriter.Indentation + 1; i++)
                            {
                                writer.WriteWhitespace(textWriter.IndentChar.ToString());
                            }

                            writer.WriteComment(kvp.Value.Comments.Wrap(80, textWriter.Indentation + 1, textWriter.IndentChar));
                        }
                        else
                        {
                            writer.WriteComment(kvp.Value.Comments);
                        }
                    }

                    writer.WritePropertyName(kvp.Key);
                    writer.WriteStartObject();

                    foreach(var item in kvp.Value)
                    {
                        if(!string.IsNullOrWhiteSpace(item.Value.Comments))
                        {
                            if(textWriter != null)
                            {
                                writer.WriteWhitespace(Environment.NewLine);

                                for(int i = 0; i < textWriter.Indentation + 1; i++)
                                {
                                    writer.WriteWhitespace(textWriter.IndentChar.ToString());
                                }

                                writer.WriteComment(item.Value.Comments.Wrap(80, textWriter.Indentation + 1, textWriter.IndentChar));
                            }
                            else
                            {
                                writer.WriteComment(item.Value.Comments);
                            }
                        }

                        writer.WritePropertyName(item.Key);
                        serializer.Serialize(writer, item.Value.Token);
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Dictionary<string, SettingsContainer>);
            }
        }
    }
}