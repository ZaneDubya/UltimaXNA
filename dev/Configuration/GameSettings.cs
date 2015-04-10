#region Usings

using UltimaXNA.Core.Input.Windows;

#endregion

namespace UltimaXNA.Configuration
{
    public sealed class GameSettings : SettingsSectionBase
    {
        public const string SectionName = "game";

        private bool m_AutoSelectLastCharacter;
        private string m_LastCharacterName;
        private bool m_AlwaysRun;
        private bool m_IsVSyncEnabled;
        private Resolution m_Resolution;
        private Mouse m_Mouse;
        private bool m_IsFixedTimeStep;

        public GameSettings()
        {
            Resolution = new Resolution(800, 600);
            Mouse = new Mouse(MouseButton.Left, MouseButton.Right);
        }

        public bool IsFixedTimeStep
        {
            get { return m_IsFixedTimeStep; }
            set { SetProperty(ref m_IsFixedTimeStep, value); }
        }

        public Mouse Mouse
        {
            get { return m_Mouse; }
            set { SetProperty(ref m_Mouse, value); }
        }

        public Resolution Resolution
        {
            get { return m_Resolution; }
            set { SetProperty(ref m_Resolution, value); }
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