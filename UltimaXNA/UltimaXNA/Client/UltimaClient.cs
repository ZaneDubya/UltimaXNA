/***************************************************************************
 *   UltimaClient.cs
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
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Entities;
using UltimaXNA.UI;
using UltimaXNA.Network;
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.Network.Packets.Server;
using UltimaXNA.TileEngine;
using UltimaXNA.Extensions;
#endregion

namespace UltimaXNA.Client
{
    class UltimaClient
    {
        public static UltimaClientStatus Status { get; protected set; }

        static string _account, _password;
        public static void SetAccountPassword(string account, string password) { _account = account; _password = password; }
        static void clearAccountPassword() { _account = string.Empty; _password = string.Empty; }

        static ClientNetwork _ClientNetwork;
        public static bool IsConnected { get { return _ClientNetwork.IsConnected; } }

        static TileEngine.IWorld _worldService;
        static UILegacy.IUIManager _LegacyUI = null;

        static UltimaClient()
        {
            Status = UltimaClientStatus.Unconnected;
            _ClientNetwork = new ClientNetwork();
            registerPackets();
        }

        public static void Initialize(Game game)
        {
            _worldService = game.Services.GetService<TileEngine.IWorld>();
            _LegacyUI = game.Services.GetService<UILegacy.IUIManager>();
        }

        public static void Update(GameTime gameTime)
        {
            _ClientNetwork.Update();
        }

        private static void registerPackets()
        {
            PacketRegistry.Damage += receive_Damage;
            PacketRegistry.MobileStatusCompact += receive_StatusInfo;
            PacketRegistry.WorldItem += receive_WorldItem;
            PacketRegistry.LoginConfirm += receive_PlayerLocaleAndBody;
            PacketRegistry.AsciiMessage += receive_AsciiMessage;
            PacketRegistry.RemoveEntity += receive_DeleteObject;
            PacketRegistry.MobileUpdate += receive_MobileUpdate;
            PacketRegistry.MovementRejected += receive_MoveRej;
            PacketRegistry.MoveAcknowledged += receive_MoveAck;
            PacketRegistry.DragEffect += receive_DragItem;
            PacketRegistry.OpenContainer += receive_Container;
            PacketRegistry.ContainerContentUpdate += receive_AddSingleItemToContainer;
            PacketRegistry.LiftRejection += receive_RejectMoveItemRequest;
            PacketRegistry.ResurrectMenu += receive_ResurrectionMenu;
            PacketRegistry.MobileAttributes += receive_MobileAttributes;
            PacketRegistry.WornItem += receive_WornItem;
            PacketRegistry.Swing += receive_OnSwing;
            PacketRegistry.SkillsList += receive_SkillsList;
            PacketRegistry.ContainerContent += receive_AddMultipleItemsToContainer;
            PacketRegistry.PersonalLightLevel += receive_PersonalLightLevel;
            PacketRegistry.OverallLightLevel += receive_OverallLightLevel;
            PacketRegistry.PopupMessage += receive_PopupMessage;
            PacketRegistry.PlaySoundEffect += receive_PlaySoundEffect;
            PacketRegistry.LoginComplete += receive_LoginComplete;
            PacketRegistry.Time += receive_Time;
            PacketRegistry.Weather += receive_SetWeather;
            PacketRegistry.TargetCursor += receive_TargetCursor;
            PacketRegistry.PlayMusic += receive_PlayMusic;
            PacketRegistry.MobileAnimation += receive_MobileAnimation;
            PacketRegistry.GraphicalEffect1 += receive_GraphicEffect;
            PacketRegistry.WarMode += receive_WarMode;
            PacketRegistry.VendorBuyList += receive_OpenBuyWindow;
            PacketRegistry.NewSubserver += receive_NewSubserver;
            PacketRegistry.MobileMoving += receive_MobileMoving;
            PacketRegistry.MobileIncoming += receive_MobileIncoming;
            PacketRegistry.DisplayMenu += receive_DisplayMenu;
            PacketRegistry.LoginRejection += receive_LoginRejection;
            PacketRegistry.CharacterListUpdate += receive_CharacterListUpdate;
            PacketRegistry.OpenPaperdoll += receive_OpenPaperdoll;
            PacketRegistry.CorpseClothing += receive_CorpseClothing;
            PacketRegistry.ServerRelay += receive_ServerRelay;
            PacketRegistry.PlayerMove += receive_PlayerMove;
            PacketRegistry.RequestNameResponse += receive_RequestNameResponse;
            PacketRegistry.VendorSellList += receive_SellList;
            PacketRegistry.UpdateCurrentHealth += receive_UpdateHealth;
            PacketRegistry.UpdateCurrentMana += receive_UpdateMana;
            PacketRegistry.UpdateCurrentStamina += receive_UpdateStamina;
            PacketRegistry.OpenWebBrowser += receive_OpenWebBrowser;
            PacketRegistry.TipNoticeWindow += receive_TipNotice;
            PacketRegistry.ServerList += receive_ServerList;
            PacketRegistry.CharactersStartingLocations += receive_CharacterList;
            PacketRegistry.ChangeCombatant += receive_ChangeCombatant;
            PacketRegistry.UnicodeMessage += receive_UnicodeMessage;
            PacketRegistry.DeathAnimation += receive_DeathAnimation;
            PacketRegistry.DisplayGumpFast += receive_DisplayGumpFast;
            PacketRegistry.ObjectHelpResponse += receive_ObjectHelpResponse;
            PacketRegistry.SupportedFeatures += receive_EnableFeatures;
            PacketRegistry.QuestArrow += receive_QuestArrow;
            PacketRegistry.SeasonChange += receive_SeasonalInformation;
            PacketRegistry.VersionRequest += receive_VersionRequest;
            PacketRegistry.GeneralInfo += receive_GeneralInfo;
            PacketRegistry.HuedEffect += receive_HuedEffect;
            PacketRegistry.MessageLocalized += receive_CLILOCMessage;
            PacketRegistry.InvalidMapEnable += receive_InvalidMapEnable;
            PacketRegistry.ParticleEffect += receive_OnParticleEffect;
            PacketRegistry.GlobalQueueCount += receive_GlobalQueueCount;
            PacketRegistry.MessageLocalizedAffix += receive_MessageLocalizedAffix;
            PacketRegistry.Extended0x78 += receive_Extended0x78;
            PacketRegistry.MegaCliloc += receive_ObjectPropertyList;
            PacketRegistry.ToolTipRevision += receive_ToolTipRevision;
            PacketRegistry.CompressedGump += receive_CompressedGump;

            PacketRegistry.RegisterNetwork(_ClientNetwork);
        }

        public static bool Connect(string ipAddressOrHostName, int port)
        {
            bool success = _ClientNetwork.Connect(ipAddressOrHostName, port);
            if (success)
            {
                Status = UltimaClientStatus.LoginServer_Connecting;
                _ClientNetwork.Send(new SeedPacket(1, 6, 0, 6, 2));
            }
            return success;
        }

        public static void Disconnect()
        {
            if (_ClientNetwork.IsConnected)
                _ClientNetwork.Disconnect();
            Status = UltimaClientStatus.Unconnected;
            clearAccountPassword();
        }

        public static void Send(ISendPacket packet)
        {
            bool success = _ClientNetwork.Send(packet);
            if (!success)
            {
                Disconnect();
            }
        }











        private static void receive_AddMultipleItemsToContainer(IRecvPacket packet)
        {
            ContainerContentPacket p = (ContainerContentPacket)packet;
            foreach (ContentItem i in p.Items)
            {
                // Add the item...
                Item iObject = add_Item(i.Serial, i.ItemID, i.Hue, i.ContainerSerial, i.Amount);
                iObject.Item_InvX = i.X;
                iObject.Item_InvY = i.Y;
                // ... and add it the container contents of the container.
                ContainerItem iContainerObject = EntitiesCollection.GetObject<ContainerItem>(i.ContainerSerial, true);
                iContainerObject.Contents.AddItem(iObject);
            }
        }

        private static void receive_AddSingleItemToContainer(IRecvPacket packet)
        {
            ContainerContentUpdatePacket p = (ContainerContentUpdatePacket)packet;

            // Add the item...
            Item iObject = add_Item(p.Serial, p.ItemId, p.Hue, p.ContainerSerial, p.Amount);
            iObject.Item_InvX = p.X;
            iObject.Item_InvY = p.Y;
            // ... and add it the container contents of the container.
            ContainerItem iContainerObject = EntitiesCollection.GetObject<ContainerItem>(p.ContainerSerial, true);
            if (iContainerObject != null)
                iContainerObject.Contents.AddItem(iObject);
            else
            {
                // Special case for game boards... the server will sometimes send us game pieces for a game board before it sends 
                // the game board! Right now, I am discarding these messages, it might be better to queue them up for when the game
                // board actually exists.
                // Let's throw an exception if anything other than a gameboard is ever sent to us.
                // if (iObject.ItemData.Name != "game piece")
                throw new Exception("Item {" + iObject.ToString() + "} received before containing object received.");
            }
        }

        private static void receive_CharacterListUpdate(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_CLILOCMessage(IRecvPacket packet)
        {
            MessageLocalizedPacket p = (MessageLocalizedPacket)packet;

            string iCliLoc = constructCliLoc(Data.StringList.Table[p.CliLocNumber].ToString(), p.Arguements);
            receive_TextMessage(p.MessageType, iCliLoc, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private static void receive_ChangeCombatant(IRecvPacket packet)
        {
            ChangeCombatantPacket p = (ChangeCombatantPacket)packet;
            if (p.Serial > 0x00000000)
                GameState.LastTarget = p.Serial;
        }

        private static void receive_CharacterList(IRecvPacket packet)
        {
            byte[] iIPAdress = new byte[4] { 127, 0, 0, 1 };
            int iAddress = BitConverter.ToInt32(iIPAdress, 0);
            CharacterCityListPacket p = (CharacterCityListPacket)packet;

            for (int i = 0; i < 6; i++)
            {
                if (p.Characters[i].Name != string.Empty)
                {
                    Send(new LoginCharacterPacket(p.Characters[i].Name, 0, iAddress));
                    return;
                }
            }

            Disconnect();
            UserInterface.Reset();
            UserInterface.ErrorPopup_Modal("No characters in this account. UltimaXNA does not support character creation yet.");
        }

        private static void receive_CompressedGump(IRecvPacket packet)
        {
            CompressedGumpPacket p = (CompressedGumpPacket)packet;
            if (p.HasData)
            {
                string[] gumpPieces = interpretGumpPieces(p.GumpData);
                _LegacyUI.AddGump(p.Serial, p.GumpID, gumpPieces, p.TextLines, p.X, p.Y);
            }
        }

        private static void receive_Container(IRecvPacket packet)
        {
            OpenContainerPacket p = (OpenContainerPacket)packet;

            // We can safely ignore 0x30 - this is the buy window.
            if (p.GumpId == 0x30)
                return;

            // Only try to open a container of type Container. Note that GameObjects can
            // have container objects and will expose them when called through GetContainerObject(int)
            // instead of GetObject(int).
            ContainerItem o = EntitiesCollection.GetObject<ContainerItem>(p.Serial, false);
            if (o is Item)
            {
                UserInterface.Container_Open(o, p.GumpId);
            }
            else
            {
                throw (new Exception("This Object has no support for a container object!"));
            }
        }

        private static void receive_CorpseClothing(IRecvPacket packet)
        {
            CorpseClothingPacket p = (CorpseClothingPacket)packet;
            Corpse e = EntitiesCollection.GetObject<Corpse>(p.CorpseSerial, false);
            e.LoadCorpseClothing(p.Items);
        }

        private static void receive_Damage(IRecvPacket packet)
        {
            DamagePacket p = (DamagePacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            UI.UIHelper.Chat_AddLine(string.Format("{0} takes {1} damage!", u.Name, p.Damage));
        }

        private static void receive_DeathAnimation(IRecvPacket packet)
        {
            DeathAnimationPacket p = (DeathAnimationPacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.PlayerSerial, false);
            Corpse c = EntitiesCollection.GetObject<Corpse>(p.CorpseSerial, false);
            c.Movement.Facing = u.Movement.Facing;
            c.MobileSerial = p.PlayerSerial;
            c.DeathAnimation();
        }

        private static void receive_DeleteObject(IRecvPacket packet)
        {
            RemoveEntityPacket p = (RemoveEntityPacket)packet;
            EntitiesCollection.RemoveObject(p.Serial);
        }

        private static void receive_DragItem(IRecvPacket packet)
        {
            DragEffectPacket p = (DragEffectPacket)packet;
            // This is sent by the server to display an item being dragged from one place to another.
            // Note that this does not actually move the item, it just displays an animation.

            // bool iSourceGround = false;
            // bool iDestGround = false;
            if (p.SourceContainer == 0xFFFFFFFF)
            {
                // iSourceGround = true;
            }

            if (p.DestContainer == 0xFFFFFFFF)
            {
                // iDestGround = true;
            }
            announce_UnhandledPacket(packet);
        }

        private static void receive_DisplayGumpFast(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_DisplayMenu(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_EnableFeatures(IRecvPacket packet)
        {
            SupportedFeaturesPacket p = (SupportedFeaturesPacket)packet;
            announce_UnhandledPacket(packet);
        }

        private static void receive_Extended0x78(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_GeneralInfo(IRecvPacket packet)
        {
            GeneralInfoPacket p = (GeneralInfoPacket)packet;
            switch (p.Subcommand)
            {
                case 0x06: // party system
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x08: // Set cursor color / set map
                    // p.MapID_MapID;
                    // 0 = Felucca, cursor not colored / BRITANNIA map
                    // 1 = Trammel, cursor colored gold / BRITANNIA map
                    // 2 = (switch to) ILSHENAR map
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x14: // return context menu
                    parseContextMenu(p.ContextMenu);
                    break;
                case 0x18: // Number of maps
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x1D: // House revision
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x04: // Close generic gump
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x19: // Extended stats: http://docs.polserver.com/packets/index.php?Packet=0xBF
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                default:
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
            }
        }

        private static void receive_GlobalQueueCount(IRecvPacket packet)
        {
            GlobalQueuePacket p = (GlobalQueuePacket)packet;
            UI.UIHelper.Chat_AddLine("System: There are currently " + p.Count + " available calls in the global queue.");
        }

        private static void receive_GraphicEffect(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_HuedEffect(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_InvalidMapEnable(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_LoginComplete(IRecvPacket packet)
        {
            // This packet is just one byte, the opcode.
            // Congrats, login complete!

            // We want to make sure we have the client object before we load the world.
            // If we don't, just set the status to login complete, which will then
            // load the world when we finally receive our client object.
            if (EntitiesCollection.MySerial != 0)
                Status = UltimaClientStatus.WorldServer_InWorld;
            else
                Status = UltimaClientStatus.WorldServer_LoginComplete;
        }

        private static void receive_LoginRejection(IRecvPacket packet)
        {
            Disconnect();
            LoginRejectionPacket p = (LoginRejectionPacket)packet;
            UserInterface.Reset();
            UserInterface.ErrorPopup_Modal(p.Reason);
        }

        private static void receive_MessageLocalizedAffix(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_MobileAttributes(IRecvPacket packet)
        {
            MobileAttributesPacket p = (MobileAttributesPacket)packet;
            announce_UnhandledPacket(packet);
        }

        private static void receive_MobileAnimation(IRecvPacket packet)
        {
            MobileAnimationPacket p = (MobileAnimationPacket)packet;

            Mobile iObject = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            iObject.Animation(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        private static void receive_MobileIncoming(IRecvPacket packet)
        {
            MobileIncomingPacket p = (MobileIncomingPacket)packet;
            Mobile mobile = EntitiesCollection.GetObject<Mobile>(p.Serial, true);
            mobile.BodyID = p.BodyID;
            mobile.Hue = (int)p.Hue;
            mobile.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z, p.Direction);
            mobile.IsFemale = p.Flags.IsFemale;
            mobile.IsPoisoned = p.Flags.IsPoisoned;
            mobile.IsBlessed = p.Flags.IsBlessed;
            mobile.IsWarMode = p.Flags.IsWarMode;
            mobile.IsHidden = p.Flags.IsHidden;
            mobile.Notoriety = p.Notoriety;
            mobile.Notoriety = p.Notoriety;

            for (int i = 0; i < p.Equipment.Length; i++)
            {
                Item item = add_Item(p.Equipment[i].Serial, p.Equipment[i].GumpId, p.Equipment[i].Hue, 0, 0);
                mobile.equipment[p.Equipment[i].Layer] = item;
                if (item.PropertyList.Hash == 0)
                    Send(new QueryPropertiesPacket(item.Serial));
            }

            if (mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                Send(new RequestNamePacket(p.Serial));
            }
        }

        private static void receive_MobileMoving(IRecvPacket packet)
        {
            MobileMovingPacket p = (MobileMovingPacket)packet;

            Mobile mobile = EntitiesCollection.GetObject<Mobile>(p.Serial, true);
            mobile.BodyID = p.BodyID;
            mobile.IsFemale = p.Flags.IsFemale;
            mobile.IsPoisoned = p.Flags.IsPoisoned;
            mobile.IsBlessed = p.Flags.IsBlessed;
            mobile.IsWarMode = p.Flags.IsWarMode;
            mobile.IsHidden = p.Flags.IsHidden;
            mobile.Notoriety = p.Notoriety;
            // Issue 16 - Pet not showing at login - http://code.google.com/p/ultimaxna/issues/detail?id=16 - Smjert
            // Since no packet arrives to add your pet, when you move and your pet follows you the client crashes
            if (mobile.Movement.DrawPosition == null)
            {
                mobile.Movement.SetPositionInstant(p.X, p.Y, p.Z, p.Direction);
            }
            else if (mobile.Movement.DrawPosition.PositionV3 == new Vector3(0, 0, 0))
            {
                mobile.Movement.SetPositionInstant(p.X, p.Y, p.Z, p.Direction);
                // Issue 16 - End
            }
            else
            {
                mobile.Move(p.X, p.Y, p.Z, p.Direction);
            }
        }

        private static void receive_MobileUpdate(IRecvPacket packet)
        {
            MobileUpdatePacket p = (MobileUpdatePacket)packet;
            Mobile mobile = EntitiesCollection.GetObject<Mobile>(p.Serial, true);
            mobile.BodyID = p.BodyID;
            mobile.IsFemale = p.Flags.IsFemale;
            mobile.IsPoisoned = p.Flags.IsPoisoned;
            mobile.IsBlessed = p.Flags.IsBlessed;
            mobile.IsWarMode = p.Flags.IsWarMode;
            mobile.IsHidden = p.Flags.IsHidden;
            mobile.Hue = (int)p.Hue;
            mobile.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z, p.Direction);

            if (mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                Send(new RequestNamePacket(p.Serial));
            }
        }

        private static void receive_MoveAck(IRecvPacket packet)
        {
            MoveAcknowledgePacket p = (MoveAcknowledgePacket)packet;
            EntitiesCollection.GetPlayerObject().Movement.MoveEventAck(p.Sequence);
            ((Mobile)EntitiesCollection.GetPlayerObject()).Notoriety = p.Notoriety;
        }

        private static void receive_MoveRej(IRecvPacket packet)
        {
            MovementRejectPacket p = (MovementRejectPacket)packet;
            EntitiesCollection.GetPlayerObject().Movement.MoveEventRej(p.Sequence, p.X, p.Y, p.Z, p.Direction);
        }

        private static void receive_NewSubserver(IRecvPacket packet)
        {
            SubServerPacket p = (SubServerPacket)packet;
            announce_UnhandledPacket(packet);
        }

        private static void receive_ObjectHelpResponse(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_ObjectPropertyList(IRecvPacket packet)
        {
            ObjectPropertyListPacket p = (ObjectPropertyListPacket)packet;

            Entity iObject = EntitiesCollection.GetObject<Entity>(p.Serial, false);
            iObject.PropertyList.Hash = p.Hash;
            iObject.PropertyList.Clear();

            for (int i = 0; i < p.CliLocs.Count; i++)
            {
                string iCliLoc = Data.StringList.Table[p.CliLocs[i]].ToString();
                if (p.Arguements[i] == string.Empty)
                {
                    iObject.PropertyList.AddProperty(iCliLoc);
                }
                else
                {
                    iObject.PropertyList.AddProperty(constructCliLoc(iCliLoc, p.Arguements[i]));
                }
            }
        }

        private static void receive_OnParticleEffect(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_OnSwing(IRecvPacket packet)
        {
            SwingPacket p = (SwingPacket)packet;
            if (p.Attacker == EntitiesCollection.MySerial)
            {
                GameState.LastTarget = p.Defender;
            }
        }

        private static void receive_OpenBuyWindow(IRecvPacket packet)
        {
            VendorBuyListPacket p = (VendorBuyListPacket)packet;
            Item iObject = EntitiesCollection.GetObject<Item>(p.VendorPackSerial, false);
            if (iObject == null)
                return;
            UserInterface.Merchant_Open(iObject, 0);
        }

        private static void receive_OpenPaperdoll(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_OpenWebBrowser(IRecvPacket packet)
        {
            OpenWebBrowserPacket p = (OpenWebBrowserPacket)packet;
            System.Diagnostics.Process.Start("iexplore.exe", p.WebsiteUrl);
        }

        private static void receive_OverallLightLevel(IRecvPacket packet)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F

            OverallLightLevelPacket p = (OverallLightLevelPacket)packet;
            // Console.WriteLine("OverallLight: {0}", p.LightLevel);
            _worldService.OverallLightning = p.LightLevel;
        }

        private static void receive_PersonalLightLevel(IRecvPacket packet)
        {
            // int iCreatureID = reader.ReadInt();
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F

            PersonalLightLevelPacket p = (PersonalLightLevelPacket)packet;
            // Console.WriteLine("PersonalLight: {0}", p.LightLevel);
            _worldService.PersonalLightning = p.LightLevel;
        }

        private static void receive_PlayerLocaleAndBody(IRecvPacket packet)
        {
            LoginConfirmPacket p = (LoginConfirmPacket)packet;

            // When loading the player object, we must load the serial before the object.
            EntitiesCollection.MySerial = p.Serial;
            PlayerMobile iPlayer = EntitiesCollection.GetObject<PlayerMobile>(p.Serial, true);
            iPlayer.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z, p.Direction);
            // iPlayer.SetFacing(p.Direction);

            // We want to make sure we have the client object before we load the world...
            if (Status == UltimaClientStatus.WorldServer_LoginComplete)
                Status = UltimaClientStatus.WorldServer_InWorld;
        }

        private static void receive_PlayerMove(IRecvPacket packet)
        {
            PlayerMovePacket p = (PlayerMovePacket)packet;
            announce_UnhandledPacket(packet);
        }

        private static void receive_PlayMusic(IRecvPacket packet)
        {
            PlayMusicPacket p = (PlayMusicPacket)packet;
            // System.Console.WriteLine ( "Play music, id={0}", p.MusicID );
            Data.Music.PlayMusic(p.MusicID);
        }

        private static void receive_PlaySoundEffect(IRecvPacket packet)
        {
            PlaySoundEffectPacket p = (PlaySoundEffectPacket)packet;
            Data.Sounds.PlaySound(p.SoundModel);
        }

        private static void receive_PopupMessage(IRecvPacket packet)
        {
            PopupMessagePacket p = (PopupMessagePacket)packet;
            announce_UnhandledPacket(packet);
        }

        private static void receive_QuestArrow(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_RejectMoveItemRequest(IRecvPacket packet)
        {
            LiftRejectionPacket p = (LiftRejectionPacket)packet;
            UI.UIHelper.Chat_AddLine("Could not pick up item: " + p.ErrorMessage);
        }

        private static void receive_RequestNameResponse(IRecvPacket packet)
        {
            RequestNameResponsePacket p = (RequestNameResponsePacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            u.Name = p.MobileName;
        }

        private static void receive_ResurrectionMenu(IRecvPacket packet)
        {
            // int iAction = reader.ReadByte();
            // 0: Server sent
            // 1: Resurrect
            // 2: Ghost
            // The only use on OSI for this packet is now sending "2C02" for the "You Are Dead" screen upon character death.
            announce_UnhandledPacket(packet);
        }

        private static void receive_SeasonalInformation(IRecvPacket packet)
        {
            // Unhandled!!! If iSeason2 = 1, then this is a season change.
            // If season change, then iSeason1 = (0=spring, 1=summer, 2=fall, 3=winter, 4 = desolation)
            announce_UnhandledPacket(packet);
        }

        private static void receive_SellList(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_ServerList(IRecvPacket packet)
        {
            ServerListPacket p = (ServerListPacket)packet;
            Send(new SelectServerPacket(p.Servers[0].Index));
        }

        private static void receive_SetWeather(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_ServerRelay(IRecvPacket packet)
        {
            ServerRelayPacket p = (ServerRelayPacket)packet;
            // Normally, upon receiving this packet you would disconnect and
            // log in to the specified server. Since we are using RunUO, we don't
            // actually need to do this.
            _ClientNetwork.IsDecompressionEnabled = true;
            Send(new GameLoginPacket(p.AccountId, _account, _password));
            clearAccountPassword();
        }

        private static void receive_SkillsList(IRecvPacket packet)
        {
            int iPacketType = 0; // reader.ReadByte();
            // 0x00: Full List of skills
            // 0xFF: Single skill update
            // 0x02: Full List of skills with skill cap for each skill
            // 0xDF: Single skill update with skill cap for skill

            switch (iPacketType)
            {
                case 0x00: //Full List of skills
                    announce_UnhandledPacket(packet, "0x00");
                    break;
                case 0xFF: // single skill update
                    announce_UnhandledPacket(packet, "0xFF");
                    break;
                case 0x02: // full List of skills with skill cap for each skill
                    announce_UnhandledPacket(packet, "0x02");
                    break;
                case 0xDF: // Single skill update with skill cap for skill
                    announce_UnhandledPacket(packet, "0xDF");
                    break;
                default:
                    announce_UnhandledPacket(packet, "Unknown subcommand " + iPacketType);
                    break;
            }
        }

        private static void receive_AsciiMessage(IRecvPacket packet)
        {
            AsciiMessagePacket p = (AsciiMessagePacket)packet;
            receive_TextMessage(p.MsgType, p.Text, p.Hue, p.Font, p.Serial, p.Name1);
        }

        private static void receive_StatusInfo(IRecvPacket packet)
        {
            MobileStatusCompactPacket p = (MobileStatusCompactPacket)packet;

            if (p.StatusType >= 6)
            {
                throw (new Exception("KR Status not handled."));
            }

            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            u.Name = p.PlayerName;
            u.Health.Update(p.CurrentHealth, p.MaxHealth);
            u.Stamina.Update(p.CurrentStamina, p.MaxStamina);
            u.Mana.Update(p.CurrentMana, p.MaxMana);
            //  other stuff unhandled !!!
        }

        private static void receive_TargetCursor(IRecvPacket packet)
        {
            TargetCursorPacket p = (TargetCursorPacket)packet;
            GameState.MouseTargeting(p.CursorID, p.CommandType);
        }

        private static void receive_Time(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_TipNotice(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_ToolTipRevision(IRecvPacket packet)
        {
            ObjectPropertyListUpdatePacket p = (ObjectPropertyListUpdatePacket)packet;
            Entity iObject = EntitiesCollection.GetObject<Entity>(p.Serial, false);
            if (iObject != null)
            {
                if (iObject.PropertyList.Hash != p.RevisionHash)
                {
                    Send(new QueryPropertiesPacket(p.Serial));
                }
            }
        }

        private static void receive_UnicodeMessage(IRecvPacket packet)
        {
            UnicodeMessagePacket p = (UnicodeMessagePacket)packet;
            receive_TextMessage(p.MsgType, p.SpokenText, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private static void receive_UpdateHealth(IRecvPacket packet)
        {
            UpdateHealthPacket p = (UpdateHealthPacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            u.Health.Update(p.Current, p.Max);
        }

        private static void receive_UpdateMana(IRecvPacket packet)
        {
            UpdateManaPacket p = (UpdateManaPacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            u.Mana.Update(p.Current, p.Max);
        }

        private static void receive_UpdateStamina(IRecvPacket packet)
        {
            UpdateStaminaPacket p = (UpdateStaminaPacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            u.Stamina.Update(p.Current, p.Max);
        }

        private static void receive_VersionRequest(IRecvPacket packet)
        {
            // Automatically respond.
            Send(new ClientVersionPacket("6.0.6.2"));
        }

        private static void receive_WarMode(IRecvPacket packet)
        {
            WarModePacket p = (WarModePacket)packet;
            GameState.WarMode = p.WarMode;
        }

        private static void receive_WorldItem(IRecvPacket packet)
        {
            WorldItemPacket p = (WorldItemPacket)packet;
            // Now create the GameObject.
            // If the iItemID < 0x4000, this is a regular game object.
            // If the iItemID >= 0x4000, then this is a multiobject.
            if (p.ItemID <= 0x4000)
            {
                Item item = add_Item(p.Serial, p.ItemID, p.Hue, unchecked((int)0xFFFFFFFF), p.StackAmount);
                item.Movement.SetPositionInstant(p.X, p.Y, p.Z, p.Direction);
            }
            else
            {
                // create a multi object. Unhandled !!!
                announce_UnhandledPacket(packet);
            }
        }

        private static void receive_WornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, 0, 0);
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.ParentSerial, false);
            u.equipment[p.Layer] = item;
            if (item.PropertyList.Hash == 0)
                Send(new QueryPropertiesPacket(item.Serial));
        }






        private static void announce_UnhandledPacket(IRecvPacket packet)
        {
            UI.UIHelper.Chat_AddLine("DEBUG: Unhandled " + packet.Name + ". <" + packet.Id + ">");
        }

        private static void announce_UnhandledPacket(IRecvPacket packet, string addendum)
        {
            UI.UIHelper.Chat_AddLine("DEBUG: Unhandled " + packet.Name + ". <" + packet.Id + ">" + " " + addendum);
        }

        private static void announce_Packet(IRecvPacket packet)
        {
            // UI.UIHelper.Chat_AddLine("DEBUG: Recv'd " + packet.Name + ". <" + packet.Id + ">");
        }

        private static void receive_TextMessage(MessageType msgType, string text, int hue, int font, Serial serial, string speakerName)
        {
            Overhead overhead;
            switch (msgType)
            {
                case MessageType.Regular:
                    overhead = EntitiesCollection.AddOverhead(msgType, serial, text, font, hue);
                    if (overhead != null)
                    {
                        overhead.SpeakerName = speakerName;
                    }
                    else
                    {
                        UI.UIHelper.Chat_AddLine(string.Format("{0} <{1}>", text, hue));
                    }
                    break;
                case MessageType.System:
                    UI.UIHelper.Chat_AddLine(string.Format("<SYSTEM> {0} <{1}>", text, hue));
                    break;
                case MessageType.Emote:
                    UI.UIHelper.Chat_AddLine(string.Format("<EMOTE> {0} <{1}>", text, hue));
                    break;
                case MessageType.Label:
                    if (serial.IsValid)
                    {
                        overhead = EntitiesCollection.AddOverhead(msgType, serial, text, font, hue);
                        overhead.SpeakerName = speakerName;
                        // Labels that are longer than the current name should be set as the name
                        if (serial.IsMobile)
                        {
                            Mobile m = EntitiesCollection.GetObject<Mobile>(serial, false);
                            if (m.Name.Length < text.Length)
                                m.Name = text;
                        }
                    }
                    else
                    {
                        UI.UIHelper.Chat_AddLine(string.Format("<LABEL> {0} <{1}>", text, hue));
                    }
                    break;
                case MessageType.Focus: // on player?
                    UI.UIHelper.Chat_AddLine(string.Format("<FOCUS> {0} <{1}>", text, hue));
                    break;
                case MessageType.Whisper:
                    UI.UIHelper.Chat_AddLine(string.Format("<WHISPER> {0} <{1}>", text, hue));
                    break;
                case MessageType.Yell:
                    UI.UIHelper.Chat_AddLine(string.Format("<YELL> {0} <{1}>", text, hue));
                    break;
                case MessageType.Spell:
                    UI.UIHelper.Chat_AddLine(string.Format("<SPELL> {0} <{1}>", text, hue));
                    break;
                case MessageType.UIld:
                    UI.UIHelper.Chat_AddLine(string.Format("<UILD> {0} <{1}>", text, hue));
                    break;
                case MessageType.Alliance:
                    UI.UIHelper.Chat_AddLine(string.Format("<ALLIANCE> {0} <{1}>", text, hue));
                    break;
                case MessageType.Command:
                    UI.UIHelper.Chat_AddLine(string.Format("<COMMAND> {0} <{1}>", text, hue));
                    break;
                default:
                    UI.UIHelper.Chat_AddLine("ERROR UNKNOWN COMMAND:" + msgType.ToString());
                    break;
            }
        }

        private static void parseContextMenu(ContextMenuNew context)
        {
            if (context.HasContextMenu)
            {
                if (context.CanBuy)
                {
                    Send(new ContextMenuResponsePacket(context.Serial, (short)context.ContextEntry("Buy").ResponseCode));
                }
            }
            else
            {
                // no context menu entries are handled. Send a double click.
                Send(new DoubleClickPacket(context.Serial));
            }
        }

        private static string[] interpretGumpPieces(string gumpData)
        {
            List<string> i = new List<string>(); ;
            bool isData = true;
            int dataIndex = 0;
            while (isData)
            {
                if (gumpData.Substring(dataIndex) == "\0")
                {
                    isData = false;
                }
                else
                {
                    int begin = gumpData.IndexOf("{ ", dataIndex);
                    int end = gumpData.IndexOf(" }", dataIndex + 1);
                    if ((begin != -1) && (end != -1))
                    {
                        string sub = gumpData.Substring(begin + 2, end - begin - 2);
                        // iConstruct = iConstruct.Substring(0, iBeginReplace) + iArgs[i] + iConstruct.Substring(iEndReplace + 1, iConstruct.Length - iEndReplace - 1);
                        i.Add(sub);
                        dataIndex += end - begin + 2;
                    }
                    else
                    {
                        isData = false;
                    }
                }
            }

            return i.ToArray();
        }

        private static string constructCliLoc(string nBase, string nArgs)
        {
            string[] iArgs = nArgs.Split('\t');
            for (int i = 0; i < iArgs.Length; i++)
            {
                if ((iArgs[i].Length > 0) && (iArgs[i].Substring(0, 1) == "#"))
                {
                    int clilocID = Convert.ToInt32(iArgs[i].Substring(1));
                    iArgs[i] = Data.StringList.Table[clilocID].ToString();
                }
            }

            string iConstruct = nBase;
            for (int i = 0; i < iArgs.Length; i++)
            {
                int iBeginReplace = iConstruct.IndexOf('~', 0);
                int iEndReplace = iConstruct.IndexOf('~', iBeginReplace + 1);
                if ((iBeginReplace != -1) && (iEndReplace != -1))
                {
                    iConstruct = iConstruct.Substring(0, iBeginReplace) + iArgs[i] + iConstruct.Substring(iEndReplace + 1, iConstruct.Length - iEndReplace - 1);
                }
                else
                {
                    iConstruct = nBase;
                }

            }
            return iConstruct;
        }

        private static Item add_Item(Serial serial, int itemID, int nHue, int containerSerial, int amount)
        {
            Item item;
            if (itemID == 0x2006)
            {
                // special case for corpses.
                item = EntitiesCollection.GetObject<Corpse>((int)serial, true);
                Network.Packets.Server.ContainerContentPacket.NextContainerContentsIsPre6017 = true;
            }
            else
            {
                if (Data.TileData.ItemData[itemID].Container)
                    item = EntitiesCollection.GetObject<ContainerItem>((int)serial, true);
                else
                    item = EntitiesCollection.GetObject<Item>((int)serial, true);
            }
            item.Amount = amount;
            item.ItemID = itemID;
            item.Hue = nHue;
            item.Item_ContainedWithinSerial = containerSerial;

            return item;
        }
    }
}
