/***************************************************************************
 *   AnimationFrame.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.IO;
using UltimaXNA.Core.Resources;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public sealed class AnimationFrame : IAnimationFrame
    {
        public Point Center
        {
            get;
            private set;
        }

        public Texture2D Texture
        {
            get;
            private set;
        }

        private const int DoubleXor = (0x200 << 22) | (0x200 << 12);

        public static readonly AnimationFrame NullFrame = new AnimationFrame();
        public static readonly AnimationFrame[] NullFrames = { NullFrame };

        private AnimationFrame()
        {
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            Texture = provider.GetItemTexture(1);
            Center = new Point(0, 0);
        }

        public unsafe AnimationFrame(GraphicsDevice graphics, ushort[] palette, BinaryFileReader reader, SittingTransformation sitting)
        {
            int xCenter = reader.ReadShort();
            int yCenter = reader.ReadShort();

            int width = reader.ReadUShort();
            int height = reader.ReadUShort();

            // Fix for animations with no pixels.
            if ((width == 0) || (height == 0))
            {
                Texture = null;
                return;
            }

            if (sitting == SittingTransformation.StandSouth)
            {
                xCenter += 8;
                width += 8;
                height += 4;
            }

            ushort[] data = new ushort[width * height];
            /*for (int i = 0; i < data.Length; i++)
                data[i] = 0xFFFF;*/

            // somewhere around the waist of a typical mobile animation, we take twelve rows of pixels,
            // discard every third, and shift every remaining row (total of eight) one pixel to the left
            // or right (depending on orientation), for a total skew of eight pixels.

            fixed (ushort* pData = data)
            {
                ushort* dataRef = pData;

                int dataRead = 0;

                int header;
                while ((header = reader.ReadInt()) != 0x7FFF7FFF)
                {
                    header ^= DoubleXor;

                    int x = ((header >> 22) & 0x3FF) + xCenter - 0x200;
                    int y = ((header >> 12) & 0x3FF) + yCenter + height - 0x200;

                    if (sitting == SittingTransformation.StandSouth)
                    {
                        const int skew_start = -17;
                        const int skew_end = skew_start - 16;
                        int iy = y - height - yCenter;
                        if (iy > skew_start)
                        {
                            // pixels below the skew
                            x -= 8;
                            y -= 4;
                        }
                        else if (iy > skew_end)
                        {
                            // pixels within the skew
                            if ((iy - skew_end) % 4 == 0)
                            {
                                reader.Position += (header & 0xFFF);
                                continue;
                            }
                            else
                            {
                                x -= (iy - skew_end) / 2;
                                y -= (iy - skew_end) / 4;
                            }
                        }
                    }

                    ushort* cur = dataRef + y * width + x;
                    ushort* end = cur + (header & 0xFFF);

                    int filecounter = 0;
                    byte[] filedata = reader.ReadBytes(header & 0xFFF);

                    while (cur < end)
                        *cur++ = palette[filedata[filecounter++]];

                    dataRead += header & 0xFFF;
                }

                Metrics.ReportDataRead(dataRead);
            }

            Center = new Point(xCenter, yCenter);

            Texture = new Texture2D(graphics, width, height, false, SurfaceFormat.Bgra5551);
            Texture.SetData<ushort>(data);
        }

        public enum SittingTransformation
        {
            None,
            StandSouth,
            MountNorth
        }
    }
}
