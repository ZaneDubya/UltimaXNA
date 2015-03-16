using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.XGUI.Elements;

namespace InterXLib.XGUI.Support
{
    public class RadioButtonGroup
    {
        private List<RadioButton> m_Buttons = new List<RadioButton>();
        private bool m_AllowNoneClicked = false;
        private bool m_AllowMultipleClicked = false;

        public RadioButtonGroup(bool allow_none_clicked = false, bool allow_multiple_clicked = false)
        {
            m_AllowNoneClicked = allow_none_clicked;
            m_AllowMultipleClicked = allow_multiple_clicked;
        }

        private static List<RadioButton> s_ClickedButtons = new List<RadioButton>();
        public RadioButton[] GetClickedButton()
        {
            s_ClickedButtons.Clear();

            foreach (RadioButton button in m_Buttons)
            {
                if (button.IsDown)
                {
                    s_ClickedButtons.Add(button);
                }
            }

            if (s_ClickedButtons.Count == 0)
                return null;
            else
                return s_ClickedButtons.ToArray();
        }

        internal void AddButton(RadioButton button)
        {
            if (!m_Buttons.Contains(button))
                m_Buttons.Add(button);
            if (!m_AllowNoneClicked && GetClickedButton() == null)
                button.IsDown = true;
        }

        internal void RemoveButton(RadioButton button)
        {
            if (m_Buttons.Contains(button))
                m_Buttons.Remove(button);
            if (!m_AllowNoneClicked && GetClickedButton() == null && m_Buttons.Count > 0)
                m_Buttons[0].IsDown = true;
        }

        internal void ButtonClicked(RadioButton button)
        {
            button.IsDown = !button.IsDown;
            if (!m_AllowNoneClicked && GetClickedButton() == null)
                button.IsDown = true;

            if (!m_AllowMultipleClicked)
            {
                foreach (RadioButton b in m_Buttons)
                {
                    b.IsDown = (b == button);
                }
            }
        }
    }
}
