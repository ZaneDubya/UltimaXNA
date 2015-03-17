using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Entity.Support
{
    public class MobileFlags
    {
        /// <summary>
        /// These are the only flags sent by RunUO
        /// 0x02 = female
        /// 0x04 = poisoned
        /// 0x08 = blessed/yellow health bar
        /// 0x40 = warmode
        /// 0x80 = hidden
        /// </summary>
        private byte m_flags;

        public bool IsFemale { get { return ((m_flags & 0x02) != 0); } }
        public bool IsPoisoned { get { return ((m_flags & 0x04) != 0); } }
        public bool IsBlessed { get { return ((m_flags & 0x08) != 0); } }
        public bool IsWarMode
        {
            get { return ((m_flags & 0x40) != 0); }
            set
            {
                if (value == true)
                {
                    m_flags |= 0x40;
                }
                else
                {
                    m_flags &= unchecked((byte)(~0x40));
                }
            }
        }
        public bool IsHidden { get { return ((m_flags & 0x80) != 0); } }

        public MobileFlags(byte flags)
        {
            m_flags = flags;
        }
    }
}
