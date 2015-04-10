namespace UltimaXNA.Configuration
{
    public sealed class DebugSettings : SettingsSectionBase
    {
        public const string SectionName = "debug";

        private bool m_IsConsoleEnabled;
        private bool m_ShowFps;
        private bool m_ShowDataRead;
        private bool m_ShowDataReadBreakdown;
        private bool m_ShowUIOutlines;
        private bool m_LogPackets;

        public bool LogPackets
        {
            get { return m_LogPackets; }
            set { SetProperty(ref m_LogPackets, value); }
        }
        
        public bool ShowUIOutlines
        {
            get { return m_ShowUIOutlines; }
            set { SetProperty(ref m_ShowUIOutlines, value); }
        }

        public bool ShowDataReadBreakdown
        {
            get { return m_ShowDataReadBreakdown; }
            set { SetProperty(ref m_ShowDataReadBreakdown, value); }
        }

        public bool ShowDataRead
        {
            get { return m_ShowDataRead; }
            set { SetProperty(ref m_ShowDataRead, value); }
        }

        public bool ShowFps
        {
            get { return m_ShowFps; }
            set { SetProperty(ref m_ShowFps, value); }
        }

        public bool IsConsoleEnabled
        {
            get { return m_IsConsoleEnabled; }
            set { SetProperty(ref m_IsConsoleEnabled, value); }
        }
    }
}