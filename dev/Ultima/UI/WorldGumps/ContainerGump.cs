/***************************************************************************
 *   ContainerGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class ContainerGump : Gump
    {
        ContainerData m_data;
        ContainerItem m_item;

        public ContainerGump(AEntity containerItem, int gumpID)
            : base(containerItem.Serial, 0)
        {
            m_data = ContainerData.Get(gumpID);
            m_item = (ContainerItem)containerItem;
            m_item.SetCallbacks(OnItemUpdated, OnItemDisposed);
            IsMoveable = true;
            AddControl(new GumpPicContainer(this, 0, 0, m_data.GumpID, 0, m_item));
        }

        public override void Dispose()
        {
            m_item.ClearCallBacks(OnItemUpdated, OnItemDisposed);
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        void OnItemUpdated(AEntity entity)
        {
            // delete any items in our pack that are no longer in the container.
            List<AControl> ControlsToRemove = new List<AControl>();
            foreach (AControl c in Children)
            {
                if (c is ItemGumpling && !m_item.Contents.Contains(((ItemGumpling)c).Item))
                {
                    ControlsToRemove.Add(c);
                }
            }
            foreach (AControl c in ControlsToRemove)
                Children.Remove(c);

            // add any items in the container that are not in our pack.
            foreach (Item item in m_item.Contents)
            {
                bool controlForThisItem = false;
                foreach (AControl c in Children)
                {
                    if (c is ItemGumpling && ((ItemGumpling)c).Item == item)
                    {
                        controlForThisItem = true;
                        break;
                    }
                }
                if (!controlForThisItem)
                {
                    AddControl(new ItemGumpling(this, item));
                }
            }
        }

        void OnItemDisposed(AEntity entity)
        {
            Dispose();
        }
    }
}
