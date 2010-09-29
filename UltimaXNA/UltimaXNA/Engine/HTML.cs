using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UltimaXNA
{
    public enum enumHTMLAlignments
    {
        Default = 0,
        Left = 0,
        Center = 1,
        Right = 2
    }

    public enum enumHTMLFonts
    {
        Default = 1,
        Big = 0,
        Medium = 1,
        Small = 2
    }

    public class HTMLCharacter
    {
        public char Character = ' ';
        public bool IsBold = false;
        public bool IsItalic = false;
        bool _isUnderlined = false;
        public bool IsUnderlined
        {
            get
            {
                if (HREF != null)
                {
                    return HREF.Underline;
                }
                else
                {
                    return _isUnderlined;
                }
            }
            set { _isUnderlined = value; }
        }
        public enumHTMLFonts Font = enumHTMLFonts.Default;
        public enumHTMLAlignments Alignment = enumHTMLAlignments.Default;
        public Color Color = Color.White;
        public HREFDescription HREF = null;
        public bool IsHREF { get { return HREF != null; } }

        public HTMLCharacter(char character)
        {
            Character = character;
        }
    }

    public class HTMLReader
    {
        List<HTMLCharacter> _characters;
        public List<HTMLCharacter> Characters
        {
            get
            {
                return _characters;
            }
        }

        public string Text
        {
            get
            {
                string text = string.Empty;
                for (int i = 0; i < _characters.Count; i++)
                {
                    text += _characters[i].Character;
                }
                return text;
            }
        }

        public int Length
        {
            get
            {
                return _characters.Count;
            }
        }

        public HTMLReader(string inText, bool parseHTML)
        {
            _characters = decodeText(inText, parseHTML);
        }

        private List<HTMLCharacter> decodeText(string inText, bool parseHTML)
        {
            List<HTMLCharacter> outText = new List<HTMLCharacter>();
            List<string> openTags = new List<string>();
            Color currentColor = Color.White;
            List<HREFDescription> openHREFs = new List<HREFDescription>();

            // if this is not HTML, do not parse tags. Otherwise search out and interpret tags.
            for (int i = 0; i < inText.Length; i++)
            {
                if ((inText[i] == '<') && parseHTML)
                {
                    bool isClosing = false;
                    string tag = readHTMLTag(inText, ref i, ref isClosing);
                    // Tags currently unimplemented but which we should include:
                    // div, strong, perhaps a couple of others. Beyond that, extended html? Not yet.
                    switch (tag.ToUpper())
                    {
                        case "BR":
                            HTMLCharacter c = new HTMLCharacter('\n');
                            outText.Add(c);
                            break;
                        case "B":
                            editOpenTags(openTags, isClosing, "b");
                            break;
                        case "I":
                            editOpenTags(openTags, isClosing, "i");
                            break;
                        case "U":
                            editOpenTags(openTags, isClosing, "u");
                            break;
                        case "BIG":
                            editOpenTags(openTags, isClosing, "big");
                            break;
                        case "SMALL":
                            editOpenTags(openTags, isClosing, "small");
                            break;
                        case "CENTER":
                            editOpenTags(openTags, isClosing, "center");
                            break;
                        case "LEFT":
                            editOpenTags(openTags, isClosing, "left");
                            break;
                        case "RIGHT":
                            editOpenTags(openTags, isClosing, "right");
                            break;
                        default:
                            // some tags have additional data, so we must parse them seperately.
                            if (tag.Length >= 1 && tag.StartsWith("A", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (!isClosing)
                                {
                                    // hyperlink with attributes
                                    HREFDescription href = new HREFDescription();
                                    List<string> attributes = getAttributesFromTag(tag);
                                    for (int j = 0; j < attributes.Count; j++)
                                    {
                                        string attributeName = attributes[j].Substring(0, attributes[j].IndexOf('='));
                                        if (attributeName.ToUpper() == "HREF")
                                        {
                                            // href tag: href="address"
                                            href.HREF = getTextFromWithinQuotes(attributes[j]);
                                        }
                                        else if (attributeName.ToUpper() == "STYLE")
                                        {
                                            string style = getTextFromWithinQuotes(attributes[j]);
                                            string[] styles = style.Split(';');
                                            for (int k = 0; k < styles.Length; k++)
                                            {
                                                if (styles[k] == string.Empty)
                                                    continue;
                                                string styleType = styles[k].Substring(0, styles[k].IndexOf(':')).Trim();
                                                if (styleType == "colorhue")
                                                {
                                                    int hue = getHueFromString(styles[k].Substring(styles[k].IndexOf('#') + 1));
                                                    if (hue != -1)
                                                        href.UpHue = hue;
                                                }
                                                else if (styleType == "hoverhue")
                                                {
                                                    int hue = getHueFromString(styles[k].Substring(styles[k].IndexOf('#') + 1));
                                                    if (hue != -1)
                                                        href.OverHue = hue;
                                                }
                                                else if (styleType == "activatehue")
                                                {
                                                    int hue = getHueFromString(styles[k].Substring(styles[k].IndexOf('#') + 1));
                                                    if (hue != -1)
                                                        href.DownHue = hue;
                                                }
                                                else if (styleType == "text-decoration")
                                                {
                                                    string decoration = styles[k].Substring(styles[k].IndexOf(':') + 1).Trim();
                                                    if (decoration == "none")
                                                    {
                                                        href.Underline = false;
                                                    }
                                                    else
                                                    {
                                                        // unknown decoration / combination of decorations.
                                                    }
                                                }
                                                else
                                                {
                                                    // unknown style type!
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // unknown attribute!
                                        }
                                    }
                                    openHREFs.Add(href);
                                }
                                else
                                {
                                    // closing a hyperlink - restore previous address, if any.
                                    if (openHREFs.Count > 0)
                                        openHREFs.RemoveAt(openHREFs.Count - 1);
                                }
                            }
                            else if (tag.Length > 8 && tag.StartsWith("BASEFONT", StringComparison.InvariantCultureIgnoreCase))
                            {
                                // basefont with subtags. Should recode this to use the new getAttributesFromTag() routine.
                                string[] subtags = tag.Split(' ');
                                // go through each tag
                                for (int j = 1; j < subtags.Length; j++)
                                {
                                    string subtag = subtags[j];
                                    if (subtag.Length == 13 && subtag.StartsWith("COLOR=#", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        // get the color! we expect a six character color, which is why
                                        // we specified .Length == 13 in the above if statement.
                                        string color = subtag.Substring(7, subtag.Length - 7);
                                        // convert the hex values to colors
                                        currentColor = Utility.ColorFromHexString(color);
                                    }
                                }
                            }
                            else
                            {
                                // unknown tag. Just include it in the text (there may well be a better way to do this).
                                addCharacter('<', outText, openTags, currentColor, openHREFs);
                                if (isClosing)
                                    addCharacter('/', outText, openTags, currentColor, openHREFs);
                                for (int j = 0; j < tag.Length; j++)
                                    addCharacter(tag[j], outText, openTags, currentColor, openHREFs);
                                addCharacter('>', outText, openTags, currentColor, openHREFs);

                            }
                            break;
                    }
                }
                else
                {
                    addCharacter(inText[i], outText, openTags, currentColor, openHREFs);
                }
            }
            return outText;
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

        void addCharacter(char inText, List<HTMLCharacter> outText, List<string> openTags, Color currentColor, List<HREFDescription> openHREFs)
        {
            HTMLCharacter c = new HTMLCharacter(inText);
            c.IsBold = hasTag(openTags, "b");
            c.IsItalic = hasTag(openTags, "i");
            c.IsUnderlined = hasTag(openTags, "u");
            string fontTag = lastTag(openTags, new string[] { "big", "small" });
            switch (fontTag)
            {
                case "big":
                    c.Font = enumHTMLFonts.Big;
                    break;
                case "small":
                    c.Font = enumHTMLFonts.Small;
                    break;
            }
            string alignment = lastTag(openTags, new string[] { "center", "left", "right" });
            switch (alignment)
            {
                case "center":
                    c.Alignment = enumHTMLAlignments.Center;
                    break;
                case "left":
                    c.Alignment = enumHTMLAlignments.Left;
                    break;
                case "right":
                    c.Alignment = enumHTMLAlignments.Right;
                    break;
            }
            c.Color = currentColor;
            if (openHREFs.Count > 0)
                c.HREF = openHREFs[openHREFs.Count - 1];
            outText.Add(c);
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
            if (text[i + 1] == '/')
            {
                i = i + 1;
                isClosingTag = true;
            }

            int closingBracket = text.IndexOf('>', i);
            string tag = text.Substring(i + 1, closingBracket - i - 1);
            i = closingBracket;
            return tag;
        }
    }

    public class HREFRegions
    {
        List<HREFRegion> _regions = new List<HREFRegion>();

        public List<HREFRegion> Regions
        {
            get
            {
                return _regions;
            }
        }

        public int Count
        {
            get { return _regions.Count; }
        }

        public void AddRegion(Rectangle r, HREFDescription href)
        {
            _regions.Add(new HREFRegion(r, _regions.Count, href));
        }

        public HREFRegion RegionfromPoint(Point p)
        {
            int index = -1;
            for (int i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].Area.Contains(p))
                    index = i;
            }
            if (index == -1)
                return null;
            else
                return _regions[index];
        }

        public HREFRegion Region(int index)
        {
            return _regions[index];
        }
    }

    public class HREFRegion
    {
        public Rectangle Area;
        public int Index;
        public HREFDescription Data;

        public HREFRegion(Rectangle r, int i, HREFDescription data)
        {
            Area = r;
            Data = data;
            Index = i;
        }
    }

    public class HREFDescription
    {
        public string HREF;
        public int UpHue = 1;
        public int OverHue = 31;
        public int DownHue = 11;
        public bool Underline = true;
    }
}
