using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace InterXLib
{
    public class Settings
    {
        private static List<Setting> m_Updates = new List<Setting>();
        private static bool m_Squelch = false;
        private static Point m_Resolution = new Point(640, 480);

        public bool HasUpdates
        {
            get
            {
                if (m_Updates.Count > 0)
                    return true;
                return false;
            }
        }

        public Setting GetNextUpdate()
        {
            if (m_Updates.Count > 0)
            {
                Setting s = m_Updates[0];
                m_Updates.RemoveAt(0);
                return s;
            }
            else
            {
                return Setting.None;
            }
        }

        public void ClearUpdates()
        {
            m_Updates.Clear();
        }

        public static bool SquelchUpdates
        {
            get { return m_Squelch; }
            set { m_Squelch = value; }
        }

        private static void addUpdate(Setting setting)
        {
            if (!m_Squelch && !m_Updates.Contains(setting))
                m_Updates.Add(setting);
        }

        public static Point Resolution
        {
            get { return m_Resolution; }
            set { m_Resolution = value; addUpdate(Setting.Resolution); }
        }

        public enum Setting
        {
            None,
            Resolution
        }

        public static float SecondsForDoubleClick = 0.5f;
        public static Vector2 ScreenDPI;
        
    }
}
