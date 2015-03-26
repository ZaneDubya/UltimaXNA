using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using UltimaXNA.UltimaData.FontsNew;
using UltimaXNA.Core.Diagnostics;

namespace UltimaXNA.UltimaData.FontsUnused
{
    public static class TextASCII
    {
        public const int FontCount = 10;
        private static FontASCII[] m_fonts = new FontASCII[FontCount];

        private static bool m_initialized;
        private static GraphicsDevice m_graphicsDevice;

        static TextASCII()
        {

        }

        internal static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (!m_initialized)
            {
                m_initialized = true;
                m_graphicsDevice = graphicsDevice;

                string path = FileManager.GetFilePath("fonts.mul");
                InternalLoadFonts(path);
            }
        }

        private static void InternalLoadFonts(string path)
        {
            if (path != null)
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
                {
                    for (int i = 0; i < FontCount; i++)
                    {
                        m_fonts[i] = new FontASCII(m_graphicsDevice, reader);
                    }
                    Metrics.ReportDataRead((int)reader.BaseStream.Length);
                }
            }
        }

        /*internal static DisplayString DisplayText(string text, int fontId, int wrapWidth)
        {
            FontASCII font = InternalGetFont(fontId);

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
        }*/

        private static FontASCII InternalGetFont(int font)
        {
            if (font < 0 || font > 9)
            {
                return m_fonts[3]; // default font is index 3.
            }

            return m_fonts[font];
        }
    }
}
