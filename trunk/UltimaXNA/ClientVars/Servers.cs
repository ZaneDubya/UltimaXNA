/***************************************************************************
 *   Servers.cs
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
using UltimaXNA.Core.Extensions;
using UltimaXNA.Interface.Input;
using UltimaXNA.Interface.TileEngine;
using UltimaXNA.UILegacy;

namespace UltimaXNA.ClientVars
{
    class Servers
    {
        static ServerListEntry[] _serverListPacket;
        public static ServerListEntry[] List { get { return _serverListPacket; } set { _serverListPacket = value; } }
    }
}
