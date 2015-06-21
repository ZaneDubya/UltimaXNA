/***************************************************************************
 *   Reader.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Core.UI.HTML.Atoms;
#endregion

namespace UltimaXNA.Core.UI.HTML
{
    public class Reader
    {
        // ================================================================================
        // Private variables
        // ================================================================================
        private static string[] m_FontTags = new string[] { "big", "small", "medium", "basefont" };
        private static string[] m_AlignmentTags = new string[] { "center", "left", "right" };

        // ================================================================================
        // Private services
        // ================================================================================
        private IUIResourceProvider m_ResourceProvider;

        // ================================================================================
        // Public properties
        // ================================================================================
        public List<AAtom> Atoms
        {
            get;
            private set;
        }

        public string Text
        {
            get
            {
                string text = string.Empty;
                for (int i = 0; i < Atoms.Count; i++)
                {
                    text += Atoms[i].ToString();
                }
                return text;
            }
        }

        public int Length
        {
            get
            {
                return Atoms.Count;
            }
        }

        // ================================================================================
        // Ctor, Init, Dispose, Update, and Draw
        // ================================================================================
        public Reader(string inText)
        {
            m_ResourceProvider = ServiceRegistry.GetService<IUIResourceProvider>();

            Atoms = decodeText(inText);
        }

        private List<AAtom> decodeText(string inText)
        {
            List<AAtom> atoms = new List<AAtom>();
            StyleManager tags = new StyleManager(m_ResourceProvider);

            // if this is not HTML, do not parse tags. Otherwise search out and interpret tags.
            bool parseHTML = true;
            if (!parseHTML)
            {
                for (int i = 0; i < inText.Length; i++)
                {
                    addCharacter(inText[i], atoms, tags);
                }
            }
            else
            {
                Parsing.HTMLparser parser = new Parsing.HTMLparser(inText);
                Parsing.HTMLchunk chunk;

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
                                tags.OpenTag(chunk);
                                atom = new ImageAtom(tags.Style);
                                tags.CloseOneTag(chunk);
                                break;
                            // ======================================================================
                            // Every other element is not interpreted, but rendered as text. Easy!
                            // ======================================================================
                            default:
                                for (int i = 0; i < chunk.iChunkLength; i++)
                                {
                                    addCharacter(char.Parse(inText.Substring(i + chunk.iChunkOffset, 1)), atoms, tags);
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

        void addCharacter(char inText, List<AAtom> outHTML, StyleManager openTags)
        {
            CharacterAtom c = new CharacterAtom(openTags.Style, inText);
            outHTML.Add(c);
        }
    }
}