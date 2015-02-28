/***************************************************************************
 *   HTMLImages.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaGUI.HTML
{
    class HTMLImages
    {
        List<HTMLImage> _images = new List<HTMLImage>();

        public List<HTMLImage> Images
        {
            get
            {
                return _images;
            }
        }

        public int Count
        {
            get { return _images.Count; }
        }

        public void AddImage(Rectangle area, Texture2D image)
        {
            _images.Add(new HTMLImage(area, image));
        }

        public void AddImage(Rectangle area, Texture2D image, Texture2D overimage, Texture2D downimage)
        {
            AddImage(area, image);
            _images[_images.Count - 1].ImageOver = overimage;
            _images[_images.Count - 1].ImageDown = downimage;
        }

        public void Clear()
        {
            _images.Clear();
        }
    }

    public class HTMLImage
    {
        public Rectangle Area;
        public Texture2D Image;
        public Texture2D ImageOver;
        public Texture2D ImageDown;
        public int RegionIndex = -1;

        public HTMLImage(Rectangle area, Texture2D image)
        {
            Area = area;
            Image = image;
        }
    }
}
