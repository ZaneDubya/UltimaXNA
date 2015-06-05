using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Core.UI
{
    class ControlAndMetaData
    {
        public UILayer Layer
        {
            get;
            set;
        }

        public AControl Control
        {
            get;
            private set;
        }

        public ControlAndMetaData(AControl control)
        {
            Control = control;
        }
    }
}
