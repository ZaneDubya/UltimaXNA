using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.World.Maps;

namespace UltimaXNA.Ultima.World.Entities.Items.Containers
{
    class SpellBook : Container
    {
        private static ushort[] m_SpellBookItemIDs = new ushort[] {
            0xE3B, // bugged or static item spellbook? Not wearable.
            0xEFA, // standard, wearable spellbook
            0x2252, // paladin
            0x2253, // necro
            0x238C, // bushido
            0x23A0, // ninjitsu
            0x2D50 // spellweaving
        };

        public static bool IsSpellBookItem(ushort itemID)
        {
            return m_SpellBookItemIDs.Contains<ushort>(itemID);
        }

        public SpellBook(Serial serial, Map map)
            : base(serial, map)
        {

        }
    }
}
