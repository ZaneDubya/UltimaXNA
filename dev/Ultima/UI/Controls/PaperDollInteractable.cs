/***************************************************************************
 *   PaperDollInteractable.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Input.Windows;
using Microsoft.Xna.Framework;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.Entities.Mobiles;
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.Entities.Items.Containers;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    class PaperDollInteractable : Gump
    {
        bool m_isFemale;
        bool m_isElf;

        WorldModel m_World;

        public PaperDollInteractable(AControl owner, int page, int x, int y)
            : base(0, 0)
        {
            m_owner = owner;
            Position = new Point(x, y);

            m_World = UltimaServices.GetService<WorldModel>();
        }

        public override void Update(double totalMS, double frameMS)
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
            base.Update(totalMS, frameMS);
        }

        void dblclick_Backpack(int x, int y, MouseButton button)
        {
            Container backpack = ((Mobile)m_sourceEntity).Backpack;
            m_World.Interaction.DoubleClick(backpack);
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
    }
}
