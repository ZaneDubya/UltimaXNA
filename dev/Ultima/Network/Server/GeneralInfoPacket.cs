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
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class GeneralInfoPacket : RecvPacket
    {
        public readonly short Subcommand;
        #region others
        /// <summary>
        /// Subcommand 0x04: Close a generic gump.
        /// </summary>
        public int CloseGumpTypeID
        {
            get;
            private set;
        }
        public int CloseGumpButtonID
        {
            get;
            private set;
        }

        /// <summary>
        /// Subcommand 0x08: The index of the map the player is located within.
        /// </summary>
        public byte MapID
        {
            get;
            private set;
        }

        /// <summary>
        /// Subcommand 0x14: A context menu.
        /// </summary>
        public ContextMenuData ContextMenu
        {
            get;
            private set;
        }

        /// <summary>
        /// Subcommand 0x18: The count of map diffs that were received.
        /// </summary>
        public int MapDiffsCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Subcommand 0x1B: the contents of a spellbook.
        /// </summary>
        public SpellbookData Spellbook
        {
            get;
            private set;
        }

        /// <summary>
        /// Subcommand 0x19: the serial of the mobile which the extended stats must be applied to.
        /// </summary>
        public Serial ExtendedStatsSerial;
        public StatLocks ExtendedStatsLocks
        {
            get;
            private set;
        }

        /// <summary>
        /// Subcommand 0x1D: The revision hash of a custom house.
        /// </summary>
        public HouseRevisionState HouseRevisionState
        {
            get;
            private set;
        }
        #endregion

        #region partySystem
        public ushort partyMessageHue = 68;
        public string partyMessage = string.Empty;
        public string partyMessager = string.Empty;
        #endregion
        public GeneralInfoPacket(PacketReader reader)
            : base(0xBF, "General Information")
        {
            Subcommand = reader.ReadInt16();

            switch (Subcommand)
            {
                case 0x04: // Close generic gump
                    CloseGumpTypeID = reader.ReadInt32();
                    CloseGumpButtonID = reader.ReadInt32();
                    break;
                case 0x06:
                    receivePartySystem(reader);//party system incomplete
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

        private void receivePartySystem(PacketReader r)
        {
            UltimaGame m_Engine = ServiceRegistry.GetService<UltimaGame>();
            int num = r.ReadByte();
            switch (num)
            {
                case 1:
                    {
                        //pvSrc.ReturnName = "Party Member List";
                        int num2 = r.ReadByte();
                        Mobile[] mobileArray = new Mobile[num2];
                        for (int i = 0; i < num2; i++)
                        {
                            mobileArray[i] = WorldModel.Entities.GetObject<Mobile>((Serial)r.ReadInt32(), false);
                            mobileArray[i].QueryStats();
                        }
                        Party.State = PartyState.Joined;
                        Party.Members = mobileArray;

                        int num4 = (20 + Settings.UserInterface.WindowResolution.Height) - 50;
                        for (int j = 0; j < num2; j++)
                        {
                            if (!mobileArray[j].IsClientEntity)
                            {
                                if (mobileArray[j].StatusBar == null)
                                {
                                    mobileArray[j].OpenStatus(false);
                                    if (mobileArray[j].StatusBar != null)
                                    {
                                        //mobileArray[j].StatusBar.Gump.X = ((Engine.GameX + Engine.GameWidth) - 30) - mobileArray[j].StatusBar.Gump.Width;
                                        //mobileArray[j].StatusBar.Gump.Y = num4 - mobileArray[j].StatusBar.Gump.Height;
                                        //num4 -= mobileArray[j].StatusBar.Gump.Height + 5;
                                    }
                                }
                                else
                                {
                                    //num4 -= mobileArray[j].StatusBar.Gump.Height + 5;
                                }
                            }
                        }
                        return;
                    }
                case 2:
                    {
                        //pvSrc.ReturnName = "Remove Party Member";
                        int num6 = r.ReadByte();
                        int num7 = r.ReadInt32();
                        Mobile[] mobileArray2 = new Mobile[num6];
                        for (int k = 0; k < num6; k++)
                        {
                            mobileArray2[k] = WorldModel.Entities.GetObject<Mobile>((Serial)r.ReadInt32(), false);
                            mobileArray2[k].QueryStats();
                        }
                        Party.State = PartyState.Joined;
                        Party.Members = mobileArray2;
                        return;
                    }
                case 3:
                case 4:
                    {
                        string str2;
                        //IHue hue;
                        //pvSrc.ReturnName = (num == 3) ? "Private Party Message" : "Public Party Message";
                        int serial = r.ReadInt32();
                        string str = r.ReadUnicodeString();
                        Mobile mobile = WorldModel.Entities.GetObject<Mobile>((Serial)serial, false);
                        if (((mobile == null) || ((str2 = mobile.Name) == null)) || ((str2 = str2.Trim()).Length <= 0))
                        {
                            str2 = "Someone";
                        }
                        if (str == "I'm stunned !!")
                        {
                            partyMessageHue = 34;//i need from option menu
                            if (mobile != null)
                            {
                                //Engine.Sounds.PlaySound(0x157, mobile.X, mobile.Y, mobile.Z);
                            }
                        }
                        else if (str.StartsWith("I stunned ") && str.EndsWith(" !!"))
                        {
                            partyMessageHue = 34;//i need from option menu
                            if (mobile != null)
                            {
                                //Engine.Sounds.PlaySound(0x1e1, mobile.X, mobile.Y, mobile.Z);
                            }
                        }
                        else if (str.StartsWith("Changing last target to "))
                        {
                            partyMessageHue = 53;//i need from option menu
                        }
                        else if (num == 3)//public party message
                        {
                            partyMessageHue = 58;//i need from option menu
                        }
                        else
                        {
                            partyMessageHue = 68;//i need from option menu
                        }
                        partyMessage = str;
                        partyMessager = str2;
                        //Engine.AddTextMessage(string.Format("<{0}{1}> {2}", (num == 3) ? "Whisper: " : "", str2, str), Engine.DefaultFont, hue);
                        return;
                    }
                case 7:
                    {
                        //pvSrc.ReturnName = "Party Invitation";
                        int num10 = r.ReadInt32();
                        Party.State = PartyState.Joining;
                        Party.Leader = WorldModel.Entities.GetObject<Mobile>((Serial)num10, false);
                        return;
                    }
            }
            partyMessage = "Unknown Party Message";
            //pvSrc.ReturnName = "Unknown Party Message";
            //pvSrc.Trace();
        }

        private void receiveSpellBookContents(PacketReader reader)
        {
            ushort unknown = reader.ReadUInt16(); // always 1
            Serial serial = (Serial)reader.ReadInt32();
            ushort itemID = reader.ReadUInt16();
            ushort spellbookType = reader.ReadUInt16(); // 1==regular, 101=necro, 201=paladin, 401=bushido, 501=ninjitsu, 601=spellweaving
            ulong spellBitfields = reader.ReadUInt32() + (((ulong)reader.ReadUInt32()) << 32); // first bit of first byte = spell #1, second bit of first byte = spell #2, first bit of second byte = spell #8, etc 

            Spellbook = new SpellbookData(serial, itemID, spellbookType, spellBitfields);
        }

        void receiveExtendedStats(PacketReader reader)
        {
            int clientFlag = reader.ReadByte(); // (0x2 for 2D client, 0x5 for KR client) 
            ExtendedStatsSerial = (Serial)reader.ReadInt32();
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
                ExtendedStatsLocks = new StatLocks(strengthLock, dexterityLock, inteligenceLock);
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
            HouseRevisionState = new HouseRevisionState(s, hash);
        }

        void receiveMapDiffManifest(PacketReader reader)
        {
            MapDiffsCount = reader.ReadInt32();
            for (int i = 0; i < MapDiffsCount; i++)
            {
                int mapPatches = reader.ReadInt32();
                int staticPatches = reader.ReadInt32();
            }
        }

        void receiveContextMenu(PacketReader reader)
        {
            reader.ReadByte(); // unknown, always 0x00
            int subcommand = reader.ReadByte(); // 0x01 for 2D, 0x02 for KR
            ContextMenu = new ContextMenuData(reader.ReadInt32());
            int contextMenuChoiceCount = reader.ReadByte();

            for (int i = 0; i < contextMenuChoiceCount; i++)
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
        }
    }
}
