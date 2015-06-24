using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Ultima.Data
{
    static class Magery
    {
        public static SpellInfo[] Spells;

        static Magery()
        {
            Spells = new SpellInfo[] {
                new SpellInfo("Clumsy", "Uus Jux", Reagents.Bloodmoss, Reagents.Nightshade),
            };
        }

        public static string[] CircleNames = new string[] {
            "First Circle", "Second Circle", "Third Circle", "Fourth Circle",
            "Fifth Circle", "Sixth Circle", "Seventh Circle", "Eighth Circle" };
    }
}
