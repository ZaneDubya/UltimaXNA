﻿/***************************************************************************
 *                                  Body.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.IO;

namespace UltimaXNA.Ultima.Data
{
    public enum BodyType : byte
    {
        Empty,
        Monster,
        Sea,
        Animal,
        Human,
        Equipment
    }

    public struct Body
    {
        private int m_BodyID;

        private static BodyType[] m_Types;

        static Body()
        {
            string path = @"data/bodytable.cfg";

            if (File.Exists(path))
            {
                using (StreamReader ip = new StreamReader(path))
                {
                    m_Types = new BodyType[0x1000];

                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        if (line.Length == 0 || line.StartsWith("#"))
                            continue;

                        string[] split = line.Split('\t');

                        BodyType type;
                        int bodyID;

                        if (int.TryParse(split[0], out bodyID) && Enum.TryParse(split[1], true, out type) && bodyID >= 0 && bodyID < m_Types.Length)
                        {
                            m_Types[bodyID] = type;
                        }
                        else
                        {
                            Console.WriteLine("Warning: Invalid bodyTable entry:");
                            Console.WriteLine(line);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Warning: Data/bodyTable.cfg does not exist");

                m_Types = new BodyType[0];
            }
        }

        public Body(int bodyID)
        {
            m_BodyID = bodyID;
        }

        public BodyType Type
        {
            get
            {
                if (m_BodyID >= 0 && m_BodyID < m_Types.Length)
                    return m_Types[m_BodyID];
                else
                    return BodyType.Empty;
            }
        }

        public bool IsHumanoid
        {
            get
            {
                return m_BodyID >= 0
                    && m_BodyID < m_Types.Length
                    && m_Types[m_BodyID] == BodyType.Human
                    && m_BodyID != 402
                    && m_BodyID != 403
                    && m_BodyID != 607
                    && m_BodyID != 608
                    && m_BodyID != 694
                    && m_BodyID != 695
                    && m_BodyID != 970;
            }
        }

        public bool IsGargoyle
        {
            get
            {
                return m_BodyID == 666
                    || m_BodyID == 667
                    || m_BodyID == 694
                    || m_BodyID == 695;
            }
        }

        public bool IsMale
        {
            get
            {
                return m_BodyID == 183
                    || m_BodyID == 185
                    || m_BodyID == 400
                    || m_BodyID == 402
                    || m_BodyID == 605
                    || m_BodyID == 607
                    || m_BodyID == 666
                    || m_BodyID == 694
                    || m_BodyID == 750;
            }
        }

        public bool IsFemale
        {
            get
            {
                return m_BodyID == 184
                    || m_BodyID == 186
                    || m_BodyID == 401
                    || m_BodyID == 403
                    || m_BodyID == 606
                    || m_BodyID == 608
                    || m_BodyID == 667
                    || m_BodyID == 695
                    || m_BodyID == 751;
            }
        }

        public bool IsGhost
        {
            get
            {
                return m_BodyID == 402
                    || m_BodyID == 403
                    || m_BodyID == 607
                    || m_BodyID == 608
                    || m_BodyID == 694
                    || m_BodyID == 695
                    || m_BodyID == 970;
            }
        }

        public bool IsMonster
        {
            get
            {
                return m_BodyID >= 0
                    && m_BodyID < m_Types.Length
                    && m_Types[m_BodyID] == BodyType.Monster;
            }
        }

        public bool IsAnimal
        {
            get
            {
                return m_BodyID >= 0
                    && m_BodyID < m_Types.Length
                    && m_Types[m_BodyID] == BodyType.Animal;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return m_BodyID >= 0
                    && m_BodyID < m_Types.Length
                    && m_Types[m_BodyID] == BodyType.Empty;
            }
        }

        public bool IsSea
        {
            get
            {
                return m_BodyID >= 0
                    && m_BodyID < m_Types.Length
                    && m_Types[m_BodyID] == BodyType.Sea;
            }
        }

        public bool IsEquipment
        {
            get
            {
                return m_BodyID >= 0
                    && m_BodyID < m_Types.Length
                    && m_Types[m_BodyID] == BodyType.Equipment;
            }
        }

        public int BodyID
        {
            get
            {
                return m_BodyID;
            }
        }

        public static implicit operator int(Body a)
        {
            return a.m_BodyID;
        }

        public static implicit operator Body(int a)
        {
            return new Body(a);
        }

        public override string ToString()
        {
            return string.Format("0x{0:X}", m_BodyID);
        }

        public override int GetHashCode()
        {
            return m_BodyID;
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Body)) return false;

            return ((Body)o).m_BodyID == m_BodyID;
        }

        public static bool operator ==(Body l, Body r)
        {
            return l.m_BodyID == r.m_BodyID;
        }

        public static bool operator !=(Body l, Body r)
        {
            return l.m_BodyID != r.m_BodyID;
        }

        public static bool operator >(Body l, Body r)
        {
            return l.m_BodyID > r.m_BodyID;
        }

        public static bool operator >=(Body l, Body r)
        {
            return l.m_BodyID >= r.m_BodyID;
        }

        public static bool operator <(Body l, Body r)
        {
            return l.m_BodyID < r.m_BodyID;
        }

        public static bool operator <=(Body l, Body r)
        {
            return l.m_BodyID <= r.m_BodyID;
        }
    }
}