using UltimaXNA.Ultima.IO.FontsNew;

namespace UltimaXNA.Core.UI.HTML.Atoms
{
    public class CharacterAtom : AAtom
    {
        private IUIResourceProvider m_Provider;

        public override int Width
        {
            get
            {
                if (Character < 32)
                    return 0;
                else
                    return m_Provider.GetUnicodeFont((int)Font).GetCharacter(Character).Width + (Style_IsBold ? 1 : 0) + 1;
            }
        }

        public override int Height
        {
            get
            {
                return m_Provider.GetUnicodeFont((int)Font).Height;
            }
        }

        public char Character = '\0';

        public CharacterAtom(IUIResourceProvider provider, char c)
        {
            m_Provider = provider;
            Character = c;
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }
}
