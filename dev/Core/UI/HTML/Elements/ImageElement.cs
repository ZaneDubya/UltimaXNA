/***************************************************************************
 *   ImageAtom.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML.Elements
{
    public class ImageElement : AElement
    {
        public HtmlImage AssociatedImage
        {
            get;
            set;
        }

        public int ImgSrc = -1;
        public int ImgSrcOver = -1;
        public int ImgSrcDown = -1;

        private int m_Width = 0, m_Height = 0;

        public override int Width
        {
            set
            {
                m_Width = value;
            }
            get
            {
                if (m_Width != 0)
                    return m_Width;
                return AssociatedImage.Texture.Width;
            }
        }

        public override int Height
        {
            set
            {
                m_Height = value;
            }
            get
            {
                if (m_Height != 0)
                    return m_Height;
                return AssociatedImage.Texture.Height;
            }
        }

        public ImageTypes ImageType
        {
            get;
            private set;
        }

        public ImageElement(StyleState style, ImageTypes imageType = ImageTypes.UI)
            : base(style)
        {
            ImageType = imageType;
        }

        public override string ToString()
        {
            return string.Format("<img {0} {1}>", ImgSrc, ImageType.ToString());
        }

        public enum ImageTypes
        {
            UI,
            Item
        }
    }
}
