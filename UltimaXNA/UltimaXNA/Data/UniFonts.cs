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

        public unsafe void WriteToBuffer(Color* rPtr, ref int dx, ref int dy, int linewidth, int maxHeight, int baseLine, bool isBold, bool isItalic, bool isUnderlined, Color color)
        {
            if (hasTexture)
            {
                fixed (Color* cPtr = _textureData)
                {
                    for (int iy = 0; (iy < Height) && (iy < maxHeight); iy++)
                    {
                        Color* src = ((Color*)cPtr) + (Width * iy);
                        Color* dest = (((Color*)rPtr) + (linewidth * (iy + dy + YOffset)) + dx);
                        if (isItalic)
                        {
                            dest += (baseLine - YOffset - iy - 1) / 2;
                        }

                        for (int k = 0; k < Width; k++)
                        {
                            if (*src != Color.TransparentBlack)
                            {
                                *dest = color;
                                if (isBold)
                                {
                                    *(dest + 1) = color;
                                }
                            }
                            dest++;
                            src++;
                        }
                    }
                }
            }

            if (isUnderlined)
            {
                if (baseLine >= maxHeight)
                    return;
                Color* dest = (((Color*)rPtr) + (linewidth * (baseLine)) + dx);
                int w = isBold ? Width + 2 : Width + 1;
                for (int k = 0; k < w; k++)
                {
                    *dest++ = Color.White;
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
        private int _baseline = 0;
        public int Baseline { get { return _baseline; } set { _baseline = value; } }

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
            Baseline = GetCharacter('M').Height + GetCharacter('M').YOffset;
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
                if (index < 128 && (_characters[index].Height + _characters[index].YOffset + 2) > Height)
                {
                    Height = _characters[index].Height + _characters[index].YOffset + 2;
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
                int maxHeight = 0;
                for (int iFont = 0; iFont < 7; iFont++)
                {
                    string path = FileManager.GetFilePath("unifont" + (iFont == 0 ? "" : iFont.ToString()) + ".mul");
                    if (path != null)
                    {
                        _fonts[iFont] = new UniFont();
                        _fonts[iFont].Initialize(_graphicsDevice, new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
                        if (_fonts[iFont].Height > maxHeight)
                            maxHeight = _fonts[iFont].Height;
                    }
                }

                for (int iFont = 0; iFont < 7; iFont++)
                    _fonts[iFont].Height = maxHeight;
            }
        }

        static Dictionary<string, Texture2D> _TextTextureCache;

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

        unsafe static Texture2D getTexture(string textToRender, int fontId, bool isHTML)
        {
            
            HTMLReader reader = new HTMLReader(textToRender);
            int width = 0, height = 0;
            getTextDimensions(reader, out width, out height);

            if (width == 0) // empty text string
                return new Texture2D(_graphicsDevice, 1, 1);

            Color[] resultData = new Color[width * height];

            int dx = 0;
            int dy = 0;

            unsafe
            {
                fixed (Color* rPtr = resultData)
                {
                    for (int i = 0; i < reader.Length; ++i)
                    {
                        HTMLCharacter c = reader.Characters[i];
                        UniFont font = _fonts[(int)c.Font];

                        if (c.Character == '\n')
                        {
                            dx = 0;
                            dy += font.Baseline;
                        }
                        else
                        {
                            UniCharacter character = font.GetCharacter(c.Character);
                            character.WriteToBuffer(rPtr, ref dx, ref dy, width, font.Height, font.Baseline, c.IsBold, c.IsItalic, c.IsUnderlined, c.Color);
                            dx += character.Width + 1;
                            if (c.IsBold)
                                dx++;
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(_graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);
            result.SetData<Color>(resultData);
            return result;
        }

        static void getTextDimensions(HTMLReader reader, out int width, out int height)
        {
            width = 0;
            height = _fonts[0].Height;
            int maxwidth = 0; // the longest line yet
            int extrawidth = 0; // for italic characters, which need a little more room for their slant.

            for (int i = 0; i < reader.Length; ++i)
            {
                HTMLCharacter c = reader.Characters[i];
                UniFont font = _fonts[(int)c.Font];

                if (c.Character == '\n')
                {
                    if (width + extrawidth > maxwidth)
                        maxwidth = width + extrawidth;
                    height += font.Baseline;
                    width = 0;
                }
                else
                {
                    int charwidth = font.GetCharacter(c.Character).Width;
                    width += charwidth + 1;
                    if (c.IsBold)
                        width++;
                    if (c.IsItalic)
                        extrawidth = font.Height / 2;
                    else
                    {
                        extrawidth -= charwidth;
                        if (extrawidth < 0)
                            extrawidth = 0;
                    }
                }

            }

            width += extrawidth;
            if (maxwidth > width)
                width = maxwidth;
        }

        private unsafe static void DEBUG_fillWithBlack(Color* rPtr, int width, int height)
        {
            // Fills the background with black
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

    internal enum enumHTMLAlignments
    {
        Default = 0,
        Left = 0,
        Center = 1,
        Right = 2
    }

    internal enum enumHTMLFonts
    {
        Default = 1,
        Big = 0,
        Medium = 1,
        Small = 2
    }

    internal class HTMLCharacter
    {
        public char Character = ' ';
        public bool IsBold = false;
        public bool IsItalic = false;
        public bool IsUnderlined = false;
        public enumHTMLFonts Font = enumHTMLFonts.Default;
        public enumHTMLAlignments Alignment = enumHTMLAlignments.Default;
        public Color Color = Color.White;

        public HTMLCharacter(char character)
        {
            Character = character;
        }
    }

    internal class HTMLReader
    {
        List<HTMLCharacter> _characters;
        public List<HTMLCharacter> Characters
        {
            get
            {
                return _characters;
            }
        }

        public string Text
        {
            get
            {
                string text = string.Empty;
                for (int i = 0; i < _characters.Count; i++)
                {
                    text += _characters[i].Character;
                }
                return text;
            }
        }

        public int Length
        {
            get
            {
                return _characters.Count;
            }
        }

        public HTMLReader(string inText)
        {
            _characters = decodeHTML(inText);
        }

        List<HTMLCharacter> decodeHTML(string inText)
        {
            List<HTMLCharacter> outText = new List<HTMLCharacter>();
            List<string> openTags = new List<string>();
            Color currentColor = Color.White;

            // search for tags
            for (int i = 0; i < inText.Length; i++)
            {
                if (inText[i] == '<')
                {
                    bool isClosing = false;
                    string tag = readHTMLTag(inText, ref i, ref isClosing);
                    switch (tag)
                    {
                        case "BR":
                            HTMLCharacter c = new HTMLCharacter('\n');
                            outText.Add(c);
                            break;
                        case "B":
                            editOpenTags(openTags, isClosing, "b");
                            break;
                        case "I":
                            editOpenTags(openTags, isClosing, "i");
                            break;
                        case "U":
                            editOpenTags(openTags, isClosing, "u");
                            break;
                        case "BIG":
                            editOpenTags(openTags, isClosing, "big");
                            break;
                        case "SMALL":
                            editOpenTags(openTags, isClosing, "small");
                            break;
                        default:
                            // this might be a basefont tag...
                            if (tag.Length > 8 && tag.StartsWith("BASEFONT"))
                            {
                                string[] subtags = tag.Split(' ');
                                // go through each tag
                                for (int j = 1; j < subtags.Length; j++)
                                {
                                    string subtag = subtags[j];
                                    if (subtag.Length == 13 && subtag.StartsWith("COLOR=#"))
                                    {
                                        // get the color! we expect a six character color, which is why
                                        // we specified .Length == 13 in the above if statement.
                                        string color = subtag.Substring(7, subtag.Length - 7);
                                        // convert the hex values to colors
                                        currentColor = Utility.ColorFromHexString(color);
                                    }
                                }
                            }
                            else
                            {
                                // unknown tag
                            }
                            break;
                    }
                }
                else
                {
                    // What's left: alignment (center, left, right), hyperlinks
                    HTMLCharacter c = new HTMLCharacter(inText[i]);
                    c.IsBold = hasTag(openTags, "b");
                    c.IsItalic = hasTag(openTags, "i");
                    c.IsUnderlined = hasTag(openTags, "u");
                    string fontTag = lastTag(openTags, new string[] { "big", "small" });
                    switch (fontTag)
                    {
                        case "big":
                            c.Font = enumHTMLFonts.Big;
                            break;
                        case "small":
                            c.Font = enumHTMLFonts.Small;
                            break;
                    }
                    c.Color = currentColor;
                    outText.Add(c);
                }
            }

            return outText;
        }

        static bool hasTag(List<string> tags, string tag)
        {
            if (tags.IndexOf(tag) != -1)
                return true;
            else
                return false;
        }

        static void editOpenTags(List<string> tags, bool isClosing, string tag)
        {
            if (!isClosing)
            {
                tags.Add(tag);
            }
            else
            {
                int i = tags.LastIndexOf(tag);
                if (i != -1)
                {
                    tags.RemoveAt(i);
                }
            }
        }

        static string lastTag(List<string> tags, string[] checkTags)
        {
            int index = int.MaxValue;
            string tag = string.Empty;

            for (int i = 0; i < checkTags.Length; i++)
            {
                int j = tags.LastIndexOf(checkTags[i]);
                if (j != -1 && j < index)
                {
                    index = tags.LastIndexOf(checkTags[i]);
                    tag = checkTags[i];
                }
            }

            return tag;
        }

        string readHTMLTag(string text, ref int i, ref bool isClosingTag)
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
    }
}
