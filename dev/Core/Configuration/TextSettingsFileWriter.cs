/***************************************************************************
 *   TextSettingsFileWriter.cs
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
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
#endregion

namespace UltimaXNA.Core.Configuration
{
    class TextSettingsFileWriter
    {
        private readonly string m_Filename;
        private readonly JavaScriptSerializer m_Serializer;
        public TextSettingsFileWriter(string filename)
        {
            m_Filename = filename;
            m_Serializer = new JavaScriptSerializer();
        }

        public void Serialize(Dictionary<string, ASettingsSection> sections)
        {
            // Save a copy of the old settings file, if it exists.
            if (File.Exists(m_Filename))
                File.Copy(m_Filename, m_Filename + ".bak", true);

            StringBuilder result = new StringBuilder();
            foreach (KeyValuePair<string, ASettingsSection> section in sections)
            {
                // [SectionName]
                result.AppendLine(string.Format("[{0}]", section.Key));
                // For each property in Section, write a line with the format: PropertyName=PropertyValue
                IEnumerable<PropertyInfo> properties = section.Value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    if (!IsSerializableProperty(property))
                        continue;

                    string key = property.Name;

                    if (property.PropertyType.IsArray)
                    {
                        object value = property.GetValue(section.Value, null);
                        string sValue = (value != null) ? StringifyArray(property.PropertyType, (Array)value) : string.Empty;
                        result.AppendLine(string.Format("{0}={1}", key, sValue));
                    }
                    else if (property.PropertyType.IsClass && property.PropertyType != typeof(String))
                    {
                        object value = property.GetValue(section.Value, null);
                        string json = m_Serializer.Serialize(value);
                        result.AppendLine(string.Format("{0}={1}", key, json));
                    }
                    else
                    {
                        object value = property.GetValue(section.Value, null);
                        string sValue = (value != null) ? Convert.ToString(value) : string.Empty;
                        result.AppendLine(string.Format("{0}={1}", key, sValue));
                    }
                }

                // [/SectionName]
                result.AppendLine(string.Format("[/{0}]", section.Key));
                result.AppendLine();
            }

            File.WriteAllText(m_Filename, result.ToString());

            result = null;
        }

        public void Deserialize(Dictionary<string, ASettingsSection> sections)
        {
            if (!File.Exists(m_Filename))
                return;

            Dictionary<string, Dictionary<string, string>> settingSections = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> settingSection = null;
            string currentSectionName = string.Empty;

            string[] content = File.ReadAllLines(m_Filename);

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i] == string.Empty)
                {
                    // empty lines are skipped
                    continue;
                }
                else if (content[i][0] == '#')
                {
                    // # comments are skipped
                    continue;
                }
                else if (content[i].Length >= 3 && content[i][0] == '[' && content[i][content[i].Length - 1] == ']')
                {
                    // [section] or [/section]
                    if (content[i][1] == '/')
                    {
                        string sectionName = content[i].Substring(2, content[i].Length - 3);
                        if (settingSection == null)
                        {
                            throw new Exception(string.Format("Found closing section tag settings file while not in open section. Line:{0}\n{1}",
                                i, content[i]));
                        }
                        else if (currentSectionName != sectionName)
                        {
                            throw new Exception(string.Format("Closing section tag in settings file does not match open section. Line:{0}\n{1}",
                                i, content[i]));
                        }
                        currentSectionName = string.Empty;
                        settingSection = null;
                    }
                    else
                    {
                        string sectionName = content[i].Substring(1, content[i].Length - 2);
                        if (settingSection != null)
                        {
                            throw new Exception(string.Format("Found new section in settings file while in open section. Line:{0}\n{1}",
                                i, content[i]));
                        }
                        settingSection = new Dictionary<string, string>();
                        currentSectionName = sectionName;
                        settingSections.Add(sectionName, settingSection);
                    }
                }
                else
                {
                    string[] line = content[i].Split('=');
                    if (line.Length != 2)
                    {
                        throw new Exception(string.Format("Bad line in settings file. Line:{0}\n{1}",
                                i, content[i]));
                    }
                    if (settingSection == null)
                    {
                        throw new Exception(string.Format("Found settings line in settings file outside of open section. Line:{0}\n{1}",
                                i, content[i]));

                    }
                    if (settingSection.ContainsKey(line[0]))
                    {
                        throw new Exception(string.Format("Found duplicate setting in settings file. Line:{0}\n{1}",
                                i, content[i]));
                    }
                    settingSection.Add(line[0], line[1]);
                }
            }

            foreach (KeyValuePair<string, ASettingsSection> section in sections)
            {
                // get the settings loaded from the file for this section.
                if (!settingSections.TryGetValue(section.Key, out settingSection))
                    continue;

                // For each property in Section, try to find the appropriate settings value.
                IEnumerable<PropertyInfo> properties = section.Value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    if (!IsSerializableProperty(property))
                        continue;

                    string value;
                    if (!settingSection.TryGetValue(property.Name, out value))
                        continue;

                    if (property.PropertyType.IsArray)
                    {
                        Array array;
                        if (TryArrayifyString(property.PropertyType, value, out array))
                        {
                            property.SetValue(section.Value, Convert.ChangeType(array, property.PropertyType), null);
                        }
                    }
                    else if (property.PropertyType.IsClass && property.PropertyType != typeof(String))
                    {
                        object cValue = m_Serializer.Deserialize(value, property.PropertyType);
                        property.SetValue(section.Value, Convert.ChangeType(cValue, property.PropertyType), null);
                    }
                    else
                    {
                        property.SetValue(section.Value, Convert.ChangeType(value, property.PropertyType), null);
                    }
                }
            }
        }

        public string StringifyArray(Type type, Array array)
        {
            string json = m_Serializer.Serialize(array);
            return json;
        }

        public bool TryArrayifyString(Type type, string value, out Array array)
        {
            array = null;

            try
            {
                array = (Array)m_Serializer.Deserialize(value, type);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool IsSerializableProperty(PropertyInfo property)
        {
            // If not writable then cannot null it; if not readable then cannot check it's value
            if (!property.CanWrite || !property.CanRead)
                return false;
            // Get and set methods have to be public
            MethodInfo mget = property.GetGetMethod(false);
            MethodInfo mset = property.GetSetMethod(false);
            if (mget == null)
                return false;
            if (mset == null)
                return false;
            return true;
        }

        public Dictionary<string, ASettingsSection> Unserialize()
        {
            return null;
        }
    }
}
