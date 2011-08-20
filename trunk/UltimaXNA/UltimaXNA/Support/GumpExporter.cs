using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Support
{
    class GumpExporter
    {
        public static void DEBUG_ExportAll()
        {
            for (int i = 0; i < 0x10000; i++)
            {
                Directory.CreateDirectory("gfx/");
                try
                {
                    Texture2D t = Data.Gumps.GetGumpXNA(i);
                    if (t != null)
                    {
                        Stream stream = File.OpenWrite(string.Format("gfx/{1} ({0}).png", i.ToString("X4"), i.ToString()));
                        t.SaveAsPng(stream, t.Width, t.Height);
                    }
                }
                catch
                {

                }
            }
        }
    }
}
