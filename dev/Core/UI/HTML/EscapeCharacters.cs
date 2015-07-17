/***************************************************************************
 *   EscapeCharacters.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace UltimaXNA.Core.UI.HTML
{
    class EscapeCharacters
    {
        public static List<Tuple<char, string>> Chars;

        static EscapeCharacters()
        {
            Chars = new List<Tuple<char, string>>();
            AddChar('<', "&lt;");
            AddChar('>', "&gt;");
        }

        static void AddChar(char ch, string str)
        {
            Chars.Add(new Tuple<char, string>(ch, str));
        }

        public static string ReplaceEscapeCharacters(string value)
        {
            for (int i = 0; i < Chars.Count; i++)
                value = value.Replace(Chars[i].Item2, Chars[i].Item1.ToString());
            return value;
        }

        public static bool TryMatchChar(char ch, out string escapedCharacter)
        {
            escapedCharacter = null;
            for (int i = 0; i < Chars.Count; i++)
            {
                if (Chars[i].Item1 == ch)
                {
                    escapedCharacter = Chars[i].Item2;
                    return true;
                }
            }
            return false;
        }

        public static bool TryFindEscapeCharacterBackwards(string text, int findStart, out int charLength)
        {
            charLength = 0;

            if (findStart >= text.Length)
                return false;

            if (text[findStart] == ';')
            {
                for (int i = 0; i < Chars.Count; i++)
                {
                    if (findStart - Chars[i].Item2.Length + 1 < 0)
                        continue;
                    string findEscaped = text.Substring(findStart - Chars[i].Item2.Length + 1, Chars[i].Item2.Length);
                    if (findEscaped == Chars[i].Item2)
                    {
                        charLength = Chars[i].Item2.Length;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
