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
#region usings
using UltimaXNA.Core.ComponentModel;
#endregion

namespace UltimaXNA.Configuration
{
    /// <summary>
    /// A class that describes a resolution width height pair. Defaults to 1024x768.
    /// </summary>
    public class ResolutionConfig : NotifyPropertyChangedBase
    {
        private int m_Height = 768;
        private int m_Width = 1024;

        public ResolutionConfig()
        {

        }

        public ResolutionConfig(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height
        {
            get { return m_Height; }
            set { SetProperty(ref m_Height, value); }
        }

        public int Width
        {
            get { return m_Width; }
            set { SetProperty(ref m_Width, value); }
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", m_Width, m_Height);
        }
    }
}