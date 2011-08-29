/***************************************************************************
 *   TextRenderer.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Data;
using UltimaXNA.Graphics;
using UltimaXNA.UILegacy.HTML;

namespace UltimaXNA.UILegacy
{
    class TextRenderer
    {
        private Texture2D _texture;
        private HTMLParser _reader;
        private int _width = 0, _height = 0;
        public int Width
        {
            get
            {
                if (_texture != null)
                    return _texture.Width;
                else
                {
                    checkResize();
                    return _width;
                }
            }
        }
        public int Height
        {
            get
            {
                if (_texture != null)
                    return _texture.Height;
                else
                {
                    checkResize();
                    return _height;
                }
            }
        }

        private HTMLRegions _href;
        public HTMLRegions HREFRegions { get { return _href; } }

        private HTMLImages _images;
        public HTMLImages Images { get { return _images; } }

        private int _activeHREF = -1;
        public int ActiveHREF
        {
            get { return _activeHREF; }
            set { _activeHREF = value; }
        }
        private bool _activeHREF_usedownhue = false;
        public bool ActiveHREF_UseDownHue
        {
            get { return _activeHREF_usedownhue; }
            set { _activeHREF_usedownhue = value; }
        }

        private bool _mustResize = true;
        private bool _mustRender = true;

        private bool _asHTML = false;
        public bool AsHTML
        {
            get { return _asHTML; }
            set
            {
                if (_asHTML != value)
                {
                    _mustResize = true;
                    _mustRender = true;
                    _asHTML = value;
                }
            }
        }

        private int _maxWidth = 0;
        public int MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                if (_maxWidth != value)
                {
                    _mustResize = true;
                    _mustRender = true;
                    _maxWidth = value;
                }
            }
        }

        private string _text = string.Empty;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _mustResize = true;
                    _mustRender = true;
                    _text = value;
                }
            }
        }

        private int _hue = 0;
        public int Hue
        {
            get { return _hue; }
            set { _hue = value; }
        }
        private int hueButNotIfHTML
        {
            get { return _asHTML ? 0 : _hue; }
        }

        private bool _hueTransparent = false;
        public bool Transparent
        {
            get { return _hueTransparent; }
            set { _hueTransparent = value; }
        }

        public TextRenderer(string text, int maxwidth, bool asHTML)
        {
            Text = text;
            MaxWidth = maxwidth;
            AsHTML = asHTML;
            _href = new HTMLRegions();
            _images = new HTMLImages();
        }

        public void Draw(SpriteBatchUI sb, Point2D position)
        {
            Draw(sb, new Rectangle(position.X, position.Y, Width, Height), 0, 0);
        }

        public void Draw(SpriteBatchUI sb, Rectangle destRectangle, int xScroll, int yScroll)
        {
            checkRender(sb.GraphicsDevice);
            
            Rectangle sourceRectangle;

            if (xScroll > _texture.Width)
                return;
            else if (xScroll < -MaxWidth)
                return;
            else
                sourceRectangle.X = xScroll;

            if (yScroll > _texture.Height)
                return;
            else if (yScroll < - Height)
                return;
            else
                sourceRectangle.Y = yScroll;

            int maxX = sourceRectangle.X + destRectangle.Width;
            if (maxX <= _texture.Width)
                sourceRectangle.Width = destRectangle.Width;
            else
            {
                sourceRectangle.Width = _texture.Width - sourceRectangle.X;
                destRectangle.Width = sourceRectangle.Width;
            }

            int maxY = sourceRectangle.Y + destRectangle.Height;
            if (maxY <= _texture.Height)
            {
                sourceRectangle.Height = destRectangle.Height;
            }
            else
            {
                sourceRectangle.Height = _texture.Height - sourceRectangle.Y;
                destRectangle.Height = sourceRectangle.Height;
            }

            sb.Draw2D(_texture, destRectangle, sourceRectangle, hueButNotIfHTML, false, _hueTransparent);

            foreach (HTMLRegion r in _href.Regions)
            {
                Point2D position;
                Rectangle sourceRect;
                if (clipRectangle(new Point2D(xScroll, yScroll), r.Area, destRectangle, out position, out sourceRect))
                {
                    // only draw the font in a different color if this is a HREF region.
                    // otherwise it is a dummy region used to notify images that they are
                    // being mouse overed.
                    if (r.HREFAttributes != null)
                    {
                        int hue = 0;
                        if (r.Index == _activeHREF)
                            if (_activeHREF_usedownhue)
                                hue = r.HREFAttributes.DownHue;
                            else
                                hue = r.HREFAttributes.OverHue;
                        else
                            hue = r.HREFAttributes.UpHue;

                        sb.Draw2D(_texture, position,
                            sourceRect, hue, false, false);
                    }
                }
            }

            foreach (HTMLImage image in _images.Images)
            {
                Point2D position;
                Rectangle sourceRect;
                if (clipRectangle(new Point2D(xScroll, yScroll), image.Area, destRectangle, out position, out sourceRect))
                {
                    // are we mouse over this image?
                    sourceRect.X = 0;
                    sourceRect.Y = 0;
                    Texture2D texture = null;

                    if (image.RegionIndex == _activeHREF)
                    {
                        if (_activeHREF_usedownhue)
                            texture = image.ImageDown;
                        if (texture == null)
                            texture = image.ImageOver;
                    }

                    if (texture == null)
                        texture = image.Image;

                    sb.Draw2D(texture, position,
                        sourceRect, 0, false, false);
                }
            }
        }

        private void checkResize()
        {
            if (_mustResize)
            {
                _mustResize = false;
                resizeAndParse(Text, MaxWidth, AsHTML);
            }
        }

        private void checkRender(GraphicsDevice graphics)
        {
            if (_mustRender)
            {
                checkResize();
                _mustRender = false;
                _texture = writeTexture(graphics, _reader, _width, _height);
            }
        }

        private void resizeAndParse(string textToRender, int maxWidth, bool parseHTML)
        {
            if (_reader != null)
                _reader = null;
            _reader = new HTMLParser(textToRender, parseHTML);

            _href.Clear();
            _images.Clear();

            if (maxWidth < 0)
            {
                _width = 0;
            }
            else
            {
                if (maxWidth == 0)
                {
                    getTextDimensions(_reader, Data.ASCIIText.MaxWidth, 0, out _width, out _height);
                }
                else
                {
                    getTextDimensions(_reader, maxWidth, 0, out _width, out _height);
                }
            }

            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
        }

        Texture2D writeTexture(GraphicsDevice graphics, HTMLParser reader, int width, int height)
        {
            if (_width == 0) // empty text string
                return new Texture2D(graphics, 1, 1);

            Color[] resultData = new Color[width * height];
            // for (int i = 0; i < resultData.Length; i++)
            // resultData[i] = Color.LimeGreen;

            int dy = 0, lineheight = 0;

            unsafe
            {
                fixed (Color* rPtr = resultData)
                {
                    int[] alignedTextX = new int[3];
                    List<HTMLParser_Atom>[] alignedAtoms = new List<HTMLParser_Atom>[3];
                    for (int i = 0; i < 3; i++)
                        alignedAtoms[i] = new List<HTMLParser_Atom>();

                    for (int i = 0; i < reader.Length; i++)
                    {
                        HTMLParser_Atom atom = reader.Atoms[i];
                        alignedAtoms[(int)atom.Alignment].Add(atom);

                        if (atom.IsThisAtomALineBreak || (i == reader.Length - 1))
                        {
                            // write left aligned text.
                            int dx;
                            if (alignedAtoms[0].Count > 0)
                            {
                                alignedTextX[0] = dx = 0;
                                writeTexture_Line(alignedAtoms[0], rPtr, ref dx, dy, width, height, ref lineheight, true);
                            }

                            // centered text. We need to get the width first. Do this by drawing the line with var draw = false.
                            if (alignedAtoms[1].Count > 0)
                            {
                                dx = 0;
                                writeTexture_Line(alignedAtoms[1], rPtr, ref dx, dy, width, height, ref lineheight, false);
                                alignedTextX[1] = dx = width / 2 - dx / 2;
                                writeTexture_Line(alignedAtoms[1], rPtr, ref dx, dy, width, height, ref lineheight, true);
                            }

                            // right aligned text.
                            if (alignedAtoms[2].Count > 0)
                            {
                                dx = 0;
                                writeTexture_Line(alignedAtoms[2], rPtr, ref dx, dy, width, height, ref lineheight, false);
                                alignedTextX[2] = dx = width - dx;
                                writeTexture_Line(alignedAtoms[2], rPtr, ref dx, dy, width, height,ref lineheight, true);
                            }

                            // get HREF regions for html.
                            getHREFRegions(_href, alignedAtoms, alignedTextX, dy);

                            // clear the aligned text lists so we can fill them up in our next pass.
                            for (int j = 0; j < 3; j++)
                            {
                                alignedAtoms[j].Clear();
                            }

                            dy += lineheight;
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
            result.SetData<Color>(resultData);
            return result;
        }

        // pass bool = false to get the width of the line to be drawn without actually drawing anything. Useful for aligning text.
        unsafe void writeTexture_Line(List<HTMLParser_Atom> atoms, Color* rPtr, ref int x, int y, int linewidth, int maxHeight, ref int lineheight, bool draw)
        {
            for (int i = 0; i < atoms.Count; i++)
            {
                UniFont font = UniText.Fonts[(int)atoms[i].Font];
                if (lineheight < font.Lineheight)
                    lineheight = font.Lineheight;

                if (draw)
                {
                    if (atoms[i] is HTMLParser_AtomCharacter)
                    {
                        HTMLParser_AtomCharacter atom = (HTMLParser_AtomCharacter)atoms[i];
                        UniCharacter character = font.GetCharacter(atom.Character);
                        // HREF links should be colored white, because we will hue them at runtime.
                        Color color = atom.IsHREF ? new Color(255, 255, 255) : atom.Color;
                        character.WriteToBuffer(rPtr, x, y, linewidth, maxHeight, font.Baseline,
                            atom.Style_IsBold, atom.Style_IsItalic, atom.Style_IsUnderlined, atom.Style_IsOutlined, color);
                    }
                    else if (atoms[i] is HTMLParser_AtomImageGump)
                    {
                        HTMLParser_AtomImageGump atom = (HTMLParser_AtomImageGump)atoms[i];
                        if (lineheight < atom.Height)
                            lineheight = atom.Height;
                            _images.AddImage(new Rectangle(x, y + (lineheight - atom.Height) / 2, atom.Width, atom.Height),
                                atom.Texture, Data.Gumps.GetGumpXNA(atom.ValueOver), Data.Gumps.GetGumpXNA(atom.ValueDown));
                            atom.AssociatedImage = _images.Images[_images.Count - 1];
                    }
                }
                x += atoms[i].Width;
            }
        }

        void getHREFRegions(HTMLRegions regions, List<HTMLParser_Atom>[] text, int[] x, int y)
        {
            for (int alignment = 0; alignment < 3; alignment++)
            {
                // variables for the open href region
                bool isRegionOpen = false;
                HTMLRegion region = null;
                int regionHeight = 0;
                int additionalwidth = 0;

                int dx = x[alignment];
                for (int i = 0; i < text[alignment].Count; i++)
                {
                    HTMLParser_Atom atom = text[alignment][i];

                    if ((region == null && atom.HREFAttributes != null) ||
                        (region != null && atom.HREFAttributes != region.HREFAttributes))
                    {
                        // close the current href tag if one is open.
                        if (isRegionOpen)
                        {
                            region.Area.Width = (dx - region.Area.X) + additionalwidth;
                            region.Area.Height = (y + regionHeight - region.Area.Y);
                            isRegionOpen = false;
                            region = null;
                        }

                        // did we open a href?
                        if (atom.HREFAttributes != null)
                        {
                            isRegionOpen = true;
                            region = regions.AddRegion(atom.HREFAttributes);
                            region.Area.X = dx;
                            region.Area.Y = y;
                            regionHeight = 0;
                        }
                    }

                    if (atom is HTMLParser_AtomImageGump)
                    {
                        // we need regions for images so that we can do mouse over images.
                        // if we're currently in an open href region, we'll use that one.
                        // if we don't have an open region, we'll create one just for this image.
                        HTMLImage image = ((HTMLParser_AtomImageGump)atom).AssociatedImage;
                        if (image != null)
                        {
                            if (!isRegionOpen)
                            {
                                region = regions.AddRegion(atom.HREFAttributes);
                                isRegionOpen = true;
                                region.Area.X = dx;
                                region.Area.Y = y;
                                regionHeight = 0;
                            }

                            image.RegionIndex = region.Index;
                        }
                    }

                    dx += atom.Width;

                    if (atom is HTMLParser_AtomCharacter && ((HTMLParser_AtomCharacter)atom).Style_IsItalic)
                        additionalwidth = 2;
                    else
                        additionalwidth = 0;

                    if (isRegionOpen && atom.Height > regionHeight)
                        regionHeight = atom.Height;
                }

                // we've reached the last atom in this set.
                // if a href tag is still open, close it.
                if (isRegionOpen)
                {
                    region.Area.Width = (dx - region.Area.X);
                    region.Area.Height = (y + regionHeight - region.Area.Y);
                }
            }
        }

        void getTextDimensions(HTMLParser reader, int maxwidth, int maxheight, out int width, out int height)
        {
            width = 0; height = 0;
            int lineheight = 0;
            int widestline = 0;
            int descenderheight = 0;
            List<HTMLParser_Atom> word = new List<HTMLParser_Atom>();
            int additionalwidth = 0; // for italic + outlined characters, which need a little more room for their slant/outline.
            int word_width = 0;

            for (int i = 0; i < reader.Length; ++i)
            {
                word_width += reader.Atoms[i].Width;
                additionalwidth -= reader.Atoms[i].Width;
                if (additionalwidth < 0)
                    additionalwidth = 0;

                if (lineheight < reader.Atoms[i].Height)
                    lineheight = reader.Atoms[i].Height;

                if (reader.Atoms[i].IsThisAtomALineBreak)
                {
                    if (width + additionalwidth > widestline)
                        widestline = width + additionalwidth;
                    height += lineheight;
                    descenderheight = 0;
                    lineheight = 0;
                    width = 0;
                }
                else
                {
                    word.Add(reader.Atoms[i]);

                    // we may need to add additional width for special style characters.
                    if (reader.Atoms[i] is HTMLParser_AtomCharacter)
                    {
                        HTMLParser_AtomCharacter atom = (HTMLParser_AtomCharacter)reader.Atoms[i];
                        UniFont font = UniText.Fonts[(int)atom.Font];
                        UniCharacter ch = UniText.Fonts[(int)atom.Font].GetCharacter(atom.Character);

                        // italic characters need a little extra width if they are at the end of the line.
                        if (atom.Style_IsItalic)
                            additionalwidth = font.Height / 2;
                        if (atom.Style_IsOutlined)
                            additionalwidth += 1;
                        if (ch.YOffset + ch.Height - lineheight > descenderheight)
                            descenderheight = ch.YOffset + ch.Height - lineheight;
                    }
                    if (reader.Atoms[i].Alignment != enumHTMLAlignments.Left)
                        widestline = maxwidth;

                    if (i == reader.Length - 1 || reader.Atoms[i + 1].CanBreakAtThisAtom)
                    {
                        // Now make sure this line can fit the word.
                        if (width + word_width + additionalwidth <= maxwidth)
                        {
                            // it can fit!
                            width += word_width + additionalwidth;
                            word_width = 0;
                            word.Clear();
                            // if this word is followed by a space, does it fit? If not, drop it entirely and insert \n after the word.
                            if (!(i == reader.Length - 1) && reader.Atoms[i + 1].IsThisAtomABreakingSpace)
                            {
                                int charwidth = reader.Atoms[i + 1].Width;
                                if (width + charwidth <= maxwidth)
                                {
                                    // we can fit an extra space here.
                                    width += charwidth;
                                    i++;
                                }
                                else
                                {
                                    // can't fit an extra space on the end of the line. replace the space with a \n.
                                    ((HTMLParser_AtomCharacter)reader.Atoms[i + 1]).Character = '\n';
                                }
                            }
                        }
                        else
                        {
                            // this word cannot fit in the current line.
                            if ((width > 0) && (i - word.Count >= 0))
                            {
                                // if this is the last word in a line. Replace the last space character with a line break
                                // and back up to the beginning of this word.
                                if (reader.Atoms[i - word.Count].IsThisAtomABreakingSpace)
                                {
                                    ((HTMLParser_AtomCharacter)reader.Atoms[i - word.Count]).Character = '\n';
                                    i = i - word.Count - 1;
                                }
                                else
                                {
                                    reader.Atoms.Insert(i - word.Count, new HTMLParser_AtomCharacter('\n'));
                                    i = i - word.Count;
                                }
                                word.Clear();
                                word_width = 0;
                            }
                            else
                            {
                                // this is the only word on the line and we will need to split it.
                                // first back up until we've reached the reduced the size of the word
                                // so that it fits on one line, and split it there.
                                int iWordWidth = word_width;
                                for (int j = word.Count - 1; j >= 1; j--)
                                {
                                    int iDashWidth = UniText.Fonts[(int)word[j].Font].GetCharacter('-').Width;
                                    if (iWordWidth + iDashWidth <= maxwidth)
                                    {
                                        reader.Atoms.Insert(i - (word.Count - j) + 1, new HTMLParser_AtomCharacter('\n'));
                                        reader.Atoms.Insert(i - (word.Count - j) + 1, new HTMLParser_AtomCharacter('-'));
                                        break;
                                    }
                                    iWordWidth -= word[j].Width;
                                }
                                i -= word.Count + 2;
                                if (i < 0)
                                    i = -1;
                                word.Clear();
                                width = 0;
                                word_width = 0;
                            }
                        }
                    }
                }
            }

            width += additionalwidth;
            height += lineheight + descenderheight;
            if (widestline > width)
                width = widestline;
        }

        private bool clipRectangle(Point2D offset, Rectangle rect, Rectangle clip, out Point2D outPosition, out Rectangle outClipped)
        {
            outPosition = new Point2D();
            outClipped = new Rectangle();

            Rectangle scratchRect = rect;
            Rectangle sourceRect = rect;
            scratchRect.X += clip.X - offset.X;
            scratchRect.Y += clip.Y - offset.Y;

            if (scratchRect.Bottom < clip.Top)
                return false;
            if (scratchRect.Top < clip.Top)
            {
                sourceRect.Y += (clip.Top - scratchRect.Top);
                sourceRect.Height -= (clip.Top - scratchRect.Top);
                scratchRect.Y += (clip.Top - scratchRect.Top);
            }
            if (scratchRect.Top > clip.Bottom)
                return false;
            if (scratchRect.Bottom >= clip.Bottom)
                sourceRect.Height += (clip.Bottom - scratchRect.Bottom);

            if (scratchRect.Right < clip.Left)
                return false;
            if (scratchRect.Left < clip.Left)
            {
                sourceRect.X += (clip.Left - scratchRect.Left);
                scratchRect.Width = sourceRect.Width -= (clip.Left - scratchRect.Left);
                scratchRect.X = clip.Left;
            }
            if (scratchRect.Left > clip.Right)
                return false;
            if (scratchRect.Right >= clip.Right)
                sourceRect.Width += (clip.Right - scratchRect.Right);

            outPosition.X = scratchRect.X;
            outPosition.Y = scratchRect.Y;
            outClipped = sourceRect;
            return true;
        }
    }
}
