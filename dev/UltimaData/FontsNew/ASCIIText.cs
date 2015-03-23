using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace UltimaXNA.UltimaData.FontsNew
{
    public static class ASCIIText
    {
        public const int FontCount = 10;
        private static ASCIIFont[] m_fonts = new ASCIIFont[FontCount];

        private static bool m_initialized;
        private static GraphicsDevice m_graphicsDevice;

        static ASCIIText()
        {

        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (!m_initialized)
            {
                m_initialized = true;
                string path = FileManager.GetFilePath("fonts.mul");
                m_graphicsDevice = graphicsDevice;
                if (path != null)
                {
                    byte[] buffer;
                    int pos = 0;

                    using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
                    {
                        buffer = reader.ReadBytes((int)reader.BaseStream.Length);
                        Diagnostics.Metrics.ReportDataRead(buffer.Length);
                    }

                    for (int i = 0; i < FontCount; i++)
                    {
                        m_fonts[i] = new ASCIIFont(m_graphicsDevice, buffer, ref pos);
                    }
                }
            }
        }

        public static DisplayString DisplayText(string text, int fontId, int wrapWidth)
        {
            ASCIIFont font = InternalGetFont(fontId);

            int width = 0, height = 0;
            font.GetTextDimensions(ref text, ref width, ref height, wrapWidth);

            if (width == 0) // empty text string
                return null;

            DisplayString display = new DisplayString(font.Texture);

            int dx = 0;
            int dy = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == '\n')
                {
                    dx = 0;
                    dy += font.Height;
                }
                else if ((int)ch < 32)
                {
                    // skip this character.
                }
                else
                {
                    ACharacter character = font.GetCharacter(ch);
                    display.AddCharacter(character, new Point(dx, dy), 0);
                    dx += character.Width;
                }
            }
            return display;
        }

        private static ASCIIFont InternalGetFont(int font)
        {
            if (font < 0 || font > 9)
            {
                return m_fonts[3]; // default font is index 3.
            }

            return m_fonts[font];
        }
    }
}
