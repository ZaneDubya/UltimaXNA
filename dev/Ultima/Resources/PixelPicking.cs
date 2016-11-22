using System.Collections.Generic;

namespace UltimaXNA.Ultima.Resources
{
    class PixelPicking
    {
        const int InitialDataCount = 0x40000; // 256kb
        private Dictionary<int, int> m_IDs = new Dictionary<int, int>();
        private List<byte> m_Data = new List<byte>(InitialDataCount);

        public bool Get(int textureID, int x, int y)
        {
            int index;
            if (!m_IDs.TryGetValue(textureID, out index))
            {
                return false;
            }
            int width = ReadIntegerFromData(ref index);
            int current = 0;
            int target = x + y * width;
            bool inTransparentSpan = true;
            while (current < target)
            {
                int spanLength = ReadIntegerFromData(ref index);
                current += spanLength;
                if (target < current)
                {
                    return !inTransparentSpan;
                }
                inTransparentSpan = !inTransparentSpan;
            }
            return false;
        }

        bool Has(int textureID)
        {
            return m_IDs.ContainsKey(textureID);
        }

        public void Set(int textureID, int width, int height, ushort[] pixels)
        {
            int begin = m_Data.Count;
            WriteIntegerToData(width);
            // WriteIntegerToData(height);
            bool countingTransparent = true;
            int count = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                bool isTransparent = pixels[i] == 0;
                if (countingTransparent != isTransparent)
                {
                    WriteIntegerToData(count);
                    countingTransparent = !countingTransparent;
                    count = 0;
                }
                count += 1;
            }
            WriteIntegerToData(count);
            m_IDs.Add(textureID, begin);
        }

        private void WriteIntegerToData(int value)
        {
            while (value > 0x7f)
            {
                m_Data.Add((byte)((value & 0x7f) | 0x80));
                value >>= 7;
            }
            m_Data.Add((byte)value);
        }

        private int ReadIntegerFromData(ref int index)
        {
            int value = 0;
            int shift = 0;
            while (true)
            {
                byte data = m_Data[index++];
                value += (data & 0x7f) << shift;
                if ((data & 0x80) == 0x00)
                {
                    return value;
                }
                shift += 7;
            }
        }
    }
}
