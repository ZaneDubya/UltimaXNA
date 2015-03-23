using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaData.FontsNew
{
    public static class UniText
    {
        private static UniFont[] m_fonts;
        private static bool m_initialized;
        private static GraphicsDevice m_graphicsDevice;

        public static UniFont[] Fonts
        {
            get { return m_fonts; }
        }

        static UniText()
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
            int maxHeight = 0;
            m_fonts = new UniFont[7];
            for (int iFont = 0; iFont < 7; iFont++)
            {
                string path = FileManager.GetFilePath("unifont" + (iFont == 0 ? "" : iFont.ToString()) + ".mul");
                if (path != null)
                {
                    m_fonts[iFont] = new UniFont();
                    m_fonts[iFont].Initialize(m_graphicsDevice, new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
                    if (m_fonts[iFont].Height > maxHeight)
                        maxHeight = m_fonts[iFont].Height;
                }
            }

            for (int iFont = 0; iFont < 7; iFont++)
            {
                if (m_fonts[iFont] == null)
                    continue;
                m_fonts[iFont].Height = maxHeight;
            }
        }
    }
}
