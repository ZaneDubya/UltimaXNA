namespace UltimaXNA.Data
{
    public sealed class DebugSettings : SettingsBase
    {
        private const string Debug = "debug";

        internal DebugSettings(SettingsFile file)
            : base(file)
        {
        }

        protected override string Name
        {
            get { return Debug; }
        }

        public bool IsConsoleEnabled
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }

        public bool ShowFps
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }

        public bool ShowDataRead
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }

        public bool ShowDataReadBreakdown
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }

        public bool ShowUIOutlines
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }
    }
}