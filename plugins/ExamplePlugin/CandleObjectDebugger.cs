using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UltimaXNA.Ultima.IO;

namespace ExamplePlugin
{
    class CandleObjectDebugger
    {
        public void OutputAllCandleTextures()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "candle");
            Directory.CreateDirectory(path);

            int candleBodyID = 505;

            try
            {
                for (int action = 0; action < 30; action++)
                {
                    for (int direction = 0; direction < 8; direction++)
                    {
                        AnimationFrame[] frames = Animations.GetAnimation(candleBodyID, action, direction, 0);
                        if (frames != null)
                        {
                            for (int i = 0; i < frames.Length; i++)
                                if (frames[i] != null)
                                {
                                    UltimaXNA.Utility.SaveTexture(frames[i].Texture, Path.Combine(path, string.Format("{0}-{1}-{2}.png",
                                        action, direction, i)));
                                }
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
