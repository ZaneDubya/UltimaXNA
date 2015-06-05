using UltimaXNA.Ultima.IO.FontsNew;

namespace UltimaXNA.Core.UI.HTML.Atoms
{
    public class CharacterAtom : AAtom
    {
        public override int Width
        {
            get;
            set;
        }

        public override int Height
        {
            get;
            set;
        }

        public char Character = ' ';

        public CharacterAtom(IUIResourceProvider provider, char c)
        {
            Character = c;
            if (Character < 32)
                Width = 0;
            else
                Width = provider.GetUnicodeFont((int)Font).GetCharacter(Character).Width + (Style_IsBold ? 1 : 0) + 1;
            Height = provider.GetUnicodeFont((int)Font).Height;
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }
}
