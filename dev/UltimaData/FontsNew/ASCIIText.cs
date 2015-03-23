using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaData.FontsNew
{
    public static class ASCIIText
    {
        public const int FontCount = 10;

        private static ASCIIFont[] m_fonts = new ASCIIFont[FontCount];
        private static bool m_initialized;
        private static GraphicsDevice m_graphicsDevice;

        //QUERY: Does this really need to be exposed?
        public static ASCIIFont[] Fonts { get { return ASCIIText.m_fonts; } set { ASCIIText.m_fonts = value; } }

        public static int MaxWidth
        {
            get { return m_graphicsDevice.Viewport.Width; }
        }

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
                        m_fonts[i] = new ASCIIFont();

                        byte header = buffer[pos++];

                        for (int k = 0; k < 224; k++)
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
        }

        private static Dictionary<string, Texture2D> m_TextTextureCache;
        public static Texture2D GetTextTexture(string text, int fontId)
        {
            string hash = string.Format("<font:{0}>{1}", fontId.ToString(), text);

            if (m_TextTextureCache == null)
                m_TextTextureCache = new Dictionary<string, Texture2D>();

            if (!m_TextTextureCache.ContainsKey(hash))
            {
                Texture2D texture = getTexture(text, fontId, 0);
                m_TextTextureCache.Add(hash, texture);
            }
            return m_TextTextureCache[hash];
        }

        public static Texture2D GetTextTexture(string text, int fontId, int wrapwidth)
        {
            string hash = string.Format("<font:{0}:w:{1}>{2}", fontId.ToString(), wrapwidth.ToString(), text);

            if (m_TextTextureCache == null)
                m_TextTextureCache = new Dictionary<string, Texture2D>();

            if (!m_TextTextureCache.ContainsKey(hash))
            {
                Texture2D texture = getTexture(text, fontId, wrapwidth);
                m_TextTextureCache.Add(hash, texture);
            }
            return m_TextTextureCache[hash];
        }

        private unsafe static Texture2D getTexture(string text, int fontId, int wrapwidth)
        {
            ASCIIFont font = ASCIIFont.GetFixed(fontId);

            int width = 0, height = 0;
            if (wrapwidth == 0)
                font.GetTextDimensions(ref text, ref width, ref height, MaxWidth);
            else
                font.GetTextDimensions(ref text, ref width, ref height, wrapwidth);

            if (width == 0) // empty text string
                return new Texture2D(m_graphicsDevice, 1, 1);

            Color[] resultData = new Color[width * height];

            int dx = 0;
            int dy = 0;
            text = text.Replace(Environment.NewLine, "\n");

            unsafe
            {
                fixed (Color* rPtr = resultData)
                {
                    for (int i = 0; i < text.Length; ++i)
                    {
                        if (text.Substring(i, 1) == "\n")
                        {
                            dx = 0;
                            dy += font.Height;
                        }
                        else
                        {
                            Texture2D charTexture = font.GetTexture(text[i]);
                            Color[] charData = new Color[charTexture.Width * charTexture.Height];
                            charTexture.GetData<Color>(charData);

                            fixed (Color* cPtr = charData)
                            {
                                int starty = font.Height - charTexture.Height;
                                int maxHeight = (charTexture.Height < font.Height) ? charTexture.Height : font.Height;
                                for (int iy = 0; iy < maxHeight; ++iy)
                                {
                                    Color* src = ((Color*)cPtr) + (charTexture.Width * iy);
                                    Color* dest = (((Color*)rPtr) + (width * (iy + starty + dy)) + dx);

                                    for (int k = 0; k < charTexture.Width; ++k)
                                        *dest++ = *src++;
                                }
                            }
                            dx += charTexture.Width;
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(m_graphicsDevice, width, height, false, SurfaceFormat.Color);
            result.SetData<Color>(resultData);
            return result;
        }
    }
}
