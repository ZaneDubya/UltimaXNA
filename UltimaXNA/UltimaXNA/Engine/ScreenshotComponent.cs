using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Windows.Forms;
using System.IO;

namespace UltimaXNA
{
    public partial class ScreenshotComponent
    {
        private Microsoft.Xna.Framework.Input.Keys _screenshotKey = Microsoft.Xna.Framework.Input.Keys.F12;
        private bool _screenshotTaken;
        private string _screenshotPath = "";

        public Microsoft.Xna.Framework.Input.Keys ScreenshotKey
        {
            get { return _screenshotKey; }
            set { _screenshotKey = value; }
        }

        public string ScreenshotPath
        {
            get { return _screenshotPath; }
            set { _screenshotPath = value; }
        }

        public void Screenshot(Game game, bool always)
        {
            if (always || Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(_screenshotKey) && !_screenshotTaken)
            {
                _screenshotTaken = true;

                ResolveTexture2D dstTexture = new ResolveTexture2D(game.GraphicsDevice,
                    game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                    0,
                    game.GraphicsDevice.DisplayMode.Format);
                game.GraphicsDevice.ResolveBackBuffer(dstTexture);

                string fileName = "";

                if (_screenshotPath == "")
                {
                    _screenshotPath = Application.StartupPath;

                    if (!System.IO.Directory.Exists(_screenshotPath + "\\screenshots"))
                        System.IO.Directory.CreateDirectory(_screenshotPath + "\\screenshots");

                    _screenshotPath += "\\screenshots";
                }
                else
                {
                    //make sure path is valid
                    if (!System.IO.Directory.Exists(_screenshotPath))
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(_screenshotPath);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }

                string[] files = System.IO.Directory.GetFiles(_screenshotPath, "*.bmp");

                fileName = _screenshotPath + string.Format("\\screenshot{0}.bmp", files.Length + 1);

                int[] data = new int[400 * 300];
                dstTexture.GetData<Int32>(0, new Rectangle(0, 0, 400, 300), data, 0, 400 * 300);

                Texture2D t = new Texture2D(game.GraphicsDevice, 400, 300);
                t.SetData(data);
                t.Save(fileName, ImageFileFormat.Bmp);

                // dstTexture.Save(fileName, ImageFileFormat.Bmp);
            }
            else if (_screenshotTaken)
            {
                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyUp(_screenshotKey))
                    _screenshotTaken = false;
            }
        }
    }
}