
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
