﻿/***************************************************************************
 *   IDecompression.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
#endregion

namespace UltimaXNA.Network
{
    public interface IDecompression
    {
        void DecompressAll(ref byte[] src, int src_size, ref byte[] dest, ref int dest_size);
        bool DecompressOnePacket(ref byte[] src, int src_size, ref byte[] dest, ref int dest_size);
    }
}
