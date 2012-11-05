/***************************************************************************
 *   Characters.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Network;
using UltimaXNA.Network.Packets;
using UltimaXNA.Entity;
using UltimaXNA.Interface.Input;
using UltimaXNA.TileEngine;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaVars
{
    public static class Characters
    {
        static CharacterListEntry[] _characters;
        public static CharacterListEntry[] List { get { return _characters; } }
        public static int Length { get { return _characters.Length; } }

        static StartingLocation[] _locations;
        public static StartingLocation[] StartingLocations { get { return _locations; } }

        static int _updateValue = 0;
        public static int UpdateValue { get { return _updateValue; } }

        public static int FirstEmptySlot
        {
            get
            {
                for (int i = 0; i < _characters.Length; i++)
                {
                    if (_characters[i].Name == string.Empty)
                        return i;
                }
                return -1;
            }
        }

        public static void SetCharacterList(CharacterListEntry[] list)
        {
            _characters = list;
            _updateValue++;
        }

        public static void SetStartingLocations(StartingLocation[] list)
        {
            _locations = list;
            _updateValue++;
        }
    }
}
