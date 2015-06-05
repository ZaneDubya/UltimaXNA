using UltimaXNA.Ultima.IO.FontsNew;

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
                return TextUni.GetFont((int)Font).GetCharacter(Character).Width + (Style_IsBold ? 1 : 0) + 1;
            }
        }

        public override int Height
        {
            get
            {
                return TextUni.GetFont((int)Font).Height;
            }
        }

        public char Character = ' ';

        public CharacterAtom(char c)
        {
            Character = c;
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }
}
