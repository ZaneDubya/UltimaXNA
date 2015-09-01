using System;
using System.Collections.Generic;

namespace UltimaXNA.Ultima.Player
{
    public class JournalData
    {
        private List<string> m_JournalEntries = new List<string>();
        public List<string> JournalEntries
        {
            get { return m_JournalEntries; }
        }

        public event Action<string> OnJournalEntryAdded;

        public void AddEntry(string text)
        {
            while (m_JournalEntries.Count > 99)
                m_JournalEntries.RemoveAt(0);
            m_JournalEntries.Add(text);
            if (OnJournalEntryAdded != null)
                OnJournalEntryAdded(text);
        }
    }
}
