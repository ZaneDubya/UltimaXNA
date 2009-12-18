using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Data
{
    internal sealed class UniCharacter
    {
        bool hasTexture
        {
            get
            {
                return (_textureData == null) ? false : true;
            }
        }
        public int XOffset = 0, YOffset = 0, Width = 0, Height = 0;
        public Color[] _textureData;

        public UniCharacter()
        {

        }

        public unsafe void WriteToBuffer(Color* rPtr, ref int dx, ref int dy, int linewidth, int maxHeight)
        {
            if (hasTexture)
            {
                fixed (Color* cPtr = _textureData)
                {
                    for (int iy = 0; iy < maxHeight; ++iy)
                    {
                        Color* src = ((Color*)cPtr) + (Width * iy);
                        Color* dest = (((Color*)rPtr) + (linewidth * (iy + dy + YOffset)) + dx);

                        for (int k = 0; k < Width; ++k)
                            *dest++ = *src++;
                    }
                }
            }
        }

        public void LoadCharacter(BinaryReader reader, GraphicsDevice graphics)
        {
            this.XOffset = reader.ReadByte();
            this.YOffset = reader.ReadByte();
            this.Width = reader.ReadByte();
            this.Height = reader.ReadByte();

            // only read data if there is data...
            if ((this.Width > 0) && (this.Height > 0))
            {
                // At this point, we know we have data, so go ahead and start reading!
                _textureData = new Color[Width * Height];

                unsafe
                {
                    fixed (Color* p = _textureData)
                    {
                        for (int y = 0; y < Height; ++y)
                        {
                            byte[] scanline = reader.ReadBytes(((Width - 1) / 8) + 1);
                            int bitX = 7;
                            int byteX = 0;
                            for (int x = 0; x < Width; ++x)
                            {
                                Color color = Color.TransparentBlack;
                                if ((scanline[byteX] & (byte)Math.Pow(2, bitX)) != 0)
                                {
                                    color = Color.White;
                                }

                                p[x + y * Width] = color;
                                bitX--;
                                if (bitX < 0)
                                {
                                    bitX = 7;
                                    byteX++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    internal sealed class UniFont
    {
        GraphicsDevice _graphics = null;
        BinaryReader _reader = null;
        private UniCharacter[] _characters;

        private int _height = 0;
        public int Height { get { return _height; } set { _height = value; } }

        public UniFont()
        {
            _characters = new UniCharacter[0x10000];
        }

        public void Initialize(GraphicsDevice graphicsDevice, BinaryReader reader)
        {
            _graphics = graphicsDevice;
            _reader = reader;
            // We load the first 128 characters to 'seed' the font with correct spacing values.
            for (int iChar = 0; iChar < 128; iChar++)
            {
                GetCharacter(iChar);
            }
            // Determine the width of the space character - arbitrarily .333 the width of capital M (.333 em?).
            GetCharacter(' ').Width = GetCharacter('M').Width / 3;
        }

        public UniCharacter GetCharacter(char character)
        {
            return GetCharacter(((int)character) & 0xFFFFF);
        }

        public UniCharacter GetCharacter(int index)
        {
            if (_characters[index] == null)
            {
                _characters[index] = loadCharacter(index);
                if (index < 128 && (_characters[index].Height + _characters[index].YOffset) > Height)
                {
                    Height = _characters[index].Height + _characters[index].YOffset;
                }
            }
            return _characters[index];
        }

        UniCharacter loadCharacter(int index)
        {
            // get the lookup table - 0x10000 ints.
            _reader.BaseStream.Position = index * 4;
            int lookup = _reader.ReadInt32();

            UniCharacter character = new UniCharacter();

            if (lookup == 0)
            {
                // no character - so we just return an empty character
                return character;
            }
            else
            {
                _reader.BaseStream.Position = lookup;
                character.LoadCharacter(_reader, _graphics);
                return character;
            }
        }

        public int GetWidth(char ch)
        {
            return GetCharacter(ch).Width;
        }

        public int GetWidth(string text)
        {
            if (text == null || text.Length == 0) { return 0; }

            int width = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                width += GetCharacter(text[i]).Width;
            }

            return width;
        }

        public void GetTextDimensions(string text, ref int width, ref int height)
        {
            width = 0;
            height = Height;
            int maxwidth = 0;

            text = text.Replace(Environment.NewLine, "\n");

            for (int i = 0; i < text.Length; ++i)
            {
                char jjj = text[i];
                if (text.Substring(i, 1) == "\n")
                {
                    if (width > maxwidth)
                        maxwidth = width;
                    height += Height;
                    width = 0;
                }
                else
                {
                    width += GetCharacter(text[i]).Width + 1;
                }
            }

            if (maxwidth > width)
                width = maxwidth;
        }
    }

    public static class UniText
    {
        private static UniFont[] _fonts = new UniFont[7];
        private static bool _initialized;
        private static GraphicsDevice _graphicsDevice;

        static UniText()
        {

        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (!_initialized)
            {
                _initialized = true;
                _graphicsDevice = graphicsDevice;
                for (int iFont = 0; iFont < 7; iFont++)
                {
                    string path = FileManager.GetFilePath("unifont" + (iFont == 0 ? "" : iFont.ToString()) + ".mul");
                    if (path != null)
                    {
                        _fonts[iFont] = new UniFont();
                        _fonts[iFont].Initialize(_graphicsDevice, new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
                    }
                }
            }
            // DEBUG_SaveFonts();
        }

        static void DEBUG_SaveFonts()
        {
            for (int iFont = 0; iFont < 7; iFont++)
            {
                Texture2D texture = GetTextTexture("<big>Poplicola</big> <small>Poplicola</small> <i>Poplicola</i>", iFont, true);
                texture.Save("Font/" + iFont.ToString() + ".png", ImageFileFormat.Png);
            }
        }

        private static Dictionary<string, Texture2D> _TextTextureCache;

        public static Texture2D GetTextTexture(string text, int fontId, bool isHTML)
        {
            string hash = string.Format("<font:{0}>{1}", fontId.ToString(), text);

            if (_TextTextureCache == null)
                _TextTextureCache = new Dictionary<string, Texture2D>();

            if (!_TextTextureCache.ContainsKey(hash))
            {
                Texture2D texture = getTexture(text, fontId, isHTML);
                _TextTextureCache.Add(hash, texture);
            }
            return _TextTextureCache[hash];
        }

        private unsafe static Texture2D getTexture(string textToRender, int fontId, bool isHTML)
        {
            UniFont font;
            if (fontId < 0 || fontId > 6)
            {
                font = _fonts[1];
            }
            else
            {
                font = _fonts[fontId];
            }

            string text;
            int width = 0, height = 0;
            byte[] htmlCodes;

            if (isHTML)
            {
                decodeHTML(textToRender, out text, out htmlCodes);
                font.GetTextDimensions(text, ref width, ref height);
            }
            else
            {
                text = textToRender;
                htmlCodes = new byte[text.Length];
                font.GetTextDimensions(text, ref width, ref height);
            }

            if (width == 0) // empty text string
                return new Texture2D(_graphicsDevice, 1, 1);

            Color[] resultData = new Color[width * height];

            int dx = 0;
            int dy = 0;
            text = text.Replace(Environment.NewLine, "\n");

            unsafe
            {
                fixed (Color* rPtr = resultData)
                {
                    // DEBUG_fillWithBlack(rPtr, width, height);

                    for (int i = 0; i < text.Length; ++i)
                    {
                        if (text.Substring(i, 1) == "\n")
                        {
                            dx = 0;
                            dy += font.Height;
                        }
                        else
                        {
                            UniCharacter character = font.GetCharacter(text[i]);
                            int maxHeight = (character.Height < font.Height) ? character.Height : font.Height;
                            character.WriteToBuffer(rPtr, ref dx, ref dy, width, maxHeight);
                            dx += character.Width + 1;
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(_graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);
            result.SetData<Color>(resultData);
            return result;
        }

        private static void decodeHTML(string text, out string outText, out byte[] htmlCodes)
        {
            // right now, this doesn't actually decode any html, it just strips the tags from the text.
            char[] outChars = new char[text.Length];
            int iOut = 0;
            // search for tags
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '<')
                {
                    bool isClosing = false;
                    string tag = readHTMLTag(text, ref i, ref isClosing);
                    switch (tag)
                    {
                        case "BR":
                            outChars[iOut++] = '\n';
                            break;
                        default:
                            // unknown tag
                            break;
                    }
                }
                else
                {
                    outChars[iOut++] = text[i];
                }
            }

            outText = new string(outChars).Substring(0, iOut);
            htmlCodes = new byte[outText.Length];
        }

        private static string readHTMLTag(string text, ref int i, ref bool isClosingTag)
        {
            if (text[i + 1] == '/')
            {
                i = i + 1;
                isClosingTag = true;
            }

            int closingBracket = text.IndexOf('>', i);
            string tag = text.Substring(i + 1, closingBracket - i - 1);
            i = closingBracket;
            return tag.ToUpper();
        }

        private unsafe static void DEBUG_fillWithBlack(Color* rPtr, int width, int height)
        {
            // The following lines will fill the background with black:
            for (int y = 0; y < height; y++)
            {
                Color* dest = (((Color*)rPtr) + (width * y));
                for (int x = 0; x < width; x++)
                {
                    *dest++ = Color.Black;
                }
            }
        }
    }
}
