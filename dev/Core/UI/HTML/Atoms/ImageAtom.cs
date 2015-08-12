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

namespace UltimaXNA.Core.UI.HTML.Atoms
{
    public class ImageAtom : AAtom
    {
        public Image AssociatedImage
        {
            get;
            set;
        }

        public override int Width
        {
            set
            {
                Style.ElementWidth = value;
            }
            get
            {
                if (Style.ElementWidth != 0)
                    return Style.ElementWidth;
                return AssociatedImage.Texture.Width;
            }
        }

        public override int Height
        {
            set
            {
                Style.ElementHeight = value;
            }
            get
            {
                if (Style.ElementHeight != 0)
                    return Style.ElementHeight;
                return AssociatedImage.Texture.Height;
            }
        }

        public ImageTypes ImageType
        {
            get;
            private set;
        }

        public ImageAtom(StyleState style, ImageTypes imageType = ImageTypes.UI)
            : base(style)
        {
            ImageType = imageType;
        }

        public override string ToString()
        {
            return string.Format("<img {0} {1}>", Style.ImgSrc, ImageType.ToString());
        }

        public enum ImageTypes
        {
            UI,
            Item
        }
    }
}
