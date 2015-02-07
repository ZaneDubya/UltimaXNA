/***************************************************************************
 *   PacketRegistry.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Network.Packets.Server;
#endregion

namespace UltimaXNA.Network
{
    public static class PacketRegistry
    {
        /// <summary>
        /// Make sure you've hooked all handlers before calling this function.
        /// </summary>
        /// <param name="network"></param>
        public static void RegisterNetwork(ClientNetwork network)
        {
            network.Register<DamagePacket>(0x0B, "Damage", 0x07, new TypedPacketReceiveHandler(Damage));
            network.Register<MobileStatusCompactPacket>(0x11, "Mobile Status Compact", -1, new TypedPacketReceiveHandler(MobileStatusCompact));
            network.Register<WorldItemPacket>(0x1A, "World Item", -1, new TypedPacketReceiveHandler(WorldItem));
            network.Register<LoginConfirmPacket>(0x1B, "Login Confirm", 37, new TypedPacketReceiveHandler(LoginConfirm));
            network.Register<AsciiMessagePacket>(0x1C, "Ascii Meessage", -1, new TypedPacketReceiveHandler(AsciiMessage));
            network.Register<RemoveEntityPacket>(0x1D, "Remove Entity", 5, new TypedPacketReceiveHandler(RemoveEntity));
            network.Register<MobileUpdatePacket>(0x20, "Mobile Update", 19, new TypedPacketReceiveHandler(MobileUpdate));
            network.Register<MovementRejectPacket>(0x21, "Movement Rejection", 8, new TypedPacketReceiveHandler(MovementRejected));
            network.Register<MoveAcknowledgePacket>(0x22, "Move Acknowledged", 3, new TypedPacketReceiveHandler(MoveAcknowledged));
            network.Register<DragEffectPacket>(0x23, "Drag Effect", 26, new TypedPacketReceiveHandler(DragEffect));
            network.Register<OpenContainerPacket>(0x24, "Open Container", 7, new TypedPacketReceiveHandler(OpenContainer));
            network.Register<ContainerContentUpdatePacket>(0x25, "Container Content Update", 21, new TypedPacketReceiveHandler(ContainerContentUpdate));
            network.Register<LiftRejectionPacket>(0x27, "Lift Rejection", 2, new TypedPacketReceiveHandler(LiftRejection));
            network.Register<ResurrectionMenuPacket>(0x2C, "Resurect menu", 2, new TypedPacketReceiveHandler(ResurrectMenu));
            network.Register<MobileAttributesPacket>(0x2D, "Mob Attributes", 17, new TypedPacketReceiveHandler(MobileAttributes));
            network.Register<WornItemPacket>(0x2E, "Worn Item", 15, new TypedPacketReceiveHandler(WornItem));
            network.Register<SwingPacket>(0x2F, "Swing", 10, new TypedPacketReceiveHandler(Swing));
            network.Register<SendSkillsPacket>(0x3A, "Skills list", -1, new TypedPacketReceiveHandler(SkillsList));
            network.Register<ContainerContentPacket>(0x3C, "Container Content", -1, new TypedPacketReceiveHandler(ContainerContent));
            network.Register<PersonalLightLevelPacket>(0x4E, "Personal Light Level", 6, new TypedPacketReceiveHandler(PersonalLightLevel));
            network.Register<OverallLightLevelPacket>(0x4F, "Overall Light Level", 2, new TypedPacketReceiveHandler(OverallLightLevel));
            network.Register<PopupMessagePacket>(0x53, "Popup Message", 2, new TypedPacketReceiveHandler(PopupMessage));
            network.Register<PlaySoundEffectPacket>(0x54, "Play Sound Effect", 12, new TypedPacketReceiveHandler(PlaySoundEffect));
            network.Register<LoginCompletePacket>(0x55, "Login Complete", 1, new TypedPacketReceiveHandler(LoginComplete));
            network.Register<TimePacket>(0x5B, "Time", 4, new TypedPacketReceiveHandler(Time));
            network.Register<WeatherPacket>(0x65, "Set Weather", 4, new TypedPacketReceiveHandler(Weather));
            network.Register<TargetCursorPacket>(0x6C, "TargetCursor", 19, new TypedPacketReceiveHandler(TargetCursor));
            network.Register<PlayMusicPacket>(0x6D, "Play Music", 3, new TypedPacketReceiveHandler(PlayMusic));
            network.Register<MobileAnimationPacket>(0x6E, "Character Animation", 14, new TypedPacketReceiveHandler(MobileAnimation));
            network.Register<GraphicEffectPacket>(0x70, "Graphical Effect 1", 28, new TypedPacketReceiveHandler(GraphicalEffect1));
            network.Register<WarModePacket>(0x72, "War Mode", 5, new TypedPacketReceiveHandler(WarMode));
            network.Register<VendorBuyListPacket>(0x74, "Vendor Buy List", -1, new TypedPacketReceiveHandler(VendorBuyList));
            network.Register<SubServerPacket>(0x76, "New Subserver", 16, new TypedPacketReceiveHandler(NewSubserver));
            network.Register<MobileMovingPacket>(0x77, "Mobile Moving", 17, new TypedPacketReceiveHandler(MobileMoving));
            network.Register<MobileIncomingPacket>(0x78, "Mobile Incoming", -1, new TypedPacketReceiveHandler(MobileIncoming));
            network.Register<DisplayMenuPacket>(0x7C, "Display Menu", -1, new TypedPacketReceiveHandler(DisplayMenu));
            network.Register<LoginRejectionPacket>(0x82, "Login Rejection", 2, new TypedPacketReceiveHandler(LoginRejection));
            network.Register<DeleteCharacterResponsePacket>(0x85, "Delete Character Response", 2, new TypedPacketReceiveHandler(DeleteCharacterResponse));
            network.Register<CharacterListUpdatePacket>(0x86, "Character List Update", -1, new TypedPacketReceiveHandler(CharacterListUpdate));
            network.Register<OpenPaperdollPacket>(0x88, "Open Paperdoll", 66, new TypedPacketReceiveHandler(OpenPaperdoll));
            network.Register<CorpseClothingPacket>(0x89, "Corpse Clothing", -1, new TypedPacketReceiveHandler(CorpseClothing));
            network.Register<ServerRelayPacket>(0x8C, "ServerRelay", 11, new TypedPacketReceiveHandler(ServerRelay));
            network.Register<PlayerMovePacket>(0x97, "Player Move", 2, new TypedPacketReceiveHandler(PlayerMove));
            network.Register<RequestNameResponsePacket>(0x98, "Request Name Response", -1, new TypedPacketReceiveHandler(RequestNameResponse));
            network.Register<TargetCursorMultiPacket>(0x99, "Target Cursor Multi Object", 26, new TypedPacketReceiveHandler(TargetCursorMulti));
            network.Register<VendorSellListPacket>(0x9E, "Vendor Sell List", -1, new TypedPacketReceiveHandler(VendorSellList));
            network.Register<UpdateHealthPacket>(0xA1, "Update Current Health", 9, new TypedPacketReceiveHandler(UpdateCurrentHealth));
            network.Register<UpdateManaPacket>(0xA2, "Update Current Mana", 9, new TypedPacketReceiveHandler(UpdateCurrentMana));
            network.Register<UpdateStaminaPacket>(0xA3, "Update Current Stamina", 9, new TypedPacketReceiveHandler(UpdateCurrentStamina));
            network.Register<OpenWebBrowserPacket>(0xA5, "Open Web Browser", -1, new TypedPacketReceiveHandler(OpenWebBrowser));
            network.Register<TipNoticePacket>(0xA6, "Tip/Notice Window", -1, new TypedPacketReceiveHandler(TipNoticeWindow));
            network.Register<ServerListPacket>(0xA8, "Game Server List", -1, new TypedPacketReceiveHandler(ServerList));
            network.Register<CharacterCityListPacket>(0xA9, "Characters / Starting Locations", -1, new TypedPacketReceiveHandler(CharactersStartingLocations));
            network.Register<ChangeCombatantPacket>(0xAA, "Change Combatant", 5, new TypedPacketReceiveHandler(ChangeCombatant)); 
            network.Register<UnicodeMessagePacket>(0xAE, "Unicode Message", -1, new TypedPacketReceiveHandler(UnicodeMessage));
            network.Register<DeathAnimationPacket>(0xAF, "Death Animation", 13, new TypedPacketReceiveHandler(DeathAnimation)); 
            network.Register<DisplayGumpFastPacket>(0xB0, "Display Gump Fast", -1, new TypedPacketReceiveHandler(DisplayGumpFast)); 
            network.Register<ObjectHelpResponsePacket>(0xB7, "Object Help Response ", -1, new TypedPacketReceiveHandler(ObjectHelpResponse));
            network.Register<SupportedFeaturesPacket>(0xB9, "Supported Features", 3, new TypedPacketReceiveHandler(SupportedFeatures));
            network.Register<QuestArrowPacket>(0xBA, "Quest Arrow", 6, new TypedPacketReceiveHandler(QuestArrow));
            network.Register<SeasonChangePacket>(0xBC, "Seasonal Change", 3, new TypedPacketReceiveHandler(SeasonChange));
            network.Register<VersionRequestPacket>(0xBD, "Version Request", -1, new TypedPacketReceiveHandler(VersionRequest));
            network.Register<GeneralInfoPacket>(0xBF, "General Information", -1, new TypedPacketReceiveHandler(GeneralInfo));
            network.Register<GraphicEffectHuedPacket>(0xC0, "Hued Effect", 36, new TypedPacketReceiveHandler(HuedEffect));
            network.Register<MessageLocalizedPacket>(0xC1, "Message Localized", -1, new TypedPacketReceiveHandler(MessageLocalized));
            network.Register<InvalidMapEnablePacket>(0xC6, "Invalid Map Enable", 1, new TypedPacketReceiveHandler(InvalidMapEnable));
            network.Register<GraphicEffectExtendedPacket>(0xC7, "Particle Effect", 49, new TypedPacketReceiveHandler(ParticleEffect));
            network.Register<GlobalQueuePacket>(0xCB, "Global Queue Count", 7, new TypedPacketReceiveHandler(GlobalQueueCount));
            network.Register<MessageLocalizedAffixPacket>(0xCC, "Message Localized Affix ", -1, new TypedPacketReceiveHandler(MessageLocalizedAffix));
            network.Register<Extended0x78Packet>(0xD3, "Extended 0x78", -1, new TypedPacketReceiveHandler(Extended0x78));
            network.Register<ObjectPropertyListPacket>(0xD6, "Mega Cliloc", -1, new TypedPacketReceiveHandler(MegaCliloc));
            network.Register<CustomHousePacket>(0xD8, "Send Custom House", -1, new TypedPacketReceiveHandler(SendCustomHouse));
            network.Register<ObjectPropertyListUpdatePacket>(0xDC, "SE Introduced Revision", 9, new TypedPacketReceiveHandler(ToolTipRevision));
            network.Register<CompressedGumpPacket>(0xDD, "Compressed Gump", -1, new TypedPacketReceiveHandler(CompressedGump));

            /* Deprecated (not used by RunUO) and/or not implmented
             * Left them here incase we need to implement in the future
            network.Register<RecvPacket>(0x30, "Attack Ok", 5, OnAttackOk);
            network.Register<HealthBarStatusPacket>(0x17, "Health Bar Status Update", 12, OnHealthBarStatusUpdate);
            network.Register<KickPlayerPacket>(0x26, "Kick Player", 5, OnKickPlayer);
            network.Register<DropItemFailedPacket>(0x28, "Drop Item Failed", 5, OnDropItemFailed);
            network.Register<PaperdollClothingAddAckPacket>(0x29, "Paperdoll Clothing Add Ack", 1, OnPaperdollClothingAddAck);
            network.Register<BloodPacket>(0x2A, "Blood", 5, OnBlood);
            network.Register<RecvPacket>(0x33, "Pause Client", -1, OnPauseClient);
            network.Register<RecvPacket>(0x90, "Map Message", -1, OnMapMessage);
            network.Register<RecvPacket>(0x9C, "Help Request", -1, OnHelpRequest);
            network.Register<RecvPacket>(0xAB, "Gump Text Entry Dialog", -1, OnGumpDialog);
            network.Register<RecvPacket>(0xB2, "Chat Message", -1, OnChatMessage);
            network.Register<RecvPacket>(0xC4, "Semivisible", -1, OnSemivisible);
            network.Register<RecvPacket>(0xD2, "Extended 0x20", -1, OnExtended0x20);
            network.Register<RecvPacket>(0xDB, "Character Transfer Log", -1, OnCharacterTransferLog);
            network.Register<RecvPacket>(0xDC, "SE Introduced Revision", -1, OnToolTipRevision);
            network.Register<RecvPacket>(0xDE, "Update Mobile Status", -1, OnUpdateMobileStatus);
            network.Register<RecvPacket>(0xDF, "Buff/Debuff System", -1, OnBuffDebuff);
            network.Register<RecvPacket>(0xE2, "Mobile status/Animation update", -1, OnMobileStatusAnimationUpdate);
            network.Register<RecvPacket>(0xF0, "Krrios client special", -1, OnKrriosClientSpecial);
            */
        }

        public static event TypedPacketReceiveHandler Damage;
        public static event TypedPacketReceiveHandler MobileStatusCompact;
        public static event TypedPacketReceiveHandler WorldItem;
        public static event TypedPacketReceiveHandler LoginConfirm;
        public static event TypedPacketReceiveHandler AsciiMessage;
        public static event TypedPacketReceiveHandler RemoveEntity;
        public static event TypedPacketReceiveHandler MobileUpdate;
        public static event TypedPacketReceiveHandler MovementRejected;
        public static event TypedPacketReceiveHandler MoveAcknowledged;
        public static event TypedPacketReceiveHandler DragEffect;
        public static event TypedPacketReceiveHandler OpenContainer;
        public static event TypedPacketReceiveHandler ContainerContentUpdate;
        public static event TypedPacketReceiveHandler LiftRejection;
        public static event TypedPacketReceiveHandler ResurrectMenu;
        public static event TypedPacketReceiveHandler MobileAttributes;
        public static event TypedPacketReceiveHandler WornItem;
        public static event TypedPacketReceiveHandler Swing;
        public static event TypedPacketReceiveHandler SkillsList;
        public static event TypedPacketReceiveHandler ContainerContent;
        public static event TypedPacketReceiveHandler PersonalLightLevel;
        public static event TypedPacketReceiveHandler OverallLightLevel;
        public static event TypedPacketReceiveHandler PopupMessage;
        public static event TypedPacketReceiveHandler PlaySoundEffect;
        public static event TypedPacketReceiveHandler LoginComplete;
        public static event TypedPacketReceiveHandler Time;
        public static event TypedPacketReceiveHandler Weather;
        public static event TypedPacketReceiveHandler TargetCursor;
        public static event TypedPacketReceiveHandler PlayMusic;
        public static event TypedPacketReceiveHandler MobileAnimation;
        public static event TypedPacketReceiveHandler GraphicalEffect1;
        public static event TypedPacketReceiveHandler WarMode;
        public static event TypedPacketReceiveHandler VendorBuyList;
        public static event TypedPacketReceiveHandler NewSubserver;
        public static event TypedPacketReceiveHandler MobileMoving;
        public static event TypedPacketReceiveHandler MobileIncoming;
        public static event TypedPacketReceiveHandler DisplayMenu;
        public static event TypedPacketReceiveHandler LoginRejection;
        public static event TypedPacketReceiveHandler DeleteCharacterResponse;
        public static event TypedPacketReceiveHandler CharacterListUpdate;
        public static event TypedPacketReceiveHandler OpenPaperdoll;
        public static event TypedPacketReceiveHandler CorpseClothing;
        public static event TypedPacketReceiveHandler ServerRelay;
        public static event TypedPacketReceiveHandler PlayerMove;
        public static event TypedPacketReceiveHandler RequestNameResponse;
        public static event TypedPacketReceiveHandler TargetCursorMulti;
        public static event TypedPacketReceiveHandler VendorSellList;
        public static event TypedPacketReceiveHandler UpdateCurrentHealth;
        public static event TypedPacketReceiveHandler UpdateCurrentMana;
        public static event TypedPacketReceiveHandler UpdateCurrentStamina;
        public static event TypedPacketReceiveHandler OpenWebBrowser;
        public static event TypedPacketReceiveHandler TipNoticeWindow;
        public static event TypedPacketReceiveHandler ServerList;
        public static event TypedPacketReceiveHandler CharactersStartingLocations;
        public static event TypedPacketReceiveHandler ChangeCombatant;
        public static event TypedPacketReceiveHandler UnicodeMessage;
        public static event TypedPacketReceiveHandler DeathAnimation;
        public static event TypedPacketReceiveHandler DisplayGumpFast;
        public static event TypedPacketReceiveHandler ObjectHelpResponse;
        public static event TypedPacketReceiveHandler SupportedFeatures;
        public static event TypedPacketReceiveHandler QuestArrow;
        public static event TypedPacketReceiveHandler SeasonChange;
        public static event TypedPacketReceiveHandler VersionRequest;
        public static event TypedPacketReceiveHandler GeneralInfo;
        public static event TypedPacketReceiveHandler HuedEffect;
        public static event TypedPacketReceiveHandler MessageLocalized;
        public static event TypedPacketReceiveHandler InvalidMapEnable;
        public static event TypedPacketReceiveHandler ParticleEffect;
        public static event TypedPacketReceiveHandler GlobalQueueCount;
        public static event TypedPacketReceiveHandler MessageLocalizedAffix;
        public static event TypedPacketReceiveHandler Extended0x78;
        public static event TypedPacketReceiveHandler MegaCliloc;
        public static event TypedPacketReceiveHandler ToolTipRevision;
        public static event TypedPacketReceiveHandler CompressedGump;
        public static event TypedPacketReceiveHandler SendCustomHouse;

        /* Deprecated (not used by RunUO) and/or not implmented
         * Left them here incase we need to implement in the future
        public static event TypedPacketReceiveHandler AttackOk;
        public static event TypedPacketReceiveHandler HealthBarStatusUpdate;
        public static event TypedPacketReceiveHandler KickPlayer;
        public static event TypedPacketReceiveHandler DropItemFailed;
        public static event TypedPacketReceiveHandler PaperdollClothingAddAck;
        public static event TypedPacketReceiveHandler Blood;
        public static event TypedPacketReceiveHandler PauseClient;
        public static event TypedPacketReceiveHandler GraphicalEffect1;
        public static event TypedPacketReceiveHandler CorpseClothing;
        public static event TypedPacketReceiveHandler MapMessage;
        public static event TypedPacketReceiveHandler HelpRequest;
        public static event TypedPacketReceiveHandler GumpDialog;
        public static event TypedPacketReceiveHandler ChatMessage;
        public static event TypedPacketReceiveHandler Semivisible;
        public static event TypedPacketReceiveHandler Extended0x20;
        public static event TypedPacketReceiveHandler MegaCliloc;
        
        public static event TypedPacketReceiveHandler CharacterTransferLog;
        public static event TypedPacketReceiveHandler ToolTipRevision;
        public static event TypedPacketReceiveHandler UpdateMobileStatus;
        public static event TypedPacketReceiveHandler BuffDebuff;
        public static event TypedPacketReceiveHandler MobileStatusAnimationUpdate;
        public static event TypedPacketReceiveHandler KrriosClientSpecial;
        */
    }
}
