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
                new SpellDefinition("Clumsy", 0, 0x1B58, Reagents.Bloodmoss, Reagents.Nightshade),
                new SpellDefinition("Create Food", 1, 0x1B59, Reagents.Garlic, Reagents.Ginseng, Reagents.MandrakeRoot),
                new SpellDefinition("Feeblemind", 2, 0x1B5A, Reagents.Nightshade, Reagents.Ginseng),
                new SpellDefinition("Heal", 3, 0x1B5B, Reagents.Garlic, Reagents.Ginseng, Reagents.SpidersSilk),
                new SpellDefinition("Magic Arrow", 4, 0x1B5C, Reagents.SulfurousAsh),
                new SpellDefinition("Night Sight", 5, 0x1B5D, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Reactive Armor", 6, 0x1B5E, Reagents.Garlic, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Weaken", 7, 0x1B5F, Reagents.Garlic, Reagents.Nightshade),
                // second circle
                new SpellDefinition("Agility", 8, 0x1B60, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Cunning", 9, 0x1B61, Reagents.Nightshade, Reagents.MandrakeRoot),
                new SpellDefinition("Cure", 10, 0x1B62, Reagents.Garlic, Reagents.Ginseng),
                new SpellDefinition("Harm", 11, 0x1B63, Reagents.Nightshade, Reagents.SpidersSilk),
                new SpellDefinition("Magic Trap", 12, 0x1B64, Reagents.Garlic, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Magic Untrap", 13, 0x1B65, Reagents.Bloodmoss, Reagents.SulfurousAsh),
                new SpellDefinition("Protection", 14, 0x1B66, Reagents.Garlic, Reagents.Ginseng, Reagents.SulfurousAsh),
                new SpellDefinition("Strength", 15, 0x1B67, Reagents.MandrakeRoot, Reagents.Nightshade),
                // third circle
                new SpellDefinition("Bless", 16, 0x1B68, Reagents.Garlic, Reagents.MandrakeRoot),
                new SpellDefinition("Fireball", 17, 0x1B69, Reagents.BlackPearl),
                new SpellDefinition("Magic Lock", 18, 0x1B6a, Reagents.Bloodmoss, Reagents.Garlic, Reagents.SulfurousAsh),
                new SpellDefinition("Poison", 19, 0x1B6b, Reagents.Nightshade),
                new SpellDefinition("Telekinesis", 20, 0x1B6c, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Teleport", 21, 0x1B6d, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Unlock", 22, 0x1B6e, Reagents.Bloodmoss, Reagents.SulfurousAsh),
                new SpellDefinition("Wall of Stone", 23, 0x1B6f, Reagents.Bloodmoss, Reagents.Garlic),
                // fourth circle
                new SpellDefinition("Arch Cure", 24, 0x1B70, Reagents.Garlic, Reagents.Ginseng, Reagents.MandrakeRoot),
                new SpellDefinition("Arch Protection", 25, 0x1B71, Reagents.Garlic, Reagents.Ginseng, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Curse", 26, 0x1B72, Reagents.Garlic, Reagents.Nightshade, Reagents.SulfurousAsh),
                new SpellDefinition("Fire Field", 27, 0x1B73, Reagents.BlackPearl, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Greater Heal", 28, 0x1B74, Reagents.Garlic, Reagents.Ginseng, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Lightning", 29, 0x1B75, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Mana Drain", 30, 0x1B76, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Recall", 31, 0x1B77, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                // fifth circle
                new SpellDefinition("Blade Spirits", 32, 0x1B78, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.Nightshade),
                new SpellDefinition("Dispel Field", 33, 0x1B79, Reagents.BlackPearl, Reagents.Garlic, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Incognito", 34, 0x1B7a, Reagents.Bloodmoss, Reagents.Garlic, Reagents.Nightshade),
                new SpellDefinition("Magic Reflection", 35, 0x1B7b, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Mind Blast", 36, 0x1B7c, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.Nightshade, Reagents.SulfurousAsh),
                new SpellDefinition("Paralyze", 37, 0x1B7d, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Poison Field", 38, 0x1B7e, Reagents.BlackPearl, Reagents.Nightshade, Reagents.SpidersSilk),
                new SpellDefinition("Summon Creature", 39, 0x1B7f, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                // sixth circle
                new SpellDefinition("Dispel", 40, 0x1B80, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Energy Bolt", 41, 0x1B81, Reagents.BlackPearl, Reagents.Nightshade),
                new SpellDefinition("Explosion", 42, 0x1B82, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Invisibility", 43, 0x1B83, Reagents.Bloodmoss, Reagents.Nightshade),
                new SpellDefinition("Mark", 44, 0x1B84, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Mass Curse", 45, 0x1B85, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.Nightshade, Reagents.SulfurousAsh),
                new SpellDefinition("Paralyze Field", 46, 0x1B86, Reagents.BlackPearl, Reagents.Ginseng, Reagents.SpidersSilk),
                new SpellDefinition("Reveal", 47, 0x1B87, Reagents.Bloodmoss, Reagents.SulfurousAsh),
                // seventh circle
                new SpellDefinition("Chain Lightning", 48, 0x1B88, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Energy Field", 49, 0x1B89, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Flamestrike", 50, 0x1B8a, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Gate Travel", 51, 0x1B8b, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Mana Vampire", 52, 0x1B8c, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Mass Dispel", 53, 0x1B8d, Reagents.BlackPearl, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Meteor Swarm", 54, 0x1B8e, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Polymorph", 55, 0x1B8f, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                // eighth circle
                new SpellDefinition("Earthquake", 56, 0x1B90, Reagents.Bloodmoss, Reagents.Ginseng, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Energy Vortex", 57, 0x1B91, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.Nightshade),
                new SpellDefinition("Resurrection", 58, 0x1B92, Reagents.Bloodmoss, Reagents.Ginseng, Reagents.Garlic),
                new SpellDefinition("Air Elemental", 59, 0x1B93, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Summon Daemon", 60, 0x1B94, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Earth Elemental", 61, 0x1B95, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Fire Elemental", 62, 0x1B96, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Water Elemental", 63, 0x1B97, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
            };
        }

        public static string[] CircleNames = new string[] {
            "First Circle", "Second Circle", "Third Circle", "Fourth Circle",
            "Fifth Circle", "Sixth Circle", "Seventh Circle", "Eighth Circle" };
    }
}
