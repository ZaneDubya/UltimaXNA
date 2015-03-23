using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaData.FontsNew;
using UltimaXNA.UltimaGUI.HTML;

namespace UltimaXNA.UltimaGUI
{
    class RenderedTextTexture
    {
        private Texture2D m_Texture;
        private Parser m_HtmlParser;

        public int Width
        {
            get
            {
                if (m_Texture == null || m_mustRender)
                {
                    return 0;
                }

                return m_Texture.Width;
            }
        }

        public int Height
        {
            get
            {
                if (m_Texture == null || m_mustRender)
                {
                    return 0;
                }

                return m_Texture.Height;
            }
        }

        public HTMLRegions Regions
        {
            get;
            protected set;
        }

        public HTMLImages Images
        {
            get;
            protected set;
        }

        private int m_activeHREF = -1;
        public int ActiveHREF
        {
            get { return m_activeHREF; }
            set { m_activeHREF = value; }
        }

        public bool ActiveRegion_UseDownHue
        {
            get;
            set;
        }

        private bool m_mustRender = true;

        private bool m_AsHTML = false;
        public bool AsHTML
        {
            get { return m_AsHTML; }
            set
            {
                if (m_AsHTML != value)
                {
                    m_mustRender = true;
                    m_AsHTML = value;
                }
            }
        }

        private int m_MaxWidth = 200;
        public int MaxWidth
        {
            get { return m_MaxWidth; }
            set
            {
                if (m_MaxWidth != value)
                {
                    m_mustRender = true;
                    m_MaxWidth = value;
                }
            }
        }

        private string m_text = string.Empty;
        public string Text
        {
            get { return m_text; }
            set
            {
                if (m_text != value)
                {
                    m_mustRender = true;
                    m_text = value;
                }
            }
        }

        public int Hue
        {
            get;
            set;
        }

        public bool Transparent
        {
            get;
            set;
        }

        public RenderedTextTexture(string text, bool asHTML, int maxWidth = 200)
        {
            Text = text;
            AsHTML = asHTML;
            MaxWidth = maxWidth;

            Regions = new HTMLRegions();
            Images = new HTMLImages();
        }

        public void Draw(SpriteBatchUI sb, Point position)
        {
            Draw(sb, new Rectangle(position.X, position.Y, Width, Height), 0, 0);
        }

