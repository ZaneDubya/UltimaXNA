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
        public readonly Spellbooks BookType;
        public readonly byte[] SpellBitfields;

        public SpellbookData(Serial serial, ushort itemID, ushort bookTypePacketID, byte[] spellBitFields)
        {
            Serial = serial;
            ItemID = itemID;

            if (spellBitFields == null || spellBitFields.Length != 8)
            {
                BookType = Spellbooks.Unknown;
                SpellBitfields = null;
                return;
            }

            SpellBitfields = spellBitFields;

            switch (bookTypePacketID)
            {
                case 1:
                    BookType = Spellbooks.Magic;
                    break;
                case 101:
                    BookType = Spellbooks.Necro;
                    break;
                case 201:
                    BookType = Spellbooks.Paladin;
                    break;
                case 401:
                    BookType = Spellbooks.Bushido;
                    break;
                case 501:
                    BookType = Spellbooks.Ninjitsu;
                    break;
                case 601:
                    BookType = Spellbooks.Spellweaving;
                    break;
                default:
                    BookType = Spellbooks.Unknown;
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

    enum Spellbooks
    {
        Magic,
        Necro,
        Paladin,
        Bushido,
        Ninjitsu,
        Spellweaving,
        Unknown
    }
}
