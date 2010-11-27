/***************************************************************************
 *   GeneralInfoPacket.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace UltimaXNA.Client.Packets.Server
{
    public class GeneralInfoPacket : RecvPacket
    {
        readonly short _subcommand;
        public short Subcommand
        {
            get { return _subcommand; }
        }

        HouseRevisionState _revisionState;
        public HouseRevisionState HouseRevisionState
        {
            get { return _revisionState; }
        }

        ContextMenu _contextmenu;
        public ContextMenu ContextMenu
        {
            get { return _contextmenu; }
        }

        byte _mapid;
        public byte MapID
        {
            get { return _mapid; }
        }

        int _mapCount;
        public int MapCount
        {
            get { return _mapCount; }
        }

        public GeneralInfoPacket(PacketReader reader)
            : base(0xBF, "General Information")
        {
            this._subcommand = reader.ReadInt16();

            switch (this._subcommand)
            {
                case 0x06:
                    // part system, not implemented.
                    break;
                case 0x8: // Set cursor color / set map
                    _mapid = reader.ReadByte();
                    break;
                case 0x14: // return context menu
                    receiveContextMenu(reader);
                    break;
                case 0x18: // Number of maps
                    receiveMapDiffManifest(reader);
                    break;
                case 0x1D: // House revision
                    receiveHouseRevisionState(reader);
                    break;
                case 0x04: // Close generic gump
                    // !!! Not implemented yet.
                    break;
                default:
                    // do nothing. This unhandled subcommand will raise an error in UltimaClient.cs.
                    break;
            }
        }

        void receiveHouseRevisionState(PacketReader reader)
        {
            Serial s = reader.ReadInt32();
            int hash = reader.ReadInt32();
            _revisionState = new HouseRevisionState(s, hash);
        }

        void receiveMapDiffManifest(PacketReader reader)
        {
            _mapCount = reader.ReadInt32();
            for (int i = 0; i < _mapCount; i++)
            {
                int mapPatches = reader.ReadInt32();
                int staticPatches = reader.ReadInt32();
            }
        }

        void receiveContextMenu(PacketReader reader)
        {
            reader.ReadByte(); // unknown (0x00)
            int iSubCommand = reader.ReadByte(); // 0x01 for 2D, 0x02 for KR
            _contextmenu = new ContextMenu(reader.ReadInt32());
            int iNumEntriesInContext = reader.ReadByte();

            for (int i = 0; i < iNumEntriesInContext; i++)
            {
                int iUniqueID = reader.ReadUInt16();
                int iClilocID = reader.ReadUInt16() + 3000000;
                int iFlags = reader.ReadUInt16(); // 0x00=enabled, 0x01=disabled, 0x02=arrow, 0x20 = color
                int iColor = 0;
                if ((iFlags & 0x20) == 0x20)
                {
                    iColor = reader.ReadUInt16();
                }
                _contextmenu.AddItem(iUniqueID, iClilocID, iFlags, iColor);
            }
            _contextmenu.FinalizeMenu();
        }
    }
}
