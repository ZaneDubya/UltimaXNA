/***************************************************************************
 *   CreateCharacterScene.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.Network.Server;

namespace UltimaXNA.Ultima.Data
{
    sealed class SpellbookData
    {
        public readonly Serial Serial;
        public readonly ushort ItemID;
        public readonly SpellBookTypes BookType;
        public readonly ulong SpellsBitfield;

        public SpellbookData(Serial serial, ushort itemID, ushort bookTypePacketID, ulong spellBitFields)
        {
            Serial = serial;
            ItemID = itemID;

            SpellsBitfield = spellBitFields;

            switch (bookTypePacketID)
            {
                case 1:
                    BookType = SpellBookTypes.Magic;
                    break;
                case 101:
                    BookType = SpellBookTypes.Necromancer;
                    break;
                case 201:
                    BookType = SpellBookTypes.Chivalry;
                    break;
                case 401:
                    BookType = SpellBookTypes.Bushido;
                    break;
                case 501:
                    BookType = SpellBookTypes.Ninjitsu;
                    break;
                case 601:
                    BookType = SpellBookTypes.Spellweaving;
                    break;
                default:
                    BookType = SpellBookTypes.Unknown;
                    return;
            }
        }

        public static SpellBookTypes GetSpellBookTypeFromItemID(int itemID)
        {
            SpellBookTypes bookType = SpellBookTypes.Unknown;
            switch (itemID)
            {
                case 0x0E3B: // spellbook
                case 0x0EFA:
                    bookType = SpellBookTypes.Magic;
                    break;
                case 0x2252: // paladin spellbook
                    bookType = SpellBookTypes.Chivalry;
                    break;
                case 0x2253: // necromancer book
                    bookType = SpellBookTypes.Necromancer;
                    break;
                case 0x238C: // book of bushido
                    bookType = SpellBookTypes.Bushido;
                    break;
                case 0x23A0: // book of ninjitsu
                    bookType = SpellBookTypes.Ninjitsu;
                    break;
                case 0x2D50: // spell weaving book
                    bookType = SpellBookTypes.Chivalry;
                    break;
            }
            return bookType;
        }

        public static int GetOffsetFromSpellBookType(SpellBookTypes spellbooktype)
        {
            switch (spellbooktype)
            {
                case SpellBookTypes.Magic:
                    return 1;
                case SpellBookTypes.Necromancer:
                    return 101;
                case SpellBookTypes.Chivalry:
                    return 201;
                case SpellBookTypes.Bushido:
                    return 401;
                case SpellBookTypes.Ninjitsu:
                    return 501;
                case SpellBookTypes.Spellweaving:
                    return 601;
                default:
                    return 1;
            }
        }

        public SpellbookData(Container spellbook, ContainerContentPacket contents)
        {
            Serial = spellbook.Serial;
            ItemID = (ushort)spellbook.ItemID;

            BookType = GetSpellBookTypeFromItemID(spellbook.ItemID);
            if (BookType == SpellBookTypes.Unknown)
                return;

            int offset = GetOffsetFromSpellBookType(BookType);

            foreach (ItemInContainer i in contents.Items)
            {
                ulong spellBit = ((ulong)1) << ((i.Amount - offset) & 0x0000003F);
                SpellsBitfield |= spellBit;
            }
        }
    }
}
