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
        private bool m_IsVSyncEnabled;
        private bool m_IsFixedTimeStep;

        public GameSettings()
        {
            IsFixedTimeStep = true;
            IsVSyncEnabled = false;
            LastCharacterName = string.Empty;
            AutoSelectLastCharacter = false;
        }

        public bool IsFixedTimeStep
        {
            get { return m_IsFixedTimeStep; }
            set { SetProperty(ref m_IsFixedTimeStep, value); }
        }

        public bool IsVSyncEnabled
        {
            get { return m_IsVSyncEnabled; }
            set { SetProperty(ref m_IsVSyncEnabled, value); }
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