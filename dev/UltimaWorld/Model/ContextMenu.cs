﻿/***************************************************************************
 *   ContextMenu.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using System.Text;
#endregion

namespace UltimaXNA.UltimaPackets
{
    public class ContextMenuItem
    {
        private int m_responseCode;
        private string m_caption;

        public int ResponseCode 
        {
            get { return m_responseCode; } 
        } 

        public string Caption 
        {
            get { return m_caption; } 
        }

        public ContextMenuItem(int nResponseCode, int iStringID, int iFlags, int iHue)
        {
            m_caption = UltimaData.StringData.Entry(iStringID);
            m_responseCode = nResponseCode;
        }
    }

    public class ContextMenu
    {
        private List<ContextMenuItem> m_entries = new List<ContextMenuItem>();
        public ContextMenuItem ContextEntry(string iCaption)
        {
            foreach (ContextMenuItem i in m_entries)
            {
                if (i.Caption == iCaption)
                {
                    return i;
                }
            }
            return null;
        }

        private Serial m_serial;
        public Serial Serial { get { return m_serial; } }

        public bool HasContextMenu
        {
            get
            {
                return ((Merchant));
            }
        }

        public bool CanBuy = false;
        public bool CanSell = false;
        public bool Merchant { get { return ((CanBuy) || (CanSell)); } }
        // public bool HasContext_Bank = false;
        // public bool HasContext_Trainer = false;
        // public bool HasContext_Stable = false;
        // public bool HasContext_QuestGiver = false;
        // public bool HasContext_PaperDoll = false;
        // public bool HasContext_Inventory = false;

        public ContextMenu(Serial serial)
        {
            m_serial = serial;
        }

        // Add a new context menu entry.
        internal void AddItem(int nResponseCode, int nStringID, int nFlags, int nHue)
        {
            m_entries.Add(new ContextMenuItem(nResponseCode, nStringID, nFlags, nHue));
        }

        // Sets up which options are available to the new client.
        internal void FinalizeMenu()
        {
            foreach (ContextMenuItem i in m_entries)
            {
                if ((i.Caption.Length >= 5) && (i.Caption.Substring(0, 5) == "Train"))
                {
                    // train option
                }
                else
                {
                    switch (i.Caption)
                    {
                        case "Buy":
                            CanBuy = true;
                            break;
                        case "Sell":
                            CanSell = true;
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
                            throw new Exception("Unknown context item: " + i.Caption);
                    }
                }
            }
        }
    }
}
