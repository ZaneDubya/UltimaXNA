using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.Data;

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

        /// <summary>
        ///  Returns true if the parameter ItemID matches a Spellbook Item.
        /// </summary>
        /// <param name="itemID">An itemID to be tested</param>
        /// <returns>True if the itemID is a Spellbook ite, false otherwise.</returns>
        public static bool IsSpellBookItem(ushort itemID)
        {
            return m_SpellBookItemIDs.Contains<ushort>(itemID);
        }

        public SpellBookTypes BookType
        {
            get;
            private set;
        }

        private bool[] m_SpellData;

        public SpellBook(Serial serial, Map map)
            : base(serial, map)
        {
            BookType = SpellBookTypes.Unknown;
            m_SpellData = null;
        }

        public void ReceiveSpellDataFromPacket(SpellbookData data)
        {
            BookType = data.BookType;

            if (m_SpellData == null)
                m_SpellData = new bool[64];

            for (int i = 0; i < 64; i++)
                if (data.HasSpell(i))
                    m_SpellData[i] = true;
        }
    }
}
