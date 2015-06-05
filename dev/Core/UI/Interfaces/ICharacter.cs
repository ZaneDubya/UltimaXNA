
namespace UltimaXNA.Core.UI
{
    public interface ICharacter
    {
        int Height { get; set; }
        int Width { get; set; }

        int YOffset { get; }
        int XOffset { get; }

        unsafe void WriteToBuffer(uint* dstPtr, int dx, int dy, int linewidth, int maxHeight, int baseLine,
            bool isBold, bool isItalic, bool isUnderlined, bool isOutlined, uint color, uint outline);
    }
}
