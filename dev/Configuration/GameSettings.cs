#region Usings

using UltimaXNA.ComponentModel;
using UltimaXNA.Input.Windows;

#endregion

namespace UltimaXNA.Data
{
    public sealed class GameSettings : SettingsBase
    {
        private const string Game = "game";

        internal GameSettings(SettingsFile file)
            : base(file)
        {
        }
        
        protected override string Name
        {
            get { return Game; }
        }

        protected override string Comments
        {
            get { return "This section is responsible for all game engine related settings"; }
        }

        public bool AutoSelectLastCharacter
        {
            get { return GetValue(false); }
            set { SetValue(value, "Specifies wether the game will attempt to login to the last character you played"); }
        }

        public string LastCharacterName
        {
            get { return GetValue(string.Empty); }
            set { SetValue(value, "The name of the last character you logged in with"); }
        }

        public bool AlwaysRun
        {
            get { return GetValue(false); }
            set { SetValue(value, "Never walk, always run"); }
        }

        public bool IsVSyncEnabled
        {
            get { return GetValue(false); }
            set { SetValue(value, "Vertical synchronization is an option in most systems, wherein the video card is prevented from doing anything visible to the display memory until after the monitor finishes its current refresh cycle."); }
        }

        public bool IsFixedTimeStep
        {
            get { return GetValue(false); }
            set { SetValue(value, "In a fixed-step game loop, Game calls Update once the TargetElapsedTime has elapsed. After Update is called, if it is not time to call Update again, Game calls Draw. After Draw is called, if it is not time to call Update again, Game idles until it is time to call Update."); }
        }

        public Resolution Resolution
        {
            get { return GetValue(new Resolution(800, 600)); }
            set { SetValue(value, "Resolution the game will run at"); }
        }

        public Mouse Mouse
        {
            get { return GetValue(new Mouse(MouseButton.Left, MouseButton.Right), "Defines what each mouse button does, and if the mouse is enabled."); }
        }
    }

    public class Mouse : NotifyPropertyChangedBase
    {
        private MouseButton m_interactionButton;
        private MouseButton m_movementButton;
        private bool m_isEnabled;

        public Mouse(MouseButton interaction, MouseButton movement)
        {
            InteractionButton = interaction;
            MovementButton = movement;
            IsEnabled = true;
        }

        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set { SetProperty(ref m_isEnabled, value); }
        }

        public MouseButton MovementButton
        {
            get { return m_movementButton; }
            set { SetProperty(ref m_movementButton, value); }
        }

        public MouseButton InteractionButton
        {
            get { return m_interactionButton; }
            set { SetProperty(ref m_interactionButton, value); }
        }
    }
}