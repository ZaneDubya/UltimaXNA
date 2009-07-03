using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
{
    class ContextMenuItem
    {
        public int ResponseCode { get { return m_ResponseCode; } } private int m_ResponseCode;
        // private int m_StringID;
        // private int m_Flags;
        // private int m_Hue;
        private string m_Caption;
        public string Caption { get { return m_Caption; } }

        public ContextMenuItem(int nResponseCode, int iStringID, int iFlags, int iHue)
        {
            m_Caption = UltimaXNA.DataLocal.StringList.Table[iStringID].ToString();
            m_ResponseCode = nResponseCode;

        }
    }

    class ContextMenu
    {
        private List<ContextMenuItem> m_Entries = new List<ContextMenuItem>();

        public bool HasContextMenu
        {
            get
            {
                return (
                    (HasContext_Merchant) ||
                    (HasContext_Merchant));
            }
        }

        public bool CanBuy = false;
        public bool CanSell = false;
        public bool HasContext_Merchant = false;
        // public bool HasContext_Bank = false;
        // public bool HasContext_Trainer = false;
        // public bool HasContext_Stable = false;
        // public bool HasContext_QuestGiver = false;
        // public bool HasContext_PaperDoll = false;
        // public bool HasContext_Inventory = false;

        // Add a new context menu entry.
        public void AddItem(int nResponseCode, int nStringID, int nFlags, int nHue)
        {

            m_Entries.Add(new ContextMenuItem(nResponseCode, nStringID, nFlags, nHue));
        }

        public ContextMenuItem Context(string iCaption)
        {
            foreach (ContextMenuItem i in m_Entries)
            {
                if (i.Caption == iCaption)
                {
                    return i;
                }
            }
            return null;
        }

        // Sets up which options are available to the new client.
        public void FinalizeMenu()
        {
            foreach (ContextMenuItem i in m_Entries)
            {
                if ((i.Caption.Length >= 5) &&(i.Caption.Substring(0, 5) == "Train"))
                {
                    // train option
                }
                else
                {
                    switch (i.Caption)
                    {
                        case "Buy":
                            CanBuy = true;
                            HasContext_Merchant = true;
                            break;
                        case "Sell":
                            CanSell = true;
                            HasContext_Merchant = true;
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
