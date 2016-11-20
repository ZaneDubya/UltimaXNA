using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML.Elements
{
    class AutoLineBreakElement : AElement
    {
        public override bool CanBreakAtThisAtom => true;
        public override bool IsThisAtomABreakingSpace => true;
        public override bool IsThisAtomALineBreak => true;

        public override int Height
        {
            get
            {
                return Style.Font.Height;
            }

            set
            {
                
            }
        }

        public override int Width
        {
            get
            {
                return 0;
            }

            set
            {
                
            }
        }

        public AutoLineBreakElement(StyleState style) : base(style)
        {

        }

        public override string ToString() => "Break";
    }
}
