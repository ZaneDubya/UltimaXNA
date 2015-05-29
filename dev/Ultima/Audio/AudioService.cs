using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Audio
{
    public class AudioService
    {
        public void PlaySound(int soundIndex)
        {
            SoundData.PlaySound(soundIndex - 1);
        }
    }
}
