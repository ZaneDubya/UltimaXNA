/***************************************************************************
 *   Enums.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Core.UI.HTML.Styles
{
    public enum Alignments
    {
        Default = 0,
        Left = 0,
        Center = 1,
        Right = 2
    }

    public enum Fonts
    {
        Default = 1,
        UnicodeBig = 0,
        UnicodeMedium = 1,
        UnicodeSmall = 2,
        /*AsciiFontsBegin = 3,
        AsciiThickOutlined = 3, // ascii 0
        AsciiShadowed = 4, // ascii 1
        AsciiThickShadowed = 5, // ascii 2
        AsciiOutlined = 6, // ascii 3
        AsciiScriptBig = 7, // ascii 4
        AsciiScriptItalic = 8, // ascii 5
        AsciiScript = 9, // ascii 6
        AsciiScriptSoft = 10, // ascii 7
        AsciiRunes = 11, // ascii 8
        AsciiMedium = 12, // ascii 9*/
    }

    public enum Layers
    {
        /// <summary>
        /// Default value. Elements render in order, as they appear in the document flow
        /// </summary>
        Default = 0,
        /// <summary>
        /// The element is positioned relative to its first positioned ancestor element. Does not effect document flow. Does effect document width/height.
        /// </summary>
        Background = 1
    }
}
