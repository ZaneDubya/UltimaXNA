/***************************************************************************
 *   StyleState.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using UltimaXNA.Core.Resources;

namespace UltimaXNA.Core.UI.HTML.Styles
{
    public class StyleState
    {
        private StyleValue m_IsUnderlined = StyleValue.Default;
        public bool IsUnderlined
        {
            set
            {
                if (value)
                {
                    m_IsUnderlined = StyleValue.True;
                }
                else
                {
                    m_IsUnderlined = StyleValue.False;
                }
            }
            get
            {
                if (m_IsUnderlined == StyleValue.False)
                    return false;
                else if (m_IsUnderlined == StyleValue.True)
                    return true;
                else // m_IsUnderlined == TagValue.Default
                {
                    if (HREF != null)
                        return true;
                    else
                        return false;
                }
            }
        }

        public bool IsBold = false;
        public bool IsItalic = false;
        public bool IsOutlined = false;

        public bool MustDrawnOutline
        {
            get { return IsOutlined && !Font.HasBuiltInOutline; }
        }

        public IFont Font; // default value set in manager ctor.
        public Color Color = Color.White;
        public int ColorHue = 2;
        public int ActiveColorHue = 12;
        public int HoverColorHue = 24;

        public string HREF = null;
        public bool IsHREF { get { return HREF != null; } }

        public StyleState(IResourceProvider provider)
        {
            Font = provider.GetUnicodeFont((int)Fonts.Default);
        }

        public StyleState(StyleState parent)
        {
            m_IsUnderlined = parent.m_IsUnderlined;
            IsBold = parent.IsBold;
            IsItalic = parent.IsItalic;
            IsOutlined = parent.IsOutlined;
            Font = parent.Font;
            Color = parent.Color;
            ColorHue = parent.ColorHue;
            ActiveColorHue = parent.ActiveColorHue;
            HoverColorHue = parent.HoverColorHue;
            HREF = parent.HREF;
        }
    }
}
