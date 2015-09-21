/***************************************************************************
 *   HtmlDocument.cs
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
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI.HTML.Elements;
using UltimaXNA.Core.UI.HTML.Parsing;
using UltimaXNA.Core.UI.HTML.Styles;
#endregion

namespace UltimaXNA.Core.UI.HTML
{
    class HtmlDocument
    {
        private BlockElement m_Root = null;
        private bool m_CollapseToContent = false;

        // ======================================================================
        // Public properties
        // ======================================================================

        public int Width
        {
            get
            {
                return m_Root.Width;
            }
        }

        public int Height
        {
            get
            {
                return m_Root.Height;
            }
        }

        public int Ascender
        {
            get;
            private set;
        }

        public HtmlImageList Images
        {
            get;
            private set;
        }

        public HtmlLinkList Links
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

        public HtmlDocument(string html, int width, bool collapseBlocks = false)
        {
            m_Root = ParseHtmlToBlocks(html);
            m_CollapseToContent = collapseBlocks;

            Images = new HtmlImageList();
            GetAllImages(Images, m_Root);

            Links = GetAllHrefRegionsInBlock(m_Root);

            DoLayout(m_Root, width);
            if (Ascender != 0)
                m_Root.Height -= Ascender; // ascender is always negative.
            Texture = DoRender(m_Root);
        }

        public void Dispose()
        {
            // !!! we need to handle disposing the ImageList better, it references textures.
            Images.Clear();
            Images = null;
            Links.Clear();
            Links = null;
            if (Texture != null && !Texture.IsDisposed)
                Texture.Dispose();
        }

        // ======================================================================
        // Parse and create boxes
        // ======================================================================

        private static HTMLparser m_Parser;

        private BlockElement ParseHtmlToBlocks(string html)
        {
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            StyleParser styles = new StyleParser(provider);

            BlockElement root, currentBlock;
            root = currentBlock = new BlockElement("root", styles.Style); // this is the root!

            // if this is not HTML, do not parse tags. Otherwise search out and interpret tags.
            bool parseHTML = true;
            if (!parseHTML)
            {
                for (int i = 0; i < html.Length; i++)
                    currentBlock.AddAtom(new CharacterElement(styles.Style, html[i]));
            }
            else
            {
                if (m_Parser == null)
                    m_Parser = new HTMLparser();
                m_Parser.Init(html);
                HTMLchunk chunk;

                while ((chunk = ParseNext(m_Parser)) != null)
                {
                    if (!(chunk.oHTML == string.Empty))
                    {
                        // This is a span of text.
                        string text = chunk.oHTML;
                        // make sure to replace escape characters!
                        text = EscapeCharacters.ReplaceEscapeCharacters(text);
                        //Add the characters to the current box
                        for (int i = 0; i < text.Length; i++)
                            currentBlock.AddAtom(new CharacterElement(styles.Style, text[i]));
                    }
                    else
                    {
                        // This is a tag. interpret the tag and edit the openTags list.
                        // It may also be an atom, in which case we should add it to the list of atoms!
                        AElement atom = null;

                        if (chunk.bClosure && !chunk.bEndClosure)
                        {
                            styles.CloseOneTag(chunk);
                            if (currentBlock.Tag == chunk.sTag)
                            {
                                currentBlock = currentBlock.Parent;
                            }
                        }
                        else
                        {
                            bool isBlockTag = false;
                            switch (chunk.sTag)
                            {
                                // ======================================================================
                                // Anchor elements are added to the open tag collection as HREFs.
                                // ======================================================================
                                case "a":
                                    styles.InterpretHREF(chunk, null);
                                    break;
                                // ======================================================================
                                // These html elements are ignored.
                                // ======================================================================
                                case "body":
                                    break;
                                // ======================================================================
                                // These html elements are blocks but can also have styles
                                // ======================================================================
                                case "center":
                                case "left":
                                case "right":
                                case "div":
                                    atom = new BlockElement(chunk.sTag, styles.Style);
                                    styles.ParseTag(chunk, atom);
                                    isBlockTag = true;
                                    break;
                                // ======================================================================
                                // These html elements are styles, and are added to the StyleParser.
                                // ======================================================================
                                case "span":
                                case "font":
                                case "b":
                                case "i":
                                case "u":
                                case "outline":
                                case "big":
                                case "basefont":
                                case "medium":
                                case "small":
                                    styles.ParseTag(chunk, null);
                                    break;
                                // ======================================================================
                                // These html elements are added as atoms only. They cannot impart style
                                // onto other atoms.
                                // ======================================================================
                                case "br":
                                    atom = new CharacterElement(styles.Style, '\n');
                                    break;
                                case "gumpimg":
                                    // draw a gump image
                                    atom = new ImageElement(styles.Style, ImageElement.ImageTypes.UI);
                                    styles.ParseTag(chunk, atom);
                                    break;
                                case "itemimg":
                                    // draw a static image
                                    atom = new ImageElement(styles.Style, ImageElement.ImageTypes.Item);
                                    styles.ParseTag(chunk, atom);
                                    break;
                                // ======================================================================
                                // Every other element is not interpreted, but rendered as text. Easy!
                                // ======================================================================
                                default:
                                    {
                                        string text = html.Substring(chunk.iChunkOffset, chunk.iChunkLength);
                                        // make sure to replace escape characters!
                                        text = EscapeCharacters.ReplaceEscapeCharacters(text);
                                        //Add the characters to the current box
                                        for (int i = 0; i < text.Length; i++)
                                            currentBlock.AddAtom(new CharacterElement(styles.Style, text[i]));
                                    }
                                    break;
                            }

                            if (atom != null)
                            {
                                currentBlock.AddAtom(atom);
                                if (isBlockTag && !chunk.bEndClosure)
                                    currentBlock = (BlockElement)atom;
                            }

                            styles.CloseAnySoloTags();
                        }
                    }
                }
            }

            return root;
        }

        private HTMLchunk ParseNext(HTMLparser parser)
        {
            HTMLchunk chunk = parser.ParseNext();
            return chunk;
        }

        // ======================================================================
        // Layout methods
        // ======================================================================

        /// <summary>
        /// Calculates the dimensions of the root element and the position and dimensinos of every child of that element.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="width"></param>
        private void DoLayout(BlockElement root, int width)
        {
            CalculateLayoutWidthsRecursive(root);
            root.Width = width;
            LayoutElements(root);
            root.Height += 1; // for outlined chars. hack
        }

        private void CalculateLayoutWidthsRecursive(BlockElement root)
        {
            int widthMinLongest = 0, widthMin = 0;
            int widthMaxLongest = 0, widthMax = 0;
            int styleWidth = 0;

            foreach (AElement child in root.Children)
            {
                if (child is BlockElement)
                {
                    CalculateLayoutWidthsRecursive(child as BlockElement);
                    widthMin += (child as BlockElement).Layout_MinWidth;
                    widthMax += (child as BlockElement).Layout_MaxWidth;
                }
                else
                {
                    // get the child width, add it to the parent width;
                    widthMin += child.Width;
                    widthMax += child.Width;
                    // get the additional style width.
                    int styleWidthChild = 0;
                    if (child.Style.IsItalic)
                        styleWidthChild = child.Style.Font.Height / 2;
                    if (child.Style.MustDrawnOutline)
                        styleWidthChild += 2;
                    if (styleWidthChild > styleWidth)
                            styleWidth = styleWidthChild;
                }

                if (child.IsThisAtomALineBreak)
                {
                    if (widthMin + styleWidth > widthMinLongest)
                        widthMinLongest = widthMin + styleWidth;
                    if (widthMax + styleWidth > widthMaxLongest)
                        widthMaxLongest = widthMax + styleWidth;
                    widthMin = 0;
                    widthMax = 0;
                    styleWidth = 0;
                }

                if (child.IsThisAtomABreakingSpace)
                {
                    if (widthMin > widthMinLongest)
                        widthMin = 0;
                }
            }

           if (widthMinLongest < root.Width)
                widthMinLongest = root.Width;
            if (widthMaxLongest < root.Width)
                widthMaxLongest = root.Width;
            root.Layout_MinWidth = (widthMin + styleWidth > widthMinLongest) ? widthMin + styleWidth : widthMinLongest;
            root.Layout_MaxWidth = (widthMax + styleWidth > widthMaxLongest) ? widthMax + styleWidth : widthMaxLongest;
        }

        private void LayoutElements(BlockElement root)
        {
            // 1. Determine if all blocks can fit on one line with max width.
            //      -> If yes, then place all blocks!
            // 2. If not 1, can all blocks fit on one line with min width? Additionally, is it just one long block?
            //      -> If yes, then place all blocks and expand the ones that want additional width until there is no more width to fill.
            // 3. If not 2:
            //      -> Flow blocks to the next y line until all remaining blocks can fit on one line.
            //      -> Expand remaining blocks, and start all over again.
            //      -> Actually, this is not yet implemented. Any takers? :(

            int ascender = 0;

            if (root.Layout_MaxWidth <= root.Width) // 1
            {
                if (m_CollapseToContent)
                {
                    root.Width = root.Layout_MaxWidth;
                }

                foreach (AElement element in root.Children)
                {
                    if (element is BlockElement)
                        (element as BlockElement).Width = (element as BlockElement).Layout_MaxWidth;
                }
                LayoutElementsHorizontal(root, root.Layout_X, root.Layout_Y, out ascender);
            }
            else if (root.Layout_MinWidth <= root.Width || root.Layout_MinWidth == root.Layout_MaxWidth) // 2 - root.Layout_MinWidth == root.Layout_MaxWidth to account for trolls
            {
                // get the amount of extra width that we could fill.
                int extraRequestedWidth = 0;
                int extraAllowedWidth = root.Width;
                foreach (AElement element in root.Children)
                {
                    if (element is BlockElement)
                    {
                        BlockElement block = (element as BlockElement);
                        extraRequestedWidth = block.Layout_MaxWidth - block.Layout_MinWidth;
                        extraAllowedWidth -= block.Layout_MinWidth;
                    }
                }
                // distribute the extra width.
                foreach (AElement element in root.Children)
                {
                    if (element is BlockElement)
                    {
                        BlockElement block = (element as BlockElement);
                        block.Width = block.Layout_MinWidth + (int)(((float)(block.Layout_MaxWidth - block.Layout_MinWidth) / extraRequestedWidth) * extraAllowedWidth);
                        extraAllowedWidth -= block.Layout_MaxWidth - block.Layout_MinWidth;
                    }
                }
                LayoutElementsHorizontal(root, root.Layout_X, root.Layout_Y, out ascender);
            }
            else // 3
            {
                // just display an error message and call it a day?
                root.Err_Cant_Fit_Children = true;
            }

            if (ascender < Ascender)
                Ascender = ascender;
        }

        private void LayoutElementsHorizontal(BlockElement root, int x, int y, out int ascenderDelta)
        {
            int x0 = x;
            int x1 = x + root.Width;
            int height = 0, lineHeight = 0;
            ascenderDelta = 0;
            int lineBeganAtElementIndex = 0;

            for (int i = 0; i < root.Children.Count; i++)
            {
                AElement e0 = root.Children[i];
                if (e0.IsThisAtomALineBreak)
                {
                    // root alignment styles align a root's children elements within the root width.
                    if (root.Alignment == Alignments.Center)
                    {
                        int centerX = x + (x1 - x0) / 2;
                        for (int j = lineBeganAtElementIndex; j < i; j++)
                        {
                            AElement e1 = root.Children[j];
                            e1.Layout_X = centerX;
                            centerX += e1.Width;
                        }
                    }
                    else if (root.Alignment == Alignments.Right)
                    {
                        int rightX = x1 - x0;
                        for (int j = lineBeganAtElementIndex; j < i; j++)
                        {
                            AElement e1 = root.Children[j];
                            e1.Layout_X = rightX;
                            rightX += e1.Width;
                        }
                    }

                    e0.Layout_X = x0;
                    e0.Layout_Y = y;
                    y += lineHeight;
                    x0 = x;
                    x1 = x + root.Width;
                    height += lineHeight;
                    lineHeight = 0;
                    lineBeganAtElementIndex = i + 1;
                }
                else
                {
                    int wordWidth, styleWidth, wordHeight, ascender;
                    List<AElement> word = LayoutElements_GetWord(root.Children, i, out wordWidth, out styleWidth, out wordHeight, out ascender);

                    if (wordWidth + styleWidth > root.Width)
                    {
                        // Can't fit this word on even a full line. Must break it somewhere. 
                        // might as well break it as close to the line end as possible.
                        // TODO: For words VERY near the end of the line, we should not break it, but flow to the next line.
                        LayoutElements_BreakWordAtLineEnd(root.Children, i, x1 - x0, word, wordWidth, styleWidth);
                        i--;
                    }
                    else if (x0 + wordWidth + styleWidth > x1)
                    {
                        // This word is too long for this line. Flow it to the next line without breaking.
                        // TODO: we should introduce some heuristic that that super long words aren't flowed. Perhaps words 
                        // longer than 8 chars, where the break would be after character 3 and before 3 characters from the end?
                        root.Children.Insert(i, new CharacterElement(e0.Style, '\n'));
                        i--;
                    }
                    else
                    {
                        // This word can fit on the current line without breaking. Lay it out!
                        foreach (AElement e1 in word)
                        {
                            if (e1 is BlockElement)
                            {
                                Alignments alignment = (e1 as BlockElement).Alignment;
                                switch (alignment)
                                {
                                    case Alignments.Left:
                                        e1.Layout_X = x0;
                                        e1.Layout_Y = y;
                                        x0 += e1.Width;
                                        break;
                                    case Alignments.Center: // centered in the root element, not in the remaining space.
                                        e1.Layout_X = (x + root.Width - e1.Width) / 2;
                                        e1.Layout_Y = y;
                                        break;
                                    case Alignments.Right:
                                        e1.Layout_X = x1 - e1.Width;
                                        e1.Layout_Y = y;
                                        x1 -= e1.Width;
                                        break;
                                }
                                LayoutElements((e1 as BlockElement));
                            }
                            else
                            {
                                e1.Layout_X = x0;
                                e1.Layout_Y = y; // -ascender;
                                x0 += e1.Width;
                            }
                        }
                        if (y + ascender < ascenderDelta)
                            ascenderDelta = y + ascender;
                        if (wordHeight > lineHeight)
                            lineHeight = wordHeight;
                        i += word.Count - 1;
                    }
                }
                if (e0.Height > lineHeight)
                    lineHeight = e0.Height;
            }
            root.Height = height + lineHeight;
        }

        /// <summary>
        /// Gets a single word of AElements to lay out.
        /// </summary>
        /// <param name="elements">The list of elements to get the word from.</param>
        /// <param name="start">The index in the elements list to start getting the word.</param>
        /// <param name="wordWidth">The width of the word of elements.</param>
        /// <param name="styleWidth">Italic and Outlined characters need more room for the slant/outline (Bold handled differently).</param>
        /// <param name="wordHeight"></param>
        /// <param name="ascender">Additional pixels above the top of the word. Affects dimensions of the parent if word is on the first line.</param>
        /// <returns></returns>
        List<AElement> LayoutElements_GetWord(List<AElement> elements, int start, out int wordWidth, out int styleWidth, out int wordHeight, out int ascender)
        {
            List<AElement> word = new List<Elements.AElement>();
            wordWidth = 0;
            wordHeight = 0;
            styleWidth = 0;
            ascender = 0;

            for (int i = start; i < elements.Count; i++)
            {
                if (elements[i].IsThisAtomALineBreak)
                {
                    return word;
                }
                else
                {
                    word.Add(elements[i]);
                    wordWidth += elements[i].Width;
                    styleWidth -= elements[i].Width;
                    if (styleWidth < 0)
                        styleWidth = 0;
                    if (wordHeight < elements[i].Height)
                        wordHeight = elements[i].Height;

                    // we may need to add additional width for special style characters.
                    if (elements[i] is CharacterElement)
                    {
                        CharacterElement atom = (CharacterElement)elements[i];
                        IFont font = atom.Style.Font;
                        ICharacter ch = font.GetCharacter(atom.Character);

                        // italic characters need a little extra width if they are at the end of the line.
                        if (atom.Style.IsItalic)
                            styleWidth = font.Height / 2;
                        if (atom.Style.MustDrawnOutline)
                        {
                            styleWidth += 2;
                            if (-1 < ascender)
                                ascender = -1;
                        }
                        if (ch.YOffset + ch.Height > wordHeight)
                            wordHeight = ch.YOffset + ch.Height;
                        if (ch.YOffset < 0 && ascender > ch.YOffset)
                            ascender = ch.YOffset;
                    }

                    if (i == elements.Count - 1 || elements[i].CanBreakAtThisAtom)
                    {
                        return word;
                    }
                }
            }

            return word;
        }

        /// <summary>
        /// Breaks a word at its max value.
        /// </summary>
        private void LayoutElements_BreakWordAtLineEnd(List<AElement> elements, int start, int lineWidth, List<AElement> word, int wordWidth, int styleWidth)
        {
            CharacterElement lineend = new Elements.CharacterElement(word[0].Style, '\n');
            int width = lineend.Width + styleWidth + 2;
            for (int i = 0; i < word.Count; i++)
            {
                width += word[i].Width;
                if (width >= lineWidth)
                {
                    elements.Insert(start + i + 1, lineend);
                    return;
                }
            }
        }

        // ======================================================================
        // Render methods
        // ======================================================================

        /// <summary>
        /// Renders all the elements in the root branch. At the same time, also sets areas for regions and href links.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private Texture2D DoRender(BlockElement root)
        {
            SpriteBatchUI sb = ServiceRegistry.GetService<SpriteBatchUI>();
            GraphicsDevice graphics = sb.GraphicsDevice;

            if (root.Width == 0 || root.Height == 0) // empty text string
                return new Texture2D(graphics, 1, 1);

            uint[] pixels = new uint[root.Width * root.Height];

            if (root.Err_Cant_Fit_Children)
            {
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = 0xffffff00;
                Tracer.Error("Err: Block can't fit children.");
            }
            else
            {
                unsafe
                {
                    fixed (uint* ptr = pixels)
                    {
                        DoRenderBlock(root, ptr, root.Width, root.Height);
                    }
                }
            }

            Texture2D texture = new Texture2D(graphics, root.Width, root.Height, false, SurfaceFormat.Color);
            texture.SetData<uint>(pixels);
            return texture;
        }

        unsafe private void DoRenderBlock(BlockElement root, uint* ptr, int width, int height)
        {
            foreach (AElement element in root.Children)
            {
                int x = element.Layout_X;
                int y = element.Layout_Y - Ascender; // ascender is always negative.
                if (element is CharacterElement)
                {
                    IFont font = element.Style.Font;
                    ICharacter character = font.GetCharacter((element as CharacterElement).Character);
                    // HREF links should be colored white, because we will hue them at runtime.
                    uint color = element.Style.IsHREF ? 0xFFFFFFFF : Utility.UintFromColor(element.Style.Color);
                    character.WriteToBuffer(ptr, x, y, width, height, font.Baseline,
                        element.Style.IsBold, element.Style.IsItalic, element.Style.IsUnderlined, element.Style.MustDrawnOutline, color, 0xFF000008);
                    // offset y by ascender for links...
                    if (character.YOffset < 0)
                    {
                        y += character.YOffset;
                        height -= character.YOffset;
                    }
                }
                else if (element is ImageElement)
                {
                    ImageElement image = (element as ImageElement);
                    image.AssociatedImage.Area = new Rectangle(x, y, image.Width, image.Height);
                    if (element.Style.IsHREF)
                    {
                        Links.AddLink(element.Style, new Rectangle(x, y, element.Width, element.Height));
                        image.AssociatedImage.LinkIndex = Links.Count;
                    }
                }
                else if (element is BlockElement)
                {
                    DoRenderBlock(element as BlockElement, ptr, width, height);
                }

                // set href link regions
                if (element.Style.IsHREF)
                {
                    Links.AddLink(element.Style, new Rectangle(x, y, element.Width, element.Height));
                }
            }
        }

        // ======================================================================
        // Image and href link region handling
        // ======================================================================

        private void GetAllImages(HtmlImageList images, BlockElement block)
        {
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            foreach (AElement atom in block.Children)
            {
                if (atom is ImageElement)
                {
                    ImageElement img = (ImageElement)atom;
                    if (img.ImageType == ImageElement.ImageTypes.UI)
                    {
                        Texture2D standard = provider.GetUITexture(img.ImgSrc);
                        Texture2D over = provider.GetUITexture(img.ImgSrcOver);
                        Texture2D down = provider.GetUITexture(img.ImgSrcDown);
                        images.AddImage(new Rectangle(), standard, over, down);
                    }
                    else if (img.ImageType == ImageElement.ImageTypes.Item)
                    {
                        Texture2D standard, over, down;
                        standard = over = down = provider.GetItemTexture(img.ImgSrc);
                        images.AddImage(new Rectangle(), standard, over, down);
                    }
                    img.AssociatedImage = images[images.Count - 1];
                }
                else if (atom is BlockElement)
                {
                    GetAllImages(images, atom as BlockElement);
                }
            }
        }

        private HtmlLinkList GetAllHrefRegionsInBlock(BlockElement block)
        {
            return new HtmlLinkList();
        }
    }
}
