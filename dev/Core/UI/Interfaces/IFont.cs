
namespace UltimaXNA.Core.UI
{
    public interface IFont
    {
        int Baseline { get; }
        int Height { get; }
        ICharacter GetCharacter(char ch);
        bool HasBuiltInOutline { get; }
    }
}
