/***************************************************************************
 *   GeneralInfoPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class GeneralInfoPacket : RecvPacket
    {
        readonly short m_subcommand;
        public short Subcommand
        {
            get { return m_subcommand; }
        }

        HouseRevisionState m_revisionState;
        public HouseRevisionState HouseRevisionState
        {
            get { return m_revisionState; }
        }

        public Serial Serial;
        StatLocks m_locks;
        public StatLocks StatisticLocks
        {
            get { return m_locks; }
        }

        public ContextMenu ContextMenu
        {
            get;
            private set;
        }

        public byte MapID
        {
            get;
            private set;
        }

        public int MapCount
        {
            get;
            private set;
        }

        internal SpellbookData Spellbook
        {
            get;
            private set;
        }

        public GeneralInfoPacket(PacketReader reader)
            : base(0xBF, "General Information")
        {
            m_subcommand = reader.ReadInt16();

            switch (m_subcommand)
            {
                case 0x04: // Close generic gump
                    // !!! Not implemented yet.
                    break;
                case 0x06:
                    // party system, not implemented.
                    break;
                case 0x8: // Set cursor color / set map
                    MapID = reader.ReadByte();
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
                case 0x19: // Extended stats
                    receiveExtendedStats(reader);
                    break;
                case 0x1B:
                    receiveSpellBookContents(reader);
                    break;
                case 0x21: // (AOS) Ability icon confirm.
                    // no data, just (bf 00 05 00 21)
                    break;
                default:
                    // do nothing. This unhandled subcommand will raise an error in UltimaClient.cs.
                    break;
            }
        }

        private void receiveSpellBookContents(PacketReader reader)
        {
            ushort unknown = reader.ReadUInt16(); // always 1
            Serial serial = (Serial)reader.ReadInt32();
            ushort itemID = reader.ReadUInt16();
            ushort spellbookType = reader.ReadUInt16(); // 1==regular, 101=necro, 201=paladin, 401=bushido, 501=ninjitsu, 601=spellweaving
            ulong spellBitfields = reader.ReadUInt64(); // first bit of first byte = spell #1, second bit of first byte = spell #2, first bit of second byte = spell #8, etc 

            Spellbook = new SpellbookData(serial, itemID, spellbookType, spellBitfields);
        }

        void receiveExtendedStats(PacketReader reader)
        {
            int clientFlag = reader.ReadByte(); // (0x2 for 2D client, 0x5 for KR client) 
            Serial = (Serial)reader.ReadInt32();
            byte unknown0 = reader.ReadByte(); // (always 0) 
            byte lockFlags = reader.ReadByte();
            // Lock flags = 00SSDDII ( in binary )
            //     00 = up
            //     01 = down
            //     10 = locked
            // FF = update mobile status animation ( KR only )
            if (lockFlags != 0xFF)
            {
                int strengthLock = (lockFlags >> 4) & 0x03;
                int dexterityLock = (lockFlags >> 2) & 0x03;
                int inteligenceLock = (lockFlags) & 0x03;
                m_locks = new StatLocks(strengthLock, dexterityLock, inteligenceLock);
            }

            if (clientFlag == 5)
            {
                Tracer.Warn("ClientFlags == 5 in GeneralInfoPacket ExtendedStats 0x19. This is not a KR client.");
                // If(Lock flags = 0xFF) //Update mobile status animation
                //  BYTE[1] Status // Unveryfied if lock flags == FF the locks will be handled here
                //  BYTE[1] unknown (0x00) 
                //  BYTE[1] Animation 
                //  BYTE[1] unknown (0x00) 
                //  BYTE[1] Frame 
                // else
                //  BYTE[1] unknown (0x00) 
                //  BYTE[4] unknown (0x00000000)
                // endif
            }
        }

        void receiveHouseRevisionState(PacketReader reader)
        {
            Serial s = reader.ReadInt32();
            int hash = reader.ReadInt32();
            m_revisionState = new HouseRevisionState(s, hash);
        }

        void receiveMapDiffManifest(PacketReader reader)
        {
            MapCount = reader.ReadInt32();
            for (int i = 0; i < MapCount; i++)
            {
                int mapPatches = reader.ReadInt32();
                int staticPatches = reader.ReadInt32();
            }
        }

        void receiveContextMenu(PacketReader reader)
        {
            reader.ReadByte(); // unknown (0x00)
            int iSubCommand = reader.ReadByte(); // 0x01 for 2D, 0x02 for KR
            ContextMenu = new ContextMenu(reader.ReadInt32());
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
                ContextMenu.AddItem(iUniqueID, iClilocID, iFlags, iColor);
            }
            ContextMenu.FinalizeMenu();
        }
    }
}
