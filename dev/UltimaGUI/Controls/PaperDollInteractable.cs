/***************************************************************************
 *   PaperDollInteractable.cs
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

        private static EquipSlots[] s_DrawOrder = new EquipSlots[21] {
            EquipSlots.Footwear,
            EquipSlots.Legging,
            EquipSlots.Shirt,
            EquipSlots.Sleeves,
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

        public PaperDollInteractable(Control owner, int page, int x, int y)
            : base(0, 0)
        {
            m_owner = owner;
            Position = new Point(x, y);
        }

        public override void Initialize()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (hasSourceEntity)
            {
                m_isFemale = ((Mobile)m_sourceEntity).Flags.IsFemale;
                m_isElf = false;
                if (m_sourceEntityUpdateHash != ((Mobile)m_sourceEntity).UpdateTicker)
                {
                    // update our hash
                    m_sourceEntityUpdateHash = ((Mobile)m_sourceEntity).UpdateTicker;

                    // clear the existing Controls
                    ClearControls();

                    // Add the base gump - the semi-naked paper doll.
                    if (true)
                    {
                        int bodyID = 12 + (m_isElf ? 2 : 0) + (m_isFemale ? 1 : 0); // ((Mobile)m_sourceEntity).BodyID;
                        GumpPic paperdoll = (GumpPic)AddControl(new GumpPic(this, 0, 0, 0, bodyID, ((Mobile)m_sourceEntity).Hue));
                        paperdoll.HandlesMouseInput = true;
                        paperdoll.IsPaperdoll = true;
                    }

                    // Loop through the items on the mobile and create the gump pics.
                    for (int i = 0; i < s_DrawOrder.Length; i++)
                    {
                        Item item = ((Mobile)m_sourceEntity).GetItem((int)s_DrawOrder[i]);
                        if (item == null)
                            continue;

                        bool canPickUp = true;
                        switch (s_DrawOrder[i])
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
                    }
                    // If this object has a backpack, draw it last.
                    if (((Mobile)m_sourceEntity).GetItem((int)EquipSlots.Backpack) != null)
                    {
                        Item backpack = ((Mobile)m_sourceEntity).GetItem((int)EquipSlots.Backpack);
                        AddControl(new GumpPicBackpack(this, 0, -5, 0, backpack));
                        LastControl.HandlesMouseInput = true;
                        LastControl.OnMouseDoubleClick += dblclick_Backpack;
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

        //void Interaction_OnItemPickUp(ItemGumplingPaperdoll Control)
        // {
        //     Control.Dispose();
       // }

        int m_sourceEntityUpdateHash = 0x7FFFFFFF;
        AEntity m_sourceEntity = null;
        public AEntity SourceEntity
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
