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

        public SpellbookData(Serial serial, Container spellbook, ContainerContentPacket contents)
        {
            Serial = serial;
            ItemID = (ushort)spellbook.ItemID;

            switch (spellbook.ItemID)
            {
                case 0x0E3B: // spellbook
                case 0x0EFA:
                    BookType = SpellBookTypes.Magic;
                    break;
                case 0x2252: // paladin spellbook
                    BookType = SpellBookTypes.Chivalry;
                    break;
                case 0x2253: // necromancer book
                    BookType = SpellBookTypes.Necromancer;
                    break;
                case 0x238C: // book of bushido
                    BookType = SpellBookTypes.Bushido;
                    break;
                case 0x23A0: // book of ninjitsu
                    BookType = SpellBookTypes.Ninjitsu;
                    break;
                case 0x2D50: // spell weaving book
                    BookType = SpellBookTypes.Chivalry;
                    break;
            }

            foreach (ItemInContainer i in contents.Items)
            {
                ulong spellBit = (ulong)0x1 << ((i.Serial - 1) & 0x0000003F);
                SpellsBitfield |= spellBit;
            }
        }
    }
}
