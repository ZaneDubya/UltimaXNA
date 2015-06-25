using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Ultima.Data
{
    static class Magery
    {
        public static SpellDefinition[] Spells;

        static Magery()
        {
            Spells = new SpellDefinition[] {
                // first circle
                new SpellDefinition("Clumsy", 0, Reagents.Bloodmoss, Reagents.Nightshade),
                new SpellDefinition("Create Food", 0, Reagents.Garlic, Reagents.Ginseng, Reagents.MandrakeRoot),
                new SpellDefinition("Feeblemind", 0, Reagents.Nightshade, Reagents.Ginseng),
                new SpellDefinition("Heal", 0, Reagents.Garlic, Reagents.Ginseng, Reagents.SpidersSilk),
                new SpellDefinition("Magic Arrow", 0, Reagents.SulfurousAsh),
                new SpellDefinition("Night Sight", 0, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Reactive Armor", 0, Reagents.Garlic, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Weaken", 0, Reagents.Garlic, Reagents.Nightshade),
                // second circle

                // third circle

                // fourth circle

                // fifth circle

                // sixth circle

                // seventh circle

                // eighth circle

            };
        }

        public static string[] CircleNames = new string[] {
            "First Circle", "Second Circle", "Third Circle", "Fourth Circle",
            "Fifth Circle", "Sixth Circle", "Seventh Circle", "Eighth Circle" };
    }
}
