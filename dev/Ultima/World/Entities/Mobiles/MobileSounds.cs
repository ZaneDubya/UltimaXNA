/***************************************************************************
 *   MobileSounds.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System.Collections.Generic;
using UltimaXNA.Ultima.Audio;

namespace UltimaXNA.Ultima.World.Entities.Mobiles
{
    public static class MobileSounds
    {
        private static AudioService m_Audio = ServiceRegistry.GetService<AudioService>();

        private static Dictionary<Serial, MobileSoundData> m_Data = new Dictionary<Serial,MobileSoundData>();

        private static int[] m_StepSFX = new int[] { 0x12B, 0x12C };
        private static int[] m_StepMountedSFX = new int[] { 0x129, 0x12A };

        public static void ResetFootstepSounds(Mobile mobile)
        {
            if (m_Data.ContainsKey(mobile.Serial))
            {
                m_Data[mobile.Serial].LastFrame = 1.0f;
            }
        }

        public static void DoFootstepSounds(Mobile mobile, double frame)
        {
            MobileSoundData data;
            if (!m_Data.TryGetValue(mobile.Serial, out data))
            {
                data = new MobileSoundData(mobile);
                m_Data.Add(mobile.Serial, data);
            }

            bool play = (data.LastFrame < 0.5d && frame >= 0.5d) || (data.LastFrame > 0.5d && frame < 0.5d);
            if (mobile.IsMounted && !mobile.IsRunning && frame > 0.5d)
                play = false;

            if (play)
            {
                if (mobile.IsMounted && mobile.IsRunning)
                {
                    int sfx = Utility.RandomValue(0, m_StepMountedSFX.Length - 1);
                    m_Audio.PlaySound(m_StepMountedSFX[sfx]);
                }
                else
                {
                    int sfx = Utility.RandomValue(0, m_StepSFX.Length - 1);
                    m_Audio.PlaySound(m_StepSFX[sfx]);
                }
            }
            data.LastFrame = frame;
        }

        private class MobileSoundData
        {
            public Mobile Mobile
            {
                get;
                private set;
            }

            public double LastFrame = 1d;

            public MobileSoundData(Mobile mobile)
            {
                Mobile = mobile;
            }
        }
    }
}
