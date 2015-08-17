/***************************************************************************
 *   GumpDefTranslator.cs
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
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    class GumpDefTranslator
    {
        private static Dictionary<int, Tuple<int, int>> m_Translations;

        static GumpDefTranslator()
        {
            m_Translations = new Dictionary<int, Tuple<int, int>>();
            StreamReader gumpDefFile = null;

            try
            {
                gumpDefFile = new StreamReader(FileManager.GetFile("gump.def"));
            }
            catch
            {
                Tracer.Warn("GumpDefTranslator: unable to open gump.def file. No item/itemgumpling translations are available.");
                return;
            }

            try
            {
                string line;
                while ((line = gumpDefFile.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length <= 0)
                        continue;
                    if (line[0] == '#')
                        continue;
                    string[] defs = line.Replace('\t', ' ').Split(' ');
                    if (defs.Length != 3)
                        continue;

                    int inGump = int.Parse(defs[0]);
                    int outGump = int.Parse(defs[1].Replace("{", string.Empty).Replace("}", string.Empty));
                    int outHue = int.Parse(defs[2]);

                    if (m_Translations.ContainsKey(inGump))
                        m_Translations.Remove(inGump);

                    m_Translations.Add(inGump, new Tuple<int, int>(outGump, outHue));
                }
            }
            catch
            {
                Tracer.Warn("GumpDefTranslator: unable to parse gump.def file. No item/itemgumpling translations are available.");
            }

            gumpDefFile.Close();
        }

        public static bool ItemHasGumpTranslation(int gumpIndex, out int gumpIndexTranslated, out int defaultHue)
        {
            Tuple<int, int> translation;
            if (m_Translations.TryGetValue(gumpIndex, out translation))
            {
                gumpIndexTranslated = translation.Item1;
                defaultHue = translation.Item2;
                return true;
            }
            else
            {
                gumpIndexTranslated = 0;
                defaultHue = 0;
                return false;
            }
        }
    }
}
