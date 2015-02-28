using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace UltimaXNA.UltimaData.Fonts
{

    public static class UniText
    {
        private static UniFont[] _fonts;
        private static bool _initialized;
        private static GraphicsDevice _graphicsDevice;
        public static UniFont[] Fonts
        {
            get { return _fonts; }
        }

        static UniText()
        {

        }

        public static int FontHeight(int index)
        {
            return _fonts[index].Height;
        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (!_initialized)
            {
                _initialized = true;
                _graphicsDevice = graphicsDevice;
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
            _fonts = new UniFont[7];
            for (int iFont = 0; iFont < 7; iFont++)
            {
                string path = FileManager.GetFilePath("unifont" + (iFont == 0 ? "" : iFont.ToString()) + ".mul");
                if (path != null)
                {
                    _fonts[iFont] = new UniFont();
                    _fonts[iFont].Initialize(_graphicsDevice, new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
                    if (_fonts[iFont].Height > maxHeight)
                        maxHeight = _fonts[iFont].Height;
                }
            }

            for (int iFont = 0; iFont < 7; iFont++)
            {
                if (_fonts[iFont] == null)
                    continue;
                _fonts[iFont].Height = maxHeight;
            }
        }
    }
}
