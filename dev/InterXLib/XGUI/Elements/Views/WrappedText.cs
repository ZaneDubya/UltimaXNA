using InterXLib.Display;
using InterXLib.XGUI.Elements;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements.Views
{
    class WrappedText
    {
        private string m_InternalCaption = null;
        private string m_InternalCaptionWithLineBreaks = null;

        public void Draw(YSpriteBatch spritebatch, YSpriteFont font, Rectangle area, string text, float font_size, FontJustification justification, Color color)
        {
            if (text != null && text != string.Empty)
            {
                // if (m_InternalCaption != text)
                {
                    m_InternalCaption = text;
                    m_InternalCaptionWithLineBreaks = font.BreakStringIntoLines(m_InternalCaption, area.Width, font_size);
                }
                spritebatch.DrawString(font, m_InternalCaptionWithLineBreaks, new Vector2(area.X, area.Y), font_size, new Vector2(area.Width, area.Height), color: color, justification: justification);
            }
        }
    }
}
