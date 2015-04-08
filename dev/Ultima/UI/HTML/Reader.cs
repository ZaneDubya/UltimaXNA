/***************************************************************************
 *   Reader.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region Usings

using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics.Tracing;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaGUI.HTML.Atoms;
using UltimaXNA.UltimaGUI.HTML.Parsing;

#endregion

namespace UltimaXNA.UltimaGUI.HTML
{
    public class Reader
    {
        private static readonly string[] alignmentTags = {"center", "left", "right"};

        public Reader(string inText, bool parseHTML)
        {
            Atoms = decodeText(inText, parseHTML);
        }

        public List<AAtom> Atoms
        {
            get;
            private set;
        }

        public string Text
        {
            get
            {
                var text = string.Empty;
                for(var i = 0; i < Atoms.Count; i++)
                {
                    text += Atoms[i].ToString();
                }
                return text;
            }
        }

        public int Length
        {
            get { return Atoms.Count; }
        }

        private List<AAtom> decodeText(string inText, bool parseHTML)
        {
            var outAtoms = new List<AAtom>();
            var openTags = new List<string>();
            var currentColor = Color.White;
            var openHREFs = new List<HREF_Attributes>();

            // if this is not HTML, do not parse tags. Otherwise search out and interpret tags.
            if(!parseHTML)
            {
                for(var i = 0; i < inText.Length; i++)
                {
                    addCharacter(inText[i], outAtoms, openTags, currentColor, openHREFs);
                }
            }
            else
            {
                var parser = new HTMLparser(inText);
                HTMLchunk chunk;
                while((chunk = parser.ParseNext()) != null)
                {
                    if(!(chunk.oHTML == string.Empty))
                    {
                        // this is a span of text.
                        var span = chunk.oHTML;
                        // make sure to replace escape characters!
                        span = EscapeCharacters.ReplaceEscapeCharacters(span);
                        //Add the characters to the outText list.
                        for(var i = 0; i < span.Length; i++)
                        {
                            addCharacter(span[i], outAtoms, openTags, currentColor, openHREFs);
                        }
                    }
                    else
                    {
                        // this is a tag. interpret the tag and edit the openTags list.
                        var readParams = true;
                        var isClosing = chunk.bClosure;
                        switch(chunk.sTag)
                        {
                            case "font":
                                break;
                            case "br":
                                addCharacter('\n', outAtoms, openTags, currentColor, openHREFs);
                                break;
                            case "b":
                                editOpenTags(openTags, isClosing, "b");
                                break;
                            case "i":
                                editOpenTags(openTags, isClosing, "i");
                                break;
                            case "u":
                                editOpenTags(openTags, isClosing, "u");
                                break;
                            case "outline":
                                editOpenTags(openTags, isClosing, "outline");
                                break;
                            case "big":
                                editOpenTags(openTags, isClosing, "big");
                                break;
                            case "basefont":
                            case "medium":
                                editOpenTags(openTags, isClosing, "medium");
                                break;
                            case "small":
                                editOpenTags(openTags, isClosing, "small");
                                break;
                            case "center":
                                editOpenTags(openTags, isClosing, "center");
                                break;
                            case "left":
                                editOpenTags(openTags, isClosing, "left");
                                break;
                            case "right":
                                editOpenTags(openTags, isClosing, "right");
                                break;
                            case "gumpimg":
                                addGumpImage(outAtoms, openTags, openHREFs);
                                break;
                            case "span":
                                addSpan(outAtoms, openTags, openHREFs);
                                break;
                            case "a":
                                editOpenTags(openTags, isClosing, "a");
                                if(isClosing)
                                {
                                    // closing a hyperlink - restore previous address, if any.
                                    if(openHREFs.Count > 0)
                                    {
                                        openHREFs.RemoveAt(openHREFs.Count - 1);
                                    }
                                }
                                else
                                {
                                    // hyperlink with attributes
                                    var href = new HREF_Attributes();
                                    openHREFs.Add(href);
                                }
                                break;
                            default:
                                readParams = false;
                                for(var i = 0; i < chunk.iChunkLength; i++)
                                {
                                    addCharacter(char.Parse(inText.Substring(i + chunk.iChunkOffset, 1)), outAtoms, openTags, currentColor, openHREFs);
                                }
                                break;
                        }

                        if(readParams)
                        {
                            foreach(DictionaryEntry param in chunk.oParams)
                            {
                                var key = param.Key.ToString();
                                var value = param.Value.ToString();
                                if(value.EndsWith("/"))
                                {
                                    value = value.Substring(0, value.Length - 1);
                                }

                                switch(key)
                                {
                                    case "href":
                                        if(chunk.sTag == "a")
                                        {
                                            openHREFs[openHREFs.Count - 1].HREF = value;
                                        }
                                        else
                                        {
                                            Tracer.Warn("href paramater used outside of an 'a' tag link. href is ignored in this case.");
                                        }
                                        break;
                                    case "color":
                                    case "hovercolor":
                                    case "activecolor":
                                        // get the color!
                                        var color = value;
                                        if(color[0] == '#')
                                        {
                                            color = color.Substring(1);
                                        }
                                        if(color.Length == 3 || color.Length == 6)
                                        {
                                            var c = Utility.ColorFromHexString(color);
                                            if(key == "color")
                                            {
                                                currentColor = c;
                                            }
                                            if(chunk.sTag == "a")
                                            {
                                                switch(key)
                                                {
                                                    case "color":
                                                        openHREFs[openHREFs.Count - 1].UpHue = HuesXNA.GetWebSafeHue(c);
                                                        break;
                                                    case "hovercolor":
                                                        openHREFs[openHREFs.Count - 1].OverHue = HuesXNA.GetWebSafeHue(c);
                                                        break;
                                                    case "activecolor":
                                                        openHREFs[openHREFs.Count - 1].DownHue = HuesXNA.GetWebSafeHue(c);
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Tracer.Warn("Improperly formatted color:" + color);
                                        }
                                        break;
                                    case "text-decoration":
                                        switch(value)
                                        {
                                            case "none":
                                                if(chunk.sTag == "a")
                                                {
                                                    openHREFs[openHREFs.Count - 1].Underline = false;
                                                }
                                                break;
                                            default:
                                                Tracer.Warn(string.Format("Unknown text-decoration:{0}", value));
                                                break;
                                        }
                                        break;
                                    case "src":
                                    case "hoversrc":
                                    case "activesrc":
                                        switch(chunk.sTag)
                                        {
                                            case "gumpimg":
                                                if(key == "src")
                                                {
                                                    ((ImageAtom)outAtoms[outAtoms.Count - 1]).Value = int.Parse(value);
                                                }
                                                else if(key == "hoversrc")
                                                {
                                                    ((ImageAtom)outAtoms[outAtoms.Count - 1]).ValueOver = int.Parse(value);
                                                }
                                                else if(key == "activesrc")
                                                {
                                                    ((ImageAtom)outAtoms[outAtoms.Count - 1]).ValueDown = int.Parse(value);
                                                }
                                                break;
                                            default:
                                                Tracer.Warn("src param encountered within " + chunk.sTag + " which does not use this param.");
                                                break;
                                        }
                                        break;
                                    case "width":
                                        switch(chunk.sTag)
                                        {
                                            case "gumpimg":
                                            case "span":
                                                outAtoms[outAtoms.Count - 1].Width = int.Parse(value);
                                                break;
                                            default:
                                                Tracer.Warn("width param encountered within " + chunk.sTag + " which does not use this param.");
                                                break;
                                        }
                                        break;
                                    case "height":
                                        switch(chunk.sTag)
                                        {
                                            case "gumpimg":
                                            case "span":
                                                outAtoms[outAtoms.Count - 1].Width = int.Parse(value);
                                                break;
                                            default:
                                                Tracer.Warn("height param encountered within " + chunk.sTag + " which does not use this param.");
                                                break;
                                        }
                                        break;
                                    default:
                                        Tracer.Warn(string.Format("Unknown parameter:{0}", key));
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return outAtoms;
        }

        private string getTextFromWithinQuotes(string tag)
        {
            var openQuotes = tag.IndexOf('\"');
            var closeQuotes = tag.IndexOf('\"', openQuotes + 1);
            var text = tag.Substring(openQuotes + 1, closeQuotes - openQuotes - 1);
            return text;
        }

        private int getHueFromString(string hue)
        {
            var h = int.Parse(hue);
            // we can't hue real colors, we must color with hues.
            if(h > 2998 || hue.Length > 4)
            {
                return -1;
            }
            return h;
        }

        private List<string> getAttributesFromTag(string tag)
        {
            var ret = new List<string>();
            var posAttributes = tag.IndexOf(' ') + 1;
            while(posAttributes < tag.Length)
            {
                if(tag[posAttributes] != ' ')
                {
                    // read in an attribute
                    var isInQuotes = false;
                    var tagStart = posAttributes;
                    var attr = new char[tag.Length - posAttributes];
                    while(posAttributes < tag.Length && (tag[posAttributes] != ' ' || isInQuotes))
                    {
                        attr[posAttributes - tagStart] = tag[posAttributes];
                        if(tag[posAttributes] == '\"')
                        {
                            isInQuotes = !isInQuotes;
                        }
                        posAttributes++;
                    }
                    var attribute = (posAttributes - tagStart == attr.Length) ? new string(attr) : new string(attr).Remove(posAttributes - tagStart);
                    ret.Add(attribute);
                }
                posAttributes++;
            }
            return ret;
        }

        private void addSpan(List<AAtom> outHTML, List<string> openTags, List<HREF_Attributes> openHREFs)
        {
            var atom = new SpanAtom();
            atom.Alignment = getAlignmentFromOpenTags(openTags);

            if(openHREFs.Count > 0)
            {
                atom.HREFAttributes = openHREFs[openHREFs.Count - 1];
            }

            outHTML.Add(atom);
        }

        private void addGumpImage(List<AAtom> outHTML, List<string> openTags, List<HREF_Attributes> openHREFs)
        {
            var atom = new ImageAtom(-1);
            atom.Alignment = getAlignmentFromOpenTags(openTags);

            if(openHREFs.Count > 0)
            {
                atom.HREFAttributes = openHREFs[openHREFs.Count - 1];
            }

            outHTML.Add(atom);
        }

        private void addCharacter(char inText, List<AAtom> outHTML, List<string> openTags, Color currentColor, List<HREF_Attributes> openHREFs)
        {
            var c = new CharacterAtom(inText);
            c.Style_IsBold = hasTag(openTags, "b");
            c.Style_IsItalic = hasTag(openTags, "i");
            c.Style_IsUnderlined = hasTag(openTags, "u");
            c.Style_IsOutlined = hasTag(openTags, "outline");
            var fontTag = lastTag(openTags, new[] {"big", "small", "medium", "basefont"});
            switch(fontTag)
            {
                case "big":
                    c.Font = Fonts.Big;
                    break;
                case "medium":
                    c.Font = Fonts.Default;
                    break;
                case "small":
                    c.Font = Fonts.Small;
                    break;
            }

            c.Alignment = getAlignmentFromOpenTags(openTags);
            c.Color = currentColor;
            if(openHREFs.Count > 0)
            {
                c.HREFAttributes = openHREFs[openHREFs.Count - 1];
            }
            outHTML.Add(c);
        }

        private static Alignments getAlignmentFromOpenTags(List<string> openTags)
        {
            var alignment = lastTag(openTags, alignmentTags);
            switch(alignment)
            {
                case "center":
                    return Alignments.Center;
                case "left":
                    return Alignments.Left;
                case "right":
                    return Alignments.Right;
            }
            return Alignments.Default;
        }

        private static bool hasTag(List<string> tags, string tag)
        {
            if(tags.IndexOf(tag) != -1)
            {
                return true;
            }
            return false;
        }

        private static void editOpenTags(List<string> tags, bool isClosing, string tag)
        {
            if(!isClosing)
            {
                tags.Add(tag);
            }
            else
            {
                var i = tags.LastIndexOf(tag);
                if(i != -1)
                {
                    tags.RemoveAt(i);
                }
            }
        }

        private static string lastTag(List<string> tags, string[] checkTags)
        {
            var index = int.MaxValue;
            var tag = string.Empty;

            for(var i = 0; i < checkTags.Length; i++)
            {
                var j = tags.LastIndexOf(checkTags[i]);
                if(j != -1 && j < index)
                {
                    index = tags.LastIndexOf(checkTags[i]);
                    tag = checkTags[i];
                }
            }

            return tag;
        }

        private string readHTMLTag(string text, ref int i, ref bool isClosingTag)
        {
            string tag;

            if(text.Length == i + 1)
            {
                tag = text.Substring(i, 1);
                i = i + 1;
                return tag;
            }

            if(text[i + 1] == '/')
            {
                i = i + 1;
                isClosingTag = true;
            }

            var closingBracket = text.IndexOf('>', i);
            if(closingBracket == -1)
            {
                tag = text.Substring(i, 1);
                i = i + 1;
                return tag;
            }

            tag = text.Substring(i + 1, closingBracket - i - 1);
            i = closingBracket;
            return tag;
        }
    }
}