using System;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;

namespace InterXLib
{
    public static class Library
    {
        private static GraphicsDevice m_Graphics;
        private static InputState m_Input;
        private static View.Projections m_Projections;

        public static void Initialize(GraphicsDevice graphics, InputState input)
        {
            m_Graphics = graphics;
            m_Input = input;
            m_Projections = new View.Projections();
            m_Projections.Initialize();
        }

        public static View.Projections Projections
        {
            get
            {
                return m_Projections;
            }
        }

        public static InputState Input
        {
            get { return m_Input; }
        }

        public static Texture2D CreateTexture(int w, int h)
        {
            return new Texture2D(m_Graphics, w, h);
        }

        private static Random m_Random;
        public static int Random(int a, int b)
        {
            if (m_Random == null)
                m_Random = new Random(0);
            return m_Random.Next(a, b + 1);
        }

        public static float RandomF(float a, float b)
        {
            if (m_Random == null)
                m_Random = new Random(0);
            return (float)(m_Random.NextDouble() * (b - a) - a);
        }

        public static uint[] RandomTile()
        {
            uint[] data = new uint[64];
            for (int i = 0; i < 64; i++)
                data[i] = (uint)Random(0, 3);
            return data;
        }

        public static Matrix ProjectionMatrixScreen
        {
            get
            {
                return Matrix.CreateOrthographicOffCenter(0f,
                    Settings.Resolution.X,
                    Settings.Resolution.Y,
                    0f, -20000f, 20000f);
            }
        }

        private static Encoding m_UTF8, m_UTF8WithEncoding;

        public static Encoding UTF8
        {
            get
            {
                if (m_UTF8 == null)
                    m_UTF8 = new UTF8Encoding(false, false);

                return m_UTF8;
            }
        }

        public static Encoding UTF8WithEncoding
        {
            get
            {
                if (m_UTF8WithEncoding == null)
                    m_UTF8WithEncoding = new UTF8Encoding(true, false);

                return m_UTF8WithEncoding;
            }
        }

        public static long GetLongAddressValue(IPAddress address)
        {
#pragma warning disable 618
            return address.Address;
#pragma warning restore 618
        }

        public static int Clamp(int value, int low, int high)
        {
            if (value < low)
                value = low;
            if (value > high)
                value = high;
            return value;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static uint FourCharsToUInt(string four_chars)
        {
            uint value = BitConverter.ToUInt32(Encoding.ASCII.GetBytes(four_chars), 0);
            return value;
        }

        public static string UIntToFourChars(uint four_chars)
        {
            string value = Encoding.ASCII.GetString(BitConverter.GetBytes(four_chars));
            return value;
        }

        public static IEnumerable<string> GetFilesInPath(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);

            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }

        public static string UINTtoHex(uint value)
        {
            string hex = String.Format("{0:X8}", value);
            return hex;
        }
    }
}
