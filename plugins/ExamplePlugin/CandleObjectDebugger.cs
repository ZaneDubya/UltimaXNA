using System;
using System.IO;
using UltimaXNA;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Core.Resources;

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
                        // get the resource provider
                        IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                        IAnimationFrame[] frames = provider.GetAnimation(candleBodyID, action, direction, 0);
                        if (frames != null)
                        {
                            for (int i = 0; i < frames.Length; i++)
                                if (frames[i] != null)
                                {
                                    Utility.SaveTexture(frames[i].Texture, Path.Combine(path, string.Format("{0}-{1}-{2}.png",
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
