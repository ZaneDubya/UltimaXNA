using UltimaXNA.Configuration;
using UltimaXNA.Core.ComponentModel;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Patterns.IoC;

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

            _pluginSettings = Settings.CreateOrOpenSection<ExampleSettigs>(ExampleSettigs.SectionName);
            _pluginSettings.Boolean = true;
            _pluginSettings.String = "Testing the string value";
            _pluginSettings.Int = 100;
            _pluginSettings.ComplexObject =
                new ComplexSettingObject
                {
                    SomeInt = 1000, SomeString = "This is a string"
                };
        }

        public void Unload(IContainer container)
        {
        }
    }

    internal class ExampleSettigs : SettingsSectionBase
    {
        public const string SectionName = "examplePlugin";

        private bool m_Boolean;
        private string m_String;
        private int m_Int;
        private ComplexSettingObject m_ComplexObject;

        public int Int
        {
            get { return m_Int; }
            set { SetProperty(ref m_Int, value); }
        }

        public string String
        {
            get { return m_String; }
            set { SetProperty(ref m_String, value); }
        }

        public bool Boolean
        {
            get { return m_Boolean; }
            set { SetProperty(ref m_Boolean, value); }
        }

        public ComplexSettingObject ComplexObject
        {
            get { return m_ComplexObject; }
            set { SetProperty(ref m_ComplexObject, value); }
        }
    }

    public class ComplexSettingObject : NotifyPropertyChangedBase
    {
        private string _SomeString;
        private int _SomeInt;

        public ComplexSettingObject()
        {
            SomeString = "SomeString";
            SomeInt = 50;
        }

        public int SomeInt
        {
            get { return _SomeInt; }
            set { SetProperty(ref _SomeInt, value); }
        }

        public string SomeString
        {
            get { return _SomeString; }
            set { SetProperty(ref _SomeString, value); }
        }
    }
}