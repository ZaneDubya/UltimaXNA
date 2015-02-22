using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Display;
using InterXLib.XGUI.Elements;
using Microsoft.Xna.Framework;
using System.Timers;

namespace InterXLib.XGUI.Elements.Views
{
    class TextBoxView : AElementView
    {
        new TextBox Model
        {
            get
            {
                return (TextBox)base.Model;
            }
        }

        public TextBoxView(TextBox model, GUIManager manager)
            : base(model, manager)
        {
            m_CursorTimer = new Timer();
            m_CursorTimer.Elapsed += TimerElapsed;
            model.OnValueChanged += Model_OnValueChanged;
        }

        ~TextBoxView()
        {
            if (m_CursorTimer != null)
            {
                m_CursorTimer.Dispose();
                m_CursorTimer = null;
            }
        }

        protected override void LoadRenderers()
        {

        }

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            spritebatch.GUIClipRect_Push(Model.ScreenArea);

            Color background = new Color(96, 96, 96, 255);
            Color foreground = background * 1.2f;
            Color selected = Color.DarkGray;
            Color border = Color.LightGray;
            Color clrText = Color.White;

            DrawCommon_FillBackground(spritebatch, background);

            // if selection
            //      render selection rect
            spritebatch.DrawString(Font, Model.Value, new Vector2(Model.ScreenArea.X + 2, Model.ScreenArea.Y + 2), Model.FontSize, color: clrText);
            if (m_CursorVisible)
            {
                Vector2 cursor_pos = Vector2.Zero;
                if (Model.CursorPosition > 0)
                    cursor_pos = Font.MeasureString(Model.Value.Substring(0, Model.CursorPosition), Model.FontSize);
                Vector2 cursor_size = Font.MeasureString("|", Model.FontSize);
                spritebatch.DrawString(Font, "|", new Vector2(Model.ScreenArea.X + cursor_pos.X - cursor_size.X, Model.ScreenArea.Y + 2), Model.FontSize, color: clrText);
            }

            DrawCommon_Border(spritebatch, border);

            spritebatch.GUIClipRect_Pop();
        }

        protected override void InternalBeforeDraw()
        {
            if (m_HasInputFocus != Model.HasKeyboardFocus)
            {
                m_HasInputFocus = Model.HasKeyboardFocus;
                if (m_HasInputFocus)
                {
                    StartTimer();
                }
                else
                {
                    StopTimer();
                }
            }
        }

        private void Model_OnValueChanged()
        {
            if (m_HasInputFocus)
                StartTimer();
        }

        /*####################################################################*/
        /*                             Variables                              */
        /*####################################################################*/

        private bool m_HasInputFocus = false;

        /*####################################################################*/
        /*                         Cursor Blinking                            */
        /*####################################################################*/

        private bool m_CursorVisible = false;
        private Timer m_CursorTimer;

        private void ResetTimer()
        {
            m_CursorVisible = true;
            m_CursorTimer.Interval = 500;
        }

        public void StartTimer()
        {
            ResetTimer();
            m_CursorTimer.Start();
        }

        public void StopTimer()
        {
            m_CursorVisible = false;
            m_CursorTimer.Stop();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            m_CursorVisible = !m_CursorVisible;
        }

        /*####################################################################*/
        /*                         Interfaces with Model                      */
        /*####################################################################*/

        public Vector2 MeasureText(string value)
        {
            return Font.MeasureString(value, Model.FontSize);
        }
    }
}