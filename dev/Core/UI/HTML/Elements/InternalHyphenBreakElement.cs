using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML.Elements
{
    class InternalHyphenBreakElement : CharacterElement
    {
        public override bool CanBreakAtThisAtom => true;
        public override bool IsThisAtomABreakingSpace => false;
        public override bool IsThisAtomALineBreak => true;
        public override bool IsThisAtomInternalOnly => true;

        public InternalHyphenBreakElement(StyleState style) : base(style, '-')
        {

        }

        public override string ToString() => "---";
    }
}
