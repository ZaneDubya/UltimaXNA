#region File Description & Usings
//-----------------------------------------------------------------------------
// Item.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
#endregion

namespace UltimaXNA.GameObjects
{
    class Item : UltimaXNA.GameObjects.BaseObject
    {
        public int DisplayID = 0;
        public int Hue = 0;
        public int StackCount = 0;
        public int ContainedWithinGUID = 0;

        // These will be added later...
        // public int Item_Type = 0;
        // public int Item_SubType = 0;
        // public int Item_OwnerGUID = 0;
        // public int Item_ContainedWithinGUID = 0;
        // public int Item_CreatorGUID = 0;
        // public int Item_GiftedByGUID = 0;
        // public int Item_StackCount = 0;
        // public int Item_Duration = 0;
        // public int Item_PropertySeed = 0;
        // public int Item_RandomPropertiesID = 0;
        // public int Item_TextID = 0;
        // public int Item_CurrentDurability = 0;
        // public int Item_CurrentSpellCharges = 0;
        // public ItemEnchantment[] Item_Enchantments = new ItemEnchantment[6];

        private int m_ItemTypeID = 0;
        private DataLocal.ItemData m_ItemData;

        public int ItemTypeID
        {
            get { return m_ItemTypeID; }
            set
            {
                m_ItemTypeID = value;
                m_ItemData = UltimaXNA.DataLocal.TileData.ItemData[m_ItemTypeID];
                DisplayID = m_ItemData.AnimID;
            }
        }

        public Item(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.Item;

            for (int i = 0; i < 6; i++)
            {
                 // Item_Enchantments[i] = new ItemEnchantment();

            }
        }
    }
}
