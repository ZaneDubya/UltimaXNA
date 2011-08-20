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
