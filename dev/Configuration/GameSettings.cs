#region Usings
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Configuration;
#endregion

namespace UltimaXNA.Configuration
{
    public sealed class GameSettings : ASettingsSection
    {
        public const string SectionName = "game";

        private bool m_AutoSelectLastCharacter;
        private string m_LastCharacterName;
        private bool m_AlwaysRun;
        private bool m_IsVSyncEnabled;
        private Resolution m_WindowResolution;
        private Resolution m_WorldGumpResolution;
        private bool m_IsFullScreen;
        private Mouse m_Mouse;
        private bool m_IsFixedTimeStep;

        public GameSettings()
        {
            WindowResolution = new Resolution(800, 600);
            WorldGumpResolution = new Resolution(640, 480);
            IsFullScreen = false;
            Mouse = new Mouse(MouseButton.Left, MouseButton.Right);
            IsFixedTimeStep = true;
        }

        public bool IsFixedTimeStep
        {
            get { return m_IsFixedTimeStep; }
            set { SetProperty(ref m_IsFixedTimeStep, value); }
        }

        public bool IsFullScreen
        {
            get { return m_IsFullScreen; }
            set { SetProperty(ref m_IsFullScreen, value); }
        }

        public Mouse Mouse
        {
            get { return m_Mouse; }
            set { SetProperty(ref m_Mouse, value); }
        }

        public Resolution WindowResolution
        {
            get { return m_WindowResolution; }
            set { SetProperty(ref m_WindowResolution, value); }
        }

        public Resolution WorldGumpResolution
        {
            get { return m_WorldGumpResolution; }
            set { SetProperty(ref m_WorldGumpResolution, value); }
        }

        public bool IsVSyncEnabled
        {
            get { return m_IsVSyncEnabled; }
            set { SetProperty(ref m_IsVSyncEnabled, value); }
        }

        public bool AlwaysRun
        {
            get { return m_AlwaysRun; }
            set { SetProperty(ref m_AlwaysRun, value); }
        }

        public string LastCharacterName
        {
            get { return m_LastCharacterName; }
            set { SetProperty(ref m_LastCharacterName, value); }
        }

        public bool AutoSelectLastCharacter
        {
            get { return m_AutoSelectLastCharacter; }
            set { SetProperty(ref m_AutoSelectLastCharacter, value); }
        }
    }
}