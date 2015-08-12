/***************************************************************************
 *   SpanAtom.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML.Atoms
{
    public class SpanAtom : AAtom
    {
        private int m_width = 0;
        public override int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        private int m_height = 0;
        public override int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public SpanAtom(StyleState style)
            : base(style)
        {
            m_height = Style.Font.Height;
            m_width = Style.ElementWidth;
        }
    }
}
