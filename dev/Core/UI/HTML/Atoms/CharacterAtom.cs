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
                {
                    ICharacter ch = Style.Font.GetCharacter(Character);
                    return ch.Width + ch.ExtraWidth + (Style.IsBold ? 1 : 0);
                }
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
