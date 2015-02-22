using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using InterXLib.Display;
using InterXLib.Input.Windows;
using System.Collections.Generic;
using System.Text;

namespace InterXLib.XGUI.Elements
{
    public class TextBox : AElement
    {
        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.TextBoxView(this, Manager);
        }

        protected override Patterns.MVC.AController CreateController()
        {
            return new Controllers.TextBoxController(this, Manager);
        }

        public TextBox(AElement parent, int page)
            : base(parent, page)
        {
            HandlesMouseInput = true;
            HandlesKeyboardInput = true;
        }

        /*####################################################################*/
        /*                             Variables                              */
        /*####################################################################*/

        private int m_CursorPosition;
        private int? m_SelectCursorPosition;

        private int m_MaxLength = 40;
        private List<char> m_Contents = new List<char>();

        /// <summary>
        /// The current location of the cursor in the array
        /// </summary>
        public int CursorPosition
        {
            get
            {
                return m_CursorPosition;
            }
            set
            {
                m_CursorPosition = Clamp(value, 0, Length);
            }
        }

        /// <summary>
        /// All characters between SelectCursorPosition and CursorPosition are selected 
        /// when SelectCursorPosition != null. Cannot be the same value as CursorPosition.
        /// </summary>
        public int? SelectCursorPosition
        {
            get
            {
                return m_SelectCursorPosition;
            }
            set
            {
                if (value.HasValue)
                {
                    if (value.Value != CursorPosition)
                    {
                        m_SelectCursorPosition = Clamp(value.Value, 0, Length);
                    }
                }
                else
                {
                    m_SelectCursorPosition = null;
                }
            }
        }

        /// <summary>
        /// Current number of characters in this TextBox.
        /// </summary>
        public int Length
        {
            get { return m_Contents.Count; }
        }

        /// <summary>
        /// Maximum number of character in the textbox.
        /// </summary>
        public int MaxLength { get { return m_MaxLength; } }

        /// <summary>
        /// The current characters in the textbox in string format
        /// </summary>
        public string Value
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < m_Contents.Count; i++)
                    sb.Append(m_Contents[i]);
                return sb.ToString();
            }
            set
            {
                if (value != Value)
                {
                    SetText(value);
                }
            }
        }

        /// <summary>
        /// Returns true when there is selected text
        /// </summary>
        public bool HasSelected { get { return SelectCursorPosition.HasValue && SelectCursorPosition.Value != CursorPosition; } }

        /*####################################################################*/
        /*                         Value Alteration                           */
        /*####################################################################*/

        /// <summary>
        /// Replace the text in the textbox with the specified string.
        /// </summary>
        /// <param name="value">A string that overrides the current text of the textbox.</param>
        public void SetText(string value)
        {
            if (value == null)
                value = string.Empty;
            int x = value.IndexOf('\0');
            if (x != -1) { value = value.Substring(0, x); }

            m_Contents.Clear();
            for (int i = 0; i < value.Length; i++)
                m_Contents.Add(value[i]);

            CursorPosition = Length;
            SelectCursorPosition = null;
            InternalOnValueChanged();
        }

        /// <summary>
        /// Insert an individual character after the cursor then move the 
        /// cursor forward one.
        /// </summary>
        /// <param name="character">The character to insert. Will not be inserted if the 
        /// textbox is full or if the character is not supported by the current font.</param>
        public void Insert(char character)
        {
            if ((short)character < 32 && (short)character > 255)
                return;
            if (!(Length < MaxLength))
                return;

            //Shift everything right once then insert the character into the gap
            m_Contents.Insert(CursorPosition, character);

            //Cursor needs to shift right
            CursorPosition++;
            InternalOnValueChanged();
        }

        /// <summary>
        /// Insert a string into the textbox at the cursor's location.
        /// </summary>
        /// <param name="value">The string to insert.</param>
        public void Insert(string value)
        {
            foreach (var character in value)
            {
                Insert(character);
            }
        }

        /// <summary>
        /// Remove the character to the right of the cursor. If the cursor is after 
        /// the last character do nothing.
        /// </summary>
        public void Delete()
        {
            if (HasSelected)
            {
                RemoveSelected();
                return;
            }

            if (Length == 0 || CursorPosition == Length) { return; }

            int start = CursorPosition + 1;
            m_Contents.RemoveAt(CursorPosition);
            InternalOnValueChanged();
        }

        /// <summary>
        /// Remove the character before the cursor. If the cursor is before the 
        /// first character do nothing.
        /// </summary>
        public void BackSpace()
        {
            if (CursorPosition == 0)
                return;
            CursorPosition--;
            Delete();
        }

        /// <summary>
        /// Removes the currently highlighted text.
        /// </summary>
        public void RemoveSelected()
        {
            if (!HasSelected)
                return;

            Point range = GetSelectionRange();

            Value = Value.Remove(range.X, range.Y - range.X);
            CursorPosition = range.X;

            SelectCursorPosition = null;

            if (CursorPosition >= range.Y && CursorPosition <= range.Y)
            {
                CursorPosition = range.X;
            }
            InternalOnValueChanged();
        }

        /*####################################################################*/
        /*                             Selection                              */
        /*####################################################################*/

        public void SetSelection(Point local_location)
        {
            SelectCursorPosition = CharAt(local_location);
        }

        public void SetTextCursor(Point local_location)
        {
            CursorPosition = CharAt(local_location);
        }

        public int CharAt(Point local_location)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Length; i++)
            {
                sb.Append(m_Contents[i]);
                //Rectangle that encompasses the current character

                Vector2 str_size = ((Views.TextBoxView)GetView()).MeasureText(sb.ToString());
                if (local_location.X <= str_size.X)
                    return i;
            }

            //Missed a character so return the end.
            return Length;
        }

        public Point GetSelectionRange()
        {
            if (!SelectCursorPosition.HasValue)
                return Point.Zero;
            var selected = SelectCursorPosition.Value;

            return new Point(
                (selected < CursorPosition) ? selected : CursorPosition,
                (selected < CursorPosition) ? CursorPosition : selected);
        }

        public string GetSelected()
        {
            if (!HasSelected)
                return String.Empty;

            Point range = GetSelectionRange();

            return Value.Substring(range.X, range.Y - range.X);
        }

        /*####################################################################*/
        /*                              Helpers                               */
        /*####################################################################*/

        private static int Clamp(int value, int min, int max)
        {
            if (value > max)
                return max;
            else if (value < min)
                return min;
            else
                return value;
        }

        /*####################################################################*/
        /*                               Events                               */
        /*####################################################################*/

        public XGUIAction OnValueChanged;
        private void InternalOnValueChanged()
        {
            if (OnValueChanged != null)
                OnValueChanged();
        }
    }
}