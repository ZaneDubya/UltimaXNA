namespace UltimaXNA.UltimaData.FontsNew
{
    internal abstract class ACharacter
    {
        public int Height;
        public int Width;
        public int XOffset = 0, YOffset = 0;

        protected uint[] m_PixelData;

        public unsafe void WriteToBuffer(uint* dstPtr, int dx, int dy, int linewidth, int maxHeight, int baseLine,
            bool isBold, bool isItalic, bool isUnderlined, bool isOutlined, uint color, uint outline)
        {
            if (m_PixelData != null)
            {
                fixed (uint* srcPtr = m_PixelData)
                {
                    for (var y = 0; (y < Height) && (y + dy < maxHeight); y++)
                    {
                        var src = srcPtr + (Width*y);
                        var dest = (dstPtr + (linewidth*(y + dy + YOffset)) + dx);
                        if (isItalic)
                        {
                            dest += (baseLine - YOffset - y - 1)/2;
                        }

                        for (var x = 0; x < Width; x++)
                        {
                            if (*src != 0x00000000)
                            {
                                if (isOutlined)
                                {
                                    for (var iy = -1; iy <= 1; iy++)
                                    {
                                        var idest = dest + (iy*linewidth);
                                        if (*idest == 0x00000000)
                                            *idest = outline;
                                        if (iy == 0)
                                        {
                                            if (isBold)
                                            {
                                                if (*(src - 1) == 0x00000000)
                                                {
                                                    *(idest) = outline;
                                                    *(idest + 1) = color;
                                                }
                                                else
                                                {
                                                    *(idest + 1) = color;
                                                }
                                                *(idest + 2) = color;
                                            }
                                            else
                                            {
                                                *(idest + 1) = color;
                                            }
                                        }
                                        else
                                        {
                                            if (*(idest + 1) == 0x00000000)
                                                *(idest + 1) = outline;
                                        }

                                        if (*(idest + 2) == 0x00000000)
                                            *(idest + 2) = outline;
                                        if (isBold)
                                        {
                                            if (*(idest + 3) == 0x00000000)
                                                *(idest + 3) = outline;
                                        }
                                    }
                                }
                                else
                                {
                                    *dest = color;
                                    if (isBold)
                                        *(dest + 1) = color;
                                }
                            }
                            dest++;
                            src++;
                        }
                    }
                }
            }

            if (isUnderlined)
            {
                var underlineAtY = dy + baseLine + 2;
                if (underlineAtY >= maxHeight)
                    return;
                var dest = (dstPtr + (linewidth*(underlineAtY)) + dx);
                var w = isBold ? Width + 2 : Width + 1;
                for (var k = 0; k < w; k++)
                {
                    if (isOutlined)
                    {
                        for (var iy = -1; iy <= 1; iy++)
                        {
                            var idest = dest + (iy*linewidth);
                            if (*idest == 0x00000000)
                                *idest = outline;
                            if (iy == 0)
                                *(idest + 1) = color;
                            else
                            {
                                if (*(idest + 1) == 0x00000000)
                                    *(idest + 1) = outline;
                            }

                            if (*(idest + 2) == 0x00000000)
                                *(idest + 2) = outline;
                        }
                    }
                    else
                    {
                        *dest = color;
                    }
                    dest++;
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