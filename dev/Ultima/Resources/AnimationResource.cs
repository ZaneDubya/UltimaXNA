/***************************************************************************
 *   AnimationResource.cs
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
using System.IO;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.IO;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    internal sealed class AnimationResource
    {
        private FileIndex m_FileIndex = new FileIndex("Anim.idx", "Anim.mul", 0x40000, 6);
        public FileIndex FileIndex { get { return m_FileIndex; } }

        private FileIndex m_FileIndex2 = new FileIndex("Anim2.idx", "Anim2.mul", 0x10000, -1);
        public FileIndex FileIndex2 { get { return m_FileIndex2; } }

        private FileIndex m_FileIndex3 = new FileIndex("Anim3.idx", "Anim3.mul", 0x20000, -1);
        public FileIndex FileIndex3 { get { return m_FileIndex3; } }

        private FileIndex m_FileIndex4 = new FileIndex("Anim4.idx", "Anim4.mul", 0x20000, -1);
        public FileIndex FileIndex4 { get { return m_FileIndex4; } }

        private FileIndex m_FileIndex5 = new FileIndex("Anim5.idx", "Anim5.mul", 0x20000, -1);
        public FileIndex FileIndex5 { get { return m_FileIndex5; } }

        private IAnimationFrame[][][][] m_Cache;
        private GraphicsDevice m_Graphics;
        private int[] m_Table;

        public AnimationResource(GraphicsDevice graphics)
        {
            m_Graphics = graphics;
        }

        public int GetAnimationFrameCount(int body, int action, int direction, int hue)
        {
            IAnimationFrame[] frames = GetAnimation(body, action, direction, hue);
            if (frames == null)
                return 0;
            return frames.Length;
        }

        public IAnimationFrame[] GetAnimation(int body, int action, int direction, int hue)
        {
            int animIndex;
            FileIndex fileIndex;
            int length, extra;
            bool patched;

            GetIndexes(ref body, ref hue, action, direction, out animIndex, out fileIndex);

            IAnimationFrame[] f = CheckCache(body, action, direction);
            if (f != null)
                return f;

            BinaryFileReader reader = fileIndex.Seek(animIndex, out length, out extra, out patched);
            if (reader == null)
                return null;

            IAnimationFrame[] frames = LoadAnimation(reader);
            return m_Cache[body][action][direction] = frames;
        }

        private IAnimationFrame[] LoadAnimation(BinaryFileReader reader)
        {
            ushort[] palette = GetPalette(reader); // 0x100 * 2 = 0x0200 bytes
            int read_start = (int)reader.Position; // save file position after palette.

            int frameCount = reader.ReadInt(); // 0x04 bytes

            int[] lookups = new int[frameCount]; // frameCount * 0x04 bytes
            for (int i = 0; i < frameCount; ++i) { lookups[i] = reader.ReadInt(); }

            IAnimationFrame[] frames = new AnimationFrame[frameCount];
            for (int i = 0; i < frameCount; ++i)
            {
                if (lookups[i] < lookups[0])
                {
                    frames[i] = AnimationFrame.Empty; // Fix for broken animations, per issue13
                }
                else
                {
                    reader.Seek(read_start + lookups[i], SeekOrigin.Begin);
                    frames[i] = new AnimationFrame(m_Graphics, palette, reader);
                }
            }
            return frames;
        }

        /*public byte[] GetData(int body, int action, int direction, int hue)
        {
            int animIndex;
            FileIndex fileIndex;
            int length, extra;
            bool patched;

            if (body >= 0x1000)
                return null;

            getIndexes(ref body, ref hue, action, direction, out animIndex, out fileIndex);
            BinaryFileReader reader = fileIndex.Seek(animIndex, out length, out extra, out patched);
            if (reader == null)
                return null;

            return reader.ReadBytes(length);
        }*/

        private ushort[] GetPalette(BinaryFileReader reader)
        {
            ushort[] pal = new ushort[0x100];
            for (int i = 0; i < 0x100; ++i)
            {
                pal[i] = (ushort)(reader.ReadUShort() | 0x8000);
            }
            return pal;
        }

        private IAnimationFrame[] CheckCache(int body, int action, int direction)
        {
            // Make sure the cache is complete.
            // max number of bodies is about 0x1000
            if (m_Cache == null) m_Cache = new IAnimationFrame[0x1000][][][];
            if (m_Cache[body] == null)
                m_Cache[body] = new IAnimationFrame[35][][];
            if (m_Cache[body][action] == null)
                m_Cache[body][action] = new IAnimationFrame[8][];
            if (m_Cache[body][action][direction] == null)
                m_Cache[body][action][direction] = new IAnimationFrame[1];
            if (m_Cache[body][action][direction][0] != null)
                return m_Cache[body][action][direction];
            else
                return null;
        }

        private void GetIndexes(ref int body, ref int hue, int action, int direction, out int index, out FileIndex fileIndex)
        {
            Translate(ref body, ref hue);

            int fileType = BodyConverter.Convert(ref body);
            switch (fileType)
            {
                default:
                case 1:
                    {
                        fileIndex = m_FileIndex;

                        if (body < 200)
                            index = body * 110;
                        else if (body < 400)
                            index = 22000 + ((body - 200) * 65);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                case 2:
                    {
                        fileIndex = m_FileIndex2;

                        if (body < 200)
                            index = body * 110;
                        else
                            index = 22000 + ((body - 200) * 65);

                        break;
                    }
                case 3:
                    {
                        fileIndex = m_FileIndex3;

                        if (body < 300)
                            index = body * 65;
                        else if (body < 400)
                            index = 33000 + ((body - 300) * 110);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                case 4:
                    {
                        fileIndex = m_FileIndex4;

                        if (body < 200)
                            index = body * 110;
                        else if (body < 400)
                            index = 22000 + ((body - 200) * 65);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                // Issue 60 - Missing (or wrong) object animations - http://code.google.com/p/ultimaxna/issues/detail?id=60 - Smjert
                case 5:
                    {
                        fileIndex = m_FileIndex5;
                        if ((body < 200) && (body != 34)) // looks strange, though it works.
                            index = body * 110;
                        else if (body < 400)
                            index = 22000 + ((body - 200) * 65);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                // Issue 60 - End
            }

            index += action * 5;

            if (direction <= 4)
            {
                index += direction;
            }
            else
            {
                index += direction - (direction - 4) * 2;
            }
        }

        public void Translate(ref int body)
        {
            if (m_Table == null)
                LoadTable();

            if (body <= 0 || body >= m_Table.Length)
            {
                body = 0;
                return;
            }

            body = (m_Table[body] & 0x7FFF);
        }

        public void Translate(ref int body, ref int hue)
        {
            if (m_Table == null)
                LoadTable();

            if (body <= 0 || body >= m_Table.Length)
            {
                body = 0;
                return;
            }

            int table = m_Table[body];

            if ((table & (1 << 31)) != 0)
            {
                body = table & 0x7FFF;

                int vhue = (hue & 0x3FFF) - 1;

                if (vhue < 0 || vhue >= HueData.HueCount)
                    hue = (table >> 15) & 0xFFFF;
            }
        }

        private void LoadTable()
        {
            int count = 400 + ((m_FileIndex.Index.Length - 35000) / 175);

            m_Table = new int[count];

            for (int i = 0; i < count; ++i)
            {
                object o = BodyTable.m_Entries[i];

                if (o == null || BodyConverter.Contains(i))
                {
                    m_Table[i] = i;
                }
                else
                {
                    BodyTableEntry bte = (BodyTableEntry)o;

                    m_Table[i] = bte.m_OldID | (1 << 31) | (((bte.m_NewHue ^ 0x8000) & 0xFFFF) << 15);
                }
            }
        }
    }
} 