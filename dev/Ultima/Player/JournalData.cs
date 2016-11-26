using System;
using System.Collections.Generic;

namespace UltimaXNA.Ultima.Player
{
    public class JournalData
    {
        private List<JournalEntry> m_JournalEntries = new List<JournalEntry>();
        public List<JournalEntry> JournalEntries
        {
            get { return m_JournalEntries; }
        }

        public event Action<JournalEntry> OnJournalEntryAdded;

        public void AddEntry(string text, int font, ushort hue, string speakerName, bool asUnicode)
        {
            while (m_JournalEntries.Count > 99)
                m_JournalEntries.RemoveAt(0);
            m_JournalEntries.Add(new JournalEntry(text, font, hue, speakerName, asUnicode));
            OnJournalEntryAdded?.Invoke(m_JournalEntries[m_JournalEntries.Count - 1]);
        }
    }

    public class JournalEntry
    {
        public readonly string Text;
        public readonly int Font;
        public readonly ushort Hue;
        public readonly string SpeakerName;
        public readonly bool AsUnicode;

        public JournalEntry(string text, int font, ushort hue, string speakerName, bool asUnicode)
        {
            Text = text;
            Font = font;
            Hue = hue;
            SpeakerName = speakerName;
            AsUnicode = asUnicode;
        }
    }
}
