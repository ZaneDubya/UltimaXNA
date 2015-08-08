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

        public static readonly AnimationFrame Empty = new AnimationFrame();
        public static readonly AnimationFrame[] EmptyFrames = new AnimationFrame[1] { Empty };

        private AnimationFrame()
        {

        }

        public unsafe AnimationFrame(GraphicsDevice graphics, ushort[] palette, BinaryFileReader reader)
        {
            int xCenter = reader.ReadShort();
            int yCenter = reader.ReadShort();

            int width = reader.ReadUShort();
            int height = reader.ReadUShort();

            // Fix for animations with no IO.
            if ((width == 0) || (height == 0))
            {
                Texture = null;
                return;
            }

            ushort[] data = new ushort[width * height];

            int header;

            int xBase = xCenter - 0x200;
            int yBase = (yCenter + height) - 0x200;

            fixed (ushort* pData = data)
            {
                ushort* dataRef = pData;
                int delta = width;

                int dataRead = 0;

                dataRef += xBase;
                dataRef += (yBase * delta);

                while ((header = reader.ReadInt()) != 0x7FFF7FFF)
                {
                    header ^= DoubleXor;

                    ushort* cur = dataRef + ((((header >> 12) & 0x3FF) * delta) + ((header >> 22) & 0x3FF));
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
    }
}
