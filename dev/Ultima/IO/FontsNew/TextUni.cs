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
#region usings
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using UltimaXNA.Core.UI.Fonts;
#endregion

namespace UltimaXNA.Ultima.IO.FontsNew
{
    public static class TextUni
    {
        public static int UniFontCount = 7;
        private static AFont[] m_UnicodeFonts = new AFont[UniFontCount];

        public static int AsciiFontCount = 10;
        private static AFont[] m_AsciiFonts = new AFont[AsciiFontCount];

        internal static AFont GetUniFont(int index)
        {
            if (index < 0 || index >= UniFontCount)
                return m_UnicodeFonts[0];
            return m_UnicodeFonts[index];
        }

        internal static AFont GetAsciiFont(int index)
        {
            if (index < 0 || index >= AsciiFontCount)
                return m_AsciiFonts[0];
            return m_AsciiFonts[index];
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
            // ==============================================================================================================
            // load Ascii fonts
            // ==============================================================================================================

            using (BinaryReader reader = new BinaryReader(new FileStream(FileManager.GetFilePath("fonts.mul"), FileMode.Open, FileAccess.Read)))
            {
                for (int iFont = 0; iFont < AsciiFontCount; iFont++)
                {
                    m_AsciiFonts[iFont] = new FontAscii();
                    m_AsciiFonts[iFont].Initialize(reader);
                }
            }


            // ==============================================================================================================
            // load Unicode fonts
            // ==============================================================================================================

            int maxHeight = 0; // because all unifonts are designed to be used together, they must all share a maxheight.

            for (int iFont = 0; iFont < UniFontCount; iFont++)
            {
                string path = FileManager.GetFilePath("unifont" + (iFont == 0 ? "" : iFont.ToString()) + ".mul");
                if (path != null)
                {
                    m_UnicodeFonts[iFont] = new FontUnicode();
                    m_UnicodeFonts[iFont].Initialize(new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
                    if (m_UnicodeFonts[iFont].Height > maxHeight)
                        maxHeight = m_UnicodeFonts[iFont].Height;
                }
            }

            for (int iFont = 0; iFont < UniFontCount; iFont++)
            {
                if (m_UnicodeFonts[iFont] == null)
                    continue;
                m_UnicodeFonts[iFont].Height = maxHeight;
            }


        }
    }
}
