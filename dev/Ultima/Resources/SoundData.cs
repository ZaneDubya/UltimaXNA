/***************************************************************************
 *   Sound.cs
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
using System.Text;
using System.Text.RegularExpressions;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.IO;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public static class SoundData
    {
        private static FileIndexBase m_Index;
        //private static Stream m_Stream;
        private static Dictionary<int, int> m_Translations;
        
        private static bool m_filesPrepared = false;

        public static bool TryGetSoundData(int soundID, out byte[] data, out string name)
        {
            // Sounds.mul is exclusively locked by the legacy client, so we need to make sure this file is available
            // before attempting to play any sounds.
            if (!m_filesPrepared)
                setupFiles();

            data = null;
            name = null;

            if (!m_filesPrepared || soundID < 0)
                return false;
            else
            {
                int length, extra;
                bool is_patched;

                BinaryFileReader reader = m_Index.Seek(soundID, out length, out extra, out is_patched);
                int streamStart = (int)reader.Position;
                int offset = (int)reader.Position;
                

                if ((offset < 0) || (length <= 0))
                {
                    if (!m_Translations.TryGetValue(soundID, out soundID))
                        return false;


                    reader = m_Index.Seek(soundID, out length, out extra, out is_patched);
                    streamStart = (int)reader.Position;
                    offset = (int)reader.Position;
                }

                if ((offset < 0) || (length <= 0))
                    return false;

                byte[] stringBuffer = new byte[40];
                data = new byte[length - 40];

                reader.Seek((long)(offset), SeekOrigin.Begin);
                stringBuffer = reader.ReadBytes(40);
                data = reader.ReadBytes(length - 40);

                name = Encoding.ASCII.GetString(stringBuffer).Trim();
                var end = name.IndexOf("\0");
                name = name.Substring(0, end);
                Metrics.ReportDataRead((int)reader.Position - streamStart);

                return true;
            }
        }

        private static void setupFiles()
        {
            try
            {
                m_Index = FileManager.IsUopFormat ? FileManager.CreateFileIndex("soundLegacyMUL.uop", 0xFFF, false, ".dat") : FileManager.CreateFileIndex("soundidx.mul", "sound.mul", 0x1000, -1); // new BinaryReader(new FileStream(FileManager.GetFilePath("soundidx.mul"), FileMode.Open));
               // m_Stream = new FileStream(FileManager.GetFilePath("sound.mul"), FileMode.Open);
                m_filesPrepared = true;
            }
            catch
            {
                m_filesPrepared = false;
                return;
            }

            Regex reg = new Regex(@"(\d{1,3}) \x7B(\d{1,3})\x7D (\d{1,3})", RegexOptions.Compiled);

            m_Translations = new Dictionary<int, int>();

            string line;
            using (StreamReader reader = new StreamReader(FileManager.GetFilePath("Sound.def")))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (((line = line.Trim()).Length != 0) && !line.StartsWith("#"))
                    {
                        Match match = reg.Match(line);

                        if (match.Success)
                        {
                            m_Translations.Add(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                        }
                    }
                }
            }
        }

        
    }
}