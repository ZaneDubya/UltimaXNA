using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Ultima.Audio
{
    class UOMusic
    {
        public int Id;
        public string Name;
        public bool DoLoop;

        public UOMusic()
        {
            Id = -1;
            Name = "";
            DoLoop = false;
        }

        public UOMusic(int id, string name, bool doLoop)
        {
            Id = id;
            Name = name;
            DoLoop = doLoop;
        }
    }
}
