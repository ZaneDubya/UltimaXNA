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

        public unsafe void WriteToBuffer(Color* rPtr, int dx, int dy, int linewidth, int maxHeight, int baseLine, bool isBold, bool isItalic, bool isUnderlined, Color color)
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
                    *dest++ = color;
                }
            }
        }

        public void LoadCharacter(BinaryReader reader, GraphicsDevice graphics)
        {
            int readerStart = (int)reader.BaseStream.Position;

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

            Metrics.ReportDataRead((int)reader.BaseStream.Position - readerStart);
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
    }

    class UniTextCache
    {
        UniTextCacheEnty[] _entries;
        static int CacheSize = 0x100;
        int _lastEntry = -1;
        public UniTextCache()
        {
            _entries = new UniTextCacheEnty[CacheSize];
        }

        public void Add(string text, Texture2D texture, HREFRegions href)
        {
            _lastEntry++;
            if (_lastEntry == CacheSize)
                _lastEntry = 0;

            if (_entries[_lastEntry] != null)
                _entries[_lastEntry].Dispose();
            _entries[_lastEntry] = new UniTextCacheEnty(text, texture, href);
        }

        public Texture2D GetTexture(string text)
        {
            int i = getIndexToEntry(text);
            if (i == -1)
                return null;
            else
                return _entries[i].Texture;
        }

        public HREFRegions GetHref(string text)
        {
            int i = getIndexToEntry(text);
            if (i == -1)
                return null;
            else
                return _entries[i].Href;
        }

        private int getIndexToEntry(string text)
        {
            if (_lastEntry == -1)
                return -1;

            int hash = text.GetHashCode();
            int i = _lastEntry;
            bool checkedAllEntries = false;
            while (!checkedAllEntries)
            {
                if (_entries[i] != null)
                    if (_entries[i].Hash == hash)
                        return i;
                i = i - 1;
                if (i == -1)
                    i = CacheSize - 1;
                if (i == _lastEntry)
                    checkedAllEntries = true;
            }
            return -1;
        }

        public void Dispose()
        {
            foreach (UniTextCacheEnty e in _entries)
            {
                if (e != null)
                    e.Dispose();
            }
        }
    }

    class UniTextCacheEnty
    {
        public string Text;
        public int Hash;
        public Texture2D Texture;
        public HREFRegions Href;

        public UniTextCacheEnty(string text, Texture2D texture, HREFRegions href)
        {
            Text = text;
            Texture = texture;
            Href = href;
            Hash = text.GetHashCode();
        }

        public void Dispose()
        {
            Text = null;
            Texture.Dispose();
            Texture = null;
            Href = null;
        }
    }

    public static class UniText
    {
        private static UniTextCache _cache;
        private static UniFont[] _fonts;
        private static bool _initialized;
        private static GraphicsDevice _graphicsDevice;

        static UniText()
        {

        }

        public static int FontHeight(int index)
        {
            return _fonts[index].Height;
        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (!_initialized)
            {
                _initialized = true;
                _graphicsDevice = graphicsDevice;
                graphicsDevice.DeviceReset += graphicsDevice_DeviceReset;
                loadFonts();
            }
        }

        static void graphicsDevice_DeviceReset(object sender, System.EventArgs e)
        {
            loadFonts();
        }

        static void loadFonts()
        {
            int maxHeight = 0;
            _fonts = new UniFont[7];
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
            {
                if (_fonts[iFont] == null)
                    continue;
                _fonts[iFont].Height = maxHeight;
            }
        }

        public static Texture2D GetTexture(string text)
        {
            return getTexture(text, 0, 0, false);
        }

        public static Texture2D GetTexture(string text, int width, int height)
        {
            return getTexture(text, width, height, false);
        }

        public static Texture2D GetTextureHTML(string text)
        {
            return getTexture(text, 0, 0, true);
        }

        public static Texture2D GetTextureHTML(string text, int width, int height)
        {
            return getTexture(text, width, height, true);
        }

        public static Texture2D GetTextureHTML(string text, int width, int height, ref HREFRegions regions)
        {
            Texture2D texture = getTexture(text, width, height, true);
            regions = _cache.GetHref(text);
            return texture;
        }

        static Texture2D getTexture(string text, int width, int height, bool parseHTML)
        {
            Texture2D t;

            if (_cache == null)
                _cache = new UniTextCache();

            // Have we already rendered this line of text?
            if ((t = _cache.GetTexture(text)) == null)
            {
                HREFRegions r = new HREFRegions();
                t = writeTexture(text, width, height, r, parseHTML);
                _cache.Add(text, t, r);
            }

            return t;
        }

        static Texture2D writeTexture(string textToRender, int w, int h, HREFRegions regions, bool parseHTML)
        {
            HTML.HTMLparser parser = new UltimaXNA.HTML.HTMLparser(textToRender);
            HTML.HTMLchunk chunk;
            while ((chunk = parser.ParseNext()) != null)
            {
                
            }
            HTMLReader reader = new HTMLReader(textToRender, parseHTML);

            int width = 0, height = 0;
            if (w == 0)
            {
                getTextDimensions(reader, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, out width, out height);
            }
            else
            {
                getTextDimensions(reader, w, h, out width, out height);
            }

            if (width == 0) // empty text string
                return new Texture2D(_graphicsDevice, 1, 1);

            Color[] resultData = new Color[width * height];
            int dy = 0, lineheight = 0;

            unsafe
            {
                fixed (Color* rPtr = resultData)
                {
                    int[] alignedTextX = new int[3];
                    List<HTMLCharacter>[] alignedText = new List<HTMLCharacter>[3];
                    for (int i = 0; i < 3; i++)
                        alignedText[i] = new List<HTMLCharacter>();

                    for (int i = 0; i < reader.Length; i++)
                    {
                        HTMLCharacter c = reader.Characters[i];
                        alignedText[(int)c.Alignment].Add(c);

                        if (c.Character == '\n' || (i == reader.Length - 1))
                        {
                            // write left aligned text.
                            int dx;
                            if (alignedText[0].Count > 0)
                            {
                                alignedTextX[0] = dx = 0;
                                writeTexture_Line(alignedText[0], rPtr, ref dx, dy, width, ref lineheight, true);
                            }

                            // centered text. We need to get the width first. Do this by drawing the line with var draw = false.
                            if (alignedText[1].Count > 0)
                            {
                                dx = 0;
                                writeTexture_Line(alignedText[1], rPtr, ref dx, dy, width, ref lineheight, false);
                                alignedTextX[1] = dx = width / 2 - dx / 2;
                                writeTexture_Line(alignedText[1], rPtr, ref dx, dy, width, ref lineheight, true);
                            }

                            // right aligned text.
                            if (alignedText[2].Count > 0)
                            {
                                dx = 0;
                                writeTexture_Line(alignedText[2], rPtr, ref dx, dy, width, ref lineheight, false);
                                alignedTextX[2] = dx = width - dx;
                                writeTexture_Line(alignedText[2], rPtr, ref dx, dy, width, ref lineheight, true);
                            }

                            // get HREF regions for html.
                            if (regions != null)
                                getHREFRegions(regions, alignedText, alignedTextX, dy);

                            // clear the aligned text lists so we can fill them up in our next pass.
                            for (int j = 0; j < 3; j++)
                            {
                                alignedText[j].Clear();
                            }

                            dy += lineheight;
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(_graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);
            result.SetData<Color>(resultData);
            return result;
        }

        static void getHREFRegions(HREFRegions regions, List<HTMLCharacter>[] text, int[] x, int y)
        {
            for (int alignment = 0; alignment < 3; alignment++)
            {
                // variables for the open href region
                bool hrefRegionOpen = false;
                Rectangle hrefRegion = new Rectangle();
                HREFDescription hrefCurrent = null;
                Point hrefOrigin = new Point();
                int hrefHeight = 0;

                int dx = x[alignment];
                for (int i = 0; i < text[alignment].Count; i++)
                {
                    HTMLCharacter c = text[alignment][i];
                    UniFont font = _fonts[(int)c.Font];
                    UniCharacter character = font.GetCharacter(c.Character);

                    if (c.HREF != hrefCurrent)
                    {
                        // close the current href tag if one is open.
                        if (hrefRegionOpen)
                        {
                            hrefRegion.Width = (dx - hrefOrigin.X);
                            hrefRegion.Height = (y + hrefHeight - hrefOrigin.Y);
                            regions.AddRegion(hrefRegion, hrefCurrent);
                            hrefRegionOpen = false;
                            hrefCurrent = null;
                        }

                        // did we open a href?
                        if (c.HREF != null)
                        {
                            hrefRegionOpen = true;
                            hrefCurrent = c.HREF;
                            hrefOrigin = new Point(dx, y);
                            hrefRegion = new Rectangle(dx, y, 0, 0);
                            hrefHeight = 0;
                        }
                    }

                    dx += (c.IsBold) ? character.Width + 2 : character.Width + 1;
                    if (hrefRegionOpen && font.Height > hrefHeight)
                        hrefHeight = font.Height;
                }

                // close the current href tag if one is open.
                if (hrefRegionOpen)
                {
                    hrefRegion.Width = (dx - hrefOrigin.X);
                    hrefRegion.Height = (y + hrefHeight - hrefOrigin.Y);
                    regions.AddRegion(hrefRegion, hrefCurrent);
                }
            }
        }

        // pass bool = false to get the width of the line to be drawn without actually drawing anything. Useful for aligning text.
        static unsafe void writeTexture_Line(List<HTMLCharacter> text, Color* rPtr, ref int x, int y, int linewidth, ref int lineheight, bool draw)
        {
            for (int i = 0; i < text.Count; i++)
            {
                HTMLCharacter c = text[i];
                UniFont font = _fonts[(int)c.Font];
                UniCharacter character = font.GetCharacter(c.Character);
                if (draw)
                {
                    Color color = c.IsHREF ? new Color(255, 255, 255) : c.Color; // HREF links should be colored white.
                    character.WriteToBuffer(rPtr, x, y, linewidth, font.Height, font.Baseline, c.IsBold, c.IsItalic, c.IsUnderlined, color);
                }
                lineheight = font.Baseline;
                x += (c.IsBold) ? character.Width + 2 : character.Width + 1;
            }
        }

        static void getTextDimensions(HTMLReader reader, int maxwidth, int maxheight, out int width, out int height)
        {
            width = 0; height = 0;
            int lineheight = 0;
            int widestline = 0;
            int italicwidth = 0; // for italic characters, which need a little more room for their slant.
            int descenderheight = 0;
            List<HTMLCharacter> word = new List<HTMLCharacter>();
            
            for (int i = 0; i < reader.Length; ++i)
            {
                HTMLCharacter c = reader.Characters[i];
                UniFont font = _fonts[(int)c.Font];
                if (lineheight < font.Baseline)
                    lineheight = font.Baseline;
                if (((int)c.Character) > 32)
                {
                    word.Add(c);
                }

                if (c.Alignment != enumHTMLAlignments.Left)
                    widestline = maxwidth;

                if (c.Character == ' ' || i == reader.Length - 1 || c.Character == '\n')
                {
                    // Size the word, character by character.
                    int wordwidth = 0;

                    if (word.Count > 0)
                    {
                        for (int j = 0; j < word.Count; j++)
                        {
                            UniCharacter ch = _fonts[(int)word[j].Font].GetCharacter(word[j].Character);
                            int charwidth = ch.Width;

                            // bold characters are one pixel wider than normal characters.
                            if (c.IsBold)
                                charwidth++;

                            // italic characters need a little extra width if they are at the end of the line.
                            if (c.IsItalic)
                                italicwidth = font.Height / 2;
                            else
                            {
                                italicwidth -= charwidth;
                                if (italicwidth < 0)
                                    italicwidth = 0;
                            }

                            if (ch.YOffset + ch.Height - font.Baseline > descenderheight)
                                descenderheight = ch.YOffset + ch.Height - font.Baseline;

                            wordwidth += charwidth + 1;
                        }
                    }

                    // Now make sure this line can fit the word.
                    if (width + wordwidth + italicwidth <= maxwidth)
                    {
                        // it can fit!
                        width += wordwidth + italicwidth;
                        word.Clear();
                        // if this word is followed by a space, does it fit? If not, drop it entirely and insert \n after the word.
                        if (c.Character == ' ')
                        {
                            int charwidth = _fonts[(int)c.Font].GetCharacter(c.Character).Width;
                            if (width + charwidth + 1 <= maxwidth)
                            {
                                // we can fit an extra space here.
                                width += charwidth + 1;
                            }
                            else
                            {
                                // can't fit an extra space on the end of the line. replace the space with a \n.
                                reader.Characters[i] = new HTMLCharacter('\n');
                                i--;
                            }
                        }
                    }
                    else
                    {
                        // if this is the last word in a line
                        if ((width > 0) && (i - word.Count >= 0))
                        {
                            reader.Characters.Insert(i - word.Count, new HTMLCharacter('\n'));
                            i = i - word.Count - 1;
                            word.Clear();
                        }
                        else
                        {
                            reader.Characters[i - 1].Character = '-';
                            int j = i;
                            while (j < reader.Length && reader.Characters[j].Character != ' ' && reader.Characters[j].Character != '\n')
                            {
                                reader.Characters.RemoveAt(j);
                            }
                            i = 0;
                            word.Clear();
                            width = 0;
                            wordwidth = 0;
                        }



                        // this word is too big, so we insert a \n character after the word. !!!
                        // this will cause the word to be cut off... this requires a better fix in the future...
                        
                        // This won't work if the first word in the line is too long, so a special case is necessary:
                        // if (i < word.Count)
                        // {
                        
                            // reader.Characters.Insert(i, new HTMLCharacter('\n'));
                            // word.Clear();
                            // wordwidth = 0;
                           //  width = 0;
                        // }
                        // else
                        // {
                        //     reader.Characters.Insert(i - word.Count, new HTMLCharacter('\n'));
                        //     i = i - word.Count - 1;
                        //     word.Clear();
                        // }
                    }

                    if (c.Character == '\n')
                    {
                        if (width + italicwidth > widestline)
                            widestline = width + italicwidth;
                        height += lineheight;
                        descenderheight = 0;
                        lineheight = 0;
                        width = 0;
                    }
                }
            }

            width += italicwidth;
            height += lineheight + descenderheight + 4;
            if (widestline > width)
                width = widestline;
        }
    }
}
