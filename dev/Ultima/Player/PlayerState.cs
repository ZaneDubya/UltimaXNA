using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaXNA.Ultima.Player
{
    class PlayerState
    {
        private static readonly PlayerState m_Instance;

        private JournalData m_Journal;
        private SkillData m_Skills;
        private StatLockData m_StatLocks;

        static PlayerState()
        {
            m_Instance = new PlayerState();

            m_Instance.m_Journal = new JournalData();
            m_Instance.m_Skills = new SkillData();
            m_Instance.m_StatLocks = new StatLockData();
        }

        public static JournalData Journaling
        {
            get { return m_Instance.m_Journal; }
        }

        public static SkillData Skills
        {
            get { return m_Instance.m_Skills; }
        }

        public static StatLockData StatLocks
        {
            get { return m_Instance.m_StatLocks; }
        }
    }
}
