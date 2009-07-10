#region File Description & Usings
//-----------------------------------------------------------------------------
// GameObject.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    class GameObject : UltimaXNA.GameObjects.BaseObject
    {
        // GameObjects can potentially have inventory (chests, for example).
        // The GUID for the container for this inventory is the same as the
        // GameObject's GUID.
        private GameObject_Container m_ContainerObject = null;
        public GameObject_Container ContainerObject
        {
            get
            {
                if (m_ContainerObject == null)
                    m_ContainerObject = new GameObject_Container(this);
                return m_ContainerObject;
            }
        }

        public GameObject(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.GameObject;
        }

        protected override void Draw(UltimaXNA.TileEngine.MapCell nCell, Vector3 nLocation, Vector3 nOffset)
        {
            nCell.AddGameObjectTile(
                new TileEngine.GameObjectTile(mObjectTypeID, nLocation, Movement.DrawFacing, this.GUID, 0));
        }

        public override void Dispose()
        {
            // if is worn, let the wearer know we are disposing.
            if (Wearer != null)
                Wearer.UnWearItem(GUID);
            base.Dispose();
        }

        public Unit Wearer;
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
        public int Item_StackCount = 0;
        public int Item_ContainedWithinGUID = 0;
        

        public int AnimationDisplayID = 0;

        private int m_Hue;
        public int Hue // Fix for large hue values per issue12 (http://code.google.com/p/ultimaxna/issues/detail?id=12) --ZDW 6/15/2009
        {
            get { return m_Hue; }
            set
            {
                if (value > 2998)
                    m_Hue = (int)(value / 32);
                else
                    m_Hue = value;
            }
        }

        private int mObjectTypeID = 0;
        public Data.ItemData ItemData;

        public int ObjectTypeID
        {
            get { return mObjectTypeID; }
            set
            {
                m_HasBeenDrawn = false;
                mObjectTypeID = value;
                ItemData = UltimaXNA.Data.TileData.ItemData[mObjectTypeID];
                AnimationDisplayID = ItemData.AnimID;
            }
        }

        // Inventory position is handled differently in this client than in the legacy UO client.
        // All items have both an X and a Y position within the container. We use X for the SlotIndex
        // which this item occupies, and the Y as a Checksum for the X value: if the Y checksum validates,
        // then we know this item belongs in slot X.
        public int Item_InvX, Item_InvY, Item_InvSlot = 0;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (m_ContainerObject != null)
                m_ContainerObject.Update(gameTime);
        }

        public override string ToString()
        {
            return base.ToString() + " | " + ItemData.Name;
        }
    }
}
