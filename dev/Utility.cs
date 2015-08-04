/***************************************************************************
 *   Utility.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA
{
    /// <summary>
    /// Utility class used to host common functions that do not fit inside a specific object.
    /// </summary>
    public class Utility
    {
        #region Buffer Formatting
        /// <summary>
        /// Formats the buffer to an output to a hex editor view of the data
        /// </summary>
        /// <param name="buffer">The buffer to be formatted</param>
        /// <returns>A System.String containing the formatted buffer</returns>
        public static string FormatBuffer(byte[] buffer)
        {
            if (buffer == null)
                return string.Empty;

            string formatted = FormatBuffer(buffer, buffer.Length);

            return formatted;
        }

        /// <summary>
        /// Formats the buffer to an output to a hex editor view of the data
        /// </summary>
        /// <param name="buffer">The buffer to be formatted</param>
        /// <param name="length">The length in bytes to format</param>
        /// <returns>A System.String containing the formatted buffer</returns>
        public static string FormatBuffer(byte[] buffer, int length)
        {
            if (buffer == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            MemoryStream ms = new MemoryStream(buffer);

            FormatBuffer(builder, ms, length);

            return builder.ToString();
        }

        /// <summary>
        /// Formats the stream buffer to an output to a hex editor view of the data
        /// </summary>
        /// <param name="input">The stream to be formatted</param>
        /// <param name="length">The length in bytes to format</param>
        /// <returns>A System.String containing the formatted buffer</returns>
        public static string FormatBuffer(Stream input, int length)
        {
            StringBuilder builder = new StringBuilder();

            FormatBuffer(builder, input, length);

            return builder.ToString();
        }


        /// <summary>
        /// Formats the stream buffer to an output to a hex editor view of the data
        /// </summary>
        /// <param name="builder">The string builder to output the formatted buffer to</param>
        /// <param name="input">The stream to be formatted</param>
        /// <param name="length">The length in bytes to format</param>
        public static void FormatBuffer(StringBuilder builder, Stream input, int length)
        {
            length = (int)Math.Min(length, input.Length);
            builder.AppendLine("        0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F");
            builder.AppendLine("       -- -- -- -- -- -- -- --  -- -- -- -- -- -- -- --");

            int byteIndex = 0;

            int whole = length >> 4;
            int rem = length & 0xF;

            for (int i = 0; i < whole; ++i, byteIndex += 16)
            {
                StringBuilder bytes = new StringBuilder(49);
                StringBuilder chars = new StringBuilder(16);

                for (int j = 0; j < 16; ++j)
                {
                    int c = input.ReadByte();

                    bytes.Append(c.ToString("X2"));

                    if (j != 7)
                    {
                        bytes.Append(' ');
                    }
                    else
                    {
                        bytes.Append("  ");
                    }

                    if (c >= 0x20 && c < 0x80)
                    {
                        chars.Append((char)c);
                    }
                    else
                    {
                        chars.Append('.');
                    }
                }

                builder.Append(byteIndex.ToString("X4"));
                builder.Append("   ");
                builder.Append(bytes);
                builder.Append("  ");
                builder.AppendLine(chars.ToString());
            }

            if (rem != 0)
            {
                StringBuilder bytes = new StringBuilder(49);
                StringBuilder chars = new StringBuilder(rem);

                for (int j = 0; j < 16; ++j)
                {
                    if (j < rem)
                    {
                        int c = input.ReadByte();

                        bytes.Append(c.ToString("X2"));

                        if (j != 7)
                        {
                            bytes.Append(' ');
                        }
                        else
                        {
                            bytes.Append("  ");
                        }

                        if (c >= 0x20 && c < 0x80)
                        {
                            chars.Append((char)c);
                        }
                        else
                        {
                            chars.Append('.');
                        }
                    }
                    else
                    {
                        bytes.Append("   ");
                    }
                }

                builder.Append(byteIndex.ToString("X4"));
                builder.Append("   ");
                builder.Append(bytes);
                builder.Append("  ");
                builder.AppendLine(chars.ToString());
            }
        }
        #endregion

        #region Encoding

        private static Encoding s_Utf8;
        private static Encoding s_Utf8WithEncoding;

        public static Encoding UTF8
        {
            get
            {
                if (s_Utf8 == null)
                    s_Utf8 = new UTF8Encoding(false, false);

                return s_Utf8;
            }
        }

        public static Encoding UTF8WithEncoding
        {
            get
            {
                if (s_Utf8WithEncoding == null)
                    s_Utf8WithEncoding = new UTF8Encoding(true, false);

                return s_Utf8WithEncoding;
            }
        }
        #endregion

        public static int GetDistanceToSqrt(int orgx, int orgy, int goalx, int goaly)
        {
            int xDelta = goalx - orgx;
            int yDelta = goaly - orgy;

            return (int)Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
        }

        public static Type GetTypeFromAppDomain(string typeName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type type = null;

            for (int i = 0; i < assemblies.Length && type == null; i++)
            {
                type = (from t in assemblies[i].GetTypes()
                        where t.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase)
                        select t).FirstOrDefault();
            }

            return type;
        }

        // Color utilities, made freely available on http://snowxna.wordpress.com/
        #region ColorUtility
        private static readonly char[] HexDigits = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static string ColorToHexString(Color color)
        {
            byte[] bytes = new byte[4];
            bytes[0] = color.A;
            bytes[1] = color.R;
            bytes[2] = color.G;
            bytes[3] = color.B;
            char[] chars = new char[8];
            for(int i = 0; i < 4; i++)
            {
                int b = bytes[i];
                chars[i * 2] = HexDigits[b >> 4];
                chars[i * 2 + 1] = HexDigits[b & 0xF];
            }
            return new string(chars);
        }

        static byte HexDigitToByte(char c)
        {
            switch(c)
            {
                case '0': return (byte)0;
                case '1': return (byte)1;
                case '2': return (byte)2;
                case '3': return (byte)3;
                case '4': return (byte)4;
                case '5': return (byte)5;
                case '6': return (byte)6;
                case '7': return (byte)7;
                case '8': return (byte)8;
                case '9': return (byte)9;
                case 'A': case 'a': return (byte)10;
                case 'B': case 'b': return (byte)11;
                case 'C': case 'c': return (byte)12;
                case 'D': case 'd': return (byte)13;
                case 'E': case 'e': return (byte)14;
                case 'F': case 'f': return (byte)15;
            }
            return (byte)0;
        }

        public static uint UintFromColor(Color color)
        {
            return (uint)((color.A << 24) | (color.B << 16) | (color.G << 8) | (color.R));
        }

        public static Color ColorFromHexString(string hex)
        {
            switch (hex.Length)
            {
                case 8:
                {
                    int a = (HexDigitToByte(hex[0]) << 4) + HexDigitToByte(hex[1]);
                    int r = (HexDigitToByte(hex[2]) << 4) + HexDigitToByte(hex[3]);
                    int g = (HexDigitToByte(hex[4]) << 4) + HexDigitToByte(hex[5]);
                    int b = (HexDigitToByte(hex[6]) << 4) + HexDigitToByte(hex[7]);
                    return new Color((byte)r, (byte)g, (byte)b, (byte)a);
                }
                case 6:
                {
                    int r = (HexDigitToByte(hex[0]) << 4) + HexDigitToByte(hex[1]);
                    int g = (HexDigitToByte(hex[2]) << 4) + HexDigitToByte(hex[3]);
                    int b = (HexDigitToByte(hex[4]) << 4) + HexDigitToByte(hex[5]);
                    return new Color((byte)r, (byte)g, (byte)b);
                }
                case 3:
                {
                    int r = (HexDigitToByte(hex[0]) << 4) + HexDigitToByte(hex[0]);
                    int g = (HexDigitToByte(hex[1]) << 4) + HexDigitToByte(hex[1]);
                    int b = (HexDigitToByte(hex[2]) << 4) + HexDigitToByte(hex[2]);
                    return new Color((byte)r, (byte)g, (byte)b);
                }
                default:
                    return Color.Black;
            }
        }

        private static readonly Dictionary<string, Color> ColorTable = new Dictionary<string, Color>()
        {
            {"white", Color.White},
            {"red", Color.Red},
            {"blue", Color.Blue},
            {"green", Color.Green},
            {"orange", Color.Orange},
            {"yellow", Color.Yellow}
            //add more colors here
        };

        public static Color? ColorFromString(string color)
        {
            Color output;
            if (!ColorTable.TryGetValue(color.ToLower(), out output)) return null;

            return output;
        }
        #endregion

        // Version string.
        static string s_VersionString;
        public static string VersionString
        {
            get
            {
                if (s_VersionString == null)
                {
                    Version v = Assembly.GetExecutingAssembly().GetName().Version;
                    DateTime d = new DateTime(v.Build * TimeSpan.TicksPerDay).AddYears(1999).AddDays(-1);
                    s_VersionString = string.Format("UltimaXNA PreAlpha Milestone {0}.{1} ({2})", v.Major, v.Minor, string.Format("{0:MMMM d, yyyy}", d));
                }
                return s_VersionString;
            }
        }

        #region To[Something]
        // Copied from RunUO Utility class
        public static bool ToBoolean(string value)
        {
            bool b;
            bool.TryParse(value, out b);

            return b;
        }

        public static double ToDouble(string value)
        {
            double d;
            double.TryParse(value, out d);

            return d;
        }

        public static TimeSpan ToTimeSpan(string value)
        {
            TimeSpan t;
            TimeSpan.TryParse(value, out t);

            return t;
        }

        public static int ToInt32(string value)
        {
            int i;

            if (value.StartsWith("0x"))
                int.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out i);
            else
                int.TryParse(value, out i);

            return i;
        }
        #endregion
        
        public static int IPAddress
        {
            get
            {
                byte[] iIPAdress = { 127, 0, 0, 1 };
                int iAddress = BitConverter.ToInt32(iIPAdress, 0);
                return iAddress;
            }
        }

        public static long GetLongAddressValue(IPAddress address)
        {
#pragma warning disable 618
            return address.Address;
#pragma warning restore 618
        }

        static Random s_Random;
        public static int RandomValue(int low, int high)
        {
            if (s_Random == null)
                s_Random = new Random();
            int rnd = s_Random.Next(low, high + 1);
            return rnd; 
        }

        public static bool IsPointThisDistanceAway(Point initial, Point final, int distance)
        {
            // fast distance. Not super accurate, but fast. Fast is good.
            if (Math.Abs(final.X - initial.X) + Math.Abs(final.Y - initial.Y) > distance)
                return true;
            else
                return false;
        }

        public static Vector3 GetHueVector(int hue)
        {
            return GetHueVector(hue, false, false);
        }

        public static Vector3 GetHueVector(int hue, bool partial, bool transparent)
        {
            if ((hue & 0x4000) != 0)
                transparent = true;
            if ((hue & 0x8000) != 0)
                partial = true;

            if (hue == 0)
                return new Vector3(0, 0, transparent ? 0.5f : 0);

            return new Vector3(hue & 0x0FFF, partial ? 2 : 1, transparent ? 0.5f : 0);
        }

        public static string GetColorFromUshort(ushort color)
        {
            const int MULTIPLIER = 0xFF / 0x1F;
            uint uintColor = (uint)(
                ((((color >> 10) & 0x1F) * MULTIPLIER)) |
                ((((color >> 5) & 0x1F) * MULTIPLIER) << 8) |
                (((color & 0x1F) * MULTIPLIER) << 16)
                );
            return string.Format("{0:X6}", uintColor);
        }

        public static string GetColorFromInt(int color)
        {
            return string.Format("{0:X6}", color);
        }

        public static int DistanceBetweenTwoPoints(Point p1, Point p2)
        {
            return Convert.ToInt32(Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)));
        }

        // Maintain an accurate count of frames per second.
        static readonly List<float> FPSHistory = new List<float>();
        internal static int UpdateFPS(double frameMS)
        {
            if (frameMS > 0)
            {
                while (FPSHistory.Count > 19)
                    FPSHistory.RemoveAt(0);
                FPSHistory.Add(1000.0f / (float)frameMS);
            }

            float count = 0.0f;
            for (int i = 0; i < FPSHistory.Count; i++)
            {
                count += FPSHistory[i];
            }

            count /= FPSHistory.Count;

            return (int)Math.Ceiling(count);
        }

        public static void SaveTexture(Texture2D texture, string path)
        {
            if (texture != null)
                texture.SaveAsPng(new FileStream(path, FileMode.Create), texture.Width, texture.Height);
        }

        public static string CapitalizeFirstCharacter(string str)
        {
            if (str == null || str == string.Empty)
                return string.Empty;
            if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string CapitalizeAllWords(string str)
        {
            if (str == null || str == string.Empty)
                return string.Empty;
            if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();

            StringBuilder sb = new StringBuilder();
            bool capitalizeNext = true;
            for (int i = 0; i < str.Length; i++)
            {
                if (capitalizeNext)
                    sb.Append(char.ToUpper(str[i]));
                else
                    sb.Append(str[i]);
                capitalizeNext = (" .,;!".Contains(str[i]));
            }
            return sb.ToString();
        }
    }
}
