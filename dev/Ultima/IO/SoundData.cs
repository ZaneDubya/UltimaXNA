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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Audio;
using UltimaXNA.Core.Diagnostics;
#endregion


namespace UltimaXNA.Ultima.IO
{
    public static class SoundData
    {
        private static BinaryReader m_Index;
        private static Stream m_Stream;
        private static Dictionary<int, int> m_Translations;
        private static Dictionary<int, UOSound> m_Sounds;
        private static bool m_filesPrepared = false;

        public static void PlaySound(int soundID)
        {
            // Sounds.mul is exclusively locked by the legacy client, so we need to make sure this file is available
            // before attempting to play any sounds.
            if (!m_filesPrepared)
                setupFiles();
            if (m_filesPrepared)
            {
                if (!m_Sounds.ContainsKey(soundID))
                {
                    m_Sounds.Add(soundID, getSound(soundID));
                }
                m_Sounds[soundID].Play();
            }
        }

        private static UOSound getSound(int soundID)
        {
            if (soundID < 0) { return null; }

            m_Index.BaseStream.Seek((long)(soundID * 12), SeekOrigin.Begin);

            int streamStart = (int)m_Index.BaseStream.Position;

            int offset = m_Index.ReadInt32();
            int length = m_Index.ReadInt32();
            int extra = m_Index.ReadInt32();

            if ((offset < 0) || (length <= 0))
            {
                if (!m_Translations.TryGetValue(soundID, out soundID)) { return null; }

                m_Index.BaseStream.Seek((long)(soundID * 12), SeekOrigin.Begin);

                offset = m_Index.ReadInt32();
                length = m_Index.ReadInt32();
                extra = m_Index.ReadInt32();
            }

            if ((offset < 0) || (length <= 0)) { return null; }

            byte[] stringBuffer = new byte[40];
            byte[] buffer = new byte[length - 40];

            m_Stream.Seek((long)(offset), SeekOrigin.Begin);
            m_Stream.Read(stringBuffer, 0, 40);
            m_Stream.Read(buffer, 0, length - 40);

            string str = System.Text.Encoding.ASCII.GetString(stringBuffer); // seems that the null terminator's not being properly recognized :/

            Metrics.ReportDataRead((int)m_Index.BaseStream.Position - streamStart);

            return new UOSound(str.Substring(0, str.IndexOf('\0')), buffer);
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

            m_Sounds = new Dictionary<int, UOSound>();
        }

        public class UOSound
        {
            public readonly string Name;
            private byte[] m_waveBuffer;
            private List<Tuple<DynamicSoundEffectInstance, float>> m_instances;

            public UOSound(string name, byte[] buffer)
            {
                Name = name;
                m_waveBuffer = buffer;
                m_instances = new List<Tuple<DynamicSoundEffectInstance, float>>();
            }

            public void Play()
            {
                float now = (float)UltimaEngine.TotalMS;

                // Check to see if any existing instances of this sound effect have stopped playing. If
                // they have, remove the reference to them so the garbage collector can collect them.
                for (int i = 0; i < m_instances.Count; i++)
                    if (m_instances[i].Item2 < now)
                    {
                        m_instances.RemoveAt(i);
                        i--;
                    }

                DynamicSoundEffectInstance instance = new DynamicSoundEffectInstance(22050, AudioChannels.Mono);
                instance.BufferNeeded += new EventHandler<EventArgs>(instance_BufferNeeded);
                instance.SubmitBuffer(m_waveBuffer);
                instance.Play();
                m_instances.Add(new Tuple<DynamicSoundEffectInstance, float>(instance,
                    now + (instance.GetSampleDuration(m_waveBuffer.Length).Milliseconds)));
            }

            void instance_BufferNeeded(object sender, EventArgs e)
            {
                // do nothing.
            }
        };
    }
}