using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML.Elements
{
    class InternalLineBreakElement : CharacterElement
    {
        public override bool CanBreakAtThisAtom => true;
        public override bool IsThisAtomABreakingSpace => false;
        public override bool IsThisAtomALineBreak => true;
        public override bool IsThisAtomInternalOnly => true;

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

        public InternalLineBreakElement(StyleState style) : base(style, '\n')
        {

        }

        public override string ToString() => "BRK";
    }
}
