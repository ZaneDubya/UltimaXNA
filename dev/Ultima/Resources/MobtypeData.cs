/***************************************************************************
 *   Mobtypes.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.IO;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public sealed class MobtypeData
    {
        private static Dictionary<int,MobtypeEntry> m_entries = new Dictionary<int,MobtypeEntry>();

        static MobtypeData()
        {
            string path = FileManager.GetFilePath("mobtypes.txt");
            {
                StreamReader stream = new StreamReader(path);
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    Metrics.ReportDataRead(line.Length);
                    if ((line != string.Empty) && (line.Substring(0, 1) != "#"))
                    {
                        string[] data = line.Split('\t');
                        int bodyID = Int32.Parse(data[0]);
                        if (m_entries.ContainsKey(bodyID))
                        {
                            m_entries.Remove(bodyID);
                        }
                        m_entries.Add(bodyID, new MobtypeEntry(data[1], data[2]));
                    }
                }
            }
        }

        public static MobType AnimationTypeXXX(int bodyID)
        {
            return m_entries[bodyID].AnimationType;
        }
    }

    public struct MobtypeEntry
    {
        public string Flags;
        public MobType AnimationType;

        public MobtypeEntry(string type, string flags)
        {
            Flags = flags;
            switch (type)
            {
                case "MONSTER":
                    AnimationType = MobType.Monster;
                    break;
                case "ANIMAL":
                    AnimationType = MobType.Animal;
                    break;
                case "SEA_MONSTER":
                    AnimationType = MobType.Monster;
                    break;
                case "HUMAN":
                    AnimationType = MobType.Humanoid;
                    break;
                case "EQUIPMENT":
                    AnimationType = MobType.Humanoid;
                    break;
                default:
                    AnimationType = MobType.Null;
                    break;
            }
        }
    }

    public enum MobType
    {
        Null = -1,
        Monster = 0,
        Animal = 1,
        Humanoid = 2
    }
}
