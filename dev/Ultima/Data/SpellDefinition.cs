using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Ultima.Data
{
    struct SpellDefinition
    {
        public readonly string Name;
        public readonly int Index;
        public readonly int GumpIconID;
        public readonly Reagents[] Regs;

        public SpellDefinition(string name, int index, int gumpIconID, params Reagents[] regs)
        {
            Name = name;
            Index = index;
            GumpIconID = gumpIconID;
            Regs = regs;
        }

        public string CreateReagentListString(string separator)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Regs.Length; i++)
            {
                switch (Regs[i])
                {
                    // britanian reagents
                    case Reagents.BlackPearl:
                        sb.Append("Black Pearl");
                        break;
                    case Reagents.Bloodmoss:
                        sb.Append("Bloodmoss");
                        break;
                    case Reagents.Garlic:
                        sb.Append("Garlic");
                        break;
                    case Reagents.Ginseng:
                        sb.Append("Ginseng");
                        break;
                    case Reagents.MandrakeRoot:
                        sb.Append("Mandrake Root");
                        break;
                    case Reagents.Nightshade:
                        sb.Append("Nightshade");
                        break;
                    case Reagents.SulfurousAsh:
                        sb.Append("Sulfurous Ash");
                        break;
                    case Reagents.SpidersSilk:
                        sb.Append("Spiders' Silk");
                        break;
                    // pagan reagents
                    case Reagents.BatWing:
                        sb.Append("Bat Wing");
                        break;
                    case Reagents.GraveDust:
                        sb.Append("Grave Dust");
                        break;
                    case Reagents.DaemonBlood:
                        sb.Append("Daemon Blood");
                        break;
                    case Reagents.NoxCrystal:
                        sb.Append("Nox Crystal");
                        break;
                    case Reagents.PigIron:
                        sb.Append("Pig Iron");
                        break;
                    default:
                        sb.Append("Unknown reagent");
                        break;
                }
                if (i < Regs.Length - 1)
                    sb.Append(separator);
            }
            return sb.ToString();
        }
    }
}
