using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.UI.HTML.Parsing;

namespace UltimaXNA.Core.UI.HTML
{
    /// <summary>
    /// Style manager. As you parse html tags, add them to a manager TagCollection.
    /// Then, when you add an element, use the manager to generate an object that will contain all the styles for that element.
    /// </summary>
    public class StyleManager
    {
        public StyleState Style;

        private IUIResourceProvider m_Provider;
        private List<HTMLchunk> m_OpenTags;
        private List<HREFAttributes> m_HREFs;
        private HREFAttributes LastHREF
        {
            get
            {
                if (m_HREFs == null)
                    return null;
                if (m_HREFs.Count == 0)
                    return null;
                return m_HREFs[m_HREFs.Count - 1];
            }
        }

        public StyleManager(IUIResourceProvider provider)
        {
            m_Provider = provider;
            m_OpenTags = new List<HTMLchunk>();
            m_HREFs = new List<HREFAttributes>();
            RecalculateStyle();
        }

        public void OpenTag(HTMLchunk chunk)
        {
            if (!chunk.bClosure || chunk.bEndClosure)
            {
                m_OpenTags.Add(chunk);
                ParseTag(chunk);
            }
            else
            {
                CloseOneTag(chunk);
            }
        }

        public void CloseOneTag(HTMLchunk chunk)
        {
            bool bMustRecalculateStyle = false;

            for (int i = m_OpenTags.Count - 1; i >= 0; i--)
            {
                if (m_OpenTags[i].sTag == chunk.sTag)
                {
                    m_OpenTags.RemoveAt(i);
                    bMustRecalculateStyle = true;
                    break;
                }
            }

            if (bMustRecalculateStyle)
            {
                RecalculateStyle();
            }
        }

        public void CloseAnySoloTags()
        {
            bool bMustRecalculateStyle = false;

            for (int i = 0; i < m_OpenTags.Count; i++)
            {
                if (m_OpenTags[i].bEndClosure)
                {
                    m_OpenTags.RemoveAt(i);
                    bMustRecalculateStyle = true;
                    i--;
                }
            }

            if (bMustRecalculateStyle)
            {
                RecalculateStyle();
            }
        }

        public void InterpretHREF(HTMLchunk chunk)
        {
            if (chunk.bEndClosure)
            {
                // solo anchors are meaningless.
            }

            if (!chunk.bClosure)
            {
                // hyperlink with attributes
                HREFAttributes href = new HREFAttributes();
                m_HREFs.Add(href);
                ParseTag(chunk);
            }
            else
            {
                // closing a hyperlink - restore previous address, if any.
                if (m_HREFs.Count > 0)
                    m_HREFs.RemoveAt(m_HREFs.Count - 1);
            }
        }

        private void RecalculateStyle()
        {
            Style = new StyleState(m_Provider);
            for (int i = 0; i < m_OpenTags.Count; i++)
            {
                ParseTag(m_OpenTags[i]);
            }
        }

