using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Ultima.Data
{
    struct SpellDefinition
    {
        public readonly string Name;
        public readonly int GumpIconID;
        public readonly Reagents[] Regs;

        public SpellDefinition(string name, int gumpIconID, params Reagents[] regs)
        {
            Name = name;
            GumpIconID = gumpIconID;
            Regs = regs;
        }
    }
}
