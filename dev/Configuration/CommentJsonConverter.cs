using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UltimaXNA.Data
{
    internal sealed class CommentJsonConverter : JsonConverter
    {
        public override bool CanWrite
        {
            get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var containers = value as Dictionary<string, SettingsContainer>;

            if (containers == null)
            {
                return;
            }

            writer.WriteStartObject();

            var textWriter = writer as JsonTextWriter;

            foreach (var kvp in containers)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Value.Comments))
                {
                    if (textWriter != null)
                    {
                        writer.WriteWhitespace(Environment.NewLine);

                        for (int i = 0; i < textWriter.Indentation; i++)
                        {
                            writer.WriteWhitespace(textWriter.IndentChar.ToString());
                        }

                        writer.WriteComment(kvp.Value.Comments.Wrap(80, textWriter.Indentation + 2, textWriter.IndentChar));
                    }
                    else
                    {
                        writer.WriteComment(kvp.Value.Comments);
                    }
                }

                writer.WritePropertyName(kvp.Key);
                writer.WriteStartObject();

                foreach (var item in kvp.Value)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value.Comments))
                    {
                        if (textWriter != null)
                        {
                            writer.WriteWhitespace(Environment.NewLine);

                            for (int i = 0; i < textWriter.Indentation * 2; i++)
                            {
                                writer.WriteWhitespace(textWriter.IndentChar.ToString());
                            }

                            writer.WriteComment(item.Value.Comments.Wrap(80, textWriter.Indentation * 2 + 2, textWriter.IndentChar));
                        }
                        else
                        {
                            writer.WriteComment(item.Value.Comments);
                        }
                    }

                    writer.WritePropertyName(item.Key);
                    serializer.Serialize(writer, item.Value.Value);
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