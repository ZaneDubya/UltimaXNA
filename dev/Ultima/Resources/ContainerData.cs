/***************************************************************************
 *   ContainerData.cs
 *   Based on code from RunUO: http://www.runuo.com
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
using Microsoft.Xna.Framework;

namespace UltimaXNA.Ultima.Resources
{
    public class ContainerData
    {
        static ContainerData()
        {
            m_Table = new Dictionary<int, ContainerData>();

            string path = @"data\containers.cfg";

            if (!File.Exists(path))
            {
                m_Default = new ContainerData(0x3C, new Rectangle(44, 65, 142, 94), 0x48);
                return;
            }

            using (StreamReader reader = new StreamReader(path))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (line.Length == 0 || line.StartsWith("#"))
                        continue;

                    try
                    {
                        string[] split = line.Split('\t');

                        if (split.Length >= 3)
                        {
                            int gumpID = Utility.ToInt32(split[0]);

                            string[] aRect = split[1].Split(' ');
                            if (aRect.Length < 4)
                                continue;

                            int x = Utility.ToInt32(aRect[0]);
                            int y = Utility.ToInt32(aRect[1]);
                            int width = Utility.ToInt32(aRect[2]);
                            int height = Utility.ToInt32(aRect[3]);

                            Rectangle bounds = new Rectangle(x, y, width, height);

                            int dropSound = Utility.ToInt32(split[2]);

                            ContainerData data = new ContainerData(gumpID, bounds, dropSound);

                            if (m_Default == null)
                                m_Default = data;

                            if (split.Length >= 4)
                            {
                                string[] aIDs = split[3].Split(',');

                                for (int i = 0; i < aIDs.Length; i++)
                                {
                                    int id = Utility.ToInt32(aIDs[i]);

                                    if (m_Table.ContainsKey(id))
                                    {
                                        Console.WriteLine(@"Warning: double ItemID entry in Data\containers.cfg");
                                    }
                                    else
                                    {
                                        m_Table[id] = data;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            if (m_Default == null)
                m_Default = new ContainerData(0x3C, new Rectangle(44, 65, 142, 94), 0x48);
        }

        private static ContainerData m_Default;
        private static Dictionary<int, ContainerData> m_Table;

        public static ContainerData Default
        {
            get { return m_Default; }
            set { m_Default = value; }
        }

        public static ContainerData GetData(int itemID)
        {
            ContainerData data = null;
            m_Table.TryGetValue(itemID, out data);

            if (data != null)
                return data;
            else
                return m_Default;
        }

        private int m_GumpID;
        private Rectangle m_Bounds;
        private int m_DropSound;

        public int GumpID { get { return m_GumpID; } }
        public Rectangle Bounds { get { return m_Bounds; } }
        public int DropSound { get { return m_DropSound; } }

        public ContainerData(int gumpID, Rectangle bounds, int dropSound)
        {
            m_GumpID = gumpID;
            m_Bounds = bounds;
            m_DropSound = dropSound;
        }
    }
}
