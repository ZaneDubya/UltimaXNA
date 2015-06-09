/***************************************************************************
 *   TextUni.cs
 *   Changes Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Ultima.IO.FontsNew
{
    public static class TextUni
    {
        public static int FontCount = 7;
        private static FontUni[] m_fonts = new FontUni[FontCount];

        internal static FontUni GetFont(int index)
        {
            if (index < 0 || index >= FontCount)
                return m_fonts[0];
            return m_fonts[index];
        }

        private static bool m_initialized;
        private static GraphicsDevice m_graphicsDevice;

        static TextUni()
        {

        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (!m_initialized)
            {
                m_initialized = true;
                m_graphicsDevice = graphicsDevice;
                graphicsDevice.DeviceReset += graphicsDevice_DeviceReset;
                loadFonts();
            }
        }

        static void graphicsDevice_DeviceReset(object sender, System.EventArgs e)
        {
            loadFonts();
        }

        static void loadFonts()
        {
            int maxHeight = 0; // because all unifonts are designed to be used together, they must all share a maxheight.

            for (int iFont = 0; iFont < FontCount; iFont++)
            {
                string path = FileManager.GetFilePath("unifont" + (iFont == 0 ? "" : iFont.ToString()) + ".mul");
                if (path != null)
                {
                    m_fonts[iFont] = new FontUni();
                    m_fonts[iFont].Initialize(m_graphicsDevice, new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
                    if (m_fonts[iFont].Height > maxHeight)
                        maxHeight = m_fonts[iFont].Height;
                }
            }

            for (int iFont = 0; iFont < FontCount; iFont++)
            {
                if (m_fonts[iFont] == null)
                    continue;
                m_fonts[iFont].Height = maxHeight;
            }
        }
    }
}
