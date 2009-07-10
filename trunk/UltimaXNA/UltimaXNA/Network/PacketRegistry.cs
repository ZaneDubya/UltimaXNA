using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Network.Packets.Server;

namespace UltimaXNA.Network
{
    internal static class PacketRegistry
    {
        /// <summary>
        /// Make sure you've hooked all handlers before calling this function.
        /// </summary>
        /// <param name="network"></param>
        internal static void RegisterNetwork(IClientNetwork network)
        {
            network.Register<DamagePacket>(0x0B, "Damage", 0x07, OnDamage);
            network.Register<MobileStatusCompactPacket>(0x11, "Mobile Status Compact", -1, OnMobileStatusCompact);
            network.Register<WorldItemPacket>(0x1A, "World Item", -1, OnWorldItem);
            network.Register<LoginConfirmPacket>(0x1B, "Login Confirm", 37, OnLoginConfirm);
            network.Register<AsciiMessagePacket>(0x1C, "Ascii Meessage", -1, OnAsciiMessage);
            network.Register<RemoveEntityPacket>(0x1D, "Remove Entity", 5, OnRemoveEntity);
            network.Register<MobileUpdatePacket>(0x20, "Mobile Update", 19, OnMobileUpdate);
            network.Register<MovementRejectPacket>(0x21, "Movement Rejection", 8, OnMovementRejected);
            network.Register<MoveAcknowledgePacket>(0x22, "Move Acknowledged", 3, OnMoveAcknowledged);
            network.Register<DragEffectPacket>(0x23, "Drag Effect", 26, OnDragEffect);
            network.Register<OpenContainerPacket>(0x24, "Open Container", 7, OnOpenContainer);
            network.Register<ContainerContentUpdatePacket>(0x25, "Container Content Update", 21, OnContainerContentUpdate);
            network.Register<LiftRejectionPacket>(0x27, "Lift Rejection", 2, OnLiftRejection);
            network.Register<ResurrectionMenuPacket>(0x2C, "Resurect menu", 2, OnResurrectMenu);
            network.Register<MobileAttributesPacket>(0x2D, "Mob Attributes", 17, OnMobileAttributes);
            network.Register<WornItemPacket>(0x2E, "Worn Item", 15, OnWornItem);
            network.Register<SwingPacket>(0x2F, "Swing", 10, OnSwing);
            network.Register<SendSkillsPacket>(0x3A, "Skills list", -1, OnSkillsList);
            network.Register<ContainerContentPacket>(0x3C, "Container Content", -1, OnContainerContent);
            network.Register<PersonalLightLevelPacket>(0x4E, "Personal Light Level", 6, OnPersonalLightLevel);
            network.Register<OverallLightLevelPacket>(0x4F, "Overall Light Level", 2, OnOverallLightLevel);
            network.Register<PopupMessagePacket>(0x53, "Popup Message", 2, OnPopupMessage);
            network.Register<PlaySoundEffectPacket>(0x54, "Play Sound Effect", 12, OnPlaySoundEffect);
            network.Register<LoginCompletePacket>(0x55, "Login Complete", 1, OnLoginComplete);
            network.Register<TimePacket>(0x5B, "Time", 4, OnTime);
            network.Register<WeatherPacket>(0x65, "Set Weather", 4, OnWeather);
            network.Register<TargetCursorPacket>(0x6C, "TargetCursor", 3, OnTargetCursor);
            network.Register<PlayMusicPacket>(0x6D, "Play Music", 3, OnPlayMusic);
            network.Register<MobileAnimationPacket>(0x6E, "Character Animation", 14, OnMobileAnimation);
            network.Register<GraphicEffectPacket>(0x70, "Graphical Effect 1", 28, OnGraphicalEffect1);
            network.Register<WarModePacket>(0x72, "War Mode", 5, OnWarMode);
            network.Register<VendorBuyListPacket>(0x74, "Vendor Buy List", -1, OnVendorBuyList);
            network.Register<SubServerPacket>(0x76, "New Subserver", 16, OnNewSubserver);
            network.Register<MobileMovingPacket>(0x77, "Mobile Moving", 17, OnMobileMoving);
            network.Register<MobileIncomingPacket>(0x78, "Mobile Incoming", -1, OnMobileIncoming);
            network.Register<DisplayMenuPacket>(0x7C, "Display Menu", -1, OnDisplayMenu); // TODO: Implement packet...DisplayMenuPacket
            network.Register<LoginRejectionPacket>(0x82, "Login Rejection", 2, OnLoginRejection);
            network.Register<CharacterListUpdatePacket>(0x86, "Character List Update", 304, OnCharacterListUpdate);// TODO: Implement packet...
            network.Register<OpenPaperdollPacket>(0x88, "Open Paperdoll", 66, OnOpenPaperdoll);
            network.Register<CorpseClothingPacket>(0x89, "Corpse Clothing", -1, OnCorpseClothing);
            network.Register<ServerRelayPacket>(0x8C, "ServerRelay", 11, OnServerRelay);
            network.Register<PlayerMovePacket>(0x97, "Player Move", 2, OnPlayerMove);
            network.Register<RequestNameResponsePacket>(0x98, "Request Name Response", -1, OnRequestNameResponse);
            network.Register<VendorSellListPacket>(0x9E, "Vendor Sell List", -1, OnVendorSellList); // TODO: Implement packet...VendorSellListPacket
            network.Register<UpdateHealthPacket>(0xA1, "Update Current Health", 9, OnUpdateCurrentHealth);
            network.Register<UpdateManaPacket>(0xA2, "Update Current Mana", 9, OnUpdateCurrentMana);
            network.Register<UpdateStaminaPacket>(0xA3, "Update Current Stamina", 9, OnUpdateCurrentStamina);
            network.Register<OpenWebBrowserPacket>(0xA5, "Open Web Browser", -1, OnOpenWebBrowser); // TODO: Implement packet...OpenWebBrowserPacket
            network.Register<TipNoticePacket>(0xA6, "Tip/Notice Window", -1, OnTipNoticeWindow); // TODO: Implement packet...ScrollMessagePacket
            network.Register<ServerListPacket>(0xA8, "Game Server List", -1, OnServerList);
            network.Register<CharacterCityListPacket>(0xA9, "Characters / Starting Locations", -1, OnCharactersStartingLocations);
            network.Register<ChangeCombatantPacket>(0xAA, "Change Combatant", 5, OnChangeCombatant); // TODO: Implement packet...
            network.Register<UnicodeMessagePacket>(0xAE, "Unicode Message", -1, OnUnicodeMessage);
            network.Register<DeathAnimationPacket>(0xAF, "Death Animation", 13, OnDeathAnimation); // TODO: Implement packet...DeathAnimationPacket
            network.Register<DisplayGumpFastPacket>(0xB0, "Display Gump Fast", -1, OnDisplayGumpFast); // TODO: Implement packet...
            network.Register<ObjectHelpResponsePacket>(0xB7, "Object Help Response ", -1, OnObjectHelpResponse);// TODO: Implement packet...
            network.Register<SupportedFeaturesPacket>(0xB9, "Supported Features", 3, OnSupportedFeatures);
            network.Register<QuestArrowPacket>(0xBA, "Quest Arrow", 6, OnQuestArrow);// TODO: Implement packet...
            network.Register<SeasonChangePacket>(0xBC, "Seasonal Change", 3, OnSeasonChange);
            network.Register<VersionRequestPacket>(0xBD, "Version Request", -1, OnVersionRequest);
            network.Register<GeneralInfoPacket>(0xBF, "General Information", -1, OnGeneralInfo);
            network.Register<HuedEffectPacket>(0xC0, "Hued Effect", 36, OnHuedEffect);// TODO: Implement packet...
            network.Register<MessageLocalizedPacket>(0xC1, "Message Localized", -1, OnMessageLocalized);
            network.Register<InvalidMapEnablePacket>(0xC6, "Invalid Map Enable", 1, OnInvalidMapEnable);// TODO: Implement packet...
            network.Register<ParticleEffectPacket>(0xC7, "Particle Effect", 49, OnParticleEffect);// TODO: Implement packet...
            network.Register<GlobalQueuePacket>(0xCB, "Global Queue Count", 7, OnGlobalQueueCount);// TODO: Implement packet...
            network.Register<MessageLocalizedPacket>(0xCC, "Message Localized Affix ", -1, OnMessageLocalizedAffix);// TODO: Implement packet...
            network.Register<Extended0x78Packet>(0xD3, "Extended 0x78", -1, OnExtended0x78);// QUERY: What is this? TODO: Implement packet...
            network.Register<ObjectPropertyListPacket>(0xD6, "Mega Cliloc", -1, OnMegaCliloc);
            network.Register<ObjectPropertyListUpdatePacket>(0xDC, "SE Introduced Revision", 9, OnToolTipRevision);
            network.Register<CompressedGumpPacket>(0xDD, "Compressed Gump", -1, OnCompressedGump);// TODO: finsih packet...

            /* Deprecated (not used by RunUO) and/or not implmented
             * Left them here incase we need to implement in the future
            network.Register<RecvPacket>(0x30, "Attack Ok", 5, OnAttackOk); Not in RunUO!
            network.Register<HealthBarStatusPacket>(0x17, "Health Bar Status Update", 12, OnHealthBarStatusUpdate);
            network.Register<KickPlayerPacket>(0x26, "Kick Player", 5, OnKickPlayer);
            network.Register<DropItemFailedPacket>(0x28, "Drop Item Failed", 5, OnDropItemFailed);
            network.Register<PaperdollClothingAddAckPacket>(0x29, "Paperdoll Clothing Add Ack", 1, OnPaperdollClothingAddAck);
            network.Register<BloodPacket>(0x2A, "Blood", 5, OnBlood);
            network.Register<RecvPacket>(0x33, "Pause Client", -1, OnPauseClient);
            network.Register<RecvPacket>(0x89, "Corpse Clothing", -1, OnCorpseClothing);
            network.Register<RecvPacket>(0x90, "Map Message", -1, OnMapMessage);
            network.Register<RecvPacket>(0x9C, "Help Request", -1, OnHelpRequest);
            network.Register<RecvPacket>(0xAB, "Gump Text Entry Dialog", -1, OnGumpDialog);
            network.Register<RecvPacket>(0xB2, "Chat Message", -1, OnChatMessage);
            network.Register<RecvPacket>(0xC4, "Semivisible", -1, OnSemivisible);
            network.Register<RecvPacket>(0xD2, "Extended 0x20", -1, OnExtended0x20);
            network.Register<RecvPacket>(0xD8, "Send Custom House", -1, OnSendCustomHouse);
            network.Register<RecvPacket>(0xDB, "Character Transfer Log", -1, OnCharacterTransferLog);
            network.Register<RecvPacket>(0xDE, "Update Mobile Status", -1, OnUpdateMobileStatus);
            network.Register<RecvPacket>(0xDF, "Buff/Debuff System", -1, OnBuffDebuff);
            network.Register<RecvPacket>(0xE2, "Mobile status/Animation update", -1, OnMobileStatusAnimationUpdate);
            network.Register<RecvPacket>(0xF0, "Krrios client special", -1, OnKrriosClientSpecial);
            */
        }