        public void Draw(SpriteBatchUI sb, Rectangle destRectangle, int xScroll, int yScroll)
        {
            checkRender(sb.GraphicsDevice);
            
            Rectangle sourceRectangle;

            if (xScroll > m_Texture.Width)
                return;
            else if (xScroll < -MaxWidth)
                return;
            else
                sourceRectangle.X = xScroll;

            if (yScroll > m_Texture.Height)
                return;
            else if (yScroll < - Height)
                return;
            else
                sourceRectangle.Y = yScroll;

            int maxX = sourceRectangle.X + destRectangle.Width;
            if (maxX <= m_Texture.Width)
                sourceRectangle.Width = destRectangle.Width;
            else
            {
                sourceRectangle.Width = m_Texture.Width - sourceRectangle.X;
                destRectangle.Width = sourceRectangle.Width;
            }

            int maxY = sourceRectangle.Y + destRectangle.Height;
            if (maxY <= m_Texture.Height)
            {
                sourceRectangle.Height = destRectangle.Height;
            }
            else
            {
                sourceRectangle.Height = m_Texture.Height - sourceRectangle.Y;
                destRectangle.Height = sourceRectangle.Height;
            }

            int hue_if_not_html = m_AsHTML ? 0 : Hue;

            sb.Draw2D(m_Texture, destRectangle, sourceRectangle, hue_if_not_html, false, Transparent);

            for (int i = 0; i < Regions.Count; i++)
            {
                HTMLRegion r = Regions[i];
                Point position;
                Rectangle sourceRect;
                if (clipRectangle(new Point(xScroll, yScroll), r.Area, destRectangle, out position, out sourceRect))
                {
                    // only draw the font in a different color if this is a HREF region.
                    // otherwise it is a dummy region used to notify images that they are
                    // being mouse overed.
                    if (r.HREFAttributes != null)
                    {
                        int hue = 0;
                        if (r.Index == m_activeHREF)
                            if (ActiveRegion_UseDownHue)
                                hue = r.HREFAttributes.DownHue;
                            else
                                hue = r.HREFAttributes.OverHue;
                        else
                            hue = r.HREFAttributes.UpHue;

                        sb.Draw2D(m_Texture, position,
                            sourceRect, hue, false, false);
                    }
                }
            }

            for (int i = 0; i < Images.Count; i++)
            {
                HTMLImage image = Images[i];
                Point position;
                Rectangle sourceRect;
                if (clipRectangle(new Point(xScroll, yScroll), image.Area, destRectangle, out position, out sourceRect))
                {
                    // are we mouse over this image?
                    sourceRect.X = 0;
                    sourceRect.Y = 0;
                    Texture2D texture = null;

                    if (image.RegionIndex == m_activeHREF)
                    {
                        if (ActiveRegion_UseDownHue)
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

        private void checkRender(GraphicsDevice graphics)
        {
            if (m_Texture == null || m_mustRender)
            {
                if (m_Texture != null)
                {
                    m_Texture.Dispose();
                    m_Texture = null;
                }

                int width, height;
                resizeAndParse(Text, MaxWidth, AsHTML, out width, out height);
                m_Texture = renderToTexture(graphics, m_HtmlParser, width, height);

                m_mustRender = false;
            }
        }

        private void resizeAndParse(string textToRender, int maxWidth, bool parseHTML, out int width, out int height)
        {
            width = 0;
            height = 0;

            if (m_HtmlParser != null)
                m_HtmlParser = null;
            m_HtmlParser = new Parser(textToRender, parseHTML);

            Regions.Clear();
            Images.Clear();

            if (maxWidth < 0)
            {
                width = 0;
            }
            else
            {
                if (maxWidth == 0)
                {
                    getTextDimensions(m_HtmlParser, 200, 0, out width, out height);
                }
                else
                {
                    getTextDimensions(m_HtmlParser, maxWidth, 0, out width, out height);
                }
            }
        }

        Texture2D renderToTexture(GraphicsDevice graphics, Parser reader, int width, int height)
        {
            if (width == 0) // empty text string
                return new Texture2D(graphics, 1, 1);

            uint[] resultData = new uint[width * height];
            // for (int i = 0; i < resultData.Length; i++)
            // resultData[i] = Color.LimeGreen;

            int dy = 0, lineheight = 0;

            unsafe
            {
                fixed (uint* rPtr = resultData)
                {
                    int[] alignedTextX = new int[3];
                    List<AHTMLAtom>[] alignedAtoms = new List<AHTMLAtom>[3];
                    for (int i = 0; i < 3; i++)
                        alignedAtoms[i] = new List<AHTMLAtom>();

                    for (int i = 0; i < reader.Length; i++)
                    {
                        AHTMLAtom atom = reader.Atoms[i];
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
                            getHREFRegions(Regions, alignedAtoms, alignedTextX, dy);

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
            result.SetData<uint>(resultData);
            return result;
        }

        // pass bool = false to get the width of the line to be drawn without actually drawing anything. Useful for aligning text.
        unsafe void writeTexture_Line(List<AHTMLAtom> atoms, uint* rPtr, ref int x, int y, int linewidth, int maxHeight, ref int lineheight, bool draw)
        {
            for (int i = 0; i < atoms.Count; i++)
            {
                AFont font = TextUni.GetFont((int)atoms[i].Font);
                if (lineheight < font.Height)
                    lineheight = font.Height;

                if (draw)
                {
                    if (atoms[i] is HTMLAtomCharacter)
                    {
                        HTMLAtomCharacter atom = (HTMLAtomCharacter)atoms[i];
                        ACharacter character = font.GetCharacter(atom.Character);
                        // HREF links should be colored white, because we will hue them at runtime.
                        uint color = atom.IsHREF ? 0xFFFFFFFF : Utility.UintFromColor(atom.Color);
                        character.WriteToBuffer(rPtr, x, y, linewidth, maxHeight, font.Baseline,
                            atom.Style_IsBold, atom.Style_IsItalic, atom.Style_IsUnderlined, atom.Style_IsOutlined, color, 0xFF000000);
                    }
                    else if (atoms[i] is HTMLImageGump)
                    {
                        HTMLImageGump atom = (HTMLImageGump)atoms[i];
                        if (lineheight < atom.Height)
                            lineheight = atom.Height;
                        Images.AddImage(new Rectangle(x, y + (lineheight - atom.Height) / 2, atom.Width, atom.Height),
                                atom.Texture, GumpData.GetGumpXNA(atom.ValueOver), GumpData.GetGumpXNA(atom.ValueDown));
                        atom.AssociatedImage = Images[Images.Count - 1];
                    }
                }
                x += atoms[i].Width;
            }
        }

        void getHREFRegions(HTMLRegions regions, List<AHTMLAtom>[] text, int[] x, int y)
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
                    AHTMLAtom atom = text[alignment][i];

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

                    if (atom is HTMLImageGump)
                    {
                        // we need regions for images so that we can do mouse over images.
                        // if we're currently in an open href region, we'll use that one.
                        // if we don't have an open region, we'll create one just for this image.
                        HTMLImage image = ((HTMLImageGump)atom).AssociatedImage;
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

                    if (atom is HTMLAtomCharacter && ((HTMLAtomCharacter)atom).Style_IsItalic)
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

        void getTextDimensions(Parser reader, int maxwidth, int maxheight, out int width, out int height)
        {
            width = 0; height = 0;
            int lineheight = 0;
            int widestline = 0;
            int descenderheight = 0;
            List<AHTMLAtom> word = new List<AHTMLAtom>();
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
                    if (reader.Atoms[i] is HTMLAtomCharacter)
                    {
                        HTMLAtomCharacter atom = (HTMLAtomCharacter)reader.Atoms[i];
                        AFont font = TextUni.GetFont((int)atom.Font);
                        ACharacter ch = font.GetCharacter(atom.Character);

                        // italic characters need a little extra width if they are at the end of the line.
                        if (atom.Style_IsItalic)
                            additionalwidth = font.Height / 2;
                        if (atom.Style_IsOutlined)
                            additionalwidth += 1;
                        if (ch.YOffset + ch.Height - lineheight > descenderheight)
                            descenderheight = ch.YOffset + ch.Height - lineheight;
                    }
                    if (reader.Atoms[i].Alignment != Alignments.Left)
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
                                    ((HTMLAtomCharacter)reader.Atoms[i + 1]).Character = '\n';
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
                                    ((HTMLAtomCharacter)reader.Atoms[i - word.Count]).Character = '\n';
                                    i = i - word.Count - 1;
                                }
                                else
                                {
                                    reader.Atoms.Insert(i - word.Count, new HTMLAtomCharacter('\n'));
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
                                    int iDashWidth = TextUni.GetFont((int)word[j].Font).GetCharacter('-').Width;
                                    if (iWordWidth + iDashWidth <= maxwidth)
                                    {
                                        reader.Atoms.Insert(i - (word.Count - j) + 1, new HTMLAtomCharacter('\n'));
                                        reader.Atoms.Insert(i - (word.Count - j) + 1, new HTMLAtomCharacter('-'));
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

        private bool clipRectangle(Point offset, Rectangle rect, Rectangle clip, out Point outPosition, out Rectangle outClipped)
        {
            outPosition = new Point();
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
