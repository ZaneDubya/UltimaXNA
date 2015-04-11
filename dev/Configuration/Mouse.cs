using UltimaXNA.Core.ComponentModel;
using UltimaXNA.Core.Input.Windows;

namespace UltimaXNA.Configuration
{
    public class Mouse : NotifyPropertyChangedBase
    {
        private MouseButton m_InteractionButton;
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
            get { return m_InteractionButton; }
            set { SetProperty(ref m_InteractionButton, value); }
        }
    }
}