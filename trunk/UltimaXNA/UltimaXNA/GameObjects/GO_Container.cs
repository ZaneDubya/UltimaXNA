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
                // if this container already contains this item, then temporarily remove it so that 
                // we don't end up with two copies.
                if (mContentsClass.ContainsItem(nObject.GUID))
                    mContentsClass.RemoveItemByGUID(nObject.GUID);
                nObject.Item_InvSlot = nSlot;
                mContentsClass[nObject.Item_InvSlot] = nObject;
            }
            else if (mContentsClass[nSlot] == nObject)
            {
                // No need to do anything here - this object is already in this slot!
            }
            else
            {
                // we need to put the other object in temporary storage...
                int iSourceSlot = nObject.Item_InvSlot;
                GameObject iSwitchItem = mContentsClass[nSlot];

                // is the dest object the same type as the source object type?
                if (nObject.ItemData.Name == mContentsClass[nSlot].ItemData.Name)
                {
                    // We are merging two objects.
                    GUI.Events.PickupItem(nObject);
                    GUI.Events.DropItem(nObject, 0, 0, 0, iSwitchItem.GUID);
                    mContentsClass.RemoveItemByGUID(nObject.GUID);
                }
                else
                {
                    // We are switching these two objects.
                    nObject.Item_InvSlot = nSlot;
                    mContentsClass[nSlot] = nObject;
                    iSwitchItem.Item_InvSlot = iSourceSlot;
                    mContentsClass[iSourceSlot] = iSwitchItem;
                }
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void AddItem(GameObject nObject)
        {
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
                    // We know the object is already in our container.
                }
                else
                {
                    // The item is not in our container. We need to place it in a slot.
                    if (nObject.Item_InvY == 0x7FFF)
                    {
                        if (mContentsClass[nObject.Item_InvX] == null)
                        {
                            nObject.Item_InvSlot = nObject.Item_InvX;
                            mContentsClass[nObject.Item_InvSlot] = nObject;
                        }
                        else
                        {
                            nObject.Item_InvSlot = mContentsClass.NextAvailableSlot;
                            mContentsClass[nObject.Item_InvSlot] = nObject;
                        }
                    }
                    else
                    {
                        nObject.Item_InvSlot = mContentsClass.NextAvailableSlot;
                        mContentsClass[nObject.Item_InvSlot] = nObject;
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
