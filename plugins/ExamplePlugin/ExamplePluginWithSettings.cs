/***************************************************************************
 *   ExamplePluginWithSettings.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

/*namespace ExamplePlugin
{
    internal sealed class ExamplePluginModule : IModule
    {
        // private ExampleSettigs m_PluginSettings;

        public string Name
        {
            get { return "UltimaXNA Example Plugin"; }
        }

        public void Load()
        {
            Tracer.Info("ExamplePlugin loaded.");

            m_PluginSettings = Settings.OpenSection<ExampleSettigs>();
            m_PluginSettings.Boolean = true;
            m_PluginSettings.String = "Testing the string value";
            m_PluginSettings.Int = 100;
            m_PluginSettings.ComplexSettingObject =
                new ComplexSettingObject
                {
                    SomeInt = 1000, SomeString = "This is a string"
                };
        }

        public void Unload()
        {

        }
    }

    internal class ExampleSettigs : SettingsSectionBase
    {
        public ExampleSettigs(SettingsFile file)
            : base(file)
        {
        }

        public bool Boolean
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }

        public string String
        {
            get { return GetValue("test"); }
            set { SetValue(value); }
        }

        public int Int
        {
            get { return GetValue(10); }
            set { SetValue(value); }
        }

        public ComplexSettingObject ComplexSettingObject
        {
            get { return GetValue(new ComplexSettingObject()); }
            set { SetValue(value); }
        }
    }

    public class ComplexSettingObject
    {
        public ComplexSettingObject()
        {
            SomeString = "SomeString";
            SomeInt = 50;
        }

        public string SomeString
        {
            get;
            set;
        }


        public int SomeInt
        {
            get;
            set;
        }
    }
}*/
