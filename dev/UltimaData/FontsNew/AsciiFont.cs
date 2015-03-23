using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaData.FontsNew
{
    class AsciiFont
    {
        public static string DataPath = "fonts.mul";

        public static void Initialize(GraphicsDevice graphics)
        {
            string path = FileManager.GetFilePath(DataPath);
            if (path != null)
            {
                byte[] buffer;
                int pos = 0;
                using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
                {
                    buffer = reader.ReadBytes((int)reader.BaseStream.Length);
                }

                Diagnostics.Metrics.ReportDataRead(buffer.Length);

                for (int i = 0; i < 10; ++i)
                {
                    m_fonts[i] = new ASCIIFont();

                    byte header = buffer[pos++];

                    for (int k = 0; k < 224; ++k)
                    {
                        int width = buffer[pos++];
                        int height = buffer[pos++];
                        pos++; // byte delimeter?

                        if (width > 0 && height > 0)
                        {
                            if (height > m_fonts[i].Height && k < 96)
                            {
                                m_fonts[i].Height = height;
                            }

                            Texture2D texture = new Texture2D(m_graphicsDevice, width, height, false, SurfaceFormat.Color);
                            Color[] pixels = new Color[width * height];

                            unsafe
                            {
                                fixed (Color* p = pixels)
                                {
                                    for (int y = 0; y < height; ++y)
                                    {
                                        for (int x = 0; x < width; ++x)
                                        {
                                            short pixel = (short)(buffer[pos++] | (buffer[pos++] << 8));
                                            Color color = Color.Transparent;

                                            if (pixel != 0)
                                            {
                                                color = new Color();
                                                color.A = 255;
                                                color.R = (byte)((pixel & 0x7C00) >> 7);
                                                color.G = (byte)((pixel & 0x3E0) >> 2);
                                                color.B = (byte)((pixel & 0x1F) << 3);
                                            }

                                            p[x + y * width] = color;
                                        }
                                    }
                                }
                            }

                            texture.SetData<Color>(pixels);
                            m_fonts[i].Characters[k] = texture;
                        }
                    }
                }
            }
        }
        

        public int LineHeight
        {
            get;
            protected set;
        }

        private AsciiCharacter[] m_Characters;

        public AsciiCharacter this[int index]
        {
            get
            {
                index = (index - 0x20) % 224;
                return m_Characters[index];
            }
        }

        public AsciiCharacter this[char ch]
        {
            get
            {
                int index = ((int)ch - 0x20) % 224;
                return m_Characters[index];
            }
        }

        public AsciiFont()
        {
            LineHeight = 0;
            m_Characters = new AsciiCharacter[224];
        }

        public int GetWidth(string text)
        {
            if (text == null || text.Length == 0)
                return 0;

            int width = 0;
            for (int i = 0; i < text.Length; ++i)
            {
                width += this[text[i]].Width;
            }

            return width;
        }
        }
    }
}
