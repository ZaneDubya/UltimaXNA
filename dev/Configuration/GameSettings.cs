#region Usings

#endregion

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

        public bool AutoSelectLastCharacter
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }

        public string LastCharacterName
        {
            get { return GetValue(string.Empty); }
            set { SetValue(value); }
        }

        public bool AlwaysRun
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }

        public bool IsVSyncEnabled
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }

        public bool IsFixedTimeStep
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }

        public Resolution Resolution
        {
            get { return GetValue(new Resolution(800, 600)); }
            set { SetValue(value); }
        }

        public Mouse Mouse
        {
            get { return GetValue(new Mouse(MouseButton.Left, MouseButton.Right)); }
        }
    }

    public class Mouse : NotifyPropertyChangedBase
    {
        private MouseButton _interactionButton;
        private MouseButton _movementButton;
        private bool _isEnabled;

        public Mouse(MouseButton interaction, MouseButton movement)
        {
            InteractionButton = interaction;
            MovementButton = movement;
            IsEnabled = true;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public MouseButton MovementButton
        {
            get { return _movementButton; }
            set { SetProperty(ref _movementButton, value); }
        }

        public MouseButton InteractionButton
        {
            get { return _interactionButton; }
            set { SetProperty(ref _interactionButton, value); }
        }
    }
}