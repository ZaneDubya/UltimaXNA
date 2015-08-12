/***************************************************************************
 *   LayoutTree.cs
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
using UltimaXNA.Core.UI.HTML.Parsing;
#endregion

namespace UltimaXNA.Core.UI.HTML
{
    class HtmlDocument
    {
        private List<HtmlBox> m_Boxes = null;

        // ======================================================================
        // Public properties
        // ======================================================================

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public HtmlImageList Images
        {
            get;
            private set;
        }

        public HtmlListList Regions
        {
            get;
            private set;
        }

        public Texture2D Texture
        {
            get;
            private set;
        }

        // ======================================================================
        // Ctor and Dipose
        // ======================================================================

        public HtmlDocument(string html, int maxWidth)
        {
            m_Boxes = new List<HtmlBox>();
        }

        public void Dispose()
        {
            // !!! we need to handle disposing the ImageList better.
            Images.Clear();
            Images = null;
            Regions.Clear();
            Regions = null;
            if (Texture != null)
                Texture.Dispose();
        }

        // ======================================================================
        // Parse methods
        // ======================================================================

        private List<AAtom> decodeText(string html, IResourceProvider provider)
        {
            List<AAtom> atoms = new List<AAtom>();
            StyleParser tags = new StyleParser(provider);

            // if this is not HTML, do not parse tags. Otherwise search out and interpret tags.
            bool parseHTML = true;
            if (!parseHTML)
            {
                for (int i = 0; i < html.Length; i++)
                {
                    addCharacter(html[i], atoms, tags);
                }
            }
            else
            {
                HTMLparser parser = new HTMLparser(html);
                HTMLchunk chunk;

                while ((chunk = parser.ParseNext()) != null)
                {
                    if (!(chunk.oHTML == string.Empty))
                    {
                        // This is a span of text.
                        string span = chunk.oHTML;
                        // make sure to replace escape characters!
                        span = EscapeCharacters.ReplaceEscapeCharacters(span);
                        //Add the characters to the outText list.
                        for (int i = 0; i < span.Length; i++)
                            addCharacter(span[i], atoms, tags);
                    }
                    else
                    {
                        // This is a tag. interpret the tag and edit the openTags list.
                        // It may also be an atom, in which case we should add it to the list of atoms!
                        AAtom atom = null;

                        switch (chunk.sTag)
                        {
                            // ======================================================================
                            // Anchor elements are added to the open tag collection as HREFs.
                            // ======================================================================
                            case "a":
                                tags.InterpretHREF(chunk);
                                break;
                            // ======================================================================
                            // These html elements are added as tags to the open tag collection.
                            // ======================================================================
                            case "body":
                            case "font":
                            case "b":
                            case "i":
                            case "u":
                            case "outline":
                            case "big":
                            case "basefont":
                            case "medium":
                            case "small":
                            case "center":
                            case "left":
                            case "right":
                                tags.OpenTag(chunk);
                                break;
                            // ======================================================================
                            // Span elements are added as atoms (for layout) and tags (for styles!)
                            // ======================================================================
                            case "span":
                                tags.OpenTag(chunk);
                                atom = new SpanAtom(tags.Style);
                                break;
                            // ======================================================================
                            // These html elements are added as atoms only. They cannot impart style
                            // onto other atoms.
                            // ======================================================================
                            case "br":
                                addCharacter('\n', atoms, tags);
                                break;
                            case "gumpimg":
                                // draw a gump image
                                tags.OpenTag(chunk);
                                atom = new ImageAtom(tags.Style, ImageAtom.ImageTypes.UI);
                                tags.CloseOneTag(chunk);
                                break;
                            case "itemimg":
                                // draw a static image
                                tags.OpenTag(chunk);
                                atom = new ImageAtom(tags.Style, ImageAtom.ImageTypes.Item);
                                tags.CloseOneTag(chunk);
                                break;
                            // ======================================================================
                            // Every other element is not interpreted, but rendered as text. Easy!
                            // ======================================================================
                            default:
                                for (int i = 0; i < chunk.iChunkLength; i++)
                                {
                                    addCharacter(char.Parse(html.Substring(i + chunk.iChunkOffset, 1)), atoms, tags);
                                }
                                break;
                        }

                        if (atom != null)
                            atoms.Add(atom);

                        tags.CloseAnySoloTags();
                    }
                }
            }

            return atoms;
        }

        void addCharacter(char html, List<AAtom> atoms, StyleParser styles)
        {
            CharacterAtom c = new CharacterAtom(styles.Style, html);
            atoms.Add(c);
        }

        // ======================================================================
        // Layout methods
        // ======================================================================

        private void DoLayoutAndRender(string html, int maxWidth)
        {
            SpriteBatchUI sb = ServiceRegistry.GetService<SpriteBatchUI>();
            int width, height, ascender;

            List<AAtom> atoms = Atomizer.AtomizeHtml(html);
            Images = GetAllImages(atoms);
            Regions.Clear();

            GetTextDimensions(atoms, maxWidth, out width, out height, out ascender);
            Texture = RenderTexture(sb.GraphicsDevice, atoms, width, height, ascender);
        }

        private HtmlImageList GetAllImages(List<AAtom> atoms)
        {
            HtmlImageList images = new HtmlImageList();

            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            foreach (AAtom atom in atoms)
            {
                if (atom is ImageAtom)
                {
                    ImageAtom img = (ImageAtom)atom;
                    if (img.ImageType == ImageAtom.ImageTypes.UI)
                    {
                        Texture2D standard = provider.GetUITexture(img.Style.ImgSrc);
                        Texture2D over = provider.GetUITexture(img.Style.ImgSrcOver);
                        Texture2D down = provider.GetUITexture(img.Style.ImgSrcDown);
                        images.AddImage(new Rectangle(), standard, over, down);
                    }
                    else if (img.ImageType == ImageAtom.ImageTypes.Item)
                    {
                        Texture2D standard, over, down;
                        standard = over = down = provider.GetItemTexture(img.Style.ImgSrc);
                        images.AddImage(new Rectangle(), standard, over, down);
                    }
                    img.AssociatedImage = Images[Images.Count - 1];
                }
            }

            return images;
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

        void GetHREFRegions(HtmlListList regions, List<AAtom>[] text, int[] x, int y)
        {
            for (int alignment = 0; alignment < 3; alignment++)
            {
                // variables for the open href region
                bool isRegionOpen = false;
                HtmlLink region = null;
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
                            region = regions.AddLink(atom.Style.HREF, atom.Style);
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
                        HtmlImage image = ((ImageAtom)atom).AssociatedImage;
                        if (image != null)
                        {
                            if (!isRegionOpen)
                            {
                                region = regions.AddLink(atom.Style.HREF, atom.Style);
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
