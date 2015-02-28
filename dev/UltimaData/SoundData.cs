/***************************************************************************
 *   Sound.cs
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Audio;
#endregion


namespace UltimaXNA.UltimaData
{
    public static class SoundData
    {
        private static BinaryReader _Index;
        private static Stream _Stream;
        private static Dictionary<int, int> _Translations;
        private static Dictionary<int, UOSound> _Sounds;
        private static bool _filesPrepared = false;

        public static void PlaySound(int soundID)
        {
            // Sounds.mul is exclusively locked by the legacy client, so we need to make sure this file is available
            // before attempting to play any sounds.
            if (!_filesPrepared)
                setupFiles();
            if (_filesPrepared)
            {
                if (!_Sounds.ContainsKey(soundID))
                {
                    _Sounds.Add(soundID, getSound(soundID));
                }
                _Sounds[soundID].Play();
            }
        }

        private static UOSound getSound(int soundID)
        {
            if (soundID < 0) { return null; }

            _Index.BaseStream.Seek((long)(soundID * 12), SeekOrigin.Begin);

            int streamStart = (int)_Index.BaseStream.Position;

            int offset = _Index.ReadInt32();
            int length = _Index.ReadInt32();
            int extra = _Index.ReadInt32();

            if ((offset < 0) || (length <= 0))
            {
                if (!_Translations.TryGetValue(soundID, out soundID)) { return null; }

                _Index.BaseStream.Seek((long)(soundID * 12), SeekOrigin.Begin);

                offset = _Index.ReadInt32();
                length = _Index.ReadInt32();
                extra = _Index.ReadInt32();
            }

            if ((offset < 0) || (length <= 0)) { return null; }

            byte[] stringBuffer = new byte[40];
            byte[] buffer = new byte[length - 40];

            _Stream.Seek((long)(offset), SeekOrigin.Begin);
            _Stream.Read(stringBuffer, 0, 40);
            _Stream.Read(buffer, 0, length - 40);

            string str = System.Text.Encoding.ASCII.GetString(stringBuffer); // seems that the null terminator's not being properly recognized :/

            UltimaVars.Metrics.ReportDataRead((int)_Index.BaseStream.Position - streamStart);

            return new UOSound(str.Substring(0, str.IndexOf('\0')), buffer);
        }

        private static void setupFiles()
        {
            try
            {
                _Index = new BinaryReader(new FileStream(FileManager.GetFilePath("soundidx.mul"), FileMode.Open));
                _Stream = new FileStream(FileManager.GetFilePath("sound.mul"), FileMode.Open);
                _filesPrepared = true;
            }
            catch
            {
                _filesPrepared = false;
                return;
            }

            Regex reg = new Regex(@"(\d{1,3}) \x7B(\d{1,3})\x7D (\d{1,3})", RegexOptions.Compiled);

            _Translations = new Dictionary<int, int>();

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
                            _Translations.Add(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                        }
                    }
                }
            }

            _Sounds = new Dictionary<int, UOSound>();
        }

        public class UOSound
        {
            public readonly string Name;
            private byte[] _waveBuffer;
            private List<Pair<DynamicSoundEffectInstance, float>> _instances;

            public UOSound(string name, byte[] buffer)
            {
                Name = name;
                _waveBuffer = buffer;
                _instances = new List<Pair<DynamicSoundEffectInstance, float>>();
            }

            public void Play()
            {
                float now = UltimaVars.EngineVars.TheTime;

                // Check to see if any existing instances of this sound effect have stopped playing. If
                // they have, remove the reference to them so the garbage collector can collect them.
                for (int i = 0; i < _instances.Count; i++)
                    if (_instances[i].ItemB < now)
                    {
                        _instances.RemoveAt(i);
                        i--;
                    }

                DynamicSoundEffectInstance instance = new DynamicSoundEffectInstance(22050, AudioChannels.Mono);
                instance.BufferNeeded += new EventHandler<EventArgs>(instance_BufferNeeded);
                instance.SubmitBuffer(_waveBuffer);
                instance.Play();
                _instances.Add(new Pair<DynamicSoundEffectInstance, float>(instance,
                    now + (instance.GetSampleDuration(_waveBuffer.Length).Milliseconds / 1000f)));
            }

            void instance_BufferNeeded(object sender, EventArgs e)
            {
                // do nothing.
            }
        };
    }
}