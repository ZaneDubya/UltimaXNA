﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaXNA.Data;
using UltimaXNA.Diagnostics.Tracing;
using UltimaXNA.Patterns.IoC;

namespace ExamplePlugin
{
    internal sealed class ExamplePluginModule : IModule
    {
        private ExampleSettigs _pluginSettings;

        public string Name
        {
            get { return "UltimaXNA Example Plugin"; }
        }

        public void Load(IContainer container)
        {
            Tracer.Info("Example plugin loaded.");

            _pluginSettings = Settings.OpenSection<ExampleSettigs>();
            _pluginSettings.Boolean = true;
            _pluginSettings.String = "Testing the string value";
            _pluginSettings.Int = 100;
            _pluginSettings.ComplexSettingObject =
                new ComplexSettingObject
                {
                    SomeInt = 1000, SomeString = "This is a string"
                };

        }

        public void Unload(IContainer container)
        {

        }
    }

    internal class ExampleSettigs : SettingsBase
    {
        public ExampleSettigs(SettingsFile file)
            : base(file)
        {
        }

        protected override string Name
        {
            get { return "exampleSettings"; }
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
}