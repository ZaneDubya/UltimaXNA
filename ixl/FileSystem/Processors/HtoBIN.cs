using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib;

namespace InterXLib.FileSystem.Processors
{
    class HtoBIN : AProcessor
    {
        public override bool ExcludeThisFileFromLPK(string filepath)
        {
            return false;
        }

        public override bool TryProcess(string filename, byte[] data, bool allow_compression_of_files, out ProcessedFile processed_file)
        {
            processed_file = null;

            // extension must be .h (header file).
            if (!InternalCheckExtension(filename, ".h"))
                return false;

            // must be a single array of numbers, separated by commas. We allow for hex (0x####) and base ten (#####) numbers.
            StringBuilder sb_data = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                char c = (char)data[i];
                if (c == '/' && (char)data[i + 1] == '/')
                {
                    // ignore the rest of the line.
                    bool newline = false;
                    while (!newline)
                    {
                        c = (char)data[i++];
                        if (c == '\n' || c == '\r')
                        {
                            newline = true;
                            i--;
                        }
                    }
                }
                else if (Char.IsNumber(c) || c == 'x')
                    sb_data.Append(c);
                else if (c == ',')
                    sb_data.Append(c);
            }
            if (sb_data.Length == 0)
                return false;
            if (sb_data[sb_data.Length - 1] == ',')
                sb_data.Remove(sb_data.Length - 1, 1);
            string[] all_numbers = sb_data.ToString().Split(',');
            List<byte> all_data = new List<byte>();
            for (int i = 0; i < all_numbers.Length; i++)
            {
                if (!InternalCanProcessThisNumber(all_numbers[i], all_data))
                    return false;
            }
            processed_file = new ProcessedFile(filename, all_data.ToArray(), true);
            return true;
        }

        private bool InternalCanProcessThisNumber(string number, List<byte> data)
        {
            number = number.ToLower();
            if (InternalHasHexPrefix(number))
            {
                int bytesize = (number.Length - 2) / 2;
                if (bytesize > 8)
                    return false;
                long value;
                if (!InternalProcessHexNumber(number.Substring(2, number.Length - 2), out value))
                    return false;
                for (int i = 0; i < bytesize; i++)
                {
                    data.Add((byte)(value & 0xFF));
                    value = value >> 8;
                }
                return true;
            }
            else
            {
                // interpret as bytes.
                int value = int.Parse(number);
                if (value < 0 || value > 255)
                    return false;
                data.Add((byte)value);
                return true;
            }
        }

        private bool InternalHasHexPrefix(string number)
        {
            return (number.Length > 2 && number.Substring(0, 2) == "0x");
        }

        char[] hex_digits = new char[16] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        private bool InternalProcessHexNumber(string number, out long value)
        {
            value = 0;
            for (int i = 0; i < number.Length; i++)
            {
                int index = Array.IndexOf(hex_digits, number[i]);
                if (index == -1)
                    return false;
                value = (value << 4) + index;
            }
            return true;
        }
    }
}