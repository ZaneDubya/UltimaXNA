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
using System;
using System.Collections.Generic;
using System.Text;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI.HTML.Elements;
using UltimaXNA.Core.UI.HTML.Parsing;
using UltimaXNA.Core.UI.HTML.Styles;
#endregion

namespace UltimaXNA.Core.UI.HTML
{
    class HtmlDocument
    {
        static HTMLparser m_Parser = new HTMLparser();
        static HtmlRenderer m_Renderer = new HtmlRenderer();

        string m_CachedHtml;
        int m_MaxWidth;
        HtmlImageList m_Images;
        BlockElement m_Root;
        bool m_CollapseToContent;
        Action<int> m_OnPageOverflow;
        HtmlLinkList m_Links;

        public int Width => m_Root.Width;
        public int Height => m_Root.Height;

        public int Ascender
        {
            get;
            private set;
        }

        public int MaxLineCount
        {
            get;
            private set;
        }

        public HtmlImageList Images
        {
            get
            {
                if (m_Images == null)
                {
                    return HtmlImageList.Empty;
                }
                return m_Images;
            }
        }

        public HtmlLinkList Links => m_Links == null ? HtmlLinkList.Empty : m_Links;

        public string TextWithLineBreaks
        {
            get
            {
                if (m_Root == null)
                {
                    return string.Empty;
                }
                StringBuilder text = new StringBuilder(512);
                for (int i = 0; i < m_Root.Children.Count; i++)
                {
                    AElement e = m_Root.Children[i];
                    if (e is CharacterElement)
                    {
                        text.Append((e as CharacterElement).Character);
                    }
                }
                return text.ToString();
            }
        }

        public Texture2D Render() => m_Renderer.Render(m_Root, Ascender, Links);

        // ============================================================================================================
        // Ctor and Dipose
        // ============================================================================================================

        public HtmlDocument(string html, int width, bool collapseContent = false)
        {
            SetHtml(html, width, collapseContent);
        }

        ~HtmlDocument()
        {
            m_OnPageOverflow = null;
        }

        // ============================================================================================================
        // SetHtml, Render, Reset
        // ============================================================================================================

        public void SetHtml(string html, int width, bool collapseContent = false)
        {
            if (html == m_CachedHtml && m_MaxWidth == width && m_CollapseToContent == collapseContent)
            {
                return;
            }
            m_CachedHtml = html;
            m_MaxWidth = width;
            m_CollapseToContent = collapseContent;
            Reset();
            if (string.IsNullOrEmpty(html))
            {
                m_Root = null;
            }
            else
            {
                m_Root = ParseHtmlToBlocks(html);
                GetAllImages(m_Root);
                m_Links = GetAllHrefRegionsInBlock(m_Root);
                DoLayout(m_Root, width);
                if (Ascender != 0)
                {
                    m_Root.Height -= Ascender; // ascender should always be negative.
                }
            }
        }

        public void SetMaxLines(int max, Action<int> onPageOverflow)
        {
            MaxLineCount = max;
            m_OnPageOverflow = onPageOverflow;
        }

        public void Reset()
        {
            // TODO: we need to handle disposing the ImageList better, it references textures.
            Images?.Clear();
            m_Links?.Clear();
        }

        // ============================================================================================================
        // Parse and create boxes
        // ============================================================================================================

