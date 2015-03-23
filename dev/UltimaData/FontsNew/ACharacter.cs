using Microsoft.Xna.Framework;

namespace UltimaXNA.UltimaData.FontsNew
{
    abstract internal class ACharacter
    {
        public int Width, Height;
        public int XOffset = 0, YOffset = 0;

        protected uint[] m_PixelData;

        public unsafe void WriteToBuffer(uint* dstPtr, int dx, int dy, int linewidth, int maxHeight, int baseLine,
            bool isBold, bool isItalic, bool isUnderlined, bool isOutlined, uint color, uint outline)
        {
            int width = Width + (isOutlined ? 2 : 0);
            int height = Height + (isOutlined ? 2 : 0);
            int yoffset = YOffset + (isOutlined ? 1 : 0);

            fixed (uint* srcPtr = m_PixelData)
            {
                for (int y = 0; (y < height) && (y + dy < maxHeight); y++)
                {
                    uint* src = ((uint*)srcPtr) + (width * y);
                    uint* dest = (((uint*)dstPtr) + (linewidth * (y + dy + yoffset)) + dx);
                    if (isItalic)
                    {
                        dest += (baseLine - yoffset - y - 1) / 2;
                    }

                    for (int x = 0; x < width; x++)
                    {
                        if (*src != 0x00000000)
                        {
                            *dest = color;
                            if (isBold)
                                *(dest + 1) = color;
                        }
                        dest++;
                        src++;
                    }
                }
            }

            if (isUnderlined)
            {
                int underlineAtY = dy + baseLine + 2;
                if (underlineAtY >= maxHeight)
                    return;
                uint* dest = (((uint*)dstPtr) + (linewidth * (underlineAtY)) + dx);
                int w = isBold ? width + 2 : width + 1;
                for (int k = 0; k < w; k++)
                {
                    *dest++ = color;
                }
            }
        }

        // Rectangle TextureBounds'

        /*internal void WriteTextureData(uint[] textureData)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    textureData[(y + TextureBounds.Y) * Width + (x + TextureBounds.X)] = m_PixelData[y * Width + x];
                }
            }
            m_PixelData = null;
        }*/
    }
}
