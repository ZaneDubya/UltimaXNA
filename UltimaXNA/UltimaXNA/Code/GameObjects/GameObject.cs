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
    delegate void EVENT_AutomaticMoveWithinContainer(BaseObject nThis);

    class GameObject : UltimaXNA.GameObjects.BaseObject
    {
        // GameObjects can potentially have inventory (chests, for example).
        // The GUID for the container for this inventory is the same as the
        // GameObject's GUID.
        private Container m_ContainerObject = null;
        public Container ContainerObject
        {
            get
            {
                if (m_ContainerObject == null)
                    m_ContainerObject = new Container(this.GUID);
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

        public event EVENT_AutomaticMoveWithinContainer SendPacket_MoveItemWithinContainer;

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
        // Used for Inventory position.
        public int Item_InvY_SlotChecksum = 0;
        public int Item_InvX_SlotIndex = 0;

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
        public DataLocal.ItemData ItemData;

        public int ObjectTypeID
        {
            get { return mObjectTypeID; }
            set
            {
                m_HasBeenDrawn = false;
                mObjectTypeID = value;
                ItemData = UltimaXNA.DataLocal.TileData.ItemData[mObjectTypeID];
                AnimationDisplayID = ItemData.AnimID;
            }
        }

        public void Item_MoveToNewSlot(int nX)
        {
            this.Item_InvX_SlotIndex = nX;
            this.Item_InvY_SlotChecksum = m_InvYChecksum(nX);
            SendPacket_MoveItemWithinContainer(this);
        }

        public bool Item_ChecksumValidates()
        {
            if (this.Item_InvY_SlotChecksum == m_InvYChecksum(this.Item_InvX_SlotIndex))
                return true;
            else
                return false;
        }

        private int m_InvYChecksum(int nInvX)
        {
            return nInvX ^ 0x7fff;
        }
    }
}
