using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UltimaXNA.Configuration
{
    public struct SavedGumpConfig
    {
        public string GumpType;
        public Dictionary<string, object> GumpData;

        /// <summary>
        /// A description of a gump that has been saved.
        /// </summary>
        /// <param name="gumpType">The gump's type (no namespace)</param>
        /// <param name="gumpData"></param>
        public SavedGumpConfig(Type gumpType, Dictionary<string, object> gumpData)
        {
            GumpType = gumpType.ToString();
            GumpData = gumpData;
        }
    }
}
