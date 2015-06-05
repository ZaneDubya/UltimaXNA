
namespace UltimaXNA.Core.UI
{
    public class ControlMetaData
    {
        public UILayer Layer
        {
            get;
            set;
        }

        /// <summary>
        /// Controls that are Modal appear on top of all other controls and block input to all other controls and the world.
        /// </summary>
        public bool IsModal
        {
            get;
            set;
        }

        public AControl Control
        {
            get;
            private set;
        }

        public ControlMetaData(AControl control)
        {
            Control = control;
        }
    }
}
