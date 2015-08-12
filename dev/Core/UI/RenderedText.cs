/***************************************************************************
 *   RenderedText.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI.HTML;
using UltimaXNA.Core.UI.HTML.Atoms;
using UltimaXNA.Core.UI.HTML.Styles;
#endregion

namespace UltimaXNA.Core.UI
{
    /// <summary>
    /// A texture containing rendered text. Can interpret html. Will automatically update.
    /// </summary>
    class RenderedText
    {
        // private services
        IResourceProvider m_ResourceProvider;

        public string Text
        {
            get { return m_Text; }
            set
            {
                if (m_Text != value)
                {
                    m_MustRender = true;
                    m_Text = value;
                }
            }
        }

        public Texture2D Texture
        {
            get
            {
                RenderIfNecessary();
                return m_Texture;
            }
        }

        public int Width
        {
            get
            {
                if (Text == null)
                    return 0;
                return Texture.Width;
            }
        }

        public int Height
        {
            get
            {
                if (Text == null)
                    return 0;
                return Texture.Height;
            }
        }

        public Regions Regions
        {
            get;
            private set;
        }

        public int MouseOverRegionID
        {
            get;
            set;
        }

        public bool IsMouseDown
        {
            get;
            set;
        }

        public Images Images
        {
            get;
            private set;
        }

        public int MaxWidth
        {
            get { return m_MaxWidth; }
            set
            {
                if (value <= 0)
                    value = DefaultRenderedTextWidth;
                if (m_MaxWidth != value)
                {
                    m_MustRender = true;
                    m_MaxWidth = value;
                }
            }
        }

        private const int DefaultRenderedTextWidth = 200;

        private Texture2D m_Texture;
        private string m_Text = string.Empty;
        private bool m_MustRender = true;
        private int m_MaxWidth;

        public RenderedText(string text, int maxWidth = DefaultRenderedTextWidth)
        {
            m_ResourceProvider = ServiceRegistry.GetService<IResourceProvider>();

            Text = text;
            MaxWidth = maxWidth;

            Regions = new Regions();
            Images = new Images();
        }

        // ======================================================================
        // Draw methods
        // ======================================================================

        public void Draw(SpriteBatchUI sb, Point position, Vector3? hueVector = null)
        {
            Draw(sb, new Rectangle(position.X, position.Y, Width, Height), 0, 0, hueVector);
        }

        public void Draw(SpriteBatchUI sb, Rectangle destRectangle, int xScroll, int yScroll, Vector3? hueVector = null)
        {
            if (Text == null)
                return;

            Rectangle sourceRectangle;

            if (xScroll > Width)
                return;
            else if (xScroll < -MaxWidth)
                return;
            else
                sourceRectangle.X = xScroll;

            if (yScroll > Height)
                return;
            else if (yScroll < -Height)
                return;
            else
                sourceRectangle.Y = yScroll;

            int maxX = sourceRectangle.X + destRectangle.Width;
            if (maxX <= Width)
                sourceRectangle.Width = destRectangle.Width;
            else
            {
                sourceRectangle.Width = Width - sourceRectangle.X;
                destRectangle.Width = sourceRectangle.Width;
            }

            int maxY = sourceRectangle.Y + destRectangle.Height;
            if (maxY <= Height)
            {
                sourceRectangle.Height = destRectangle.Height;
            }
            else
            {
                sourceRectangle.Height = Height - sourceRectangle.Y;
                destRectangle.Height = sourceRectangle.Height;
            }

            sb.Draw2D(m_Texture, destRectangle, sourceRectangle, hueVector.HasValue ? hueVector.Value : Vector3.Zero);

            for (int i = 0; i < Regions.Count; i++)
            {
                Region r = Regions[i];
                Point position;
                Rectangle sourceRect;
                if (ClipRectangle(new Point(xScroll, yScroll), r.Area, destRectangle, out position, out sourceRect))
                {
                    // only draw the font in a different color if this is a HREF region.
                    // otherwise it is a dummy region used to notify images that they are
                    // being mouse overed.
                    if (r.HREF != null)
                    {
                        int linkHue = 0;
                        if (r.Index == MouseOverRegionID)
                            if (IsMouseDown)
                                linkHue = r.HREF.DownHue;
                            else
                                linkHue = r.HREF.OverHue;
                        else
                            linkHue = r.HREF.UpHue;

                        sb.Draw2D(m_Texture, new Vector3(position.X, position.Y, 0),
                            sourceRect, Utility.GetHueVector(linkHue));
                    }
                }
            }

            for (int i = 0; i < Images.Count; i++)
            {
                Image image = Images[i];
                Point position;
                Rectangle sourceRect;
                if (ClipRectangle(new Point(xScroll, yScroll), image.Area, destRectangle, out position, out sourceRect))
                {
                    Rectangle srcImage = new Rectangle(
                        sourceRect.X - image.Area.X, sourceRect.Y - image.Area.Y, 
                        sourceRect.Width, sourceRect.Height);
                    Texture2D texture = null;

                    // is the mouse over this image?
                    if (image.RegionIndex == MouseOverRegionID)
                    {
                        if (IsMouseDown)
                            texture = image.TextureDown;
                        if (texture == null)
                            texture = image.TextureOver;
                        if (texture == null)
                            texture = image.Texture;
                    }

                    if (texture == null)
                        texture = image.Texture;

                    if (srcImage.Width > texture.Width)
                        srcImage.Width = texture.Width;
                    if (srcImage.Height > texture.Height)
                        srcImage.Height = texture.Height;

                    sb.Draw2D(texture, new Vector3(position.X, position.Y, 0),
                        srcImage, Utility.GetHueVector(0, false, false));
                }
            }
        }

        private bool ClipRectangle(Point offset, Rectangle srcRect, Rectangle clipTo, out Point posClipped, out Rectangle srcClipped)
        {
            posClipped = new Point(clipTo.X + srcRect.X - offset.X, clipTo.Y + srcRect.Y - offset.Y);
            srcClipped = srcRect;

            Rectangle dstClipped = srcRect;
            dstClipped.X += clipTo.X - offset.X;
            dstClipped.Y += clipTo.Y - offset.Y;

            if (dstClipped.Bottom < clipTo.Top)
                return false;
            if (dstClipped.Top < clipTo.Top)
            {
                srcClipped.Y += (clipTo.Top - dstClipped.Top);
                srcClipped.Height -= (clipTo.Top - dstClipped.Top);
                posClipped.Y += (clipTo.Top - dstClipped.Top);
            }
            if (dstClipped.Top > clipTo.Bottom)
                return false;
            if (dstClipped.Bottom > clipTo.Bottom)
                srcClipped.Height += (clipTo.Bottom - dstClipped.Bottom);

            if (dstClipped.Right < clipTo.Left)
                return false;
            if (dstClipped.Left < clipTo.Left)
            {
                srcClipped.X += (clipTo.Left - dstClipped.Left);
                srcClipped.Width -= (clipTo.Left - dstClipped.Left);
                posClipped.X += (clipTo.Left - dstClipped.Left);
            }
            if (dstClipped.Left > clipTo.Right)
                return false;
            if (dstClipped.Right > clipTo.Right)
                srcClipped.Width += (clipTo.Right - dstClipped.Right);

            return true;
        }

        // ======================================================================
        // Parse / Layout methods
        // ======================================================================

        private void RenderIfNecessary()
        {
            if (m_Texture == null || m_MustRender)
            {
                if (m_Texture != null)
                {
                    m_Texture.Dispose();
                    m_Texture = null;
                }

                if (Text != null)
                {
                    
                    SpriteBatchUI sb = ServiceRegistry.GetService<SpriteBatchUI>();
                    int width, height, ascender;

                    List<AAtom> atoms = Atomizer.AtomizeHtml(Text);
                    GetAllImages(atoms);
                    Regions.Clear();

                    GetTextDimensions(atoms, MaxWidth, out width, out height, out ascender);
                    m_Texture = RenderTexture(sb.GraphicsDevice, atoms, width, height, ascender);

                    m_MustRender = false;
                }
            }
        }

        private void GetAllImages(List<AAtom> atoms)
        {
            Images.Clear();

            foreach (AAtom atom in atoms)
            {
                if (atom is ImageAtom)
                {
                    ImageAtom img = (ImageAtom)atom;
                    if (img.ImageType == ImageAtom.ImageTypes.UI)
                    {
                        Texture2D standard = m_ResourceProvider.GetUITexture(img.Style.ImgSrc);
                        Texture2D over = m_ResourceProvider.GetUITexture(img.Style.ImgSrcOver);
                        Texture2D down = m_ResourceProvider.GetUITexture(img.Style.ImgSrcDown);
                        Images.AddImage(new Rectangle(), standard, over, down);
                    }
                    else if (img.ImageType == ImageAtom.ImageTypes.Item)
                    {
                        Texture2D standard, over, down;
                        standard = over = down = m_ResourceProvider.GetItemTexture(img.Style.ImgSrc);
                        Images.AddImage(new Rectangle(), standard, over, down);
                    }
                    img.AssociatedImage = Images[Images.Count - 1];
                }
            }
        }

        Texture2D RenderTexture(GraphicsDevice graphics, List<AAtom> atoms, int width, int height, int ascender)
        {
            if (width == 0) // empty text string
                return new Texture2D(graphics, 1, 1);

            int dy = 0, lineheight = 0;

            if (ascender < 0)
            {
                height = height - ascender;
                dy = -ascender;
            }

            uint[] resultData = new uint[width * height];
            /* DEBUG PURPOSES: Fill background with green.
             * for (int i = 0; i < resultData.Length; i++)
                resultData[i] = 0xff00ff00;*/

            unsafe
            {
                fixed (uint* rPtr = resultData)
                {
                    int[] alignedTextX = new int[3];
                    List<AAtom>[] alignedAtoms = new List<AAtom>[3];
                    for (int i = 0; i < 3; i++)
                        alignedAtoms[i] = new List<AAtom>();

                    for (int i = 0; i < atoms.Count; i++)
                    {
                        AAtom atom = atoms[i];
                        alignedAtoms[(int)atom.Style.Alignment].Add(atom);

                        if (atom.IsThisAtomALineBreak || (i == atoms.Count - 1))
                        {
                            // write left aligned text.
                            int dx;
                            if (alignedAtoms[0].Count > 0)
                            {
                                alignedTextX[0] = dx = 0;
                                RenderTextureLine(alignedAtoms[0], rPtr, ref dx, dy, width, height, ref lineheight, true);
                            }

                            // centered text. We need to get the width first. Do this by drawing the line with draw = false.
                            if (alignedAtoms[1].Count > 0)
                            {
                                dx = 0;
                                RenderTextureLine(alignedAtoms[1], rPtr, ref dx, dy, width, height, ref lineheight, false);
                                alignedTextX[1] = dx = width / 2 - dx / 2;
                                RenderTextureLine(alignedAtoms[1], rPtr, ref dx, dy, width, height, ref lineheight, true);
                            }

                            // right aligned text.
                            if (alignedAtoms[2].Count > 0)
                            {
                                dx = 0;
                                RenderTextureLine(alignedAtoms[2], rPtr, ref dx, dy, width, height, ref lineheight, false);
                                alignedTextX[2] = dx = width - dx;
                                RenderTextureLine(alignedAtoms[2], rPtr, ref dx, dy, width, height, ref lineheight, true);
                            }

                            // get HREF regions for html.
                            GetHREFRegions(Regions, alignedAtoms, alignedTextX, dy);

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

        // draw = false to get the width of the line to be drawn without actually drawing anything. Useful for aligning text.
        unsafe void RenderTextureLine(List<AAtom> atoms, uint* rPtr, ref int x, int y, int linewidth, int maxHeight, ref int lineheight, bool draw)
        {
            for (int i = 0; i < atoms.Count; i++)
            {
                IFont font = atoms[i].Style.Font;
                if (lineheight < font.Height)
                    lineheight = font.Height;
                if (lineheight < atoms[i].Height + atoms[i].Style.ElementTopOffset)
                    lineheight = atoms[i].Height + atoms[i].Style.ElementTopOffset;

                if (draw)
                {
                    if (atoms[i] is CharacterAtom)
                    {
                        CharacterAtom atom = (CharacterAtom)atoms[i];
                        ICharacter character = font.GetCharacter(atom.Character);
                        // HREF links should be colored white, because we will hue them at runtime.
                        uint color = atom.Style.IsHREF ? 0xFFFFFFFF : Utility.UintFromColor(atom.Style.Color);
                        character.WriteToBuffer(rPtr, x, y, linewidth, maxHeight, font.Baseline,
                            atom.Style.IsBold, atom.Style.IsItalic, atom.Style.IsUnderlined, atom.Style.IsOutlined, color, 0xFF000008);
                    }
                    else if (atoms[i] is ImageAtom)
                    {
                        ImageAtom atom = (atoms[i] as ImageAtom);
                        atom.AssociatedImage.Area = new Rectangle(x, y + ((lineheight - atom.Height + atoms[i].Style.ElementTopOffset) / 2), atom.Width, atom.Height);
                    }
                }
                x += atoms[i].Width;
            }
        }

        void GetHREFRegions(Regions regions, List<AAtom>[] text, int[] x, int y)
        {
            for (int alignment = 0; alignment < 3; alignment++)
            {
                // variables for the open href region
                bool isRegionOpen = false;
                Region region = null;
                int regionHeight = 0;
                int additionalwidth = 0;

                int dx = x[alignment];
                for (int i = 0; i < text[alignment].Count; i++)
                {
                    AAtom atom = text[alignment][i];

                    if ((region == null && atom.Style.HREF != null) ||
                        (region != null && atom.Style.HREF != region.HREF))
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
                        if (atom.Style.HREF != null)
                        {
                            isRegionOpen = true;
                            region = regions.AddRegion(atom.Style.HREF);
                            region.Area.X = dx;
                            region.Area.Y = y;
                            regionHeight = 0;
                        }
                    }

                    if (atom is ImageAtom)
                    {
                        // we need regions for images so that we can do mouse over images.
                        // if we're currently in an open href region, we'll use that one.
                        // if we don't have an open region, we'll create one just for this image.
                        Image image = ((ImageAtom)atom).AssociatedImage;
                        if (image != null)
                        {
                            if (!isRegionOpen)
                            {
                                region = regions.AddRegion(atom.Style.HREF);
                                isRegionOpen = true;
                                region.Area.X = dx;
                                region.Area.Y = y;
                                regionHeight = 0;
                            }

                            image.RegionIndex = region.Index;
                        }
                    }

                    dx += atom.Width;

                    if (atom is CharacterAtom && ((CharacterAtom)atom).Style.IsItalic)
                        additionalwidth = 2;
                    else if (atom is CharacterAtom && ((CharacterAtom)atom).Style.IsOutlined)
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

        void GetTextDimensions(List<AAtom> atoms, int maxwidth, out int width, out int height, out int ascender)
        {
            // default values for out variables.
            width = 0;
            height = 0;
            ascender = 0;
            // local variables
            int descenderHeight = 0;
            int lineHeight = 0;
            int styleWidth = 0; // italic + outlined characters need more room for the slant/outline.
            int widestLine = 0;
            int wordWidth = 0;
            bool firstLine = true;
            List<AAtom> word = new List<AAtom>();

            bool hasLeftAlignment = false;
            bool hasAnyOtherAlignment = false;
            for (int i = 0; i < atoms.Count; i++)
            {
                hasLeftAlignment = hasLeftAlignment | (atoms[i].Style.Alignment == Alignments.Left);
                hasAnyOtherAlignment = hasAnyOtherAlignment | (atoms[i].Style.Alignment != Alignments.Left);
            }
            if (hasAnyOtherAlignment && hasLeftAlignment)
            {
                widestLine = maxwidth;
            }

            for (int i = 0; i < atoms.Count; i++)
            {
                wordWidth += atoms[i].Width;
                styleWidth -= atoms[i].Width;
                if (styleWidth < 0)
                    styleWidth = 0;

                if (lineHeight < atoms[i].Height + atoms[i].Style.ElementTopOffset)
                {
                    lineHeight = atoms[i].Height + atoms[i].Style.ElementTopOffset;
                }

                if (atoms[i].IsThisAtomALineBreak)
                {
                    if (width + styleWidth > widestLine)
                        widestLine = width + styleWidth;
                    height += lineHeight;
                    descenderHeight = 0;
                    lineHeight = 0;
                    width = 0;
                    firstLine = false;
                }
                else
                {
                    word.Add(atoms[i]);

                    // we may need to add additional width for special style characters.
                    if (atoms[i] is CharacterAtom)
                    {
                        CharacterAtom atom = (CharacterAtom)atoms[i];
                        IFont font = atom.Style.Font;
                        ICharacter ch = font.GetCharacter(atom.Character);

                        // italic characters need a little extra width if they are at the end of the line.
                        if (atom.Style.IsItalic)
                            styleWidth = font.Height / 2;
                        if (atom.Style.IsOutlined)
                            styleWidth += 2;
                        if (ch.YOffset + ch.Height - lineHeight > descenderHeight)
                            descenderHeight = ch.YOffset + ch.Height - lineHeight;
                        if (ch.YOffset < 0 && firstLine && ascender > ch.YOffset)
                            ascender = ch.YOffset;
                    }

                    if (i == atoms.Count - 1 || atoms[i + 1].CanBreakAtThisAtom)
                    {
                        // Now make sure this line can fit the word.
                        if (width + wordWidth + styleWidth <= maxwidth)
                        {
                            // it can fit!
                            width += wordWidth + styleWidth;
                            wordWidth = 0;
                            word.Clear();
                            // if this word is followed by a space, does it fit? If not, drop it entirely and insert \n after the word.
                            if (!(i == atoms.Count - 1) && atoms[i + 1].IsThisAtomABreakingSpace)
                            {
                                int charwidth = atoms[i + 1].Width;
                                if (width + charwidth <= maxwidth)
                                {
                                    // we can fit an extra space here.
                                    width += charwidth;
                                    i++;
                                }
                                else
                                {
                                    // can't fit an extra space on the end of the line. replace the space with a \n.
                                    ((CharacterAtom)atoms[i + 1]).Character = '\n';
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
                                if (atoms[i - word.Count].IsThisAtomABreakingSpace)
                                {
                                    ((CharacterAtom)atoms[i - word.Count]).Character = '\n';
                                    i = i - word.Count - 1;
                                }
                                else
                                {
                                    StyleState inheritedStyle = atoms[i - word.Count].Style;
                                    atoms.Insert(i - word.Count, new CharacterAtom(inheritedStyle, '\n'));
                                    i = i - word.Count;
                                }
                                word.Clear();
                                wordWidth = 0;
                            }
                            else
                            {
                                // this is the only word on the line and we will need to split it.
                                // first back up until we've reached the reduced the size of the word
                                // so that it fits on one line, and split it there.
                                int iWordWidth = wordWidth;
                                for (int j = word.Count - 1; j >= 1; j--)
                                {
                                    int iDashWidth = word[j].Style.Font.GetCharacter('-').Width;
                                    if (iWordWidth + iDashWidth <= maxwidth)
                                    {
                                        StyleState inheritedStyle = atoms[i - (word.Count - j) + 1].Style;
                                        atoms.Insert(i - (word.Count - j) + 1, new CharacterAtom(inheritedStyle, '\n'));
                                        atoms.Insert(i - (word.Count - j) + 1, new CharacterAtom(inheritedStyle, '-'));
                                        break;
                                    }
                                    iWordWidth -= word[j].Width;
                                }
                                i -= word.Count + 2;
                                word.Clear();
                                width = 0;
                                wordWidth = 0;
                                if (i < 0)
                                {
                                    // the texture size is too small to hold this small number of characters. This is a problem.
                                    i = -1;
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            width += styleWidth;
            height += lineHeight + descenderHeight;
            if (widestLine > width)
                width = widestLine;
        }

        
    }
}
