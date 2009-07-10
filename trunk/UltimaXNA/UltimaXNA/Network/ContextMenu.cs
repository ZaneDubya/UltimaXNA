﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
{
    public class ContextMenuItemNew
    {
        private int _responseCode;
        private string _caption;

        public int ResponseCode 
        {
            get { return _responseCode; } 
        } 

        public string Caption 
        {
            get { return _caption; } 
        }

        public ContextMenuItemNew(int nResponseCode, int iStringID, int iFlags, int iHue)
        {
            _caption = Data.StringList.Table[iStringID].ToString();
            _responseCode = nResponseCode;
        }
    }

    public class ContextMenuNew
    {
        private List<ContextMenuItemNew> _entries = new List<ContextMenuItemNew>();
        public ContextMenuItemNew ContextEntry(string iCaption)
        {
            foreach (ContextMenuItemNew i in _entries)
            {
                if (i.Caption == iCaption)
                {
                    return i;
                }
            }
            return null;
        }

        private Serial _serial;
        public Serial Serial { get { return _serial; } }

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

        public ContextMenuNew(Serial serial)
        {
            _serial = serial;
        }

        // Add a new context menu entry.
        internal void AddItem(int nResponseCode, int nStringID, int nFlags, int nHue)
        {
            _entries.Add(new ContextMenuItemNew(nResponseCode, nStringID, nFlags, nHue));
        }

        // Sets up which options are available to the new client.
        internal void FinalizeMenu()
        {
            foreach (ContextMenuItemNew i in _entries)
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
