/***************************************************************************
 *   IRecvPacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
    public interface IRecvPacket
    {
        int Id { get; }
        string Name { get; }
    }
}
