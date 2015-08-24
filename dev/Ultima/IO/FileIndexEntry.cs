using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace UltimaXNA.Ultima.IO
{
    [StructLayout(LayoutKind.Sequential, Pack = 0x1)]
    public struct FileIndexEntry
    {
        public int lookup;
        public int length;
        public int extra;
    }
}
