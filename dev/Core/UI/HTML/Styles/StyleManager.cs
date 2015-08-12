/***************************************************************************
 *   StyleManager.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.UI.HTML.Parsing;
using UltimaXNA.Core.Resources;
#endregion

namespace UltimaXNA.Core.UI.HTML.Styles
{
    /// <summary>
    /// Style manager. As you parse html tags, add them to a manager TagCollection.
    /// Then, when you add an element, use the manager to generate an object that will contain all the styles for that element.
    /// </summary>
    public class StyleManager
    {
        public StyleState Style;

        private IResourceProvider m_Provider;
        private List<OpenTag> m_OpenTags;

        public StyleManager(IResourceProvider provider)
        {
            m_Provider = provider;
            m_OpenTags = new List<OpenTag>();
            RecalculateStyle();
        }

        public void OpenTag(HTMLchunk chunk)
        {
            OpenTag tag = new OpenTag(chunk);

            if (!chunk.bClosure || chunk.bEndClosure)
            {
                m_OpenTags.Add(tag);
                ParseTag(tag);
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
                // solo anchor elements are meaningless.
            }

            if (!chunk.bClosure)
            {
                // hyperlink with attributes
                Style.HREF = new HREFAttributes();
                OpenTag tag = new OpenTag(chunk);
                ParseTag(tag);
            }
            else
            {
                // closing a hyperlink. NOTE: Recalculating the styles will NOT restore the previous link. Is this worth fixing?
                RecalculateStyle();
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

        private void ParseTag(OpenTag tag)
        {
            switch (tag.sTag)
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

            foreach (DictionaryEntry param in tag.oParams)
            {
                // get key and value for this tag param
                string key = param.Key.ToString();
                string value = param.Value.ToString();
                if (value.StartsWith("0x"))
                    value = Utility.ToInt32(value).ToString();
                // trim trailing forward slash.
                if (value.EndsWith("/"))
                    value = value.Substring(0, value.Length - 1);

                switch (key)
                {
                    case "href":
                        // href paramater can only be used on 'anchor' tags.
                        if (tag.sTag == "a")
                        {
                            Style.HREF.HREF = value;
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
                            if (tag.sTag == "a")
                            {
                                switch (key)
                                {
                                    case "color":
                                        Style.HREF.UpHue = m_Provider.GetWebSafeHue(c.Value);
                                        break;
                                    case "hovercolor":
                                        Style.HREF.OverHue = m_Provider.GetWebSafeHue(c.Value);
                                        break;
                                    case "activecolor":
                                        Style.HREF.DownHue = m_Provider.GetWebSafeHue(c.Value);
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
                        switch (tag.sTag)
                        {
                            case "gumpimg":
                            case "itemimg":
                                if (key == "src")
                                    Style.ImgSrc = int.Parse(value);
                                else if (key == "hoversrc")
                                    Style.ImgSrcOver = int.Parse(value);
                                else if (key == "activesrc")
                                    Style.ImgSrcDown = int.Parse(value);
                                break;
                            default:
                                Tracer.Warn("{0} param encountered within {1} tag which does not use this param.", key, tag.sTag);
                                break;
                        }
                        break;
                    case "width":
                        switch (tag.sTag)
                        {
                            case "gumpimg":
                            case "itemimg":
                            case "span":
                                Style.ElementWidth = int.Parse(value);
                                break;
                            default:
                                Tracer.Warn("width param encountered within " + tag.sTag + " which does not use this param.");
                                break;
                        }
                        break;
                    case "height":
                        switch (tag.sTag)
                        {
                            case "gumpimg":
                            case "itemimg":
                            case "span":
                                Style.ElementHeight = int.Parse(value);
                                break;
                            default:
                                Tracer.Warn("height param encountered within " + tag.sTag + " which does not use this param.");
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
            value = value.Trim();
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
                            Tracer.Warn("Unknown font-family parameter:{0}", value);
                    }
                    break;
                case "text-decoration":
                    string[] param = value.Trim().Split(' ');
                    for (int i = 0; i < param.Length; i++)
                    {
                        if (param[i] == "none")
                        {
                            Style.IsUnderlined = false;
                        }
                        else if (param[i] == "underline")
                        {
                            Style.IsUnderlined = true;
                        }
                        else
                        {
                            // other possibilities? overline|line-through|initial|inherit;
                            Tracer.Warn("Unknown text-decoration parameter:{0}", param[i]);
                        }
                    }
                    break;
                case "layer":
                    {
                        if (value == "default")
                        {
                            Style.Layer = Layers.Default;
                        }
                        else if (value == "background")
                        {
                            Style.Layer = Layers.Background;
                        }
                        else
                        {
                            // other possibilities? fixed|relative|initial|inherit;
                            Tracer.Warn("Unknown position parameter:{0}", value);
                        }
                    }
                    break;
                case "top":
                    {
                        int topValue = 0;
                        if (int.TryParse(value, out topValue))
                        {
                            Style.ElementTopOffset = topValue;
                        }
                    }
                    break;
                case "left":
                    {
                        int leftValue = 0;
                        if (int.TryParse(value, out leftValue))
                        {
                            Style.ElementLeftOffset = leftValue;
                        }
                    }
                    break;
                default:
                    Tracer.Warn("Unknown style parameter:{0}", key);
                    break;
            }
        }
    }
}
