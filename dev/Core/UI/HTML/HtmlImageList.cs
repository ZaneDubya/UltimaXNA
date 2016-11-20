/***************************************************************************
 *   Images.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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

namespace UltimaXNA.Core.UI.HTML
{
    class HtmlImageList
    {
        public static HtmlImageList Empty = new HtmlImageList();

        List<HtmlImage> m_images = new List<HtmlImage>();

        public HtmlImage this[int index]
        {
            get
            {
                if (m_images.Count == 0)
                    return null;
                if (index >= m_images.Count)
                    index = m_images.Count - 1;
                if (index < 0)
                    index = 0;
                return m_images[index];
            }
        }

        public int Count
        {
            get { return m_images.Count; }
        }

        public void AddImage(Rectangle area, Texture2D image)
        {
            m_images.Add(new HtmlImage(area, image));
        }

        public void AddImage(Rectangle area, Texture2D image, Texture2D overimage, Texture2D downimage)
        {
            AddImage(area, image);
            m_images[m_images.Count - 1].TextureOver = overimage;
            m_images[m_images.Count - 1].TextureDown = downimage;
        }

        public void Clear()
        {
            m_images.Clear();
        }
    }

    
}
