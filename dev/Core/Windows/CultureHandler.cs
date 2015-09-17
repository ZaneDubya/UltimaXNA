/***************************************************************************
 *   CultureHandler.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System.Text;
using UltimaXNA.Core.Diagnostics.Tracing;

namespace UltimaXNA.Core.Windows
{
    static class CultureHandler
    {
        private static Encoding s_Encoding = null;

        public static void InvalidateEncoder()
        {
            s_Encoding = null;
        }

        public static char TranslateChar(char inputChar)
        {
            if (s_Encoding == null)
                s_Encoding = GetCurrentEncoding();
            char[] chars = s_Encoding.GetChars(new byte[] { (byte)inputChar });
            return chars[0];
        }

        private static Encoding GetCurrentEncoding()
        {
            Encoding encoding = Encoding.GetEncoding((int)NativeMethods.GetCurrentCodePage());

            Tracer.Debug("Keyboard: Using encoding {0} (Code page {1}).", encoding.EncodingName, encoding.CodePage);

            return encoding;
        }
    }
}
