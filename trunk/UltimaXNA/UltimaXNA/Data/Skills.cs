﻿/***************************************************************************
 *   Skills.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UltimaXNA.Data
{
    public class Skills
    {
        public static int DefaultLength = 55;

        private static FileIndex m_FileIndex = new FileIndex("Skills.idx", "Skills.mul", DefaultLength, -1);
        public static FileIndex FileIndex { get { return m_FileIndex; } }

        private static Skill[] m_List = new Skill[DefaultLength];
        public static Skill[] List { get { return m_List; } }
        private static string[] _listNames = null;
        public static string[] ListNames
        {
            get
            {
                if (_listNames == null)
                {
                    _listNames = new string[m_List.Length];
                    for (int i = 0; i < m_List.Length; i++)
                    {
                        _listNames[i] = m_List[i].Name;
                    }
                }
                return _listNames;
            }
        }

        public static void Initialize()
        {
            for (int i = 0; i < DefaultLength; i++)
                GetSkill(i);
        }

        public static Skill GetSkill(int index)
        {
            if (m_List[index] != null)
                return m_List[index];

            int length, extra;
            bool patched;
            Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);

            if (stream == null)
                return m_List[index] = new Skill(SkillData.DefaultData);

            return m_List[index] = LoadSkill(index, stream);
        }

        private static unsafe Skill LoadSkill(int index, Stream stream)
        {
            BinaryReader bin = new BinaryReader(stream);

            int nameLength = m_FileIndex.Index[index].length - 2;
            int extra = m_FileIndex.Index[index].extra;

            byte[] set1 = new byte[1];
            byte[] set2 = new byte[nameLength];
            byte[] set3 = new byte[1];

            bin.Read(set1, 0, 1);
            bin.Read(set2, 0, nameLength);
            bin.Read(set3, 0, 1);

            bool useBtn = Skills.ToBool(set1);
            string name = Skills.ToString(set2);

            return new Skill(new SkillData(index, name, useBtn, extra, set3[0]));
        }

        public static string ToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(ToString(data[i]));
            }

            return sb.ToString();
        }

        public static bool ToBool(byte[] data)
        {
            return BitConverter.ToBoolean(data, 0);
        }

        public static string ToString(byte b)
        {
            return ToString((char)b);
        }

        public static string ToString(char c)
        {
            return c.ToString();
        }

        public static string ToHexidecimal(int input, int length)
        {
            return String.Format("0x{0:X{1}}", input, length);
        }
    }

    public class Skill
    {
        private SkillData m_Data = null;
        public SkillData Data { get { return m_Data; } }

        private int m_Index = -1;
        public int Index { get { return m_Index; } }

        private bool m_UseButton = false;
        public bool UseButton { get { return m_UseButton; } set { m_UseButton = value; } }

        private string m_Name = String.Empty;
        public string Name { get { return m_Name; } set { m_Name = value; } }

        private SkillCategory m_Category = null;
        public SkillCategory Category { get { return m_Category; } set { m_Category = value; } }

        private byte m_Unknown = 0x0;
        public byte Unknown { get { return m_Unknown; } }

        public int ID { get { return m_Index + 1; } }

        public Skill(SkillData data)
        {
            m_Data = data;

            m_Index = m_Data.Index;
            m_UseButton = m_Data.UseButton;
            m_Name = m_Data.Name;
            m_Category = m_Data.Category;
            m_Unknown = m_Data.Unknown;
        }

        public void ResetFromData()
        {
            m_Index = m_Data.Index;
            m_UseButton = m_Data.UseButton;
            m_Name = m_Data.Name;
            m_Category = m_Data.Category;
            m_Unknown = m_Data.Unknown;
        }

        public void ResetFromData(SkillData data)
        {
            m_Data = data;
            m_Index = m_Data.Index;
            m_UseButton = m_Data.UseButton;
            m_Name = m_Data.Name;
            m_Category = m_Data.Category;
            m_Unknown = m_Data.Unknown;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1:X4}) {2} {3}", m_Index, m_Index, m_UseButton ? "[x]" : "[ ]", m_Name);
        }
    }

    public sealed class SkillData
    {
        public static SkillData DefaultData { get { return new SkillData(-1, "null", false, 0, 0x0); } }

        private int m_Index = -1;
        public int Index { get { return m_Index; } }

        private string m_Name = String.Empty;
        public string Name { get { return m_Name; } }

        private int m_Extra = 0;
        public int Extra { get { return m_Extra; } }

        private bool m_UseButton = false;
        public bool UseButton { get { return m_UseButton; } }

        private byte m_Unknown = 0x0;
        public byte Unknown { get { return m_Unknown; } }

        private SkillCategory m_Category = null;
        public SkillCategory Category { get { return m_Category; } }

        public int NameLength { get { return m_Name.Length; } }

        public SkillData(int index, string name, bool useButton, int extra, byte unk)
        {
            m_Index = index;
            m_Name = name;
            m_UseButton = useButton;
            m_Extra = extra;
            m_Unknown = unk;
        }
    }

    public class SkillCategories
    {
        private static SkillCategory[] m_List = new SkillCategory[0];
        public static SkillCategory[] List { get { return m_List; } }

        private SkillCategories()
        { }

        public static SkillCategory GetCategory(int index)
        {
            if (m_List.Length > 0)
            {
                if (index < m_List.Length)
                    return m_List[index];
            }

            m_List = LoadCategories();

            if (m_List.Length > 0)
                return GetCategory(index);

            return new SkillCategory(SkillCategoryData.DefaultData);
        }

        private static unsafe SkillCategory[] LoadCategories()
        {
            SkillCategory[] list = new SkillCategory[0];

            string grpPath = FileManager.GetFilePath("skillgrp.mul");

            if (grpPath == null)
            { return new SkillCategory[0]; }
            else
            {
                List<SkillCategory> toAdd = new List<SkillCategory>();

                using (FileStream stream = new FileStream(grpPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryReader bin = new BinaryReader(stream);

                    byte[] START = new byte[4]; //File Start Offset

                    bin.Read(START, 0, 4);

                    int index = 0;

                    long
                        x = stream.Length,
                        y = 0;


                    while (y < x) //Position < Length
                    {
                        string name = ParseName(stream);
                        long fileIndex = stream.Position - name.Length;

                        if (name.Length > 0)
                        {
                            toAdd.Add(new SkillCategory(new SkillCategoryData(fileIndex, index, name)));

                            y = stream.Position;

                            ++index;
                        }
                    }
                }

                if (toAdd.Count > 0)
                {
                    list = new SkillCategory[toAdd.Count];

                    for (int i = 0; i < toAdd.Count; i++)
                    {
                        list[i] = toAdd[i];
                    }

                    toAdd.Clear();
                }
            }

            return list;
        }

        private static unsafe string ParseName(Stream stream)
        {
            BinaryReader bin = new BinaryReader(stream);

            string tempName = String.Empty;

            bool esc = false;

            while (!esc && bin.PeekChar() != -1)
            {
                byte[] DATA = new byte[1];

                bin.Read(DATA, 0, 1);

                char c = (char)DATA[0];

                if (Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c) || Char.IsPunctuation(c))
                {
                    tempName += Skills.ToString(c);
                    continue;
                }

                esc = true;
            }

            return tempName.Trim();
        }
    }

    public class SkillCategory
    {
        private SkillCategoryData m_Data = null;
        public SkillCategoryData Data { get { return m_Data; } }

        private int m_Index = -1;
        public int Index { get { return m_Index; } }

        private string m_Name = String.Empty;
        public string Name { get { return m_Name; } }

        public SkillCategory(SkillCategoryData data)
        {
            m_Data = data;
            m_Index = m_Data.Index;
            m_Name = m_Data.Name;
        }

        public void ResetFromData()
        {
            m_Index = m_Data.Index;
            m_Name = m_Data.Name;
        }

        public void ResetFromData(SkillCategoryData data)
        {
            m_Data = data;
            m_Index = m_Data.Index;
            m_Name = m_Data.Name;
        }
    }

    public sealed class SkillCategoryData
    {
        public static SkillCategoryData DefaultData { get { return new SkillCategoryData(0, -1, "null"); } }

        private long m_FileIndex = -1;
        public long FileIndex { get { return m_FileIndex; } }

        private int m_Index = -1;
        public int Index { get { return m_Index; } }

        private string m_Name = String.Empty;
        public string Name { get { return m_Name; } }

        public SkillCategoryData(long fileIndex, int index, string name)
        {
            m_FileIndex = fileIndex;
            m_Index = index;
            m_Name = name;
        }
    }
}