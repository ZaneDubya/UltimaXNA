﻿/***************************************************************************
 *   MobileEquipment.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Entity
{
    public class MobileEquipment
    {
        private Item[] _Equipment;
        private Mobile _Owner;
        private int _UpdateTicker = 0;

        public int UpdateTicker
        {
            get { return _UpdateTicker; }
        }

        public MobileEquipment(Mobile nOwner)
        {
            _Equipment = new Item[(int)EquipLayer.LastValid + 1];
            _Owner = nOwner;
        }

        public Item this[int nIndex]
        {
            get
            {
                if (nIndex > (int)EquipLayer.LastValid)
                    return null;
                return _Equipment[nIndex];
            }
            set
            {
                if (value == null)
                {
                    if (_Equipment[nIndex] != null)
                    {
                        _Equipment[nIndex].Dispose();
                        _Equipment[nIndex] = null;
                    }
                }
                else
                {
                    _Equipment[nIndex] = value;
                    value.Parent = _Owner;
                }
                _UpdateTicker++;
            }
        }

        public void ClearEquipment()
        {
            for (int i = 0; i <= (int)EquipLayer.LastValid; i++)
            {
                if (this[i] != null)
                {
                    this[i].Parent = null;
                    this[i] = null;
                }
            }
            _UpdateTicker++;
        }

        public void RemoveBySerial(Serial serial)
        {
            for (int i = 0; i <= (int)EquipLayer.LastValid; i++)
            {
                if (this[i] != null)
                    if (this[i].Serial == serial)
                    {
                        this[i].Parent = null;
                        this[i] = null;
                    }
            }
            _UpdateTicker++;
        }
    }

    public enum EquipLayer : int
    {
        /// <summary>
        /// Invalid layer.
        /// </summary>
        Body = 0x00,
        /// <summary>
        /// First valid layer. Equivalent to <c>Layer.OneHanded</c>.
        /// </summary>
        FirstValid = 0x01,
        /// <summary>
        /// One handed weapon.
        /// </summary>
        OneHanded = 0x01,
        /// <summary>
        /// Two handed weapon or shield.
        /// </summary>
        TwoHanded = 0x02,
        /// <summary>
        /// Shoes.
        /// </summary>
        Shoes = 0x03,
        /// <summary>
        /// Pants.
        /// </summary>
        Pants = 0x04,
        /// <summary>
        /// Shirts.
        /// </summary>
        Shirt = 0x05,
        /// <summary>
        /// Helmets, hats, and masks.
        /// </summary>
        Helm = 0x06,
        /// <summary>
        /// Gloves.
        /// </summary>
        Gloves = 0x07,
        /// <summary>
        /// Rings.
        /// </summary>
        Ring = 0x08,
        /// <summary>
        /// Talismans.
        /// </summary>
        Talisman = 0x09,
        /// <summary>
        /// Gorgets and necklaces.
        /// </summary>
        Neck = 0x0A,
        /// <summary>
        /// Hair.
        /// </summary>
        Hair = 0x0B,
        /// <summary>
        /// Half aprons.
        /// </summary>
        Waist = 0x0C,
        /// <summary>
        /// Torso, inner layer.
        /// </summary>
        InnerTorso = 0x0D,
        /// <summary>
        /// Bracelets.
        /// </summary>
        Bracelet = 0x0E,
        /// <summary>
        /// Unused.
        /// </summary>
        Unused_xF = 0x0F,
        /// <summary>
        /// Beards and mustaches.
        /// </summary>
        FacialHair = 0x10,
        /// <summary>
        /// Torso, outer layer.
        /// </summary>
        MiddleTorso = 0x11,
        /// <summary>
        /// Earings.
        /// </summary>
        Earrings = 0x12,
        /// <summary>
        /// Arms and sleeves.
        /// </summary>
        Arms = 0x13,
        /// <summary>
        /// Cloaks.
        /// </summary>
        Cloak = 0x14,
        /// <summary>
        /// Backpacks.
        /// </summary>
        Backpack = 0x15,
        /// <summary>
        /// Torso, outer layer.
        /// </summary>
        OuterTorso = 0x16,
        /// <summary>
        /// Leggings, outer layer.
        /// </summary>
        OuterLegs = 0x17,
        /// <summary>
        /// Leggings, inner layer.
        /// </summary>
        InnerLegs = 0x18,
        /// <summary>
        /// Last valid non-internal layer. Equivalent to <c>Layer.InnerLegs</c>.
        /// </summary>
        LastUserValid = 0x18,
        /// <summary>
        /// Mount item layer.
        /// </summary>
        Mount = 0x19,
        /// <summary>
        /// Vendor 'buy pack' layer.
        /// </summary>
        ShopBuy = 0x1A,
        /// <summary>
        /// Vendor 'resale pack' layer.
        /// </summary>
        ShopResale = 0x1B,
        /// <summary>
        /// Vendor 'sell pack' layer.
        /// </summary>
        ShopSell = 0x1C,
        /// <summary>
        /// Bank box layer.
        /// </summary>
        Bank = 0x1D,
        /// <summary>
        /// Last valid layer. Equivalent to <c>Layer.Bank</c>.
        /// </summary>
        LastValid = 0x1D
    }
}
