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
                new SpellDefinition("Clumsy", 0x1B58, Reagents.Bloodmoss, Reagents.Nightshade),
                new SpellDefinition("Create Food", 0x1B59, Reagents.Garlic, Reagents.Ginseng, Reagents.MandrakeRoot),
                new SpellDefinition("Feeblemind", 0x1B5A, Reagents.Nightshade, Reagents.Ginseng),
                new SpellDefinition("Heal", 0x1B5B, Reagents.Garlic, Reagents.Ginseng, Reagents.SpidersSilk),
                new SpellDefinition("Magic Arrow", 0x1B5C, Reagents.SulfurousAsh),
                new SpellDefinition("Night Sight", 0x1B5D, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Reactive Armor", 0x1B5E, Reagents.Garlic, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Weaken", 0x1B5F, Reagents.Garlic, Reagents.Nightshade),
                // second circle
                new SpellDefinition("Agility", 0x1B60, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Cunning", 0x1B61, Reagents.Nightshade, Reagents.MandrakeRoot),
                new SpellDefinition("Cure", 0x1B62, Reagents.Garlic, Reagents.Ginseng),
                new SpellDefinition("Harm", 0x1B63, Reagents.Nightshade, Reagents.SpidersSilk),
                new SpellDefinition("Magic Trap", 0x1B64, Reagents.Garlic, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Magic Untrap", 0x1B65, Reagents.Bloodmoss, Reagents.SulfurousAsh),
                new SpellDefinition("Protection", 0x1B66, Reagents.Garlic, Reagents.Ginseng, Reagents.SulfurousAsh),
                new SpellDefinition("Strength", 0x1B67, Reagents.MandrakeRoot, Reagents.Nightshade),
                // third circle
                new SpellDefinition("Bless", 0x1B68, Reagents.Garlic, Reagents.MandrakeRoot),
                new SpellDefinition("Fireball", 0x1B69, Reagents.BlackPearl),
                new SpellDefinition("Magic Lock", 0x1B6a, Reagents.Bloodmoss, Reagents.Garlic, Reagents.SulfurousAsh),
                new SpellDefinition("Poison", 0x1B6b, Reagents.Nightshade),
                new SpellDefinition("Telekinesis", 0x1B6c, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Teleport", 0x1B6d, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Unlock", 0x1B6e, Reagents.Bloodmoss, Reagents.SulfurousAsh),
                new SpellDefinition("Wall of Stone", 0x1B6f, Reagents.Bloodmoss, Reagents.Garlic),
                // fourth circle
                new SpellDefinition("Arch Cure", 0x1B70, Reagents.Garlic, Reagents.Ginseng, Reagents.MandrakeRoot),
                new SpellDefinition("Arch Protection", 0x1B71, Reagents.Garlic, Reagents.Ginseng, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Curse", 0x1B72, Reagents.Garlic, Reagents.Nightshade, Reagents.SulfurousAsh),
                new SpellDefinition("Fire Field", 0x1B73, Reagents.BlackPearl, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Greater Heal", 0x1B74, Reagents.Garlic, Reagents.Ginseng, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Lightning", 0x1B75, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Mana Drain", 0x1B76, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Recall", 0x1B77, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                // fifth circle
                new SpellDefinition("Blade Spirits", 0x1B78, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.Nightshade),
                new SpellDefinition("Dispel Field", 0x1B79, Reagents.BlackPearl, Reagents.Garlic, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Incognito", 0x1B7a, Reagents.Bloodmoss, Reagents.Garlic, Reagents.Nightshade),
                new SpellDefinition("Magic Reflection", 0x1B7b, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Mind Blast", 0x1B7c, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.Nightshade, Reagents.SulfurousAsh),
                new SpellDefinition("Paralyze", 0x1B7d, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Poison Field", 0x1B7e, Reagents.BlackPearl, Reagents.Nightshade, Reagents.SpidersSilk),
                new SpellDefinition("Summon Creature", 0x1B7f, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                // sixth circle
                new SpellDefinition("Dispel", 0x1B80, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Energy Bolt", 0x1B81, Reagents.BlackPearl, Reagents.Nightshade),
                new SpellDefinition("Explosion", 0x1B82, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Invisibility", 0x1B83, Reagents.Bloodmoss, Reagents.Nightshade),
                new SpellDefinition("Mark", 0x1B84, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot),
                new SpellDefinition("Mass Curse", 0x1B85, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.Nightshade, Reagents.SulfurousAsh),
                new SpellDefinition("Paralyze Field", 0x1B86, Reagents.BlackPearl, Reagents.Ginseng, Reagents.SpidersSilk),
                new SpellDefinition("Reveal", 0x1B87, Reagents.Bloodmoss, Reagents.SulfurousAsh),
                // seventh circle
                new SpellDefinition("Chain Lightning", 0x1B88, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Energy Field", 0x1B89, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Flamestrike", 0x1B8a, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Gate Travel", 0x1B8b, Reagents.BlackPearl, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Mana Vampire", 0x1B8c, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Mass Dispel", 0x1B8d, Reagents.BlackPearl, Reagents.Garlic, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Meteor Swarm", 0x1B8e, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Polymorph", 0x1B8f, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                // eighth circle
                new SpellDefinition("Earthquake", 0x1B90, Reagents.Bloodmoss, Reagents.Ginseng, Reagents.MandrakeRoot, Reagents.SulfurousAsh),
                new SpellDefinition("Energy Vortex", 0x1B91, Reagents.BlackPearl, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.Nightshade),
                new SpellDefinition("Resurrection", 0x1B92, Reagents.Bloodmoss, Reagents.Ginseng, Reagents.Garlic),
                new SpellDefinition("Summon Air Elemental", 0x1B93, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Summon Daemon", 0x1B94, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Summon Earth Elemental", 0x1B95, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
                new SpellDefinition("Summon Fire Elemental", 0x1B96, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk, Reagents.SulfurousAsh),
                new SpellDefinition("Summon Water Elemental", 0x1B97, Reagents.Bloodmoss, Reagents.MandrakeRoot, Reagents.SpidersSilk),
            };
        }

        public static string[] CircleNames = new string[] {
            "First Circle", "Second Circle", "Third Circle", "Fourth Circle",
            "Fifth Circle", "Sixth Circle", "Seventh Circle", "Eighth Circle" };
    }
}
