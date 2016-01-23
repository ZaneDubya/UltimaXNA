#region usings
using Microsoft.Xna.Framework;
using System;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.UI;
using UltimaXNA.Core.Windows;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    internal class KeyPressControl : AControl
    {
        public int Hue = 0;
        public int EntryID = 0;
        public int LimitSize = 0;
        public bool IsPasswordField = false;
        public string LeadingHtmlTag = string.Empty;
        public string LeadingText = string.Empty;
        private string _text = "";

        public string Text { get { return _text; } set { _text = value; } }

        public override bool HandlesMouseInput
        {
            get { return base.HandlesMouseInput & IsEditable; }
            set
            {
                base.HandlesMouseInput = value;
            }
        }

        public override bool HandlesKeyboardFocus
        {
            get { return base.HandlesKeyboardFocus & IsEditable; }
            set
            {
                base.HandlesKeyboardFocus = value;
            }
        }

        private bool m_IsFocused = false;
        public bool isChanged { get; set; }
        private RenderedText m_RenderedText;

        public KeyPressControl(AControl parent)
                : base(parent)
        {
            HandlesMouseInput = true;
            HandlesKeyboardFocus = true;
        }

        public KeyPressControl(AControl parent, string[] arguements, string[] lines)
                : this(parent)
        {
            int x, y, width, height, hue, entryID, textIndex;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            hue = ServerRecievedHueTransform(Int32.Parse(arguements[5]));
            LeadingHtmlTag = "<basefont color=#000000><big>";
            entryID = Int32.Parse(arguements[6]);
            textIndex = Int32.Parse(arguements[7]);
            buildGumpling(x, y, width, height, entryID);
        }

        public KeyPressControl(AControl parent, int x, int y, int width, int height, int entryID)
                : this(parent)
        {
            buildGumpling(x, y, width, height, entryID);
        }

        private void buildGumpling(int x, int y, int width, int height, int entryID)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            EntryID = entryID;
            Text = "Any Press";
            LimitSize = 10;
            m_RenderedText = new RenderedText(string.Empty, 2048, true);
            isChanged = false;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (UserInterface.KeyboardFocusControl == this)
            {
                // if we're not already focused, turn the carat on immediately.
                // if we're using the legacy carat, keep it visible. Else blink it every x seconds.
                if (!m_IsFocused)
                {
                    m_IsFocused = true;
                }
            }
            else
            {
                m_IsFocused = false;
            }
            if (Text == "Any Press")
                Hue = 33;
            else
                Hue = 2;
            m_RenderedText.Text = LeadingHtmlTag + LeadingText + (IsPasswordField ? new string('*', Text.Length) : Text);

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            if (IsEditable)
            {
                if (m_RenderedText.Width <= Width)
                {
                    m_RenderedText.Draw(spriteBatch, position, Utility.GetHueVector(Hue));
                }
                else
                {
                    int textOffset = m_RenderedText.Width - (Width);
                    m_RenderedText.Draw(spriteBatch, new Rectangle(position.X, position.Y, m_RenderedText.Width - textOffset, m_RenderedText.Height), textOffset, 0, Utility.GetHueVector(Hue));
                }
            }
            else
            {
                m_RenderedText.Draw(spriteBatch, new Rectangle(position.X, position.Y, Width, Height), 0, 0, Utility.GetHueVector(Hue));
            }

            base.Draw(spriteBatch, position);
        }

        protected override void OnKeyboardInput(InputEventKeyboard e)
        {
            if (e.Alt || e.Shift || e.Control || isChanged)
                return;

            if (((int)e.KeyCode >= (int)WinKeys.A && (int)e.KeyCode <= (int)WinKeys.Z) ||
                ((int)e.KeyCode >= (int)WinKeys.F1 && (int)e.KeyCode <= (int)WinKeys.F12))
            {
                Text = e.KeyCode.ToString();
            }
            else if ((int)e.KeyCode >= (int)WinKeys.A + 32 && (int)e.KeyCode <= (int)WinKeys.Z + 32)
            {
                if (e.KeyCode.ToString().StartsWith("NumPad"))
                    Text = e.KeyCode.ToString();
                else if (e.KeyCode.ToString().Length == 1)
                    Text = e.KeyCode.ToString();
            }
            else if ((int)e.KeyCode >= (int)WinKeys.D0 && (int)e.KeyCode <= (int)WinKeys.D9)
            {
                if (char.IsDigit(e.KeyChar))
                    Text = e.KeyChar.ToString();
            }
            else if (e.KeyCode == WinKeys.NumPad0)//interesting :)
            {
                Text = e.KeyCode.ToString();
            }
            else
            {
                return;
            }
            isChanged = true;
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            isChanged = false;
            Text = "Any Press";
        }
    }
}