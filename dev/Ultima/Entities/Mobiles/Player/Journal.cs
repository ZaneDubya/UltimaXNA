using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UltimaGUI.WorldGumps;

namespace UltimaXNA.UltimaVars
{
    static class Journal
    {
        private static List<string> m_JournalEntries = new List<string>();
        public static List<string> JournalEntries
        {
            get { return m_JournalEntries; }
        }

        public static Action<string> OnJournalEntryAdded = null;

        public static void AddEntry(string text)
        {
            while (m_JournalEntries.Count > 99)
                m_JournalEntries.RemoveAt(0);
            m_JournalEntries.Add(text);
            if (OnJournalEntryAdded != null)
                OnJournalEntryAdded(text);
        }
    }
}
