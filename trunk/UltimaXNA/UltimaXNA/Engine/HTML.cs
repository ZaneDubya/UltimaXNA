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
        public bool IsUnderlined = false;
        public enumHTMLFonts Font = enumHTMLFonts.Default;
        public enumHTMLAlignments Alignment = enumHTMLAlignments.Default;
        public Color Color = Color.White;
        public string HREF = string.Empty;

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

        public HTMLReader(string inText)
        {
            _characters = decodeHTML(inText);
        }

        List<HTMLCharacter> decodeHTML(string inText)
        {
            List<HTMLCharacter> outText = new List<HTMLCharacter>();
            List<string> openTags = new List<string>();
            Color currentColor = Color.White;
            List<string> openHREFs = new List<string>();

            // search for tags
            for (int i = 0; i < inText.Length; i++)
            {
                if (inText[i] == '<')
                {
                    bool isClosing = false;
                    string tag = readHTMLTag(inText, ref i, ref isClosing);
                    switch (tag)
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
                        default:
                            // some tags have additional data, so we must parse them seperately.
                            if (tag.Length >= 1 && tag.StartsWith("A"))
                            {
                                if (!isClosing)
                                {
                                    // hyperlink with subtags
                                    string[] subtags = tag.Split(' ');
                                    // go through each tag
                                    for (int j = 1; j < subtags.Length; j++)
                                    {
                                        string subtag = subtags[j];
                                        if (subtag.Length > 6 && subtag.StartsWith("HREF=\""))
                                        {
                                            // href tag: HREF="address"
                                            int openQuotes = subtag.IndexOf('\"');
                                            int closeQuotes = subtag.IndexOf('\"', openQuotes + 1);
                                            string hrefAddress = subtag.Substring(openQuotes + 1, closeQuotes - openQuotes - 1);
                                            openHREFs.Add(hrefAddress);
                                        }
                                    }
                                }
                                else
                                {
                                    // closing a hyperlink - restore previous address.
                                    if (openHREFs.Count > 0)
                                    {
                                        openHREFs.RemoveAt(openHREFs.Count - 1);
                                    }
                                }
                            }
                            else if (tag.Length > 8 && tag.StartsWith("BASEFONT"))
                            {
                                // basefont with subtags
                                string[] subtags = tag.Split(' ');
                                // go through each tag
                                for (int j = 1; j < subtags.Length; j++)
                                {
                                    string subtag = subtags[j];
                                    if (subtag.Length == 13 && subtag.StartsWith("COLOR=#"))
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
                                // unknown tag
                            }
                            break;
                    }
                }
                else
                {
                    // What's left: alignment (center, left, right), div, strong, couple others. Beyond that, extended html? Not yet.
                    HTMLCharacter c = new HTMLCharacter(inText[i]);
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
                    c.Color = currentColor;
                    if (openHREFs.Count > 0)
                        c.HREF = openHREFs[openHREFs.Count - 1];
                    outText.Add(c);
                }
            }

            return outText;
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
            return tag.ToUpper();
        }
    }

    public class HREFRegions
    {
        List<HREFRegion> _regions = new List<HREFRegion>();

        public void AddRegion(Rectangle r, string href)
        {
            _regions.Add(new HREFRegion(r, href, _regions.Count));
        }

        public HREFRegion HREFfromPoint(Point p)
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
    }

    public class HREFRegion
    {
        public Rectangle Area;
        public string HREF;
        public int Index;

        public HREFRegion(Rectangle r, string h, int i)
        {
            Area = r;
            HREF = h;
            Index = i;
        }
    }
}
