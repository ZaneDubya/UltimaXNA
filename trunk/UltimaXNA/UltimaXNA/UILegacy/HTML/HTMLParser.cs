using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics;

namespace UltimaXNA.UILegacy.HTML
{
    public class HTMLParser
    {
        List<HTMLParser_Atom> _atoms;
        public List<HTMLParser_Atom> Atoms
        {
            get
            {
                return _atoms;
            }
        }

        public string Text
        {
            get
            {
                string text = string.Empty;
                for (int i = 0; i < _atoms.Count; i++)
                {
                    text += _atoms[i].ToString();
                }
                return text;
            }
        }

        public int Length
        {
            get
            {
                return _atoms.Count;
            }
        }

        public HTMLParser(string inText, bool parseHTML)
        {
            _atoms = decodeText(inText, parseHTML);
        }

        private List<HTMLParser_Atom> decodeText(string inText, bool parseHTML)
        {
            List<HTMLParser_Atom> outAtoms = new List<HTMLParser_Atom>();
            List<string> openTags = new List<string>();
            Color currentColor = Color.White;
            List<HREF_Attributes> openHREFs = new List<HREF_Attributes>();

            // if this is not HTML, do not parse tags. Otherwise search out and interpret tags.
            if (!parseHTML)
            {
                for (int i = 0; i < inText.Length; i++)
                {
                    addCharacter(inText[i], outAtoms, openTags, currentColor, openHREFs);
                }
            }
            else
            {
                Parsing.HTMLparser parser = new Parsing.HTMLparser(inText);
                Parsing.HTMLchunk chunk;
                while ((chunk = parser.ParseNext()) != null)
                {
                    if (!(chunk.oHTML == ""))
                    {
                        // this is text. add the characters to the outText list.
                        for (int i = 0; i < chunk.oHTML.Length; i++)
                            addCharacter(chunk.oHTML[i], outAtoms, openTags, currentColor, openHREFs);
                    }
                    else
                    {
                        // this is a tag. interpret the tag and edit the openTags list.
                        bool isClosing = chunk.bClosure;
                        switch (chunk.sTag)
                        {
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
                                if (isClosing)
                                {
                                    // closing a hyperlink - restore previous address, if any.
                                    if (openHREFs.Count > 0)
                                        openHREFs.RemoveAt(openHREFs.Count - 1);
                                }
                                else
                                {
                                    // hyperlink with attributes
                                    HREF_Attributes href = new HREF_Attributes();
                                    openHREFs.Add(href);
                                }
                                break;
                            default:
                                Logger.Warn(string.Format("Unknown html tag:{0}", chunk.sTag));
                                break;
                        }

                        foreach (DictionaryEntry param in chunk.oParams)
                        {
                            string key = param.Key.ToString();
                            string value = param.Value.ToString();
                            if (value.EndsWith("/"))
                                value = value.Substring(0, value.Length - 1);

                            switch (key)
                            {
                                case "href":
                                    if (chunk.sTag == "a")
                                    {
                                        openHREFs[openHREFs.Count - 1].HREF = value;
                                    }
                                    else
                                    {
                                        Logger.Warn("href paramater used outside of an 'a' tag link. href is ignored in this case.");
                                    }
                                    break;
                                case "color":
                                case "hovercolor":
                                case "activecolor":
                                    // get the color!
                                    string color = value;
                                    if (color[0] == '#')
                                        color = color.Substring(1);
                                    if (color.Length == 3 || color.Length == 6)
                                    {
                                        Color c = Utility.ColorFromHexString(color);
                                        if (key == "color")
                                            currentColor = c;
                                        if (chunk.sTag == "a")
                                        {
                                            switch (key)
                                            {
                                                case "color":
                                                    openHREFs[openHREFs.Count - 1].UpHue = Data.HuesXNA.GetWebSafeHue(c);
                                                    break;
                                                case "hovercolor":
                                                    openHREFs[openHREFs.Count - 1].OverHue = Data.HuesXNA.GetWebSafeHue(c);
                                                    break;
                                                case "activecolor":
                                                    openHREFs[openHREFs.Count - 1].DownHue = Data.HuesXNA.GetWebSafeHue(c);
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                        Logger.Warn("Improperly formatted color:" + color);
                                    break;
                                case "text-decoration":
                                    switch (value)
                                    {
                                        case "none":
                                            if (chunk.sTag == "a")
                                                openHREFs[openHREFs.Count - 1].Underline = false;
                                            break;
                                        default:
                                            Logger.Warn(string.Format("Unknown text-decoration:{0}", value));
                                            break;
                                    }
                                    break;
                                case "src":
                                case "hoversrc":
                                case "activesrc":
                                    switch (chunk.sTag)
                                    {
                                        case "gumpimg":
                                            if (key == "src")
                                                ((HTMLParser_AtomImageGump)outAtoms[outAtoms.Count - 1]).Value = int.Parse(value);
                                            else if (key == "hoversrc")
                                                ((HTMLParser_AtomImageGump)outAtoms[outAtoms.Count - 1]).ValueOver = int.Parse(value);
                                            else if (key == "activesrc")
                                                ((HTMLParser_AtomImageGump)outAtoms[outAtoms.Count - 1]).ValueDown = int.Parse(value);
                                            break;
                                        default:
                                            Logger.Warn("src param encountered within " + chunk.sTag + " which does not use this param.");
                                            break;
                                    }
                                    break;
                                case "width":
                                    switch (chunk.sTag)
                                    {
                                        case "gumpimg":
                                        case "span":
                                            outAtoms[outAtoms.Count - 1].Width = int.Parse(value);
                                            break;
                                        default:
                                            Logger.Warn("width param encountered within " + chunk.sTag + " which does not use this param.");
                                            break;
                                    }
                                    break;
                                case "height":
                                    switch (chunk.sTag)
                                    {
                                        case "gumpimg":
                                        case "span":
                                            outAtoms[outAtoms.Count - 1].Width = int.Parse(value);
                                            break;
                                        default:
                                            Logger.Warn("height param encountered within " + chunk.sTag + " which does not use this param.");
                                            break;
                                    }
                                    break;
                                default:
                                    Logger.Warn(string.Format("Unknown parameter:{0}", key));
                                    break;
                            }
                        }
                    }
                }
            }

            return outAtoms;
        }

        string getTextFromWithinQuotes(string tag)
        {
            int openQuotes = tag.IndexOf('\"');
            int closeQuotes = tag.IndexOf('\"', openQuotes + 1);
            string text = tag.Substring(openQuotes + 1, closeQuotes - openQuotes - 1);
            return text;
        }

        int getHueFromString(string hue)
        {
            int h = int.Parse(hue);
            // we can't hue real colors, we must color with hues.
            if (h > 2998 || hue.Length > 4)
                return -1;
            else
                return h;
        }

        List<string> getAttributesFromTag(string tag)
        {
            List<string> ret = new List<string>();
            int posAttributes = tag.IndexOf(' ') + 1;
            while (posAttributes < tag.Length)
            {
                if (tag[posAttributes] != ' ')
                {
                    // read in an attribute
                    bool isInQuotes = false;
                    int tagStart = posAttributes;
                    char[] attr = new char[tag.Length - posAttributes];
                    while (posAttributes < tag.Length && (tag[posAttributes] != ' ' || isInQuotes))
                    {
                        attr[posAttributes - tagStart] = tag[posAttributes];
                        if (tag[posAttributes] == '\"')
                            isInQuotes = Utility.ToggleBoolean(isInQuotes);
                        posAttributes++;
                    }
                    string attribute = (posAttributes - tagStart == attr.Length) ? new string(attr) : new string(attr).Remove(posAttributes - tagStart);
                    ret.Add(attribute);
                }
                posAttributes++;
            }
            return ret;
        }

        void addSpan(List<HTMLParser_Atom> outHTML, List<string> openTags, List<HREF_Attributes> openHREFs)
        {
            HTMLParser_AtomSpan atom = new HTMLParser_AtomSpan();
            atom.Alignment = getAlignmentFromOpenTags(openTags);

            if (openHREFs.Count > 0)
                atom.HREFAttributes = openHREFs[openHREFs.Count - 1];

            outHTML.Add(atom);
        }

        void addGumpImage(List<HTMLParser_Atom> outHTML, List<string> openTags, List<HREF_Attributes> openHREFs)
        {
            HTMLParser_AtomImageGump atom = new HTMLParser_AtomImageGump(-1);
            atom.Alignment = getAlignmentFromOpenTags(openTags);

            if (openHREFs.Count > 0)
                atom.HREFAttributes = openHREFs[openHREFs.Count - 1];

            outHTML.Add(atom);
        }

        void addCharacter(char inText, List<HTMLParser_Atom> outHTML, List<string> openTags, Color currentColor, List<HREF_Attributes> openHREFs)
        {
            HTMLParser_AtomCharacter c = new HTMLParser_AtomCharacter(inText);
            c.Style_IsBold = hasTag(openTags, "b");
            c.Style_IsItalic = hasTag(openTags, "i");
            c.Style_IsUnderlined = hasTag(openTags, "u");
            string fontTag = lastTag(openTags, new string[] { "big", "small", "medium", "basefont" });
            switch (fontTag)
            {
                case "big":
                    c.Font = enumHTMLFonts.Big;
                    break;
                case "medium":
                    c.Font = enumHTMLFonts.Default;
                    break;
                case "small":
                    c.Font = enumHTMLFonts.Small;
                    break;
            }

            c.Alignment = getAlignmentFromOpenTags(openTags);
            c.Color = currentColor;
            if (openHREFs.Count > 0)
                c.HREFAttributes = openHREFs[openHREFs.Count - 1];
            outHTML.Add(c);
        }

        static string[] alignmentTags = new string[] { "center", "left", "right" };
        static enumHTMLAlignments getAlignmentFromOpenTags(List<string> openTags)
        {
            string alignment = lastTag(openTags, alignmentTags);
            switch (alignment)
            {
                case "center":
                    return enumHTMLAlignments.Center;
                case "left":
                    return enumHTMLAlignments.Left;
                case "right":
                    return enumHTMLAlignments.Right;
            }
            return enumHTMLAlignments.Default;
        }

        static bool hasTag(List<string> tags, string tag)
        {
            if (tags.IndexOf(tag) != -1)
                return true;
            else
                return false;
        }

        static void editOpenTags(List<string> tags, bool isClosing, string tag)
        {
            if (!isClosing)
            {
                tags.Add(tag);
            }
            else
            {
                int i = tags.LastIndexOf(tag);
                if (i != -1)
                {
                    tags.RemoveAt(i);
                }
            }
        }

        static string lastTag(List<string> tags, string[] checkTags)
        {
            int index = int.MaxValue;
            string tag = string.Empty;

            for (int i = 0; i < checkTags.Length; i++)
            {
                int j = tags.LastIndexOf(checkTags[i]);
                if (j != -1 && j < index)
                {
                    index = tags.LastIndexOf(checkTags[i]);
                    tag = checkTags[i];
                }
            }

            return tag;
        }

        string readHTMLTag(string text, ref int i, ref bool isClosingTag)
        {
            string tag;

            if (text.Length == i + 1)
            {
                tag = text.Substring(i, 1);
                i = i + 1;
                return tag;
            }

            if (text[i + 1] == '/')
            {
                i = i + 1;
                isClosingTag = true;
            }

            int closingBracket = text.IndexOf('>', i);
            if (closingBracket == -1)
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
