/***************************************************************************
 *   GO_Container.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Entities
{
    // This is the class which contains the contents of a container's slots.
    class ContainerContents
    {
        private const int _NumberOfSlots = 0x100;
        private Item[] _SlotContents = new Item[_NumberOfSlots];

        // LastSlotOccupied is set to the value of the last slot within the container which has an object
        // within it. This is useful for: 1. Knowing how far through the container array to iterate when
        // looking for an object; 2. Knowing how many slots a ContainerFrame should show.
        private int _LastSlotOccupied = 0;
        public int LastSlotOccupied { get { return _LastSlotOccupied; } } 

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
                for (int i = 0; i < _NumberOfSlots; i++)
                {
                    if (_SlotContents[i] == null)
                        return i;
                }
                throw (new Exception("No open slot!"));
            }
        }

        public Item this[int nIndex]
        {
            get
            {
                if (nIndex > _NumberOfSlots)
                    return null;
                return _SlotContents[nIndex];
            }
            set
            {
                _SlotContents[nIndex] = value;
                UpdateTicker++;
                updateLastSlot(nIndex);
            }
        }

        // This should be called whenever we change the contents of a slot.
        // * nSlotIndex: the Index of the slot being changed.
        private void updateLastSlot(int nIndex)
        {
            // iSlotOccupied: set to true if the slot is currently occupied, false if not.
            bool isSlotOccupied = _SlotContents[nIndex] != null ? true : false;

            if (isSlotOccupied)
            {
                // This slot has an item in it. Is it the last slot occupied?
                // If so, set mLastSlotOccupied to the index of this slot.
                if (_LastSlotOccupied < nIndex)
                    _LastSlotOccupied = nIndex;
            }
            else
            {
                // This slot has been vacated. If it used to be the last slot occupied, count back until
                // you find an occupied slot.
                if (_LastSlotOccupied == nIndex)
                {
                    for (int i = _LastSlotOccupied; i >= 0; i--)
                    {
                        if (_SlotContents[i] != null)
                        {
                            _LastSlotOccupied = i;
                            return;
                        }
                    }
                }
            }
        }

        // Does mSlots[] contain an item with Serial = serial?
        public bool ContainsItem(Serial serial)
        {
            for (int i = 0; i < _NumberOfSlots; i++)
            {
                if (_SlotContents[i] != null)
                {
                    if (_SlotContents[i].Serial == serial)
                        return true;
                }
            }
            return false;
        }

        public void RemoveItemBySerial(Serial serial)
        {
            for (int i = 0; i < _NumberOfSlots; i++)
            {
                if (_SlotContents[i] != null)
                {
                    if (_SlotContents[i].Serial == serial)
                    {
                        _SlotContents[i] = null;
                        UpdateTicker++;
                    }
                }
            }
        }
    }

    public class GameObject_Container
    {
        // The parent object. All Containers are part of GameObjects. We need a way to reference them.
        private Item _ParentObject;
        // All the contents of the container are kept in the mContents class,
        // unless they are being moved between slots or into or out of the container.
        private ContainerContents _contents = new ContainerContents();
        // Update tickers are referenced by the GUI - when this value changes, the GUI knows to update.
        public int UpdateTicker { get { return _contents.UpdateTicker; } }
        // Get the last occupied slot, so the GUI knows how many slots to draw.
        public int LastSlotOccupied { get { return _contents.LastSlotOccupied; } }

        public GameObject_Container(Item nParent)
        {
            _ParentObject = nParent;
        }

        public void Event_MoveItemToSlot(Item nObject, int nSlot)
        {
            // Is the destination slot empty?
            if (_contents[nSlot] == null)
            {
                // if this container already contains this item, then temporarily remove it so that 
                // we don't end up with two copies.
                if (_contents.ContainsItem(nObject.Serial))
                    _contents.RemoveItemBySerial(nObject.Serial);
                nObject.Item_InvSlot = nSlot;
                _contents[nObject.Item_InvSlot] = nObject;
            }
            else if (_contents[nSlot] == nObject)
            {
                // No need to do anything here - this object is already in this slot!
            }
            else
            {
                // we need to put the other object in temporary storage...
                int iSourceSlot = nObject.Item_InvSlot;
                Item iSwitchItem = _contents[nSlot];

                // is the dest object the same type as the source object type?
                if (nObject.ItemData.Name == _contents[nSlot].ItemData.Name)
                {
                    // We are merging two objects.
                    GUI.Events.PickupItem(nObject);
                    GUI.Events.DropItem(nObject, 0, 0, 0, iSwitchItem.Serial);
                    _contents.RemoveItemBySerial(nObject.Serial);
                }
                else
                {
                    // We are switching these two objects.
                    nObject.Item_InvSlot = nSlot;
                    _contents[nSlot] = nObject;
                    iSwitchItem.Item_InvSlot = iSourceSlot;
                    _contents[iSourceSlot] = iSwitchItem;
                }
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void AddItem(Item item)
        {
            // The server often sends as list of all the items in a container.
            // We want to filter out items we already have in our list.
            if (_contents.ContainsItem(item.Serial))
            {
                // We know the object is already in our container.
            }
            else
            {
                // The item is not in our container. We need to place it in a slot.
                addItem(item);
            }
        }

        private void addItem(Item item)
        {
            if (item.Item_InvY == 0x7FFF)
            {
                if (_contents[item.Item_InvX] == null)
                {
                    item.Item_InvSlot = item.Item_InvX;
                    _contents[item.Item_InvSlot] = item;
                }
                else
                {
                    item.Item_InvSlot = _contents.NextAvailableSlot;
                    _contents[item.Item_InvSlot] = item;
                }
            }
            else
            {
                item.Item_InvSlot = _contents.NextAvailableSlot;
                _contents[item.Item_InvSlot] = item;
            }
        }

        public void RemoveItem(Serial serial)
        {
            _contents.RemoveItemBySerial(serial);
        }

        public Item GetContents(int nIndex)
        {
            return _contents[nIndex];
        }
    }
}
