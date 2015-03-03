/***************************************************************************
 *   PaperDollInteractable.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Network;
using UltimaXNA.Entity;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets.Server;

namespace UltimaXNA.UltimaGUI.Controls
{
    class PaperDollInteractable : Gump
    {
        public enum EquipSlots : int
        {
            Body = 0,
            RightHand = 1,
            LeftHand = 2,
            Footwear = 3,
            Legging = 4,
            Shirt = 5,
            Head = 6,
            Gloves = 7,
            Ring = 8,
            Talisman = 9,
            Neck = 10,
            Hair = 11,
            Belt = 12,
            Chest = 13,
            Bracelet = 14,
            Unused = 15,
            FacialHair = 16,
            Sash = 17,
            Earring = 18,
            Sleeves = 19,
            Back = 20,
            Backpack = 21,
            Robe = 22,
            Skirt = 23,
            // skip 24, inner legs (!!! do we really skip this?)
        }

        private EquipSlots[] drawOrder = new EquipSlots[20] {
            EquipSlots.Footwear,
            EquipSlots.Legging,
            EquipSlots.Shirt,
            EquipSlots.Gloves,
            EquipSlots.Ring,
            EquipSlots.Talisman,
            EquipSlots.Neck,
            EquipSlots.Belt,
            EquipSlots.Chest,
            EquipSlots.Bracelet,
            EquipSlots.Hair,
            EquipSlots.FacialHair,
            EquipSlots.Head,
            EquipSlots.Sash,
            EquipSlots.Earring,
            EquipSlots.Back,
            EquipSlots.Skirt,
            EquipSlots.Robe,
            EquipSlots.LeftHand,
            EquipSlots.RightHand
        };

        bool m_isFemale;
        bool m_isElf;

        DropWidget m_dropWidgetPaperdoll;
        DropWidget m_dropWidgetBackpack;

        public PaperDollInteractable(Control owner, int page, int x, int y)
            : base(0, 0)
        {
            m_owner = owner;
            Position = new Point2D(x, y);
        }

        public override void Initialize()
        {
            m_dropWidgetPaperdoll = new DropWidget(onItemDropPaperdoll, onItemOverPaperdoll);
            m_dropWidgetBackpack = new DropWidget(onItemDropBackpack, onItemOverBackpack);
        }

        public override void Update(GameTime gameTime)
        {
            if (hasSourceEntity)
            {
                m_isFemale = ((Mobile)m_sourceEntity).IsFemale;
                m_isElf = false;
                if (m_sourceEntityUpdateHash != ((Mobile)m_sourceEntity).UpdateTicker)
                {
                    // update our hash
                    m_sourceEntityUpdateHash = ((Mobile)m_sourceEntity).UpdateTicker;

                    // clear the existing Controls
                    ClearControls();
                    m_dropWidgetPaperdoll.ClearDropTargets();
                    m_dropWidgetBackpack.ClearDropTargets();

                    // Add the base gump - the semi-naked paper doll.
                    if (true)
                    {
                        int bodyID = 12 + (m_isElf ? 2 : 0) + (m_isFemale ? 1 : 0); // ((Mobile)m_sourceEntity).BodyID;
                        AddControl(new GumpPic(this, 0, 0, 0, bodyID, ((Mobile)m_sourceEntity).Hue));
                        LastControl.HandlesMouseInput = true;
                        m_dropWidgetPaperdoll.AddDropTarget(LastControl);
                    }

                    // Loop through the items on the mobile and create the gump pics.
                    for (int i = 0; i < drawOrder.Length; i++)
                    {
                        Item item = ((Mobile)m_sourceEntity).GetItem((int)drawOrder[i]);
                        if (item == null)
                            continue;

                        bool canPickUp = true;
                        switch (drawOrder[i])
                        {
                            case EquipSlots.FacialHair:
                            case EquipSlots.Hair:
                                canPickUp = false;
                                break;
                            default:
                                break;
                        }

                        AddControl(new ItemGumplingPaperdoll(this, 0, 0, item));
                        ((ItemGumplingPaperdoll)LastControl).SlotIndex = (int)i;
                        ((ItemGumplingPaperdoll)LastControl).IsFemale = m_isFemale;
                        ((ItemGumplingPaperdoll)LastControl).CanPickUp = canPickUp;
                        m_dropWidgetPaperdoll.AddDropTarget(LastControl);
                    }
                    // If this object has a backpack, draw it last.
                    if (((Mobile)m_sourceEntity).GetItem((int)EquipSlots.Backpack) != null)
                    {
                        AddControl(new GumpPic(this, 0, -5, 0, 0xC4F6, 0));
                        LastControl.HandlesMouseInput = true;
                        LastControl.OnMouseDoubleClick += dblclick_Backpack;
                        m_dropWidgetBackpack.AddDropTarget(LastControl);
                    }
                }
            }
            base.Update(gameTime);
        }

        void dblclick_Backpack(int x, int y, MouseButton button)
        {
            Container i = ((Mobile)m_sourceEntity).Backpack;
            UltimaInteraction.DoubleClick(i);
        }

        void onItemDropPaperdoll()
        {
            UltimaInteraction.WearItem(UltimaInteraction.Cursor.HoldingItem);
        }

        void onItemOverPaperdoll()
        {

        }

        void onItemDropBackpack()
        {
            UltimaInteraction.DropItemToContainer(UltimaInteraction.Cursor.HoldingItem, (Container)((Mobile)m_sourceEntity).GetItem((int)EquipSlots.Backpack));
        }

        void onItemOverBackpack()
        {

        }

        //void Interaction_OnItemPickUp(ItemGumplingPaperdoll Control)
        // {
        //     Control.Dispose();
       // }

        int m_sourceEntityUpdateHash = 0x7FFFFFFF;
        BaseEntity m_sourceEntity = null;
        public BaseEntity SourceEntity
        {
            set
            {
                if ((value is Mobile) || value is PlayerMobile)
                {
                    m_sourceEntity = value;
                    m_sourceEntityUpdateHash = 0x7FFFFFFF;
                }
            }
            get
            {
                return m_sourceEntity;
            }
        }
        bool hasSourceEntity
        {
            get
            {
                return (m_sourceEntity == null) ? false : true;
            }
        }
    }
}
