/***************************************************************************
 *   PaperdollLargeUninteractable.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Rendering;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class PaperdollLargeUninteractable : Control
    {
        public enum EquipSlots : int
        {
            Body = 0, First = Body,
            RightHand = 1,
            LeftHand = 2,
            Footwear = 3,
            Legging = 4,
            Shirt = 5,
            Head = 5,
            Gloves = 7,
            Ring = 8,
            Talisman = 9,
            Neck = 10,
            Hair = 11,
            Belt = 12,
            Chest = 13,
            Bracelet = 14,
            // skip 15, unused
            FacialHair = 16,
            Sash = 17,
            Earring = 18,
            Sleeves = 19,
            Back = 20,
            Backpack = 21,
            Robe = 22,
            Skirt = 23,
            // skip 24, inner legs (!!! do we really skip this?)
            Max = 23,
        }

        int[] _equipmentSlots = new int[(int)EquipSlots.Max];
        int[] _hueSlots = new int[(int)EquipSlots.Max];

        bool _isFemale;
        public int Gender { set { _isFemale = (value == 1) ? true : false; } }
        bool _isElf;
        public int Race { set { _isElf = (value == 1) ? true : false; } }

        public bool IsCharacterCreation = false;

        public void SetSlotEquipment(EquipSlots slot, int gumpID)
        {
            _equipmentSlots[(int)slot] = gumpID;
        }
        public void SetSlotHue(EquipSlots slot, int gumpID)
        {
            _hueSlots[(int)slot] = gumpID;
        }

        public int this[EquipSlots slot]
        {
            set { _equipmentSlots[(int)slot] = value; }
            get { return _equipmentSlots[(int)slot]; }
        }

        PaperdollLargeUninteractable(Control owner, int page)
            : base(owner, page)
        {

        }

        public PaperdollLargeUninteractable(Control owner, int page, int x, int y)
            : this(owner, page)
        {
            Position = new Point2D(x, y);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            drawLargePaperdoll_Noninteractable(spriteBatch);
        }

        int equipmentSlot(EquipSlots slotID)
        {
            return _equipmentSlots[(int)slotID];
        }

        int hueSlot(EquipSlots slotID)
        {
            return _hueSlots[(int)slotID];
        }

        void drawLargePaperdoll_Noninteractable(SpriteBatchUI spriteBatch)
        {
            EquipSlots[] slotsToDraw = new EquipSlots[6] { EquipSlots.Body, EquipSlots.Footwear, EquipSlots.Legging, EquipSlots.Shirt, EquipSlots.Hair, EquipSlots.FacialHair };
            for (int i = 0; i < slotsToDraw.Length; i++)
            {
                int bodyID = 0, hue = hueSlot(slotsToDraw[i]);
                bool hueGreyPixelsOnly = true;

                switch (slotsToDraw[i])
                {
                    case EquipSlots.Body:
                        if (_isElf)
                            bodyID = (_isFemale ? 1893 : 1894);
                        else
                            bodyID = (_isFemale ? 1888 : 1889);
                        break;
                    case EquipSlots.Footwear:
                        bodyID = (_isFemale ? 1891 : 1890);
                        hue = 900;
                        break;
                    case EquipSlots.Legging:
                        bodyID = (_isFemale ? 1892 : 1848);
                        hue = 348;
                        break;
                    case EquipSlots.Shirt:
                        bodyID = (_isFemale ? 1812 : 1849);
                        hue = 792;
                        break;
                    case EquipSlots.Hair:
                        if (equipmentSlot(EquipSlots.Hair) != 0)
                        {
                            bodyID = _isFemale ?
                                UltimaData.HairStyles.FemaleGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.Hair)) :
                                UltimaData.HairStyles.MaleGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.Hair));
                            hueGreyPixelsOnly = false;
                        }
                        break;
                    case EquipSlots.FacialHair:
                        if (equipmentSlot(EquipSlots.FacialHair) != 0)
                        {
                            bodyID = _isFemale ?
                                0 : UltimaData.HairStyles.FacialHairGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.FacialHair));
                            hueGreyPixelsOnly = false;
                        }
                        break;
                }

                if (bodyID != 0)
                    spriteBatch.Draw2D(UltimaData.GumpData.GetGumpXNA(bodyID), Position, hue, hueGreyPixelsOnly, false);
            }
        }
    }
}