        internal static TypedPacketReceiveHandler OnDamage;
        internal static TypedPacketReceiveHandler OnMobileStatusCompact;
        internal static TypedPacketReceiveHandler OnWorldItem;
        internal static TypedPacketReceiveHandler OnLoginConfirm;
        internal static TypedPacketReceiveHandler OnAsciiMessage;
        internal static TypedPacketReceiveHandler OnRemoveEntity;
        internal static TypedPacketReceiveHandler OnMobileUpdate;
        internal static TypedPacketReceiveHandler OnMovementRejected;
        internal static TypedPacketReceiveHandler OnMoveAcknowledged;
        internal static TypedPacketReceiveHandler OnDragEffect;
        internal static TypedPacketReceiveHandler OnOpenContainer;
        internal static TypedPacketReceiveHandler OnContainerContentUpdate;
        internal static TypedPacketReceiveHandler OnLiftRejection;
        internal static TypedPacketReceiveHandler OnResurrectMenu;
        internal static TypedPacketReceiveHandler OnMobileAttributes;
        internal static TypedPacketReceiveHandler OnWornItem;
        internal static TypedPacketReceiveHandler OnSwing;
        internal static TypedPacketReceiveHandler OnSkillsList;
        internal static TypedPacketReceiveHandler OnContainerContent;
        internal static TypedPacketReceiveHandler OnPersonalLightLevel;
        internal static TypedPacketReceiveHandler OnOverallLightLevel;
        internal static TypedPacketReceiveHandler OnPopupMessage;
        internal static TypedPacketReceiveHandler OnPlaySoundEffect;
        internal static TypedPacketReceiveHandler OnLoginComplete;
        internal static TypedPacketReceiveHandler OnTime;
        internal static TypedPacketReceiveHandler OnWeather;
        internal static TypedPacketReceiveHandler OnTargetCursor;
        internal static TypedPacketReceiveHandler OnPlayMusic;
        internal static TypedPacketReceiveHandler OnMobileAnimation;
        internal static TypedPacketReceiveHandler OnGraphicalEffect1;
        internal static TypedPacketReceiveHandler OnWarMode;
        internal static TypedPacketReceiveHandler OnVendorBuyList;
        internal static TypedPacketReceiveHandler OnNewSubserver;
        internal static TypedPacketReceiveHandler OnMobileMoving;
        internal static TypedPacketReceiveHandler OnMobileIncoming;
        internal static TypedPacketReceiveHandler OnDisplayMenu;
        internal static TypedPacketReceiveHandler OnLoginRejection;
        internal static TypedPacketReceiveHandler OnCharacterListUpdate;
        internal static TypedPacketReceiveHandler OnOpenPaperdoll;
        internal static TypedPacketReceiveHandler OnCorpseClothing;
        internal static TypedPacketReceiveHandler OnServerRelay;
        internal static TypedPacketReceiveHandler OnPlayerMove;
        internal static TypedPacketReceiveHandler OnRequestNameResponse;
        internal static TypedPacketReceiveHandler OnVendorSellList;
        internal static TypedPacketReceiveHandler OnUpdateCurrentHealth;
        internal static TypedPacketReceiveHandler OnUpdateCurrentMana;
        internal static TypedPacketReceiveHandler OnUpdateCurrentStamina;
        internal static TypedPacketReceiveHandler OnOpenWebBrowser;
        internal static TypedPacketReceiveHandler OnTipNoticeWindow;
        internal static TypedPacketReceiveHandler OnServerList;
        internal static TypedPacketReceiveHandler OnCharactersStartingLocations;
        internal static TypedPacketReceiveHandler OnChangeCombatant;
        internal static TypedPacketReceiveHandler OnUnicodeMessage;
        internal static TypedPacketReceiveHandler OnDeathAnimation;
        internal static TypedPacketReceiveHandler OnDisplayGumpFast;
        internal static TypedPacketReceiveHandler OnObjectHelpResponse;
        internal static TypedPacketReceiveHandler OnSupportedFeatures;
        internal static TypedPacketReceiveHandler OnQuestArrow;
        internal static TypedPacketReceiveHandler OnSeasonChange;
        internal static TypedPacketReceiveHandler OnVersionRequest;
        internal static TypedPacketReceiveHandler OnGeneralInfo;
        internal static TypedPacketReceiveHandler OnHuedEffect;
        internal static TypedPacketReceiveHandler OnMessageLocalized;
        internal static TypedPacketReceiveHandler OnInvalidMapEnable;
        internal static TypedPacketReceiveHandler OnParticleEffect;
        internal static TypedPacketReceiveHandler OnGlobalQueueCount;
        internal static TypedPacketReceiveHandler OnMessageLocalizedAffix;
        internal static TypedPacketReceiveHandler OnExtended0x78;
        internal static TypedPacketReceiveHandler OnMegaCliloc;
        internal static TypedPacketReceiveHandler OnToolTipRevision;
        internal static TypedPacketReceiveHandler OnCompressedGump;
        

