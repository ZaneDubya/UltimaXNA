using System.Linq;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.World.Maps;

namespace UltimaXNA.Ultima.World.Entities.Items.Containers
{
    class SpellBook : Container
    {
        static ushort[] m_SpellBookItemIDs = {
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

        ulong m_SpellsBitfield;
        public bool HasSpell(int circle, int index)
        {
            index = ((3 - circle % 4) + (circle / 4) * 4) * 8 + (index - 1);
            ulong flag = ((ulong)1) << index;
            return (m_SpellsBitfield & flag) == flag;
        }

        public SpellBook(Serial serial, Map map)
            : base(serial, map)
        {
            BookType = SpellBookTypes.Unknown;
            m_SpellsBitfield = 0;
        }

        public void ReceiveSpellData(SpellBookTypes sbType, ulong sbBitfield)
        {
            bool entityUpdated = false;
            if (BookType != sbType)
            {
                BookType = sbType;
                entityUpdated = true;
            }

            if (m_SpellsBitfield != sbBitfield)
            {
                m_SpellsBitfield = sbBitfield;
                entityUpdated = true;
            }
            if (entityUpdated)
            {
                m_OnUpdated?.Invoke(this);
            }
        }
    }
}