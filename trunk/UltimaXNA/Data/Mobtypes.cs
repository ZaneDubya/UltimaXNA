/***************************************************************************
 *   Mobtypes.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using System.IO;
using System;
#endregion

namespace UltimaXNA.Data
{
    public sealed class Mobtypes
    {
        private static Dictionary<int,MobtypeEntry> _entries = new Dictionary<int,MobtypeEntry>();

        static Mobtypes()
        {
            string path = FileManager.GetFilePath("mobtypes.txt");
            {
                StreamReader stream = new StreamReader(path);
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    ClientVars.Metrics.ReportDataRead(line.Length);
                    if ((line != string.Empty) && (line.Substring(0, 1) != "#"))
                    {
                        string[] data = line.Split('\t');
                        int bodyID = Int32.Parse(data[0]);
                        if (_entries.ContainsKey(bodyID))
                        {
                            _entries.Remove(bodyID);
                        }
                        _entries.Add(bodyID, new MobtypeEntry(data[1], data[2]));
                    }
                }
            }
        }

        public static MobType AnimationTypeXXX(int bodyID)
        {
            return _entries[bodyID].AnimationType;
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
