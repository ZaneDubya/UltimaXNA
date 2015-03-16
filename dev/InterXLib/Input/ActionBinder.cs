using InterXLib.Input.Windows;
using System.Collections.Generic;

namespace InterXLib.Input
{
    public class ActionBinder
    {
        private List<ActionBinding> m_Actions;

        internal ActionBinder()
        {
            m_Actions = new List<ActionBinding>();
        }

        public void AddBinding(BindingDevice device, int button_index, int action)
        {
            m_Actions.Add(new ActionBinding(device, button_index, action));
        }

        internal void Update(float frame_time)
        {
            for (int i = 0; i < m_Actions.Count; i++)
                m_Actions[i].Update(frame_time);
        }

        public bool IsPress(int action)
        {
            for (int i = 0; i < m_Actions.Count; i++)
                if (m_Actions[i].Action == action && m_Actions[i].State == ActionState.Press)
                    return true;
            return false;
        }

        public bool IsRelease(int action)
        {
            for (int i = 0; i < m_Actions.Count; i++)
                if (m_Actions[i].Action == action && m_Actions[i].State == ActionState.Release)
                    return true;
            return false;
        }

        public bool IsDown(int action)
        {
            for (int i = 0; i < m_Actions.Count; i++)
                if (m_Actions[i].Action == action && 
                    (m_Actions[i].State & ActionState.Down) == ActionState.Down)
                    return true;
            return false;
        }

        public bool IsUp(int action)
        {
            for (int i = 0; i < m_Actions.Count; i++)
                if (m_Actions[i].Action == action &&
                    (m_Actions[i].State & ActionState.Down) == ActionState.Up)
                    return true;
            return false;
        }

        internal void ReceiveKeyboardInput(List<InputEventKeyboard> events)
        {
            foreach (InputEventKeyboard e in events)
            {
                for (int i = 0; i < m_Actions.Count; i++)
                {
                    if (m_Actions[i].MatchesInput(BindingDevice.Keyboard, e.KeyCodeInt))
                    {
                        m_Actions[i].ReceiveKeyboardEvent(e.EventType);
                        e.Handled = true;
                    }
                }
            }
        }

        private class ActionBinding
        {
            private int m_Action;
            internal int Action
            {
                get { return m_Action; }
            }

            private ActionState m_State = ActionState.Up;
            internal ActionState State
            {
                get { return m_State; }
            }

            private BindingDevice m_Device;
            internal BindingDevice Device
            {
                get { return m_Device; }
            }

            private int m_ButtonIndex;
            internal int ButtonIndex
            {
                get { return m_ButtonIndex; }
            }

            private bool m_InInitialRepeatPeriod = false;
            private float m_InitialRepeatSeconds = 1.0f;
            private float m_RepeatSeconds = 0.4f;
            private float m_SecondsSincePress = 0f;

            internal ActionBinding(BindingDevice device, int button_index, int action)
            {
                m_Device = device;
                m_ButtonIndex = button_index;
                m_Action = action;
            }

            internal bool MatchesInput(BindingDevice device, int button_index)
            {
                if (device == m_Device && button_index == m_ButtonIndex)
                    return true;
                else
                    return false;
            }

            internal void Update(float frame_time)
            {
                if (m_State == ActionState.Down)
                {
                    m_SecondsSincePress += frame_time;
                    if (m_InInitialRepeatPeriod)
                    {
                        if (m_SecondsSincePress >= m_InitialRepeatSeconds)
                        {
                            m_InInitialRepeatPeriod = false;
                            m_SecondsSincePress -= m_InitialRepeatSeconds;
                            m_State = ActionState.Press;
                        }
                    }
                    else if (m_SecondsSincePress >= m_RepeatSeconds)
                    {
                        m_SecondsSincePress -= m_RepeatSeconds;
                        m_State = ActionState.Press;
                    }
                }
                else if (m_State == ActionState.Release)
                {
                    m_State = ActionState.Up;
                }
                else if (m_State == ActionState.Press)
                {
                    m_State = ActionState.Down;
                    m_SecondsSincePress = 0f;
                }
            }

            internal void ReceiveKeyboardEvent(KeyboardEventType ke)
            {
                switch (ke)
                {
                    case KeyboardEventType.Down:
                        m_InInitialRepeatPeriod = true;
                        m_State = ActionState.Press;
                        m_SecondsSincePress = 0f;
                        break;
                    case KeyboardEventType.Up:
                        m_State = ActionState.Release;
                        break;
                    // ignore press events - we handle our own presses.
                }
            }

            public override string ToString()
            {
                return string.Format("{0} {1} {2}", m_Device.ToString(), m_ButtonIndex, m_State.ToString());
            }
        }
    }

    public enum BindingDevice
    {
        None,
        Keyboard,
        Gamepad
    }

    public enum ActionState
    {
        Up = 0x01,
        Down = 0x02,
        Press = 0x06, // 0x04 | 0x02
        Release = 0x09, // 0x08 | 0x01
    }
}
