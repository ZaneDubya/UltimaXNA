/***************************************************************************
 *   HouseRevisionState.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Ultima.Data
{
    public class HouseRevisionState
    {
        public Serial Serial;
        public int Hash;

        public HouseRevisionState(Serial serial, int revisionHash)
        {
            Serial = serial;
            Hash = revisionHash;
        }
    }
}
