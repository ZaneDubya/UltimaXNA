using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Ultima.Data
{
    struct SpellInfo
    {
        public readonly string Name;
        public readonly string Mantra;
        public readonly Reagents[] Regs;

        public SpellInfo(string name, string mantra, params Reagents[] regs)
        {
            Name = name;
            Mantra = mantra;
            Regs = regs;
        }
    }
}
