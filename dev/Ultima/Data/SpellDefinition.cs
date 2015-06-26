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

        public string ReagentListString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (Reagents reg in Regs)
                {
                    switch (reg)
                    {
                        // britanian reagents
                        case Reagents.BlackPearl:
                            sb.Append("Black Pearl\n");
                            break;
                        case Reagents.Bloodmoss:
                            sb.Append("Bloodmoss\n");
                            break;
                        case Reagents.Garlic:
                            sb.Append("Garlic\n");
                            break;
                        case Reagents.Ginseng:
                            sb.Append("Ginseng\n");
                            break;
                        case Reagents.MandrakeRoot:
                            sb.Append("Mandrake Root\n");
                            break;
                        case Reagents.Nightshade:
                            sb.Append("Nightshade\n");
                            break;
                        case Reagents.SulfurousAsh:
                            sb.Append("Sulfurous Ash\n");
                            break;
                        case Reagents.SpidersSilk:
                            sb.Append("Spiders' Silk\n");
                            break;
                        // pagan reagents
                        case Reagents.BatWing:
                            sb.Append("Bat Wing\n");
                            break;
                        case Reagents.GraveDust:
                            sb.Append("Grave Dust\n");
                            break;
                        case Reagents.DaemonBlood:
                            sb.Append("Daemon Blood\n");
                            break;
                        case Reagents.NoxCrystal:
                            sb.Append("Nox Crystal\n");
                            break;
                        case Reagents.PigIron:
                            sb.Append("Pig Iron\n");
                            break;
                        default:
                            sb.Append("Unknown reagent\n");
                            break;
                    }
                }
                return sb.ToString();
            }
        }
    }
}
