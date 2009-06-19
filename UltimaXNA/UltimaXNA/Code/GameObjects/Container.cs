#region File Description & Usings
//-----------------------------------------------------------------------------
// Container.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
#endregion

namespace UltimaXNA.GameObjects
{
    // This is the class which contains the contents of a container's slots.
    class ContainerContents
    {
        private const int mNumSlots = 0x100;
        private GameObject[] mSlots = new GameObject[mNumSlots];

        // LastSlotOccupied is set to the value of the last slot within the container which has an object
        // within it. This is useful for: 1. Knowing how far through the container array to iterate when
        // looking for an object; 2. Knowing how many slots a ContainerFrame should show.
        public int LastSlotOccupied { get { return mLastSlotOccupied; } } private int mLastSlotOccupied = 0;

        // Whenever we change the contents of mContents in any way, we increment UpdateTicker.
        // When this value is different from the last time you checked it, you should update
        // your representation of this Container (example of something that would use this
        // variable: ContainerFrames).
        public int UpdateTicker = 0;

        // NextAvailableSlot returns the value of the first slot in mSlots[] not occupied by an item.
        public int NextAvailableSlot
        {
            get
            {
                for (int i = 0; i < mNumSlots; i++)
                {
                    if (mSlots[i] == null)
                        return i;
                }
                throw (new Exception("No open slot!"));
            }
        }

        public GameObject this [int nIndex]
        {
            get
            {
                if (nIndex > mNumSlots)
                    return null;
                return mSlots[nIndex];
            }
            set
            {
                mSlots[nIndex] = value;
                UpdateTicker++;
                m_UpdateLastSlot(nIndex);
            }
        }

        // This should be called whenever we change the contents of a slot.
        // * nSlotIndex: the Index of the slot being changed.
        private void m_UpdateLastSlot(int nIndex)
        {
            // iSlotOccupied: set to true if the slot is currently occupied, false if not.
            bool iSlotOccupied = mSlots[nIndex] != null ? true : false;

            if (iSlotOccupied)
            {
                // This slot has an item in it. Is it the last slot occupied?
                // If so, set mLastSlotOccupied to the index of this slot.
                if (mLastSlotOccupied < nIndex)
                    mLastSlotOccupied = nIndex;
            }
            else
            {
                // This slot has been vacated. If it used to be the last slot occupied, count back until
                // you find an occupied slot.
                if (mLastSlotOccupied == nIndex)
                {
                    for (int i = mLastSlotOccupied; i >= 0; i--)
                    {
                        if (mSlots[i] != null)
                        {
                            mLastSlotOccupied = i;
                            return;
                        }
                    }
                }
            }
        }

        // Does mSlots[] contain an item with GUID = nGUID?
        public bool ContainsItem(int nGUID)
        {
            for (int i = 0; i < mNumSlots; i++)
            {
                if (mSlots[i] != null)
                {
                    if (mSlots[i].GUID == nGUID)
                        return true;
                }
            }
            return false;
        }

        public void RemoveItemByGUID(int nGUID)
        {
            for (int i = 0; i < mNumSlots; i++)
            {
                if (mSlots[i] != null)
                {
                    if (mSlots[i].GUID == nGUID)
                    {
                        mSlots[i] = null;
                        UpdateTicker++;
                    }
                }
            }
        }
    }

    class Container : UltimaXNA.GameObjects.GameObject
    {
        // All the contents of the container are kept in the mContents class,
        // unless they are being moved between slots or into or out of the container.
        private ContainerContents mContentsClass = new ContainerContents();
        
        // Items that are being moved within or out of the container are in this list and not in mContents.
        // private List<Item> mContentsBeingMoved = new List<Item>();

        public int UpdateTicker { get { return mContentsClass.UpdateTicker; } }
        public int LastSlotOccupied { get { return mContentsClass.LastSlotOccupied; } } 