        /* Deprecated (not used by RunUO) and/or not implmented
         * Left them here incase we need to implement in the future
        internal static TypedPacketReceiveHandler OnAttackOk;
        internal static TypedPacketReceiveHandler OnHealthBarStatusUpdate;
        internal static TypedPacketReceiveHandler OnKickPlayer;
        internal static TypedPacketReceiveHandler OnDropItemFailed;
        internal static TypedPacketReceiveHandler OnPaperdollClothingAddAck;
        internal static TypedPacketReceiveHandler OnBlood;
        internal static TypedPacketReceiveHandler OnPauseClient;
        
        internal static TypedPacketReceiveHandler OnMapMessage;
        internal static TypedPacketReceiveHandler OnHelpRequest;
        internal static TypedPacketReceiveHandler OnGumpDialog;
        internal static TypedPacketReceiveHandler OnChatMessage;
        internal static TypedPacketReceiveHandler OnSemivisible;
        internal static TypedPacketReceiveHandler OnExtended0x20;
        internal static TypedPacketReceiveHandler OnSendCustomHouse;
        internal static TypedPacketReceiveHandler OnCharacterTransferLog;
        internal static TypedPacketReceiveHandler OnUpdateMobileStatus;
        internal static TypedPacketReceiveHandler OnBuffDebuff;
        internal static TypedPacketReceiveHandler OnMobileStatusAnimationUpdate;
        internal static TypedPacketReceiveHandler OnKrriosClientSpecial;
        */
    }
}
