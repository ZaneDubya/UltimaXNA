using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
{
    public class ContextMenuItemNew
    {
        private int _responseCode;
        private int _caption;

        public int ResponseCode 
        {
            get { return _responseCode; } 
        } 

        public int Caption 
        {
            get { return _caption; } 
        }

        public ContextMenuItemNew(int nResponseCode, int iStringID, int iFlags, int iHue)
        {
            _caption = iStringID;
            _responseCode = nResponseCode;
        }
    }

    public class ContextMenuNew
    {
        private List<ContextMenuItemNew> _entries = new List<ContextMenuItemNew>();

        public bool HasContextMenu
        {
            get
            {
                return (
                    (Merchant) ||
                    (Merchant));
            }
        }

        public bool CanBuy = false;
        public bool CanSell = false;
        public bool Merchant = false;
        // public bool HasContext_Bank = false;
        // public bool HasContext_Trainer = false;
        // public bool HasContext_Stable = false;
        // public bool HasContext_QuestGiver = false;
        // public bool HasContext_PaperDoll = false;
        // public bool HasContext_Inventory = false;

        // Add a new context menu entry.
        public void AddItem(int nResponseCode, int nStringID, int nFlags, int nHue)
        {
            _entries.Add(new ContextMenuItemNew(nResponseCode, nStringID, nFlags, nHue));
        }
    }
}
