/***************************************************************************
 *   ContextMenu.cs
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
using System.Linq;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Data
{
    public class ContextMenuData
    {
        private readonly List<ContextMenuItem> m_entries = new List<ContextMenuItem>();
        private readonly Serial m_serial;

        public bool HasBuy = false;
        public bool HasSell = false;

        // public bool HasContext_Bank = false;
        // public bool HasContext_Trainer = false;
        // public bool HasContext_Stable = false;
        // public bool HasContext_QuestGiver = false;
        // public bool HasContext_PaperDoll = false;
        // public bool HasContext_Inventory = false;

        public ContextMenuData(Serial serial)
        {
            m_serial = serial;
        }

        public Serial Serial
        {
            get { return m_serial; }
        }

        public bool HasContextMenu
        {
            get { return IsMerchant; }
        }

        public bool IsMerchant
        {
            get { return ((HasBuy) || (HasSell)); }
        }

        public ContextMenuItem GetContextEntry(string caption)
        {
            return m_entries.FirstOrDefault(i => i.Caption == caption);
        }

        // Add a new context menu entry.
        internal void AddItem(int nResponseCode, int nStringID, int nFlags, int nHue)
        {
            m_entries.Add(new ContextMenuItem(nResponseCode, nStringID, nFlags, nHue));
        }

        // Sets up which options are available to the new client.
        internal void FinalizeMenu()
        {
            foreach(ContextMenuItem item in m_entries)
            {
                if((item.Caption.Length >= 5) && (item.Caption.Substring(0, 5) == "Train"))
                {
                    // train option
                }
                else
                {
                    switch(item.Caption)
                    {
                        case "Buy":
                            HasBuy = true;
                            break;
                        case "Sell":
                            HasSell = true;
                            break;
                        case "Open Paperdoll":
                        case "Open Backpack":
                        case "Quest Conversation":
                        case "Cancel Quest":
                        case "Toggle Item Insurance":
                        case "Auto Renew Inventory Insurance":
                        case "Toggle Monster Title Display":
                        case "Tame":
                        case "Command: Guard":
                        case "Command: Follow":
                        case "Command: Drop":
                        case "Command: Kill":
                        case "Command: Stop":
                        case "Command: Stay":
                        case "Add Friend":
                        case "Remove Friend":
                        case "Transfer":
                        case "Release":
                        case "Ask Destination":
                        case "Abandon Escort":
                        case "Open Bankbox":
                        case "Add Party Member":
                            // unhandled
                            break;
                        default:
                            throw new Exception("Unknown context item: " + item.Caption);
                    }
                }
            }
        }
    }
}