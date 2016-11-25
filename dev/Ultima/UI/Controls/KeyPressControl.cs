#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.UI;
using UltimaXNA.Core.Windows;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    internal class KeyPressControl : AControl
    {
        public string LeadingHtmlTag = string.Empty;
        public WinKeys Key = WinKeys.None;
        public bool IsChanged { get; set; }
        private RenderedText m_RenderedText;

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

        public KeyPressControl(AControl parent, int x, int y, int width, int height, int entryID, WinKeys key)
                : base(parent)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            Key = key;
            m_RenderedText = new RenderedText(string.Empty, width);
            IsChanged = false;

            HandlesMouseInput = true;
            HandlesKeyboardFocus = true;
            LeadingHtmlTag = "<center><span color='#fff' style='font-family: uni2;'>";
        }

        public override void Update(double totalMS, double frameMS)
        {
            string text = Key == WinKeys.None ? "Press Any" : Key.ToString();
            m_RenderedText.Text = LeadingHtmlTag + text;

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            int hue = (Key == WinKeys.None) ? 33 : 2;

            if (IsEditable)
            {
                if (m_RenderedText.Width <= Width)
                {
                    m_RenderedText.Draw(spriteBatch, position, Utility.GetHueVector(hue));
                }
                else
                {
                    int textOffset = m_RenderedText.Width - (Width);
                    m_RenderedText.Draw(spriteBatch, new Rectangle(position.X, position.Y, m_RenderedText.Width - textOffset, m_RenderedText.Height), textOffset, 0, Utility.GetHueVector(hue));
                }
            }
            else
            {
                m_RenderedText.Draw(spriteBatch, new Rectangle(position.X, position.Y, Width, Height), 0, 0, Utility.GetHueVector(hue));
            }

            base.Draw(spriteBatch, position, frameMS);
        }

        protected override void OnKeyboardInput(InputEventKeyboard e)
        {
            if (e.Alt || e.Shift || e.Control || IsChanged)
                return;

            if (((int)e.KeyCode >= (int)WinKeys.A && (int)e.KeyCode <= (int)WinKeys.Z) ||
                ((int)e.KeyCode >= (int)WinKeys.F1 && (int)e.KeyCode <= (int)WinKeys.F12))
            {
                Key = e.KeyCode;
            }
            else if ((int)e.KeyCode >= (int)WinKeys.A + 32 && (int)e.KeyCode <= (int)WinKeys.Z + 32) // lower case?
            {
                if (e.KeyCode.ToString().StartsWith("NumPad"))
                    Key = e.KeyCode;
                else if (e.KeyCode.ToString().Length == 1)
                    Key = e.KeyCode;
            }
            else if (e.KeyCode >= WinKeys.D0 && e.KeyCode <= WinKeys.D9)
            {
                Key = e.KeyCode;
            }
            else if (e.KeyCode == WinKeys.NumPad0)//interesting :)
            {
                Key = e.KeyCode;
            }
            else
            {
                return;
            }
            IsChanged = true;
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            IsChanged = false;
            Key = WinKeys.None;
        }
    }
}