#region File Description & Usings
//-----------------------------------------------------------------------------
// Container.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    // This is the class which contains the contents of a container's slots.
    class GameObject_ContainerContents
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

        public GameObject this[int nIndex]
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

    class GameObject_Container
    {
        // The parent object. All Containers are part of GameObjects. We need a way to reference them.
        private GameObject m_ParentObject;
        // All the contents of the container are kept in the mContents class,
        // unless they are being moved between slots or into or out of the container.
        private GameObject_ContainerContents mContentsClass = new GameObject_ContainerContents();
        private List<GameObject> mUnplacedItems = new List<GameObject>();
        // Update tickers are referenced by the GUI - when this value changes, the GUI knows to update.
        public int UpdateTicker { get { return mContentsClass.UpdateTicker; } }
        // Get the last occupied slot, so the GUI knows how many slots to draw.
        public int LastSlotOccupied { get { return mContentsClass.LastSlotOccupied; } }

        public GameObject_Container(GameObject nParent)
        {
            m_ParentObject = nParent;
        }

        public void Event_MoveItemToSlot(GameObject nObject, int nSlot)
        {
            // Is the destination slot empty?
            if (mContentsClass[nSlot] == null)
            {
                int x = nSlot;
                int y = x ^ 0x7fff;
                GUI.Events.PickupItem(nObject);
                GUI.Events.DropItem(nObject, x, y, 0, m_ParentObject.GUID);
                // if this container already contains this item, then temporarily remove it so that 
                // we don't end up with two copies.

                // if (mContentsClass.ContainsItem(nObject.GUID))
                //     mContentsClass.RemoveItemByGUID(nObject.GUID);
                // nObject.Item_SlotIndex = nSlot;
                // mContentsClass[nObject.Item_SlotIndex] = nObject;
            }
            else if (mContentsClass[nSlot] == nObject)
            {
                // No need to do anything here - this object is already in this slot!
            }
            else
            {
                // we need to put the other object in temporary storage...
                int iSourceSlot = nObject.Item_SlotIndex;
                GameObject iSwitchItem = mContentsClass[nSlot];

                // is the dest object the same type as the source object type?
                if (nObject.ItemData.Name == mContentsClass[nSlot].ItemData.Name)
                {
                    // We are merging two objects.
                    mContentsClass.RemoveItemByGUID(nObject.GUID);
                    nObject.Item_SlotIndex = iSwitchItem.Item_SlotIndex;
                    nObject.Item_ContainedWithinGUID = iSwitchItem.GUID;
                }
                else
                {
                    // We are switching these two objects.
                    int x = nSlot;
                    int y = x ^ 0x7fff;
                    GUI.Events.PickupItem(nObject);
                    GUI.Events.DropItem(nObject, x, y, 0, m_ParentObject.GUID);
                    x = iSourceSlot;
                    y = x ^ 0x7fff;
                    GUI.Events.PickupItem(iSwitchItem);
                    GUI.Events.DropItem(iSwitchItem, x, y, 0, m_ParentObject.GUID);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (mUnplacedItems.Count > 0)
            {
                int x = mContentsClass.NextAvailableSlot;
                int y = x ^ 0x7fff;
                GUI.Events.PickupItem(mUnplacedItems[0]);
                GUI.Events.DropItem(mUnplacedItems[0], x, y, 0, m_ParentObject.GUID);
                mUnplacedItems.Remove(mUnplacedItems[0]);
            }
        }

        public void AddItem(GameObject nObject)
        {
            for (int i = mUnplacedItems.Count - 1; i >= 0; i--)
                if (mUnplacedItems[i].GUID == nObject.GUID)
                    mUnplacedItems.Remove(mUnplacedItems[i]);

            // The server often sends as list of all the items in a container.
            // We want to filter out items we already have in our list.
            if ((m_ParentObject.Wearer != null) && (m_ParentObject.Wearer.ObjectType != ObjectType.Player))
            {
                // We can't move items in that we don't own.
                // This is only a temporary fix! What about moving things around in boxes?
                if (mContentsClass.ContainsItem(nObject.GUID))
                {
                    // don't add, already included.
                    return;
                }
                else
                {
                    // mContentsClass[mContentsClass.NextAvailableSlot] = nObject;
                    return;
                }
            }
            else
            {
                if (mContentsClass.ContainsItem(nObject.GUID))
                {
                    // We know the object is already in our container. Just for housekeeping, we're going to clear it out
                    // of this container temporarily so even if we encounter a bug, the item won't appear to ever be
                    // 'duped' in a player's inventory.
                    mContentsClass.RemoveItemByGUID(nObject.GUID);

                    // We need to check to see if the object's SlotIndex value validates, and if the item in slot[object.InvX]
                    // is this object. If either of these checks come back as false, then we need to move the object to a new slot.
                    if (nObject.Item_SlotIndex == -1)
                    {
                        // The item's InvX does not validate. This means that the item has not yet been sorted
                        // into an inventory slot. We correct this by sorting this object into the next available
                        // slot.
                        mUnplacedItems.Add(nObject);
                        return;
                    }
                    else
                    {
                        // The item's InvX value does validate. It belongs in this slot. Check to see if the slot
                        // is unoccupied before moving this item into it.
                        if (mContentsClass[nObject.Item_SlotIndex] == null)
                        {
                            // The slot is empty. Go ahead and move the item in.
                            mContentsClass[nObject.Item_SlotIndex] = nObject;
                            return;
                        }
                        else
                        {
                            // There is something else in this slot. We will switch the two items.
                            GameObject iSwitchItem = mContentsClass[nObject.Item_SlotIndex];
                            mContentsClass.RemoveItemByGUID(iSwitchItem.GUID);
                            mContentsClass[nObject.Item_SlotIndex] = nObject;
                            mUnplacedItems.Add(iSwitchItem);
                            return;
                        }
                    }
                }
                else
                {
                    // The item is not in our container. We need to place it in a slot. First we check if
                    // this new item's SlotIndex value validates.
                    if (nObject.Item_SlotIndex == -1)
                    {
                        // The SlotIndex does not validate. We need to place it in a new slot.
                        mUnplacedItems.Add(nObject);
                        return;
                    }
                    else
                    {
                        // The item's InvX value validates, so it thinks it occupies the slot. But what
                        // if we have already placed an item in that slot? We can't double book. Move the item to
                        // the next available slot.
                        if (mContentsClass[nObject.Item_SlotIndex] != null)
                        {
                            mUnplacedItems.Add(nObject);
                            return;
                        }
                        else
                        {
                            // The object's checksum validates and the slot it wants to move to is open. Move in!
                            mContentsClass[nObject.Item_SlotIndex] = nObject;
                            return;
                        }
                    }
                }
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
