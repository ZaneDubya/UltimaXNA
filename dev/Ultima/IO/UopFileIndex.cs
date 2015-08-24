/***************************************************************************
 *   UopFileIndex.cs
 *   Based on code from OpenUO: https://github.com/jeffboulanger/OpenUO
 *      Copyright (c) 2011 OpenUO Software Team.
 *      All Rights Reserved.
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace UltimaXNA.Ultima.IO.UOP
{
    //Credit goes to Wyatt of RUOSI http://ruosi.org/load/ultima_sdk_with_uop_support/3-1-0-8
    //Thanks Wyatt, i definately didnt have time to figure this all out, was a great help, 
    //thanks for updating the ultima sdk for others like myself to reference.
    public class UopFileIndex : AFileIndex
    {
        public const int UOP_MAGIC_NUMBER = 0x50594D;
        private readonly string m_Extension;
        private readonly bool m_HasExtra;

        public UopFileIndex(string uopPath, int length, bool hasExtra, string extension)
            : base(uopPath, length)
        {
            m_Extension = extension;
            m_HasExtra = hasExtra;
        }

        protected override FileIndexEntry3D[] ReadEntries()
        {
            int length = Length;
            string dataPath = DataPath;
            FileIndexEntry3D[] entries = new FileIndexEntry3D[length];

            // In the mul file index, we read everything sequentially, and -1 is applied to invalid lookups.
            // UOP does not do this, so we need to do it ourselves.
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i].lookup = -1;
            }

            using (FileStream index = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                FileInfo fi = new FileInfo(dataPath);
                string uopPattern = Path.GetFileNameWithoutExtension(fi.Name).ToLowerInvariant();

                using (BinaryReader br = new BinaryReader(index))
                {
                    br.BaseStream.Seek(0, SeekOrigin.Begin);

                    if (br.ReadInt32() != UOP_MAGIC_NUMBER)
                    {
                        throw new ArgumentException("Bad UOP file.");
                    }

                    br.ReadInt64(); // version + signature
                    long nextBlock = br.ReadInt64();
                    br.ReadInt32(); // block capacity
                    int count = br.ReadInt32();

                    Dictionary<ulong, int> hashes = new Dictionary<ulong, int>();

                    for (int i = 0; i < length; i++)
                    {
                        string entryName = string.Format("build/{0}/{1:D8}{2}", uopPattern, i, m_Extension);
                        ulong hash = CreateHash(entryName);

                        if (!hashes.ContainsKey(hash))
                        {
                            hashes.Add(hash, i);
                        }
                    }

                    br.BaseStream.Seek(nextBlock, SeekOrigin.Begin);

                    do
                    {
                        int filesCount = br.ReadInt32();
                        nextBlock = br.ReadInt64();

                        for (int i = 0; i < filesCount; i++)
                        {
                            long offset = br.ReadInt64();
                            int headerLength = br.ReadInt32();
                            int compressedLength = br.ReadInt32();
                            int decompressedLength = br.ReadInt32();
                            ulong hash = br.ReadUInt64();
                            br.ReadUInt32();
                            short flag = br.ReadInt16();

                            int entryLength = flag == 1 ? compressedLength : decompressedLength;

                            if (offset == 0)
                            {
                                continue;
                            }

                            int idx;
                            if (hashes.TryGetValue(hash, out idx))
                            {
                                if (idx < 0 || idx > entries.Length)
                                {
                                    throw new IndexOutOfRangeException("hashes dictionary and files collection have different count of entries!");
                                }

                                entries[idx].lookup = (int)(offset + headerLength);
                                entries[idx].length = entryLength;

                                if (m_HasExtra)
                                {
                                    long curPos = br.BaseStream.Position;

                                    br.BaseStream.Seek(offset + headerLength, SeekOrigin.Begin);

                                    byte[] extra = br.ReadBytes(8);

                                    ushort extra1 = (ushort)((extra[3] << 24) | (extra[2] << 16) | (extra[1] << 8) | extra[0]);
                                    ushort extra2 = (ushort)((extra[7] << 24) | (extra[6] << 16) | (extra[5] << 8) | extra[4]);

                                    entries[idx].lookup += 8;
                                    entries[idx].extra = extra1 << 16 | extra2;

                                    br.BaseStream.Seek(curPos, SeekOrigin.Begin);
                                }
                            }
                        }
                    } while (br.BaseStream.Seek(nextBlock, SeekOrigin.Begin) != 0);
                }
            }

            return entries;
        }

        public static ulong CreateHash(string s)
        {
            uint eax, ecx, edx, ebx, esi, edi;

            eax = ecx = edx = ebx = esi = edi = 0;
            ebx = edi = esi = (uint)s.Length + 0xDEADBEEF;

            int i = 0;

            for (i = 0; i + 12 < s.Length; i += 12)
            {
                edi = (uint)((s[i + 7] << 24) | (s[i + 6] << 16) | (s[i + 5] << 8) | s[i + 4]) + edi;
                esi = (uint)((s[i + 11] << 24) | (s[i + 10] << 16) | (s[i + 9] << 8) | s[i + 8]) + esi;
                edx = (uint)((s[i + 3] << 24) | (s[i + 2] << 16) | (s[i + 1] << 8) | s[i]) - esi;

                edx = (edx + ebx) ^ (esi >> 28) ^ (esi << 4);
                esi += edi;
                edi = (edi - edx) ^ (edx >> 26) ^ (edx << 6);
                edx += esi;
                esi = (esi - edi) ^ (edi >> 24) ^ (edi << 8);
                edi += edx;
                ebx = (edx - esi) ^ (esi >> 16) ^ (esi << 16);
                esi += edi;
                edi = (edi - ebx) ^ (ebx >> 13) ^ (ebx << 19);
                ebx += esi;
                esi = (esi - edi) ^ (edi >> 28) ^ (edi << 4);
                edi += ebx;
            }

            if (s.Length - i > 0)
            {
                switch (s.Length - i)
                {
                    case 12:
                        esi += (uint)s[i + 11] << 24;
                        goto case 11;
                    case 11:
                        esi += (uint)s[i + 10] << 16;
                        goto case 10;
                    case 10:
                        esi += (uint)s[i + 9] << 8;
                        goto case 9;
                    case 9:
                        esi += s[i + 8];
                        goto case 8;
                    case 8:
                        edi += (uint)s[i + 7] << 24;
                        goto case 7;
                    case 7:
                        edi += (uint)s[i + 6] << 16;
                        goto case 6;
                    case 6:
                        edi += (uint)s[i + 5] << 8;
                        goto case 5;
                    case 5:
                        edi += s[i + 4];
                        goto case 4;
                    case 4:
                        ebx += (uint)s[i + 3] << 24;
                        goto case 3;
                    case 3:
                        ebx += (uint)s[i + 2] << 16;
                        goto case 2;
                    case 2:
                        ebx += (uint)s[i + 1] << 8;
                        goto case 1;
                    case 1:
                        ebx += s[i];
                        break;
                }

                esi = (esi ^ edi) - ((edi >> 18) ^ (edi << 14));
                ecx = (esi ^ ebx) - ((esi >> 21) ^ (esi << 11));
                edi = (edi ^ ecx) - ((ecx >> 7) ^ (ecx << 25));
                esi = (esi ^ edi) - ((edi >> 16) ^ (edi << 16));
                edx = (esi ^ ecx) - ((esi >> 28) ^ (esi << 4));
                edi = (edi ^ edx) - ((edx >> 18) ^ (edx << 14));
                eax = (esi ^ edi) - ((edi >> 8) ^ (edi << 24));

                return ((ulong)edi << 32) | eax;
            }

            return ((ulong)esi << 32) | eax;
        }

    }
}
