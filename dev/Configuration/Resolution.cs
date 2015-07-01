/***************************************************************************
 *   Resolution.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using UltimaXNA.Core.ComponentModel;
using UltimaXNA.Core.Configuration;
#endregion

namespace UltimaXNA.Configuration
{
    public class Resolution : NotifyPropertyChangedBase
    {
        private int m_height;
        private int m_width;

        public Resolution()
        {
            Width = 800;
            Height = 600;
        }

        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height
        {
            get { return m_height; }
            set { SetProperty(ref m_height, value); }
        }

        public int Width
        {
            get { return m_width; }
            set { SetProperty(ref m_width, value); }
        }
    }
}