using UltimaXNA.Ultima.IO.Fonts;

namespace UltimaXNA.Core.UI.HTML.Atoms
{
    public class CharacterAtom : AAtom
    {
        public override int Width
        {
            get
            {
                if (Character < 32)
                    return 0;
                else
                    return Style.Font.GetCharacter(Character).Width + (Style.IsBold ? 1 : 0) + 1;
            }
        }

        public override int Height
        {
            get
            {
                return Style.Font.Height;
            }
        }

        public char Character = '\0';

        public CharacterAtom(StyleState style, char c)
            : base(style)
        {
            Character = c;
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }
}
