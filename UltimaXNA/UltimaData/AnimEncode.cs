using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BKSystem.IO;

namespace UltimaXNA.Data
{
    class AnimEncode
    {
        public static void SaveData(int body, string path)
        {
            for (int i = 0; i <= 21; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    byte[] data = AnimationsXNA.GetData(body, i, j, 0, false);
                    if (data != null)
                    {
                        FileStream stream = new FileStream(string.Format(path + "\\{0}-{1}-{2}", body, i, j), FileMode.Create);
                        BinaryWriter w = new BinaryWriter(stream);
                        w.Write(data);
                        w.Close();
                    }
                }
            }
        }

        public static void TransformData(int body, string path)
        {
            for (int i = 0; i <= 21; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (File.Exists(string.Format(path + "\\{0}-{1}-{2}", body, i, j)))
                    {
                        FileStream stream = new FileStream(string.Format(path + "\\{0}-{1}-{2}", body, i, j), FileMode.Open);
                        BinaryReader bin = new BinaryReader(stream);
                        transform(bin);
                        stream.Close();
                    }
                }
            }
        }

        private static void transform(BinaryReader bin)
        {
            FrameXNA[] frames = AnimationsXNA.GetAnimation(bin);

            BitStream CellMatrixStream = new BitStream();
            BitStream CellDeltaEncodingStream = new BitStream();
            BitStream CellDataStream = new BitStream();

            int width, height, xoffset, yoffset;
            getWidthHeight(frames, out width, out height, out xoffset, out yoffset);

            uint[][] pixeldata = new uint[frames.Length][];
            for (int i = 0; i < frames.Length; i++)
                pixeldata[i] = getPixelData(frames[i], width, height,
                    xoffset - frames[i].Center.X,
                    (height + (yoffset - frames[i].Center.Y)) - frames[i].Texture.Height);

            uint[] palette = new uint[0x100];
            uint[] palettesortvalues = new uint[0x100];
            uint[] paletteSimilars = new uint[0x100];
            for (int i = 0; i < frames.Length; i++)
                getPalette(pixeldata[i], palette, palettesortvalues);
            palettesortvalues[0] = int.MaxValue;
            sortPalette(palette, palettesortvalues);
            // findSimilarShadesInPalette(palette, ref paletteSimilars);
            // combinePalette(palette, paletteSimilars, palettesortvalues);
            // sortPalette(palette, palettesortvalues, paletteSimilars);

            // debug = count num of pixels using top 16 colors.
            int topsixteen = 0; int others = 0;
            for (int i = 1; i < 17; i++)
                topsixteen += (int)palettesortvalues[i];
            for (int i = 17; i < 256; i++)
                others += (int)palettesortvalues[i];

            byte[][] framedata = new byte[frames.Length][];
            for (int i = 0; i < frames.Length; i++)
                framedata[i] = palettedFrame(pixeldata[i], palette, paletteSimilars);

            for (int i = 0; i < frames.Length; i++)
            {
                byte[][] framecells = getCells(framedata[i], width);
                byte[][] lastFrameCells;
                if (i > 0)
                    lastFrameCells = getCells(buildFrame(width, height, CellMatrixStream, CellDeltaEncodingStream, CellDataStream), width);
                else
                    lastFrameCells = new byte[0][];
                getStreams(framecells, lastFrameCells, i, width, palette, CellMatrixStream, CellDeltaEncodingStream, CellDataStream);
                byte[] newframedata = buildFrame(width, height, CellMatrixStream, CellDeltaEncodingStream, CellDataStream);
                FrameXNA frame = new FrameXNA(AnimationsXNA.DEBUG_GFX, palette, newframedata, width, height, 0, 0);
                frame.Texture.Save("test.bmp", Microsoft.Xna.Framework.Graphics.ImageFileFormat.Bmp);
            }
            int size =
                (int)(CellMatrixStream.Length >> 3) +
                (int)(CellDeltaEncodingStream.Length >> 3) +
                (int)(CellDataStream.Length >> 3);
        }

        static int emptycellswritten = 0;
        static int cellswritten = 0;
        static int bitswrittenCellMatrix = 0;
        static int bitswrittenCellDelta = 0;
        static int bitswrittenCellData = 0;

        static int maxBitsDifferentForPalette = 2;
        static int maxBitsDifferentForCell = 4;

        private static void getStreams(byte[][] celldata, byte[][] lastCellData, int frame, int width, uint[] palette, BitStream cellMatrix, BitStream cellDeltaEncoding, BitStream pixelData)
        {
            bool newLine = true;
            int x = 0;
            bool emptyCellSpan = true;
            byte cellSpanLength = 0;
            int cellMatrixEntryMaxSizeBits = 4;
            int cellMatrixEntryMaxSize = (int)Math.Pow(2, cellMatrixEntryMaxSizeBits) - 1;
            byte[] emptyCell = new byte[16];

            for (int i = 0; i < celldata.Length; i++)
            {
                if (newLine)
                {
                    newLine = false;
                    x = 0;
                }

                bool writeCell;
                if (frame == 0)
                    writeCell = !isCellEmpty(celldata[i]);
                else
                    writeCell = isCellDifferent(celldata[i], lastCellData[i], palette);

                if (writeCell)
                {
                    if (!emptyCellSpan)
                    {
                        cellSpanLength++;
                        if (cellSpanLength == cellMatrixEntryMaxSize)
                        {
                            cellMatrix.Write(cellSpanLength, 0, cellMatrixEntryMaxSizeBits);
                            cellSpanLength = 0;
                            cellMatrix.Write(cellSpanLength, 0, cellMatrixEntryMaxSizeBits);
                            bitswrittenCellMatrix += 8;
                        }
                    }
                    else
                    {
                        cellMatrix.Write(cellSpanLength, 0, cellMatrixEntryMaxSizeBits);
                        bitswrittenCellMatrix += 4;
                        cellSpanLength = 1;
                        emptyCellSpan = false;
                    }
                    byte[] lastframecell = (frame == 0) ? emptyCell : lastCellData[i];
                    writeCellData(celldata[i], lastframecell, palette, cellDeltaEncoding, pixelData);
                    cellswritten++;
                }
                else
                {
                    if (emptyCellSpan)
                    {
                        cellSpanLength++;
                        if (cellSpanLength == cellMatrixEntryMaxSize)
                        {
                            cellMatrix.Write(cellSpanLength, 0, cellMatrixEntryMaxSizeBits);
                            cellSpanLength = 0;
                            cellMatrix.Write(cellSpanLength, 0, cellMatrixEntryMaxSizeBits);
                            bitswrittenCellMatrix += 8;
                        }
                    }
                    else
                    {
                        cellMatrix.Write(cellSpanLength, 0, cellMatrixEntryMaxSizeBits);
                        bitswrittenCellMatrix += 4;
                        cellSpanLength = 1;
                        emptyCellSpan = true;
                    }
                }

                x += 4;
                if (x >= width)
                    newLine = true;
            }

            if (cellSpanLength > 0)
            {
                cellMatrix.Write(cellSpanLength, 0, cellMatrixEntryMaxSizeBits);
            }
        }

        private static readonly int[] deltaBits = new int[16] { 
            0x0001, 0x0002, 0x0004, 0x0008, 0x0010, 0x0020, 0x0040, 0x0080,
            0x0100, 0x0200, 0x0400, 0x0800, 0x1000, 0x2000, 0x4000, 0x8000 };
        private static readonly int[] andBits = new int[16] {
            0x0001, 0x0003, 0x0007, 0x000F, 0x001F, 0x003F, 0x007F, 0x00FF,
            0x01FF, 0x03FF, 0x07FF, 0x0FFF, 0x1FFF, 0x3FFF, 0x7FFF, 0xFFFF };

        private static byte[] buildFrame(int width, int height, BitStream cellMatrix, BitStream cellDeltaEncoding, BitStream pixelData)
        {
            int cellWidth = width / 4 + ((width % 4 != 0) ? 1 : 0);
            int cellHeight = height / 4 + ((height % 4 != 0) ? 1 : 0);
            byte[] frame = new byte[width * height];

            uint[] matrix = cellMatrix.GetBuffer();
            uint[] delta = cellDeltaEncoding.GetBuffer();
            uint[] pixels = pixelData.GetBuffer();

            byte[] matrix2 = byteArrayFromUintArray(matrix);
            byte[] delta2 = byteArrayFromUintArray(delta);
            byte[] pixels2 = byteArrayFromUintArray(pixels);
            byte[] totaldata = new byte[matrix2.Length + delta2.Length + pixels2.Length];
            Array.Copy(matrix2, 0, totaldata, 0, matrix2.Length);
            Array.Copy(delta2, 0, totaldata, matrix2.Length, delta2.Length);
            Array.Copy(pixels2, 0, totaldata, matrix2.Length + delta2.Length, pixels2.Length);

            byte[] compressed = new byte[0x1000000];
            int compLength = 0x1000000;
            UltimaXNA.Network.Compression.Pack(compressed, ref compLength, totaldata, totaldata.Length);

            int matrixIndex = 0, matrixOffset = 0;
            int deltaIndex = 0, deltaOffset = 0;
            int pixelsIndex = 0, pixelsOffset = 0;
            
            int cellIndex = 0;
            bool inEmptyCellSpan = true;
            bool frameComplete = false;
            while (!frameComplete)
            {
                int matrixCode = valueFromBitStream(matrix, ref matrixIndex, ref matrixOffset, 4);
                if (inEmptyCellSpan)
                {
                    cellIndex += matrixCode;
                    inEmptyCellSpan = false;
                }
                else
                {
                    for (int i = 0; i < matrixCode; i++)
                    {
                        int cellX = (cellIndex % cellWidth) * 4;
                        int cellY = (cellIndex / cellWidth) * 4;
                        int cellW = ((width - cellX) >= 4) ? 4 : width - cellX;
                        int cellH = ((height - cellY) >= 4) ? 4 : height - cellY;
                        int deltaCode = valueFromBitStream(delta, ref deltaIndex, ref deltaOffset, cellW * cellH);
                        for (int j = 0; j < cellW * cellH; j++)
                        {
                            if ((deltaCode & deltaBits[j]) != 0)
                            {
                                frame[cellX + (j % cellW) + ((cellY + (j / cellW)) * width)] = (byte)valueFromBitStream(pixels, ref pixelsIndex, ref pixelsOffset, 8);
                            }
                        }
                        cellIndex++;
                    }
                    inEmptyCellSpan = true;
                }
                if (cellIndex == cellWidth * cellHeight)
                    frameComplete = true;
            }

            return frame;
        }

        private static byte[] byteArrayFromUintArray(uint[] data)
        {
            byte[] outdata = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
            {
                int j = 4 * i;
                outdata[j + 0] = (byte)(data[i] >> 24 & 0xFF);
                outdata[j + 1] = (byte)(data[i] >> 16 & 0xFF);
                outdata[j + 2] = (byte)(data[i] >> 8 & 0xFF);
                outdata[j + 3] = (byte)(data[i] & 0xFF);
            }
            return outdata;
        }

        private static int valueFromBitStream(uint[] data, ref int index, ref int offset, int count)
        {
            int value = 0;
            if ((offset + count) > 32)
            {
                int firstcount = (32 - offset);
                value = (int)(data[index] >> ((32 - firstcount) - offset)) & andBits[firstcount - 1];
                count -= firstcount;
                value = value << count;
                offset = 0;
                index += 1;
                value += (int)(data[index] >> ((32 - count) - offset)) & andBits[count - 1];
                offset += count;
                if (offset >= 32)
                {
                    index += 1;
                    offset -= 32;
                }
            }
            else
            {
                value = (int)(data[index] >> ((32 - count) - offset)) & andBits[count - 1];
                offset += count;
                if (offset >= 32)
                {
                    index += 1;
                    offset -= 32;
                }
            }
            return value;
        }

        private static void writeCellData(byte[] cell, byte[] lastframecell, uint[] palette, BitStream cellDeltaEncoding, BitStream pixelData)
        {
            int count = 0;
            int numpixels = cell.Length;
            
            int delta = 0;
            byte[] pixels = new byte[16];
            for (int i = 0; i < numpixels; i++)
            {
                if (isPixelDifferent(cell[i], lastframecell[i], palette, maxBitsDifferentForPalette))
                {
                    delta |= (1 << i);
                    pixels[count] = cell[i];
                    count++;
                }
            }
            cellDeltaEncoding.Write(delta, 0, numpixels);
            bitswrittenCellDelta += numpixels;
            pixelData.Write(pixels, 0, count);
            bitswrittenCellData += count * 8;
            if (count == 0)
                emptycellswritten++;
        }

        private static bool isPixelDifferent(byte a, byte b, uint[] palette, int maxBitsDifferent)
        {
            if (a == b)
                return false;
            if (a == 0 && b != 0)
                return true;
            if (a != 0 && b == 0)
                return true;
            uint diff = palette[a] ^ palette[b];
            byte[] d = BitConverter.GetBytes(diff);
            d[0] = (byte)(d[0] >> 3);
            d[1] = (byte)(d[1] >> 3);
            d[2] = (byte)(d[2] >> 3);
            int delta = SumArray(d);
            if (delta <= maxBitsDifferent)
                return false;
            return true;
        }

        private static int SumArray(byte[] array)
        {
            int value = 0;
            for (int i = 0; i < array.Length; i++)
                value += array[i];
            return value;
        }

        private static int SumArray(uint[] array)
        {
            int value = 0;
            for (int i = 0; i < array.Length; i++)
                value += (int)array[i];
            return value;
        }

        private static bool isCellDifferent(byte[] data, byte[] lastData, uint[] palette)
        {
            for (int i = 0; i < data.Length; i++)
                if (isPixelDifferent(data[i], lastData[i], palette, maxBitsDifferentForCell))
                    return true;
            return false;
        }

        private static bool isCellEmpty(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
                if (data[i] != 0)
                    return false;
            return true;
        }

        private static void getWidthHeight(FrameXNA[] frames, out int width, out int height, out int maxXoffset, out int maxYoffset)
        {
            // get the offsets
            maxXoffset = 0;
            maxYoffset = 0;
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i].Center.X > maxXoffset)
                    maxXoffset = frames[i].Center.X;
                if (frames[i].Center.Y < maxYoffset)
                    maxYoffset = frames[i].Center.Y;
            }
            // get height/width using these offsets
            width = 0;
            height = 0;
            for (int i = 0; i < frames.Length; i++)
            {
                int thiswidth = frames[i].Texture.Width + (maxXoffset - frames[i].Center.X);
                if (thiswidth > width) { width = thiswidth; }
                int thisheight = frames[i].Texture.Height - (maxYoffset - frames[i].Center.Y);
                if (thisheight > height) { height = thisheight; }
            }
        }

        private static uint[] getPixelData(FrameXNA f, int width, int height, int xoffset, int yoffset)
        {
            uint[] outdata = new uint[width * height];
            uint[] pixeldata = new uint[f.Texture.Height * f.Texture.Width];
            f.Texture.GetData<uint>(pixeldata);
            for (int y = 0; y < f.Texture.Height; y++)
            {
                Array.Copy(pixeldata, y * f.Texture.Width, outdata, xoffset + (y + yoffset) * width, f.Texture.Width);
            }
            return outdata;
        }

        private static void getPalette(uint[] data, uint[] palette, uint[] sort)
        {
            for (int i = 0; i < data.Length; i++)
            {
                uint color = data[i];
                if (color == 0)
                    continue;
                else
                {
                    for (int j = 1; j < palette.Length; j++)
                    {
                        if (palette[j] == color)
                        {
                            sort[j]++;
                            break;
                        }
                        else if (palette[j] == 0)
                        {
                            palette[j] = color;
                            sort[j]++;
                            break;
                        }
                    }
                }
            }
        }

        private static void findSimilarShadesInPalette(uint[] pal, ref uint[] similarShades)
        {
            for (int i = 2; i < pal.Length; i++)
            {
                if (pal[i] != 0)
                {
                    for (int j = 1; j < i; j++)
                    {
                        if (!(isPixelDifferent((byte)i, (byte)j, pal, maxBitsDifferentForPalette)))
                        {
                            similarShades[i] = (uint)j;
                            break;
                        }
                    }
                }
            }
        }

        private static void combinePalette(uint[] pal, uint[] similarShades, uint[] sort)
        {
            for (int i = 1; i < pal.Length; i++)
            {
                if (pal[i] == 0)
                    return;
                if (similarShades[i] != 0)
                {
                    uint dest = similarShades[i];
                    uint color = pal[i];
                    uint value = sort[i];

                    similarShades[i] = 0;
                    pal[i] = 0;
                    sort[i] = 0;

                    sort[dest] += value;
                    for (int j = i + 1; j < pal.Length; j++)
                    {
                        pal[j - 1] = pal[j];
                        uint tempShade = similarShades[j];
                        if (tempShade > i)
                            tempShade -= 1;
                        similarShades[j - 1] = tempShade;
                        sort[j - 1] = sort[j];
                    }
                    pal[pal.Length - 1] = color;
                    similarShades[pal.Length - 1] = dest;
                    i--;
                }
            }
        }

        private static void sortPalette(uint[] pal, uint[] sort, uint[] indexSimilars)
        {
            uint[] palSimilars = new uint[0x100];
            for (int i = 0; i < 0x100; i++)
                palSimilars[i] = pal[indexSimilars[i]];
            sortPalette(pal, sort);
            for (int i = 0; i < 0x100; i++)
            {
                if (palSimilars[i] != 0)
                {
                    for (int j = 0; j < 0x100; j++)
                    {
                        if (pal[j] == palSimilars[i])
                        {
                            indexSimilars[i] = (uint)j;
                            break;
                        }
                    }
                }
            }
        }

        private static void sortPalette(uint[] pal, uint[] sort)
        {
            Array.Sort(sort, pal);
            Array.Reverse(sort);
            Array.Reverse(pal);
        }

        private static byte[] palettedFrame(uint[] data, uint[] palette, uint[] paletteSimilars)
        {
            byte[] outdata = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                outdata[i] = getIndexFromPalette(data[i], palette, paletteSimilars);
            }
            return outdata;
        }

        private static byte[][] getCells(byte[] data, int width)
        {
            int height = data.Length / width;
            int cellWidth = width / 4 + ((width % 4 != 0) ? 1 : 0);
            int cellHeight = height / 4 + ((height % 4 != 0) ? 1 : 0);

            byte[][] cells = new byte[cellWidth * cellHeight][];
            int cellIndex = 0;

            for (int cellY = 0; cellY < cellHeight; cellY++)
            {
                for (int cellX = 0; cellX < cellWidth; cellX++)
                {
                    int w = ((width - cellX * 4) >= 4) ? 4 : width - cellX * 4;
                    int h = ((height - cellY * 4) >= 4) ? 4 : height - cellY * 4;
                    byte[] cell = new byte[w * h];
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                        {
                            cell[y * w + x] = data[(cellX * 4 + x + (cellY * 4 + y) * width)];
                        }
                    cells[cellIndex++] = cell;
                }
            }

            return cells;
        }

        private static byte getIndexFromPalette(uint color, uint[] palette, uint[] similarShades)
        {
            if (color == 0)
                return 0;
            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i] == color)
                {
                    if (similarShades[i] != 0)
                        return (byte)(similarShades[i]);
                    else
                        return (byte)i;
                }
            }
            return 255;
        }
    }
}
