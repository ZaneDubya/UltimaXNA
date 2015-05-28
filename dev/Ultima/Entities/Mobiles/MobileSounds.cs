using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.Audio;

namespace UltimaXNA.Ultima.Entities.Mobiles
{
    public static class MobileSounds
    {
        private static AudioService m_Audio = UltimaServices.GetService<AudioService>();

        private static Dictionary<Serial, MobileSoundData> m_Data = new Dictionary<Serial,MobileSoundData>();

        public static void ResetFootstepSounds(Mobile mobile)
        {
            if (m_Data.ContainsKey(mobile.Serial))
            {
                m_Data[mobile.Serial].LastFootstep = 1.0f;
                m_Data[mobile.Serial].LastFootstepWasRight = false;
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

            bool play = (data.LastFootstep + frame >= 1d);

            if (play)
            {
                if (data.LastFootstepWasRight)
                    m_Audio.PlaySound(0x12C);
                else
                    m_Audio.PlaySound(0x12D);
                data.LastFootstepWasRight = !data.LastFootstepWasRight;
                data.LastFootstep -= 1f;
            }
            else
            {
                data.LastFootstep += frame;
            }
        }

        private class MobileSoundData
        {
            public Mobile Mobile
            {
                get;
                private set;
            }

            public bool LastFootstepWasRight = false;
            public double LastFootstep;

            public MobileSoundData(Mobile mobile)
            {
                Mobile = mobile;
            }
        }
    }
}
