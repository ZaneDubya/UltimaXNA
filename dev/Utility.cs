/***************************************************************************
 *   Utility.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using UltimaXNA.UltimaData.FontsOld;
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
        /// <param name="buffer">The stream to be formatted</param>
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
        /// <param name="buffer">The stream to be formatted</param>
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
                builder.Append(bytes.ToString());
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
                builder.Append(bytes.ToString());
                builder.Append("  ");
                builder.AppendLine(chars.ToString());
            }
        }
        #endregion

        #region Encoding
        private static Encoding utf8, utf8WithEncoding;

        public static Encoding UTF8
        {
            get
            {
                if (utf8 == null)
                    utf8 = new UTF8Encoding(false, false);

                return utf8;
            }
        }

        public static Encoding UTF8WithEncoding
        {
            get
            {
                if (utf8WithEncoding == null)
                    utf8WithEncoding = new UTF8Encoding(true, false);

                return utf8WithEncoding;
            }
        }
        #endregion

        #region Conversion
        public static bool TryConvert<TConverFrom, UConvertTo>(TConverFrom convertFrom, out UConvertTo convertTo)
        {
            convertTo = default(UConvertTo);
            bool converted = false;

            TypeConverter converter = TypeDescriptor.GetConverter(convertFrom);

            if (converter.CanConvertTo(typeof(UConvertTo)))
            {
                convertTo = (UConvertTo)converter.ConvertTo(convertFrom, typeof(UConvertTo));
                converted = true;
            }

            return converted;
        }

        public static bool TryConvert(Type convertFrom, object from, Type convertTo, out object to)
        {
            to = null;
            bool converted = false;

            TypeConverter converter = TypeDescriptor.GetConverter(convertTo);

            if (converter.CanConvertFrom(convertFrom))
            {
                to = converter.ConvertFrom(from);
                converted = true;
            }

            return converted;
        }

        public static TConverFrom ConvertReferenceType<TConverFrom, UConvertTo>(UConvertTo value)
        {
            if (value == null)
            {
                return default(TConverFrom);
            }
            else if (typeof(TConverFrom).IsAssignableFrom(value.GetType()) == true)
            {
                return (TConverFrom)((object)value);
            }

            return default(TConverFrom);
        }

        public static TConverFrom ConvertValueType<TConverFrom, UConvertTo>(UConvertTo value)
        {
            IConvertible convertible = value as IConvertible;

            if (convertible != null)
            {
                return (TConverFrom)System.Convert.ChangeType(convertible, typeof(TConverFrom));
            }

            TypeConverter converter = TypeDescriptor.GetConverter(value);

            if (converter.CanConvertTo(typeof(TConverFrom)))
            {
                return (TConverFrom)converter.ConvertTo(value, typeof(TConverFrom));
            }

            if (value == null)
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture,
                    "Unable to cast type '{0}'.", typeof(TConverFrom).Name));
            }

            return default(TConverFrom);
        }

        public static TConverFrom? ConvertNullableType<TConverFrom, UConvertTo>(UConvertTo value) where TConverFrom : struct
        {
            if (value == null)
            {
                return null;
            }

            IConvertible convertible = value as IConvertible;

            if (convertible != null)
            {
                return (TConverFrom)System.Convert.ChangeType(convertible, typeof(TConverFrom));
            }

            TypeConverter converter = TypeDescriptor.GetConverter(value);

            if (converter.CanConvertTo(typeof(TConverFrom)))
            {
                return (TConverFrom)converter.ConvertTo(value, typeof(TConverFrom));
            }

            return new TConverFrom?((TConverFrom)((object)value));
        }
        #endregion

        public static string WrapASCIIText(int fontNumber, string text, float maxLineWidth)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = string.Empty;
            }

            string[] words = text.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;
            float spaceWidth = ASCIIFont.GetFixed(fontNumber).GetWidth(" ");

            foreach (string word in words)
            {
                Vector2 size = new Vector2(ASCIIFont.GetFixed(fontNumber).GetWidth(word), ASCIIFont.GetFixed(fontNumber).Height);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }

        public static string ClipASCIIText(int fontNumber, string text, int width, int pixelBuffer)
        {
            int charIndex = text.Length;
            string textToWrite = text.Substring(0, charIndex);

            Vector2 fontDimensions = new Vector2(ASCIIFont.GetFixed(fontNumber).GetWidth(textToWrite), ASCIIFont.GetFixed(fontNumber).Height);

            while (fontDimensions.X > width - (pixelBuffer * 2))
            {
                charIndex--;
                textToWrite = text.Substring(0, charIndex);
                fontDimensions = new Vector2(ASCIIFont.GetFixed(fontNumber).GetWidth(textToWrite), ASCIIFont.GetFixed(fontNumber).Height);
            }

            return textToWrite;
        }

        public static bool InRange(IPoint2D from, IPoint2D to, int range)
        {
            return (from.X >= (to.X - range)) && (from.X <= (to.X + range)) && (from.Y >= (to.Y - range)) && (from.Y <= (to.Y + range));
        }

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
        private static char[] m_hexDigits = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

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
                chars[i * 2] = m_hexDigits[b >> 4];
                chars[i * 2 + 1] = m_hexDigits[b & 0xF];
            }
            return new string(chars);
        }

        static byte hexDigitToByte(char c)
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
                case 'A': return (byte)10;
                case 'B': return (byte)11;
                case 'C': return (byte)12;
                case 'D': return (byte)13;
                case 'E': return (byte)14;
                case 'F': return (byte)15;
            }
            return (byte)0;
        }

        public static uint UintFromColor(Color color)
        {
            return (uint)((color.A << 24) | (color.B << 16) | (color.G << 8) | (color.R));
        }

        public static Color ColorFromHexString(string hex)
        {
            if (hex.Length == 8)
            {
                int a = (hexDigitToByte(hex[0]) << 4) + hexDigitToByte(hex[1]);
                int r = (hexDigitToByte(hex[2]) << 4) + hexDigitToByte(hex[3]);
                int g = (hexDigitToByte(hex[4]) << 4) + hexDigitToByte(hex[5]);
                int b = (hexDigitToByte(hex[6]) << 4) + hexDigitToByte(hex[7]);
                return new Color((byte)r, (byte)g, (byte)b, (byte)a);
            }
            else if (hex.Length == 6)
            {
                int r = (hexDigitToByte(hex[0]) << 4) + hexDigitToByte(hex[1]);
                int g = (hexDigitToByte(hex[2]) << 4) + hexDigitToByte(hex[3]);
                int b = (hexDigitToByte(hex[4]) << 4) + hexDigitToByte(hex[5]);
                return new Color((byte)r, (byte)g, (byte)b);
            }
            else
                return Color.Black;
        }
        #endregion

        // Version string.
        static string m_versionString;
        public static string VersionString
        {
            get
            {
                if (m_versionString == null)
                {
                    Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    DateTime d = new DateTime(v.Build * TimeSpan.TicksPerDay).AddYears(1999).AddDays(-1);
                    m_versionString = string.Format("UltimaXNA PreAlpha v{0}.{1} ({2})", v.Major, v.Minor, String.Format("{0:MMMM d, yyyy}", d));
                }
                return m_versionString;
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

        public static bool ToggleBoolean(bool b)
        {
            if (b)
                return false;
            else
                return true;
        }

        public static void ToogleBoolean(ref bool b)
        {
            b = !b;
        }

        public static int IPAddress
        {
            get
            {
                byte[] iIPAdress = new byte[4] { 127, 0, 0, 1 };
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

        public static Direction DirectionFromPoints(Point from, Point to)
        {
            return DirectionFromVectors(new Vector2(from.X, from.Y), new Vector2(to.X, to.Y));
        }

        public static Direction DirectionFromVectors(Vector2 fromPosition, Vector2 toPosition)
        {
            double Angle = Math.Atan2(toPosition.Y - fromPosition.Y, toPosition.X - fromPosition.X);
            if (Angle < 0)
                Angle = Math.PI + (Math.PI + Angle);
            double piPerSegment = (Math.PI * 2f) / 8f;
            double segmentValue = (Math.PI * 2f) / 16f;
            int direction = int.MaxValue;

            for (int i = 0; i < 8; i++)
            {
                if (Angle >= segmentValue && Angle <= (segmentValue + piPerSegment))
                {
                    direction = i + 1;
                    break;
                }
                segmentValue += piPerSegment;
            }

            if (direction == int.MaxValue)
                direction = 0;

            direction = (direction >= 7) ? (direction - 7) : (direction + 1);

            return (Direction)direction;
        }

        static Random m_random;
        public static int RandomValue(int low, int high)
        {
            if (m_random == null)
                m_random = new Random();
            int rnd = m_random.Next(low, high + 1);
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

        public static Vector2 GetHueVector(int hue)
        {
            return GetHueVector(hue, false, false);
        }

        public static Vector2 GetHueVector(int hue, bool partial, bool transparent)
        {
            if (transparent)
                hue |= 0x4000;

            if (hue == 0)
                return new Vector2(0);

            int hueType = 0;

            if ((hue & 0x4000) != 0)
            {
                // transparant
                hueType |= 4;
            }
            else if ((hue & 0x8000) != 0 || partial) // partial hue
            {
                hueType |= 2;
            }
            else
            {
                hueType |= 1;
            }
            return new Vector2(hue & 0x0FFF, hueType);
        }

        public static int DistanceBetweenTwoPoints(Point p1, Point p2)
        {
            return Convert.ToInt32(Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)));
        }
    }
}
