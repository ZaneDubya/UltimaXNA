using System.Linq;
using UltimaXNA.Ultima.Data;
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

        private ulong m_SpellsBitfield;
        public bool HasSpell(int index)
        {
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
            bool entityUpdated = true;

            if (BookType != sbType)
            {
                BookType = sbType;
                entityUpdated = true;
            }

            m_SpellsBitfield = sbBitfield;

            if (entityUpdated && OnEntityUpdated != null)
                OnEntityUpdated();
        }
    }
}
