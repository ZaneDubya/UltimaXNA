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
    public class GameObject : UltimaXNA.GameObjects.BaseObject
    {
        // GameObjects can potentially have inventory (chests, for example).
        // The Serial for the container for this inventory is the same as the
        // GameObject's Serial.
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

        public GameObject(Serial serial)
            : base(serial)
        {
            ObjectType = ObjectType.GameObject;
        }

        protected override void Draw(UltimaXNA.TileEngine.MapCell nCell, Vector3 nLocation, Vector3 nOffset)
        {
            nCell.AddGameObjectTile(
                new TileEngine.GameObjectTile(mObjectTypeID, nLocation, Movement.DrawFacing, this.Serial, 0));
        }

        public override void Dispose()
        {
            // if is worn, let the wearer know we are disposing.
            if (Wearer != null)
                Wearer.UnWearItem(Serial);
            base.Dispose();
        }

        public Unit Wearer;
        public int Item_StackCount = 0;
        public Serial Item_ContainedWithinSerial = 0;
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
                _HasBeenDrawn = false;
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