        private void ParseTag(HTMLchunk chunk)
        {
            switch (chunk.sTag)
            {
                case "b":
                    Style.IsBold = true;
                    break;
                case "i":
                    Style.IsItalic = true;
                    break;
                case "u":
                    Style.IsUnderlined = true;
                    break;
                case "outline":
                    Style.IsOutlined = true;
                    break;
                case "big":
                    Style.Font = m_Provider.GetUnicodeFont((int)Fonts.UnicodeBig);
                    break;
                case "basefont":
                case "medium":
                    Style.Font = m_Provider.GetUnicodeFont((int)Fonts.UnicodeMedium);
                    break;
                case "small":
                    Style.Font = m_Provider.GetUnicodeFont((int)Fonts.UnicodeSmall);
                    break;
                case "left":
                    Style.Alignment = Alignments.Left;
                    break;
                case "center":
                    Style.Alignment = Alignments.Center;
                    break;
                case "right":
                    Style.Alignment = Alignments.Right;
                    break;
            }

            foreach (DictionaryEntry param in chunk.oParams)
            {
                // get key and value for this tag param
                string key = param.Key.ToString();
                string value = param.Value.ToString();
                // trim trailing forward slash.
                if (value.EndsWith("/"))
                    value = value.Substring(0, value.Length - 1);

                switch (key)
                {
                    case "href":
                        // href paramater can only be used on 'anchor' tags.
                        if (chunk.sTag == "a")
                        {
                            m_HREFs[m_HREFs.Count - 1].HREF = value;
                        }
                        break;
                    case "color":
                    case "hovercolor":
                    case "activecolor":
                        // get the color!
                        string color = value;
                        Color? c = null;
                        if (color[0] == '#')
                        {
                            color = color.Substring(1);
                            if (color.Length == 3 || color.Length == 6)
                            {
                                c = Utility.ColorFromHexString(color);
                            }
                        }
                        else
                        {
                            //try to parse color by name
                            c = Utility.ColorFromString(color);
                        }

                        if (c.HasValue)
                        {
                            if (key == "color")
                                Style.Color = c.Value;
                            if (chunk.sTag == "a")
                            {
                                switch (key)
                                {
                                    case "color":
                                        m_HREFs[m_HREFs.Count - 1].UpHue = m_Provider.GetWebSafeHue(c.Value);
                                        break;
                                    case "hovercolor":
                                        m_HREFs[m_HREFs.Count - 1].OverHue = m_Provider.GetWebSafeHue(c.Value);
                                        break;
                                    case "activecolor":
                                        m_HREFs[m_HREFs.Count - 1].DownHue = m_Provider.GetWebSafeHue(c.Value);
                                        break;
                                }
                            }
                        }
                        else
                            Tracer.Warn("Improperly formatted color:" + color);
                        break;
                    case "src":
                    case "hoversrc":
                    case "activesrc":
                        switch (chunk.sTag)
                        {
                            case "gumpimg":
                                if (key == "src")
                                    Style.GumpImgSrc = int.Parse(value);
                                else if (key == "hoversrc")
                                    Style.GumpImgSrcOver = int.Parse(value);
                                else if (key == "activesrc")
                                    Style.GumpImgSrcDown = int.Parse(value);
                                break;
                            default:
                                Tracer.Warn("src param encountered within " + chunk.sTag + " which does not use this param.");
                                break;
                        }
                        break;
                    case "width":
                        switch (chunk.sTag)
                        {
                            case "gumpimg":
                            case "span":
                                Style.ElementWidth = int.Parse(value);
                                break;
                            default:
                                Tracer.Warn("width param encountered within " + chunk.sTag + " which does not use this param.");
                                break;
                        }
                        break;
                    case "height":
                        switch (chunk.sTag)
                        {
                            case "gumpimg":
                            case "span":
                                Style.ElementHeight = int.Parse(value);
                                break;
                            default:
                                Tracer.Warn("height param encountered within " + chunk.sTag + " which does not use this param.");
                                break;
                        }
                        break;
                    case "style":
                        ParseStyle(value);
                        break;
                    default:
                        Tracer.Warn(string.Format("Unknown parameter:{0}", key));
                        break;
                }
            }
        }

        private void ParseStyle(string css)
        {
            if (css.Length == 0)
                return;

            string key = string.Empty;
            string value = string.Empty;
            bool inKey = true;
            for (int i = 0; i < css.Length; i++)
            {
                char ch = css[i];
                if (ch == ':' || ch == '=')
                {
                    if (inKey)
                        inKey = false;
                    else
                    {
                        Tracer.Warn(string.Format("Uninterpreted, possibly malformed style parameter:{0}", css));
                        return;
                    }
                }
                else if (ch == ';')
                {
                    if (!inKey)
                    {
                        ParseOneStyle(key, value);
                        key = string.Empty;
                        value = string.Empty;
                        inKey = true;
                    }
                    else
                    {
                        Tracer.Warn(string.Format("Uninterpreted, possibly malformed style parameter:{0}", css));
                        return;
                    }
                }
                else
                {
                    if (inKey)
                        key += ch;
                    else
                        value += ch;
                }
            }

            if (key != string.Empty && value != string.Empty)
            {
                ParseOneStyle(key, value);
            }
        }

        private void ParseOneStyle(string key, string value)
        {
            switch (key.ToLower().Trim())
            {
                case "font-family":
                    if (value.StartsWith("ascii"))
                    {
                        int index;
                        if (int.TryParse(value.Replace("ascii", string.Empty), out index))
                            Style.Font = m_Provider.GetAsciiFont(index);
                        else
                            Tracer.Warn("Unknown font-family parameter:{0}", key);
                    }
                    else if (value.StartsWith("uni"))
                    {
                        int index;
                        if (int.TryParse(value.Replace("uni", string.Empty), out index))
                            Style.Font = m_Provider.GetUnicodeFont(index);
                        else
                            Tracer.Warn("Unknown font-family parameter:{0}", key);
                    }
                    break;
                default:
                    Tracer.Warn("Unknown style parameter:{0}", key);
                    break;
            }
        }
    }
}
