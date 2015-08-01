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
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    class PaperDollInteractable : Gump
    {
        bool m_isFemale;
        bool m_isElf;

        WorldModel m_World;

        public PaperDollInteractable(AControl parent, int x, int y)
            : base(0, 0)
        {
            Parent = parent;
            Position = new Point(x, y);

            m_World = ServiceRegistry.GetService<WorldModel>();
        }

        public override void Dispose()
        {
            m_sourceEntity.OnEntityUpdated -= OnEntityUpdated;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_sourceEntity != null)
            {
                m_isFemale = ((Mobile)m_sourceEntity).Flags.IsFemale;
                m_isElf = false;
            }
            base.Update(totalMS, frameMS);
        }

        private void OnEntityUpdated()
        {
            // clear the existing Controls
            ClearControls();

            // Add the base gump - the semi-naked paper doll.
            if (true)
            {
                int bodyID = 12 + (m_isElf ? 2 : 0) + (m_isFemale ? 1 : 0); // ((Mobile)m_sourceEntity).BodyID;
                GumpPic paperdoll = (GumpPic)AddControl(new GumpPic(this, 0, 0, bodyID, ((Mobile)m_sourceEntity).Hue));
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
                    case PaperDollEquipSlots.FacialHair:
                    case PaperDollEquipSlots.Hair:
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
            // If this object has a backpack, add it last.
            if (((Mobile)m_sourceEntity).GetItem((int)PaperDollEquipSlots.Backpack) != null)
            {
                Item backpack = ((Mobile)m_sourceEntity).GetItem((int)PaperDollEquipSlots.Backpack);
                AddControl(new GumpPicBackpack(this, -7, 0, backpack));
                LastControl.HandlesMouseInput = true;
                LastControl.MouseDoubleClickEvent += On_Dblclick_Backpack;
            }
        }

        private void On_Dblclick_Backpack(AControl control, int x, int y, MouseButton button)
        {
            Container backpack = ((Mobile)m_sourceEntity).Backpack;
            m_World.Interaction.DoubleClick(backpack);
        }

        AEntity m_sourceEntity = null;
        public AEntity SourceEntity
        {
            set
            {
                if (value != m_sourceEntity)
                {
                    if (m_sourceEntity != null)
                    {
                        m_sourceEntity.OnEntityUpdated -= OnEntityUpdated;
                        m_sourceEntity = null;
                    }

                    if (value is Mobile)
                    {
                        m_sourceEntity = value;
                        // update the gump
                        OnEntityUpdated();
                        // if the entity changes in the future, update the gump again
                        m_sourceEntity.OnEntityUpdated += OnEntityUpdated;
                    }
                }
            }
            get
            {
                return m_sourceEntity;
            }
        }

        private enum PaperDollEquipSlots : int
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

        private static PaperDollEquipSlots[] s_DrawOrder = new PaperDollEquipSlots[21] {
            PaperDollEquipSlots.Footwear,
            PaperDollEquipSlots.Legging,
            PaperDollEquipSlots.Shirt,
            PaperDollEquipSlots.Sleeves,
            PaperDollEquipSlots.Gloves,
            PaperDollEquipSlots.Ring,
            PaperDollEquipSlots.Talisman,
            PaperDollEquipSlots.Neck,
            PaperDollEquipSlots.Belt,
            PaperDollEquipSlots.Chest,
            PaperDollEquipSlots.Bracelet,
            PaperDollEquipSlots.Hair,
            PaperDollEquipSlots.FacialHair,
            PaperDollEquipSlots.Head,
            PaperDollEquipSlots.Sash,
            PaperDollEquipSlots.Earring,
            PaperDollEquipSlots.Back,
            PaperDollEquipSlots.Skirt,
            PaperDollEquipSlots.Robe,
            PaperDollEquipSlots.LeftHand,
            PaperDollEquipSlots.RightHand
        };
    }
}
