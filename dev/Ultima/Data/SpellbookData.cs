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

namespace UltimaXNA.Ultima.Data
{
    sealed class SpellbookData
    {
        public readonly Serial Serial;
        public readonly ushort ItemID;
        public readonly SpellBookTypes BookType;
        public readonly byte[] SpellBitfields;

        public SpellbookData(Serial serial, ushort itemID, ushort bookTypePacketID, byte[] spellBitFields)
        {
            Serial = serial;
            ItemID = itemID;

            if (spellBitFields == null || spellBitFields.Length != 8)
            {
                BookType = SpellBookTypes.Unknown;
                SpellBitfields = null;
                return;
            }

            SpellBitfields = spellBitFields;

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

        public bool HasSpell(int index)
        {
            if (index < 0 || index >= 64)
                return false;
            int byteIndex = index / 8;
            int bitIndex = (index % 8);
            return (SpellBitfields[byteIndex] & (1 << bitIndex)) != 0;
        }

    }
}
