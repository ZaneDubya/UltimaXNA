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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI.HTML.Elements;
using UltimaXNA.Core.UI.HTML.Parsing;
using UltimaXNA.Core.UI.HTML.Styles;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.UI.HTML.Elements;
#endregion

namespace UltimaXNA.Core.UI.HTML
{
    class HtmlDocument
    {
        private BlockElement m_Root = null;
        private Texture2D m_Texture = null;

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

        public HtmlImageList Images
        {
            get;
            private set;
        }

        public HtmlLinkList Regions
        {
            get;
            private set;
        }

        public Texture2D Texture
        {
            get
            {
                if (m_Texture == null)
                    m_Texture = DoRender(m_Root);
                return m_Texture;
            }
        }

        // ======================================================================
        // Ctor and Dipose
        // ======================================================================

        public HtmlDocument(string html, int width)
        {
            m_Root = ParseHtmlToBlocks(html);
            Images = GetAllImagesInBlock(m_Root);
            Regions = new HtmlLinkList();

            DoLayout(m_Root, width);
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
        // Parse and create boxes
        // ======================================================================

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
                HTMLparser parser = new HTMLparser(html);
                HTMLchunk chunk;

                while ((chunk = parser.ParseNext()) != null)
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
        }

        private void CalculateLayoutWidthsRecursive(BlockElement root)
        {
            int widthMinLongest = 0, widthMin = 0;
            int widthMaxLongest = 0, widthMax = 0;

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
                    widthMin += child.Width;
                    widthMax += child.Width;
                }

                if (child.IsThisAtomALineBreak)
                {
                    if (widthMin > widthMinLongest)
                        widthMinLongest = widthMin;
                    if (widthMax > widthMaxLongest)
                        widthMaxLongest = widthMax;
                    widthMin = 0;
                    widthMax = 0;
                }

                if (child.IsThisAtomABreakingSpace)
                {
                    if (widthMin > widthMinLongest)
                        widthMin = 0;
                }
            }

            root.Layout_MinWidth = (widthMin > widthMinLongest) ? widthMin : widthMinLongest;
            root.Layout_MaxWidth = (widthMax > widthMaxLongest) ? widthMax : widthMaxLongest;
        }

        private void LayoutElements(BlockElement root)
        {
            // 1. Determine if all blocks can fit on one line with max width.
            //      -> If yes, then place all blocks!
            // 2. If not 1, can all blocks fit on one line with min width?
            //      -> If yes, then place all blocks and expand the ones that have additional width until there is no more width to fill.
            // 3. If not 2, flow blocks to the next y line until all remaining blocks can fit on one line.
            //      -> Expand remaining blocks, and start all over again.
            if (root.Layout_MaxWidth <= root.Width) // 1
            {
                foreach (AElement element in root.Children)
                    if (element is BlockElement)
                        (element as BlockElement).Width = (element as BlockElement).Layout_MaxWidth;
                LayoutElementsHorizontal(root);
            }
            else if (root.Layout_MinWidth <= root.Width) // 2
            {
                // get the amount of extra width that we could fill.
                int extraRequestedWidth = 0;
                foreach (AElement element in root.Children)
                    if (element is BlockElement)
                    {
                        BlockElement block = (element as BlockElement);
                        extraRequestedWidth = block.Layout_MaxWidth - block.Layout_MinWidth;
                    }
                // distribute the extra width.
                foreach (AElement element in root.Children)
                    if (element is BlockElement)
                    {
                        BlockElement block = (element as BlockElement);
                        block.Width = block.Layout_MinWidth + (int)(((float)(block.Layout_MaxWidth - block.Layout_MinWidth) / extraRequestedWidth) * extraRequestedWidth);
                        extraRequestedWidth = block.Layout_MaxWidth - block.Layout_MinWidth;
                    }
                LayoutElementsHorizontal(root);
            }
            else // 3
            {
                // just display an error message and call it a day?
                root.Err_Cant_Fit_Children = true;
            }
        }

        private void LayoutElementsHorizontal(BlockElement root)
        {
            int x0 = 0, x1 = root.Width;
            int height = 0, lineHeight = 0;
            int y = 0;
            for (int i = 0; i < root.Children.Count; i++)
            {
                AElement element = root.Children[i];
                if (element.IsThisAtomALineBreak)
                {
                    y += lineHeight;
                    x0 = 0;
                    x1 = root.Width;
                    height += lineHeight;
                    lineHeight = 0;
                }
                else if (element is BlockElement)
                {
                    Alignments alignment = (element as BlockElement).Alignment;
                    switch (alignment)
                    {
                        case Alignments.Left:
                            element.Layout_X = x0;
                            element.Layout_Y = y;
                            x0 += element.Width;
                            break;
                        case Alignments.Center: // centered in the root element, not in the remaining space.
                            element.Layout_X = (root.Width - element.Width) / 2;
                            element.Layout_Y = y;
                            break;
                        case Alignments.Right:
                            element.Layout_X = x1 - element.Width;
                            element.Layout_Y = y;
                            x1 -= element.Width;
                            break;
                    }
                    LayoutElements((element as BlockElement));

                }
                else
                {
                    element.Layout_X = x0;
                    element.Layout_Y = y;
                    x0 += element.Width;
                }
                if (element.Height > lineHeight)
                    lineHeight = element.Height;
            }
            root.Height = height + lineHeight;
        }

        // ======================================================================
        // Render methods
        // ======================================================================

        private Texture2D DoRender(BlockElement root)
        {
            SpriteBatchUI sb = ServiceRegistry.GetService<SpriteBatchUI>();
            GraphicsDevice graphics = sb.GraphicsDevice;

            if (root.Width == 0) // empty text string
                return new Texture2D(graphics, 1, 1);

            // Texture = RenderTexture(sb.GraphicsDevice, atoms, width, height, ascender);
            return null;
        }

        // ======================================================================
        // Image handling
        // ======================================================================

        private HtmlImageList GetAllImagesInBlock(BlockElement block)
        {
            HtmlImageList images = new HtmlImageList();

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
                    GetAllImagesInBlock(atom as BlockElement);
                }
            }

            return images;
        }
    }
}
