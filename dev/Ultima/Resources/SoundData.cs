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
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public static class SoundData
    {
        private static BinaryReader m_Index;
        private static Stream m_Stream;
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
                m_Index.BaseStream.Seek((long)(soundID * 12), SeekOrigin.Begin);

                int streamStart = (int)m_Index.BaseStream.Position;

                int offset = m_Index.ReadInt32();
                int length = m_Index.ReadInt32();
                int extra = m_Index.ReadInt32();

                if ((offset < 0) || (length <= 0))
                {
                    if (!m_Translations.TryGetValue(soundID, out soundID))
                        return false;

                    m_Index.BaseStream.Seek((long)(soundID * 12), SeekOrigin.Begin);

                    offset = m_Index.ReadInt32();
                    length = m_Index.ReadInt32();
                    extra = m_Index.ReadInt32();
                }

                if ((offset < 0) || (length <= 0))
                    return false;

                byte[] stringBuffer = new byte[40];
                data = new byte[length - 40];

                m_Stream.Seek((long)(offset), SeekOrigin.Begin);
                m_Stream.Read(stringBuffer, 0, 40);
                m_Stream.Read(data, 0, length - 40);

                name = Encoding.ASCII.GetString(stringBuffer);

                Metrics.ReportDataRead((int)m_Index.BaseStream.Position - streamStart);

                return true;
            }
        }

        private static void setupFiles()
        {
            try
            {
                m_Index = new BinaryReader(new FileStream(FileManager.GetFilePath("soundidx.mul"), FileMode.Open));
                m_Stream = new FileStream(FileManager.GetFilePath("sound.mul"), FileMode.Open);
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