using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Data
{
    //QUERY: Does this really need to be exposed? Shouldnt this be a child class of ASCIIText?
    public sealed class ASCIIFont
    {
        private int m_Height;
        private Texture2D[] m_Characters;

        public int Height { get { return m_Height; } set { m_Height = value; } }
        public Texture2D[] Characters { get { return m_Characters; } set { m_Characters = value; } }

        public ASCIIFont()
        {
            Height = 0;
            Characters = new Texture2D[224];
        }

        public Texture2D GetTexture(char character)
        {
            return m_Characters[(((((int)character) - 0x20) & 0x7FFFFFFF) % 224)];
        }

        public int GetWidth(char ch)
        {
            return GetTexture(ch).Width;
        }

        public int GetWidth(string text)
        {
            if (text == null || text.Length == 0) { return 0; }

            int width = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                width += GetTexture(text[i]).Width;
            }

            return width;
        }

        public void GetTextDimensions(string text, ref int width, ref int height)
        {
            width = 0;
            height = Height;
            int maxwidth = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                if ((i < text.Length - 1) && (text.Substring(i, 2) == Environment.NewLine))
                {
                    if (width > maxwidth)
                        maxwidth = width;
                    height += Height;
                    width = 0;
                    i++;
                }
                else
                {
                    width += GetTexture(text[i]).Width;
                }
            }

            if (maxwidth > width)
                width = maxwidth;
        }

        public static ASCIIFont GetFixed(int font)
        {
            if (font < 0 || font > 9)
            {
                return ASCIIText.Fonts[3];
            }

            return ASCIIText.Fonts[font];
        }
    }

    public static class ASCIIText
    {
        private static ASCIIFont[] _fonts = new ASCIIFont[10];
        private static bool _initialized;
        private static GraphicsDevice _graphicsDevice;

        //QUERY: Does this really need to be exposed?
        public static ASCIIFont[] Fonts { get { return ASCIIText._fonts; } set { ASCIIText._fonts = value; } }

        static ASCIIText()
        {

        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (!_initialized)
            {
                _initialized = true;
                string path = FileManager.GetFilePath("fonts.mul");
                _graphicsDevice = graphicsDevice;
                if (path != null)
                {
                    using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
                    {
                        for (int i = 0; i < 10; ++i)
                        {
                            _fonts[i] = new ASCIIFont();

                            byte header = reader.ReadByte();

                            for (int k = 0; k < 224; ++k)
                            {
                                byte width = reader.ReadByte();
                                byte height = reader.ReadByte();
                                reader.ReadByte(); // delimeter?

                                if (width > 0 && height > 0)
                                {
                                    if (height > _fonts[i].Height && k < 96)
                                    {
                                        _fonts[i].Height = height;
                                    }

                                    Texture2D texture = new Texture2D(_graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);
                                    Color[] pixels = new Color[width * height];

                                    unsafe
                                    {
                                        fixed (Color* p = pixels)
                                        {
                                            for (int y = 0; y < height; ++y)
                                            {
                                                for (int x = 0; x < width; ++x)
                                                {
                                                    short pixel = (short)(reader.ReadByte() | (reader.ReadByte() << 8));
                                                    Color color = Color.TransparentBlack;

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
                                    _fonts[i].Characters[k] = texture;
                                }
                            }
                        }
                    }
                }
            }
        }

        public unsafe static Texture2D GetTexture(int fontId, string text)
        {
            ASCIIFont font = ASCIIFont.GetFixed(fontId);

            int width = 0, height = 0;
            font.GetTextDimensions(text, ref width, ref height);

            if (width == 0) // empty text string
                return new Texture2D(_graphicsDevice, 1, 1);

            Color[] resultData = new Color[width * height];

            int dx = 0;
            int dy = 0;

            unsafe
            {
                fixed (Color* rPtr = resultData)
                {
                    for (int i = 0; i < text.Length; ++i)
                    {
                        if ((i < text.Length - 1) && (text.Substring(i, 2) == Environment.NewLine))
                        {
                            dx = 0;
                            dy += font.Height;
                            i++;
                        }
                        else
                        {
                            Texture2D charTexture = font.GetTexture(text[i]);
                            Color[] charData = new Color[charTexture.Width * charTexture.Height];
                            charTexture.GetData<Color>(charData);

                            fixed (Color* cPtr = charData)
                            {
                                for (int iy = 0; iy < font.Height; ++iy)
                                {
                                    Color* src = ((Color*)cPtr) + (charTexture.Width * iy);
                                    Color* dest = (((Color*)rPtr) + (width * ((iy + dy) + (font.Height - font.Height))) + dx);

                                    for (int k = 0; k < charTexture.Width; ++k)
                                        *dest++ = *src++;
                                }
                            }
                            
                            dx += charTexture.Width;
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(_graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);
            result.SetData<Color>(resultData);
            result.Save("test.png", ImageFileFormat.Png);
            return result;
        }
    }
}
