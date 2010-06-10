using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;

namespace UltimaXNA.UILegacy.Gumplings
{
    class Paperdoll : Control
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
            // skip 21, backpack
            Robe = 21,
            Skirt = 22,
            // skip 23, inner legs (!!! do we really skip this?)
            Max,
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

        float _scaleUp;

        Paperdoll(Control owner, int page)
            : base(owner, page)
        {

        }

        public Paperdoll(Control owner, int page, int x, int y, float scale)
            : this(owner, page)
        {
            Position = new Vector2(x, y);
            _scaleUp = scale;
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            // special case - character creation paperdolls are larger
            if (IsCharacterCreation)
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
                                    Data.HairStyles.FemaleGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.Hair)) :
                                    Data.HairStyles.MaleGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.Hair));
                                hueGreyPixelsOnly = false;
                            }
                            break;
                        case EquipSlots.FacialHair:
                            if (equipmentSlot(EquipSlots.FacialHair) != 0)
                            {
                                bodyID = _isFemale ?
                                    0 : Data.HairStyles.FacialHairGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.FacialHair));
                                hueGreyPixelsOnly = false;
                            }
                            break;
                    }

                    if (bodyID != 0)
                        spriteBatch.Draw(Data.Gumps.GetGumpXNA(bodyID), Position, hue, hueGreyPixelsOnly);
                }
            }
            else
            {
                for (EquipSlots i = EquipSlots.First; i < EquipSlots.Max; i++)
                {
                    int bodyID = 0;
                    int hue = hueSlot(i);
                    bool hueGreyPixelsOnly = ((hue & 0x8000) == 0x8000 ? true : false);
                    hue &= 0x7FFF;
                    switch (i)
                    {
                        case EquipSlots.Body:
                            bodyID = 12 + (_isElf ? 2 : 0) + (_isFemale ? 1 : 0);
                            break;
                        case EquipSlots.Hair:
                        case EquipSlots.FacialHair:
                            if (equipmentSlot(i) != 0)
                            {
                                Data.ItemData data = Data.TileData.ItemData[equipmentSlot(i)];
                                bodyID = data.AnimID + (_isFemale ? 60000 : 50000);
                            }
                            break;
                        default:
                            if (equipmentSlot(i) != 0)
                            {
                                Data.ItemData data = Data.TileData.ItemData[equipmentSlot(i)];
                                bodyID = data.AnimID + (_isFemale ? 60000 : 50000);
                            }
                            break;
                    }

                    if (bodyID != 0)
                        spriteBatch.Draw(Data.Gumps.GetGumpXNA(bodyID), Position, hue, hueGreyPixelsOnly);
                }
            }
        }

        int equipmentSlot(EquipSlots slotID)
        {
            return _equipmentSlots[(int)slotID];
        }

        int hueSlot(EquipSlots slotID)
        {
            return _hueSlots[(int)slotID];
        }

        public override void Update(GameTime gameTime)
        {
            if (hasSourceEntity)
            {
                if (_sourceEntityUpdateHash != ((Mobile)_sourceEntity).UpdateTicker)
                {
                    for (EquipSlots i = EquipSlots.First; i < EquipSlots.Max; i++)
                    {
                        int itemID, hue;
                        ((Mobile)_sourceEntity).GetItemInfo((int)i, out itemID, out hue);
                        _equipmentSlots[(int)i] = itemID;
                        _hueSlots[(int)i] = hue;
                    }
                }
            }
 	        base.Update(gameTime);
        }

        int _sourceEntityUpdateHash = 0x7FFFFFFF;
        Entity _sourceEntity = null;
        public Entity SourceEntity
        {
            set
            {
                if ((value is Entities.Mobile) || value is Entities.PlayerMobile)
                {
                    _sourceEntity = value;
                    _sourceEntityUpdateHash = 0x7FFFFFFF;
                }
            }
            get
            {
                return _sourceEntity;
            }
        }
        bool hasSourceEntity
        {
            get
            {
                return (_sourceEntity == null) ? false : true;
            }
        }
    }
}
