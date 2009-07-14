using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
{
    public class MobileFlags
    {
        readonly byte _flags;
        /// <summary>
        /// These are the only flags sent by RunUO
        /// 0x02 = female
        /// 0x04 = poisoned
        /// 0x08 = blessed/yellow health bar
        /// 0x40 = warmode
        /// 0x80 = hidden
        /// </summary>
        public bool IsWarMode { get { return ((_flags & 0x40) == 0x40); } }

        public MobileFlags(byte flags)
        {
            _flags = flags;
        }
    }
}