        BlockElement ParseHtmlToBlocks(string html)
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
                {
                    currentBlock.AddAtom(new CharacterElement(styles.Style, html[i]));
                }
            }
            else
            {
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
                        {
                            currentBlock.AddAtom(new CharacterElement(styles.Style, text[i]));
                        }
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
                                // ====================================================================================
                                // Anchor elements are added to the open tag collection as HREFs.
                                case "a":
                                    styles.InterpretHREF(chunk, null);
                                    break;
                                // ====================================================================================
                                // These html elements are ignored.
                                case "body":
                                    break;
                                // ====================================================================================
                                // These html elements are blocks but can also have styles
                                case "center":
                                case "left":
                                case "right":
                                case "div":
                                    atom = new BlockElement(chunk.sTag, styles.Style);
                                    styles.ParseTag(chunk, atom);
                                    isBlockTag = true;
                                    break;
                                // ====================================================================================
                                // These html elements are styles, and are added to the StyleParser.
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
                                // ====================================================================================
                                // These html elements are added as atoms only. They cannot impart style
                                // onto other atoms.
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
                                // ====================================================================================
                                // Every other element is not interpreted, but rendered as text. Easy!
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

        HTMLchunk ParseNext(HTMLparser parser)
        {
            HTMLchunk chunk = parser.ParseNext();
            return chunk;
        }

        // ============================================================================================================
        // Layout methods
        // ============================================================================================================

        /// <summary>
        /// Calculates the dimensions of the root element and the position and dimensinos of every child of that element.
        /// </summary>
        void DoLayout(BlockElement root, int width)
        {
            root.Width = width;
            CalculateLayoutWidthsRecursive(root);
            LayoutElements(root);
            root.Height += 1; // hack for outlined chars. should be checking for outlines on bottom row instead.
        }

        void CalculateLayoutWidthsRecursive(BlockElement root)
        {
            int longestBlockWidth = 0, blockWidth = 0;
            int longestLineWidth = 0, lineWidth = 0;
            int styleWidth = 0;
            bool isLastElement = false;
            for (int i = 0; i < root.Children.Count; i++)
            {
                AElement e = root.Children[i];
                isLastElement = i >= root.Children.Count - 1;
                if (e is BlockElement)
                {
                    e.Width = root.Width;
                    CalculateLayoutWidthsRecursive(e as BlockElement);
                    blockWidth += (e as BlockElement).Layout_MinWidth;
                    lineWidth += (e as BlockElement).Layout_MaxWidth;
                }
                else
                {
                    if (!e.IsThisAtomABreakingSpace)
                    {
                        blockWidth += e.Width;
                    }
                    if (e.IsThisAtomABreakingSpace || isLastElement)
                    {
                        if (blockWidth > root.Width)
                        {
                            int restartAtIndex, restartBlockWidth;
                            if (TryReduceBlockWidth(root, i - 1, blockWidth + styleWidth, out restartAtIndex, out restartBlockWidth))
                            {
                                i = restartAtIndex - 1;
                                blockWidth = restartBlockWidth;
                                lineWidth = restartBlockWidth;
                                continue;
                            }
                        }
                        if (blockWidth + styleWidth > longestBlockWidth)
                        {
                            longestBlockWidth = blockWidth + styleWidth;
                        }
                        blockWidth = 0;
                    }
                    lineWidth += e.Width;
                    if (e.Style.ExtraWidth > styleWidth)
                    {
                        styleWidth = e.Style.ExtraWidth;
                    }
                }
                if (e.IsThisAtomALineBreak || isLastElement)
                {
                    if (blockWidth + styleWidth > longestBlockWidth)
                    {
                        longestBlockWidth = blockWidth + styleWidth;
                    }
                    if (lineWidth + styleWidth > longestLineWidth)
                    {
                        longestLineWidth = lineWidth + styleWidth;
                    }
                    blockWidth = 0;
                    lineWidth = 0;
                    styleWidth = 0;
                }
            }
            root.Layout_MinWidth = longestBlockWidth;
            root.Layout_MaxWidth = longestLineWidth;
        }
        
        bool TryReduceBlockWidth(BlockElement root, int blockEndIndex, int blockWidth, out int restartAtIndex, out int restartBlockWidth)
        {
            restartAtIndex = 0;
            restartBlockWidth = 0;
            for (int i = blockEndIndex; i >= 0; i--)
            {
                AElement e = root.Children[i];
                if (e.IsThisAtomABreakingSpace)
                {
                    return false;
                }
                blockWidth -= e.Width;
                if (blockWidth <= root.Width)
                {
                    // if this is text, hyphenate, otherwise just insert a line break.
                    if (e is CharacterElement)
                    {
                        AElement hyphen = new InternalHyphenBreakElement(e.Style);
                        if (blockWidth + hyphen.Width <= root.Width)
                        {
                            root.Children.Insert(i, hyphen);
                            restartBlockWidth = blockWidth;
                            restartAtIndex = i;
                            return true;
                        }
                        continue;
                    }
                    root.Children.Insert(i, new InternalLineBreakElement(e.Style));
                    restartBlockWidth = blockWidth;
                    restartAtIndex = i;
                    return true;
                }
            }
            return false;
        }


        void LayoutElements(BlockElement root)
        {
            // 1. Determine if all blocks can fit on one line with max width.
            //      -> If yes, then place all blocks!
            // 2. If not 1, can all blocks fit on one line with min width?
            //      -> If yes, then place all blocks and expand the ones that want additional width until there is no more width to fill.
            //      ** root.Layout_MinWidth == root.Layout_MaxWidth accounts for one long word, but what if multiple words don't fit on one line?
            // 3. If 2 does not work:
            //      -> Flow blocks to the next y line until all remaining blocks can fit on one line.
            //      -> Expand remaining blocks, and start all over again.
            //      -> Actually, this is not yet implemented. Any takers?
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
                    {
                        (element as BlockElement).Width = (element as BlockElement).Layout_MaxWidth;
                    }
                }
                LayoutElementsHorizontal(root, root.Layout_X, root.Layout_Y, out ascender);
            }
            else if (root.Layout_MinWidth <= root.Width || root.Layout_MinWidth == root.Layout_MaxWidth) // 2
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
            else // 4
            {
                // At least one block cannot fit within the width.
                root.Err_Cant_Fit_Children = true;
            }
            if (ascender < Ascender)
            {
                Ascender = ascender;
            }
        }

        void LayoutElementsHorizontal(BlockElement root, int x, int y, out int ascenderDelta)
        {
            int x0 = x;
            int x1 = x + root.Width;
            int height = 0, lineHeight = 0;
            ascenderDelta = 0;
            int lineBeganAtElementIndex = 0;
            int lineCount = 0;

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
                    lineCount++;
                    if (MaxLineCount > 0 && lineCount >= MaxLineCount)
                    {
                        int index = 0;
                        for (int j = 0; j < i; j++)
                        {
                            index += root.Children[j].IsThisAtomInternalOnly ? 0 : 1;
                        }
                        root.Children.RemoveRange(i, root.Children.Count - i);
                        m_OnPageOverflow?.Invoke(index);
                        break;
                    }
                }
                else
                {
                    int wordWidth, styleWidth, wordHeight, ascender;
                    List<AElement> word = LayoutElementsGetWord(root.Children, i, out wordWidth, out styleWidth, out wordHeight, out ascender);
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
                        // This word is too long for this line. Flow it to the next line without breaking, unless it's a space character,
                        // in which case we insert it before the linebreak.
                        // TODO: we should introduce some heuristic that that super long words aren't flowed. Perhaps words 
                        // longer than 8 chars, where the break would be after character 3 and before 3 characters from the end?
                        if (word.Count == 1 && word[0].IsThisAtomABreakingSpace)
                        {
                            root.Children.Insert(i + 1, new InternalLineBreakElement(e0.Style));
                        }
                        else
                        {
                            root.Children.Insert(i, new InternalLineBreakElement(e0.Style));
                            i--;
                        }
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
                {
                    lineHeight = e0.Height;
                }
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
        List<AElement> LayoutElementsGetWord(List<AElement> elements, int start, out int wordWidth, out int styleWidth, out int wordHeight, out int ascender)
        {
            List<AElement> word = new List<AElement>();
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
                if (elements[i].CanBreakAtThisAtom)
                {
                    if (word.Count > 0)
                    {
                        return word;
                    }
                }
                word.Add(elements[i]);
                wordWidth += elements[i].Width;
                styleWidth -= elements[i].Width;
                if (styleWidth < 0)
                {
                    styleWidth = 0;
                }
                if (wordHeight < elements[i].Height)
                {
                    wordHeight = elements[i].Height;
                }
                // we may need to add additional width for special style characters.
                if (elements[i] is CharacterElement)
                {
                    CharacterElement atom = (CharacterElement)elements[i];
                    IFont font = atom.Style.Font;
                    ICharacter ch = font.GetCharacter(atom.Character);
                    // italic characters need a little extra width if they are at the end of the line.
                    if (atom.Style.IsItalic)
                    {
                        styleWidth = font.Height / 2;
                    }
                    if (atom.Style.DrawOutline)
                    {
                        styleWidth += 2;
                        if (-1 < ascender)
                        {
                            ascender = -1;
                        }
                    }
                    if (ch.YOffset + ch.Height > wordHeight)
                    {
                        wordHeight = ch.YOffset + ch.Height;
                    }
                    if (ch.YOffset < 0 && ascender > ch.YOffset)
                    {
                        ascender = ch.YOffset;
                    }
                }
                if (i == elements.Count - 1 || elements[i].CanBreakAtThisAtom)
                {
                    return word;
                }
            }
            return word;
        }

        /// <summary>
        /// Breaks a word at its max value.
        /// </summary>
        void LayoutElements_BreakWordAtLineEnd(List<AElement> elements, int start, int lineWidth, List<AElement> word, int wordWidth, int styleWidth)
        {
            InternalLineBreakElement lineend = new InternalLineBreakElement(word[0].Style);
            int width = lineend.Width + styleWidth + 2;
            for (int i = 0; i < word.Count; i++)
            {
                width += word[i].Width;
                if (width >= lineWidth)
                {
                    elements.Insert(start + i, lineend);
                    return;
                }
            }
        }

        // ============================================================================================================
        // Image and href link region handling
        // ============================================================================================================

        void GetAllImages(BlockElement block)
        {
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            foreach (AElement atom in block.Children)
            {
                if (atom is ImageElement)
                {
                    if (m_Images == null)
                    {
                        m_Images = new HtmlImageList();
                    }
                    ImageElement img = (ImageElement)atom;
                    if (img.ImageType == ImageElement.ImageTypes.UI)
                    {
                        Texture2D standard = provider.GetUITexture(img.ImgSrc);
                        Texture2D over = provider.GetUITexture(img.ImgSrcOver);
                        Texture2D down = provider.GetUITexture(img.ImgSrcDown);
                        m_Images.AddImage(new Rectangle(), standard, over, down);
                    }
                    else if (img.ImageType == ImageElement.ImageTypes.Item)
                    {
                        Texture2D standard, over, down;
                        standard = over = down = provider.GetItemTexture(img.ImgSrc);
                        m_Images.AddImage(new Rectangle(), standard, over, down);
                    }
                    img.AssociatedImage = m_Images[m_Images.Count - 1];
                }
                else if (atom is BlockElement)
                {
                    GetAllImages(atom as BlockElement);
                }
            }
        }

        HtmlLinkList GetAllHrefRegionsInBlock(BlockElement block)
        {
            return new HtmlLinkList();
        }

        // ============================================================================================================
        // Get Carat Position, given an input index into the text or a clicked position
        // ============================================================================================================

        public Point GetCaratPositionByIndex(int textIndex)
        {
            Point carat = Point.Zero;
            if (m_Root == null)
            {
                return carat;
            }
            int index = 0;
            for (int i = 0; i < m_Root.Children.Count; i++)
            {
                AElement e = m_Root.Children[i];
                if (e.IsThisAtomALineBreak)
                {
                    carat.X = 0;
                    carat.Y += e.Height;
                }
                if (index == textIndex)
                {
                    return carat;
                }
                carat.X += e.Width;
                if (e is InternalLineBreakElement)
                {
                    if (index == textIndex)
                    {
                        return carat;
                    }
                    index--;
                }
                index++;
            }
            return carat;
        }

        public int GetCaratIndexByPosition(Point pointInText)
        {
            int index = 0;
            if (m_Root == null)
            {
                return index;
            }
            Rectangle rect = new Rectangle(0,0,0,0);
            for (int i = 0; i < m_Root.Children.Count; i++)
            {
                AElement e = m_Root.Children[i];
                rect.Width = e.Width;
                rect.Height = e.Height;
                if (rect.Contains(pointInText))
                {
                    return index;
                }
                if (e.IsThisAtomALineBreak)
                {
                    rect.Width = m_MaxWidth - rect.X;
                    if (rect.Contains(pointInText))
                    {
                        return index - 1;
                    }
                    rect.X = 0;
                    rect.Y += e.Height;
                }
                if (e is InternalLineBreakElement)
                {
                    index--;
                }
                rect.X += e.Width;
                index++;
            }
            // check for click at bottom of page
            rect = new Rectangle(0, rect.Y, m_MaxWidth, 2000);
            if (rect.Contains(pointInText))
            {
                return index; // end of the last line
            }
            return -1; // don't change
        }
    }
}
