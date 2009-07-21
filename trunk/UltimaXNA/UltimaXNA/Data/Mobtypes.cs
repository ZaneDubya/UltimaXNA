/***************************************************************************
 *   Mobtypes.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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

        public static int AnimationType(int bodyID)
        {
            return _entries[bodyID].AnimationType;
        }
    }

    public struct MobtypeEntry
    {
        public string Flags;
        public int AnimationType;

        public MobtypeEntry(string type, string flags)
        {
            Flags = flags;
            switch (type)
            {
                case "MONSTER":
                    AnimationType = 0;
                    break;
                case "ANIMAL":
                    AnimationType = 1;
                    break;
                case "SEA_MONSTER":
                    AnimationType = 0;
                    break;
                case "HUMAN":
                    AnimationType = 2;
                    break;
                case "EQUIPMENT":
                    AnimationType = 2;
                    break;
                default:
                    AnimationType = -1;
                    break;
            }
        }
    }
}
