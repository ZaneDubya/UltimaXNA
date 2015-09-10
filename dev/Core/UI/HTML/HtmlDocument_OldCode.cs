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
using UltimaXNA.Core.UI.HTML.Elements;
using UltimaXNA.Core.UI.HTML.Styles;
#endregion

namespace UltimaXNA.Core.UI.HTML
{
    /// <summary>
    /// This is the old html layout engine. I am including it in case any of this old code proves useful.
    /// It will be removed when Milestone 0.7 is completed.
    /// </summary>
    class HtmlDocument_OldCode
    {
        // ======================================================================
        // Old code
        // ======================================================================

        private void DoLayoutOld(AElement root, int maxwidth, out int width, out int height, out int ascender)
        {
            // default values for out variables.
            width = 0;
            height = 0;
            ascender = 0;

            // local variables
            int descenderHeight = 0;
            int lineHeight = 0;
            int styleWidth = 0; // italic + outlined characters need more room for the slant/outline.
            int widestLine = maxwidth; // we automatically set the content to fill the specified width.
            int wordWidth = 0;
            bool firstLine = true;

            List<AElement> word = new List<AElement>();
            List<AElement> elements = null;

            for (int i = 0; i < elements.Count; i++)
            {
                wordWidth += elements[i].Width;
                styleWidth -= elements[i].Width;
                if (styleWidth < 0)
                    styleWidth = 0;

                if (lineHeight < elements[i].Height)
                {
                    lineHeight = elements[i].Height;
                }

                if (elements[i].IsThisAtomALineBreak)
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
                    word.Add(elements[i]);

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
                            styleWidth += 2;
                        if (ch.YOffset + ch.Height - lineHeight > descenderHeight)
                            descenderHeight = ch.YOffset + ch.Height - lineHeight;
                        if (ch.YOffset < 0 && firstLine && ascender > ch.YOffset)
                            ascender = ch.YOffset;
                    }

                    if (i == elements.Count - 1 || elements[i + 1].CanBreakAtThisAtom)
                    {
                        // Now make sure this line can fit the word.
                        if (width + wordWidth + styleWidth <= maxwidth)
                        {
                            // it can fit!
                            width += wordWidth + styleWidth;
                            wordWidth = 0;
                            word.Clear();
                            // if this word is followed by a space, does it fit? If not, drop it entirely and insert \n after the word.
                            if (!(i == elements.Count - 1) && elements[i + 1].IsThisAtomABreakingSpace)
                            {
                                int charwidth = elements[i + 1].Width;
                                if (width + charwidth <= maxwidth)
                                {
                                    // we can fit an extra space here.
                                    width += charwidth;
                                    i++;
                                }
                                else
                                {
                                    // can't fit an extra space on the end of the line. replace the space with a \n.
                                    ((CharacterElement)elements[i + 1]).Character = '\n';
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
                                if (elements[i - word.Count].IsThisAtomABreakingSpace)
                                {
                                    ((CharacterElement)elements[i - word.Count]).Character = '\n';
                                    i = i - word.Count - 1;
                                }
                                else
                                {
                                    StyleState inheritedStyle = elements[i - word.Count].Style;
                                    elements.Insert(i - word.Count, new CharacterElement(inheritedStyle, '\n'));
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
                                        StyleState inheritedStyle = elements[i - (word.Count - j) + 1].Style;
                                        elements.Insert(i - (word.Count - j) + 1, new CharacterElement(inheritedStyle, '\n'));
                                        elements.Insert(i - (word.Count - j) + 1, new CharacterElement(inheritedStyle, '-'));
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

        private Texture2D RenderTexture(GraphicsDevice graphics, List<AElement> atoms, int width, int height, int ascender)
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
                    List<AElement>[] alignedAtoms = new List<AElement>[3];
                    for (int i = 0; i < 3; i++)
                        alignedAtoms[i] = new List<AElement>();

                    for (int i = 0; i < atoms.Count; i++)
                    {
                        AElement atom = atoms[i];
                        // !!! alignedAtoms[(int)atom.Style.Alignment].Add(atom);

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
                            // GetHREFRegions(Regions, alignedAtoms, alignedTextX, dy);

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
        private unsafe void RenderTextureLine(List<AElement> atoms, uint* rPtr, ref int x, int y, int linewidth, int maxHeight, ref int lineheight, bool draw)
        {
            for (int i = 0; i < atoms.Count; i++)
            {
                IFont font = atoms[i].Style.Font;
                if (lineheight < font.Height)
                    lineheight = font.Height;
                if (lineheight < atoms[i].Height)
                    lineheight = atoms[i].Height;

                if (draw)
                {
                    if (atoms[i] is CharacterElement)
                    {
                        CharacterElement atom = (CharacterElement)atoms[i];
                        ICharacter character = font.GetCharacter(atom.Character);
                        // HREF links should be colored white, because we will hue them at runtime.
                        uint color = atom.Style.IsHREF ? 0xFFFFFFFF : Utility.UintFromColor(atom.Style.Color);
                        character.WriteToBuffer(rPtr, x, y, linewidth, maxHeight, font.Baseline,
                            atom.Style.IsBold, atom.Style.IsItalic, atom.Style.IsUnderlined, atom.Style.MustDrawnOutline, color, 0xFF000008);
                    }
                    else if (atoms[i] is ImageElement)
                    {
                        ImageElement atom = (atoms[i] as ImageElement);
                        atom.AssociatedImage.Area = new Rectangle(x, y + ((lineheight - atom.Height) / 2), atom.Width, atom.Height);
                    }
                }
                x += atoms[i].Width;
            }
        }

        private void GetHREFRegions(HtmlLinkList regions, List<AElement>[] text, int[] x, int y)
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
                    AElement atom = text[alignment][i];

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
                            //region = regions.AddLink(atom.Style.HREF, atom.Style);
                            //region.Area.X = dx;
                            //region.Area.Y = y;
                            //regionHeight = 0;
                        }
                    }

                    if (atom is ImageElement)
                    {
                        // we need regions for images so that we can do mouse over images.
                        // if we're currently in an open href region, we'll use that one.
                        // if we don't have an open region, we'll create one just for this image.
                        HtmlImage image = ((ImageElement)atom).AssociatedImage;
                        if (image != null)
                        {
                            if (!isRegionOpen)
                            {
                                //region = regions.AddLink(atom.Style.HREF, atom.Style);
                                //isRegionOpen = true;
                                //region.Area.X = dx;
                                //region.Area.Y = y;
                                //regionHeight = 0;
                            }

                            image.LinkIndex = region.Index;
                        }
                    }

                    dx += atom.Width;

                    if (atom is CharacterElement && ((CharacterElement)atom).Style.IsItalic)
                        additionalwidth = 2;
                    else if (atom is CharacterElement && ((CharacterElement)atom).Style.MustDrawnOutline)
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
    }
}
