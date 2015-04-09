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

        protected override string Comments
        {
            get
            {
                return @"This section is responsible for all debugging related settings.  " +
                       @"These are not needed unless you are a developer.  " +
                       @"Changing these settings may affect performance";
            }
        }

        public bool IsConsoleEnabled
        {
            get { return GetValue(false); }
            set { SetValue(value, "Allocates a console along side of the client to help with debugging"); }
        }

        public bool ShowFps
        {
            get { return GetValue(false); }
            set { SetValue(value, "Turns on the FPS counter"); }
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