        public Container(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.Container;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Event_MoveItemToSlot(GameObject nObject, int nSlot)
        {
            // Is the destination slot empty?
            if (mContentsClass[nSlot] == null)
            {
                // if this container already contains this item, then temporarily remove it so that 
                // we don't end up with two copies.
                if (mContentsClass.ContainsItem(nObject.GUID))
                {
                    mContentsClass.RemoveItemByGUID(nObject.GUID);
                }
                nObject.Item_MoveToNewSlot(nSlot);
                mContentsClass[nObject.Item_InvX_SlotIndex] = nObject;
            }
            else
            {
                // we need to put the other object in temporary storage...
                int iSourceSlot = nObject.Item_InvX_SlotIndex;
                GameObject iSwitchItem = mContentsClass[nSlot];

                // Now temporarily remove both of the items from the container.
                mContentsClass.RemoveItemByGUID(iSwitchItem.GUID);
                if (mContentsClass.ContainsItem(nObject.GUID))
                    mContentsClass.RemoveItemByGUID(nObject.GUID);
                
                // Now replace them!
                nObject.Item_MoveToNewSlot(nSlot);
                mContentsClass[nObject.Item_InvX_SlotIndex] = nObject;
                iSwitchItem.Item_MoveToNewSlot(iSourceSlot);
                mContentsClass[iSwitchItem.Item_InvX_SlotIndex] = iSwitchItem;
            }
        }

        public void AddItem(GameObject nObject)
        {
            // The server often sends as list of all the items in a container.
            // We want to filter out items we already have in our list.
            if (mContentsClass.ContainsItem(nObject.GUID))
            {
                // We know the object is already in our container. However, it might have moved
                // between slots. We need to check to see if the object's InvX value validates,
                // and if the item in slot[object.InvX] is equal to this object. If either of
                // these checks come back as false, then we need to move the object to a new slot.
                if (!nObject.Item_ChecksumValidates())
                {
                    mContentsClass.RemoveItemByGUID(nObject.GUID);
                    nObject.Item_MoveToNewSlot(mContentsClass.NextAvailableSlot);
                    mContentsClass[nObject.Item_InvX_SlotIndex] = nObject;
                    return;
                }

                if (mContentsClass[nObject.Item_InvX_SlotIndex] != nObject)
                {
                    mContentsClass.RemoveItemByGUID(nObject.GUID);
                    nObject.Item_MoveToNewSlot(mContentsClass.NextAvailableSlot);
                    mContentsClass[nObject.Item_InvX_SlotIndex] = nObject;
                    return;
                }

                // At this point, we know that the object is already in the correct slot in mContents.
                // Nothing else to do!
                return;
            }
            else
            {
                // The item is not in our container. We need to place it in a slot. First we check if
                // this new item's InvX value validates - if it doesn't, then we need to place it in a
                // new slot.
                if (!nObject.Item_ChecksumValidates())
                {
                    nObject.Item_MoveToNewSlot(mContentsClass.NextAvailableSlot);
                    mContentsClass[nObject.Item_InvX_SlotIndex] = nObject;
                    return;
                }
                
                // The item's InvX value validates, so it thinks it occupies the slot == InvX. But what
                // if we have already placed an item in that slot? We can't double book. Move the item to
                // the next available slot.
                if (mContentsClass[nObject.Item_InvX_SlotIndex] != null)
                {
                    nObject.Item_MoveToNewSlot(mContentsClass.NextAvailableSlot);
                    mContentsClass[nObject.Item_InvX_SlotIndex] = nObject;
                    return;
                }

                // The object's checksum validates and the slot it wants to move to is open. Move in!
                mContentsClass[nObject.Item_InvX_SlotIndex] = nObject;
            }
        }

        public void RemoveItem(int nGUID)
        {
            mContentsClass.RemoveItemByGUID(nGUID);
        }

        public GameObject GetContents(int nIndex)
        {
            return mContentsClass[nIndex];
        }
    }
}
