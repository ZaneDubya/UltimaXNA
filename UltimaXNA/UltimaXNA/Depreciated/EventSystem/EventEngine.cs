using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Diagnostics;
using UltimaXNA.Extensions;
using Microsoft.Xna.Framework;
using UltimaXNA.Network;

namespace UltimaXNA.EventSystem
{
    public delegate void EventEngineHandler(object sender, params object[] args);

    public class EventEngine : IEventService
    {
        Game _game;
        ILoggingService _log;
        Dictionary<int, List<IEventReceiver>> _registeredEvents;
        List<IEventReceiver> _registeredAllEvents;

        public EventEngine(Game game)
        {
            _game = game;
            _log = game.Services.GetService<ILoggingService>();

            if (_log == null)
            {
                _log = new Logger(GetType());
            }

            _registeredEvents = new Dictionary<int, List<IEventReceiver>>();
            _registeredAllEvents = new List<IEventReceiver>();
            
            #region Packet Handler Subscription
            PacketRegistry.AsciiMessage += new TypedPacketReceiveHandler(OnAsciiMessage);
            PacketRegistry.ChangeCombatant += new TypedPacketReceiveHandler(OnChangeCombatant);
            PacketRegistry.CharacterListUpdate += new TypedPacketReceiveHandler(OnCharacterListUpdate);
            PacketRegistry.CharactersStartingLocations += new TypedPacketReceiveHandler(OnCharactersStartingLocations);
            PacketRegistry.CompressedGump += new TypedPacketReceiveHandler(OnCompressedGump);
            PacketRegistry.ContainerContent += new TypedPacketReceiveHandler(OnContainerContent);
            PacketRegistry.ContainerContentUpdate += new TypedPacketReceiveHandler(OnContainerContentUpdate);
            PacketRegistry.CorpseClothing += new TypedPacketReceiveHandler(OnCorpseClothing);
            PacketRegistry.Damage += new TypedPacketReceiveHandler(OnDamage);
            PacketRegistry.DeathAnimation += new TypedPacketReceiveHandler(OnDeathAnimation);
            PacketRegistry.DisplayGumpFast += new TypedPacketReceiveHandler(OnDisplayGumpFast);
            PacketRegistry.DisplayMenu += new TypedPacketReceiveHandler(OnDisplayMenu);
            PacketRegistry.DragEffect += new TypedPacketReceiveHandler(OnDragEffect);
            PacketRegistry.GeneralInfo += new TypedPacketReceiveHandler(OnGeneralInfo);
            PacketRegistry.GlobalQueueCount += new TypedPacketReceiveHandler(OnGlobalQueueCount);
            PacketRegistry.GraphicalEffect1 += new TypedPacketReceiveHandler(OnGraphicalEffect1);
            PacketRegistry.HuedEffect += new TypedPacketReceiveHandler(OnHuedEffect);
            PacketRegistry.InvalidMapEnable += new TypedPacketReceiveHandler(OnInvalidMapEnable);
            PacketRegistry.LiftRejection += new TypedPacketReceiveHandler(OnLiftRejection);
            PacketRegistry.LoginComplete += new TypedPacketReceiveHandler(OnLoginComplete);
            PacketRegistry.LoginConfirm += new TypedPacketReceiveHandler(OnLoginConfirm);
            PacketRegistry.LoginRejection += new TypedPacketReceiveHandler(OnLoginRejection);
            PacketRegistry.MegaCliloc += new TypedPacketReceiveHandler(OnMegaCliloc);
            PacketRegistry.MessageLocalized += new TypedPacketReceiveHandler(OnMessageLocalized);
            PacketRegistry.MessageLocalizedAffix += new TypedPacketReceiveHandler(OnMessageLocalizedAffix);
            PacketRegistry.MobileAnimation += new TypedPacketReceiveHandler(OnMobileAnimation);
            PacketRegistry.MobileAttributes += new TypedPacketReceiveHandler(OnMobileAttributes);
            PacketRegistry.MobileIncoming += new TypedPacketReceiveHandler(OnMobileIncoming);
            PacketRegistry.MobileMoving += new TypedPacketReceiveHandler(OnMobileMoving);
            PacketRegistry.MobileStatusCompact += new TypedPacketReceiveHandler(OnMobileStatusCompact);
            PacketRegistry.MobileUpdate += new TypedPacketReceiveHandler(OnMobileUpdate);
            PacketRegistry.MoveAcknowledged += new TypedPacketReceiveHandler(OnMoveAcknowledged);
            PacketRegistry.MovementRejected += new TypedPacketReceiveHandler(OnMovementRejected);
            PacketRegistry.NewSubserver += new TypedPacketReceiveHandler(OnNewSubserver);
            PacketRegistry.ObjectHelpResponse += new TypedPacketReceiveHandler(OnObjectHelpResponse);
            PacketRegistry.OpenContainer += new TypedPacketReceiveHandler(OnOpenContainer);
            PacketRegistry.OpenPaperdoll += new TypedPacketReceiveHandler(OnOpenPaperdoll);
            PacketRegistry.OpenWebBrowser += new TypedPacketReceiveHandler(OnOpenWebBrowser);
            PacketRegistry.OverallLightLevel += new TypedPacketReceiveHandler(OnOverallLightLevel);
            PacketRegistry.ParticleEffect += new TypedPacketReceiveHandler(OnParticleEffect);
            PacketRegistry.PersonalLightLevel += new TypedPacketReceiveHandler(OnPersonalLightLevel);
            PacketRegistry.PlayerMove += new TypedPacketReceiveHandler(OnPlayerMove);
            PacketRegistry.PlayMusic += new TypedPacketReceiveHandler(OnPlayMusic);
            PacketRegistry.PlaySoundEffect += new TypedPacketReceiveHandler(OnPlaySoundEffect);
            PacketRegistry.PopupMessage += new TypedPacketReceiveHandler(OnPopupMessage);
            PacketRegistry.QuestArrow += new TypedPacketReceiveHandler(OnQuestArrow);
            PacketRegistry.RemoveEntity += new TypedPacketReceiveHandler(OnRemoveEntity);
            PacketRegistry.RequestNameResponse += new TypedPacketReceiveHandler(OnRequestNameResponse);
            PacketRegistry.ResurrectMenu += new TypedPacketReceiveHandler(OnResurrectMenu);
            PacketRegistry.SeasonChange += new TypedPacketReceiveHandler(OnSeasonChange);
            PacketRegistry.ServerList += new TypedPacketReceiveHandler(OnServerList);
            PacketRegistry.ServerRelay += new TypedPacketReceiveHandler(OnServerRelay);
            PacketRegistry.SkillsList += new TypedPacketReceiveHandler(OnSkillsList);
            PacketRegistry.SupportedFeatures += new TypedPacketReceiveHandler(OnSupportedFeatures);
            PacketRegistry.Swing += new TypedPacketReceiveHandler(OnSwing);
            PacketRegistry.TargetCursor += new TypedPacketReceiveHandler(OnTargetCursor);
            PacketRegistry.Time += new TypedPacketReceiveHandler(OnTime);
            PacketRegistry.TipNoticeWindow += new TypedPacketReceiveHandler(OnTipNoticeWindow);
            PacketRegistry.ToolTipRevision += new TypedPacketReceiveHandler(OnToolTipRevision);
            PacketRegistry.UnicodeMessage += new TypedPacketReceiveHandler(OnUnicodeMessage);
            PacketRegistry.UpdateCurrentHealth += new TypedPacketReceiveHandler(OnUpdateCurrentHealth);
            PacketRegistry.UpdateCurrentMana += new TypedPacketReceiveHandler(OnUpdateCurrentMana);
            PacketRegistry.UpdateCurrentStamina += new TypedPacketReceiveHandler(OnUpdateCurrentStamina);
            PacketRegistry.VendorBuyList += new TypedPacketReceiveHandler(OnVendorBuyList);
            PacketRegistry.VendorSellList += new TypedPacketReceiveHandler(OnVendorSellList);
            PacketRegistry.VersionRequest += new TypedPacketReceiveHandler(OnVersionRequest);
            PacketRegistry.WarMode += new TypedPacketReceiveHandler(OnWarMode);
            PacketRegistry.Weather += new TypedPacketReceiveHandler(OnWeather);
            PacketRegistry.WorldItem += new TypedPacketReceiveHandler(OnWorldItem);
            PacketRegistry.WornItem += new TypedPacketReceiveHandler(OnWornItem);
            #endregion*/
        }

        #region Event Calls from PacketHandler
        private void OnWornItem(IRecvPacket packet)
        {
            SendEvent(this, Events.WORN_ITEM, packet);
        }

        private void OnWorldItem(IRecvPacket packet)
        {
            SendEvent(this, Events.WORLD_ITEM, packet);
        }

        private void OnWeather(IRecvPacket packet)
        {
            SendEvent(this, Events.WEATHER, packet);
        }

        private void OnWarMode(IRecvPacket packet)
        {
            SendEvent(this, Events.WAR_MODE, packet);
        }

        private void OnVersionRequest(IRecvPacket packet)
        {
            SendEvent(this, Events.VERSION_REQUEST, packet);
        }

        private void OnVendorSellList(IRecvPacket packet)
        {
            SendEvent(this, Events.VENDOR_SELL_LIST, packet);
        }

        private void OnVendorBuyList(IRecvPacket packet)
        {
            SendEvent(this, Events.VENDOR_BUY_LIST, packet);
        }

        private void OnUpdateCurrentStamina(IRecvPacket packet)
        {
            SendEvent(this, Events.UPDATE_CURRENT_STAMINA, packet);
        }

        private void OnUpdateCurrentMana(IRecvPacket packet)
        {
            SendEvent(this, Events.UPDATE_CURRENT_MANA, packet);
        }

        private void OnUpdateCurrentHealth(IRecvPacket packet)
        {
            SendEvent(this, Events.UPDATE_CURRENT_HEALTH, packet);
        }

        private void OnUnicodeMessage(IRecvPacket packet)
        {
            SendEvent(this, Events.UNICODE_MESSAGE, packet);
        }

        private void OnToolTipRevision(IRecvPacket packet)
        {
            SendEvent(this, Events.TOOLTIP_REVISION, packet);
        }

        private void OnTipNoticeWindow(IRecvPacket packet)
        {
            SendEvent(this, Events.TIP_NOTICE_WINDOW, packet);
        }

        private void OnTime(IRecvPacket packet)
        {
            SendEvent(this, Events.TIME, packet);
        }

        private void OnTargetCursor(IRecvPacket packet)
        {
            SendEvent(this, Events.TARGET_CURSOR, packet);
        }

        private void OnSwing(IRecvPacket packet)
        {
            SendEvent(this, Events.SWING, packet);
        }

        private void OnSupportedFeatures(IRecvPacket packet)
        {
            SendEvent(this, Events.SUPPORTED_FEATURES, packet);
        }

        private void OnSkillsList(IRecvPacket packet)
        {
            SendEvent(this, Events.SKILLS_LIST, packet);
        }

        private void OnServerRelay(IRecvPacket packet)
        {
            SendEvent(this, Events.SERVER_RELAY, packet);
        }

        private void OnServerList(IRecvPacket packet)
        {
            SendEvent(this, Events.SERVER_LIST, packet);
        }

        private void OnSeasonChange(IRecvPacket packet)
        {
            SendEvent(this, Events.SEASON_CHANGE, packet);
        }

        private void OnResurrectMenu(IRecvPacket packet)
        {
            SendEvent(this, Events.RESURRECT_MENU, packet);
        }

        private void OnRequestNameResponse(IRecvPacket packet)
        {
            SendEvent(this, Events.REQUEST_NAME_RESPONSE, packet);
        }

        private void OnRemoveEntity(IRecvPacket packet)
        {
            SendEvent(this, Events.REMOVE_ENTITY, packet);
        }

        private void OnQuestArrow(IRecvPacket packet)
        {
            SendEvent(this, Events.QUEST_ARROW, packet);
        }

        private void OnPopupMessage(IRecvPacket packet)
        {
            SendEvent(this, Events.POPUP_MESSAGE, packet);
        }

        private void OnPlaySoundEffect(IRecvPacket packet)
        {
            SendEvent(this, Events.PLAY_SOUND_EFFECT, packet);
        }

        private void OnPlayMusic(IRecvPacket packet)
        {
            SendEvent(this, Events.PLAY_MUSIC, packet);
        }

        private void OnPlayerMove(IRecvPacket packet)
        {
            SendEvent(this, Events.PLAYER_MOVE, packet);
        }

        private void OnPersonalLightLevel(IRecvPacket packet)
        {
            SendEvent(this, Events.PERSONAL_LIGHT_LEVEL, packet);
        }

        private void OnParticleEffect(IRecvPacket packet)
        {
            SendEvent(this, Events.PARTICL_EEFFECT, packet);
        }

        private void OnOverallLightLevel(IRecvPacket packet)
        {
            SendEvent(this, Events.OVERALL_LIGHT_LEVEL, packet);
        }

        private void OnOpenWebBrowser(IRecvPacket packet)
        {
            SendEvent(this, Events.OPEN_WEBBROWSER, packet);
        }

        private void OnOpenPaperdoll(IRecvPacket packet)
        {
            SendEvent(this, Events.OPEN_PAPERDOLL, packet);
        }

        private void OnOpenContainer(IRecvPacket packet)
        {
            SendEvent(this, Events.OPEN_CONTAINER, packet);
        }

        private void OnObjectHelpResponse(IRecvPacket packet)
        {
            SendEvent(this, Events.OBJECT_HELP_RESPONSE, packet);
        }

        private void OnNewSubserver(IRecvPacket packet)
        {
            SendEvent(this, Events.NEW_SUBSERVER, packet);
        }

        private void OnMovementRejected(IRecvPacket packet)
        {
            SendEvent(this, Events.MOVEMENT_REJECTED, packet);
        }

        private void OnMoveAcknowledged(IRecvPacket packet)
        {
            SendEvent(this, Events.MOVE_ACKNOWLEDGED, packet);
        }

        private void OnMobileUpdate(IRecvPacket packet)
        {
            SendEvent(this, Events.MOBILE_UPDATE, packet);
        }

        private void OnMobileStatusCompact(IRecvPacket packet)
        {
            SendEvent(this, Events.MOBILE_STATUS_COMPACT, packet);
        }

        private void OnMobileMoving(IRecvPacket packet)
        {
            SendEvent(this, Events.MOBILE_MOVING, packet);
        }

        private void OnMobileIncoming(IRecvPacket packet)
        {
            SendEvent(this, Events.MOBILE_INCOMING, packet);
        }

        private void OnMobileAttributes(IRecvPacket packet)
        {
            SendEvent(this, Events.MOBILE_ATTRIBUTES, packet);
        }

        private void OnMobileAnimation(IRecvPacket packet)
        {
            SendEvent(this, Events.MOBILE_ANIMATION, packet);
        }

        private void OnMessageLocalizedAffix(IRecvPacket packet)
        {
            SendEvent(this, Events.MESSAGE_LOCALIZED_AFFIX, packet);
        }

        private void OnMessageLocalized(IRecvPacket packet)
        {
            SendEvent(this, Events.MESSAGE_LOCALIZED, packet);
        }

        private void OnMegaCliloc(IRecvPacket packet)
        {
            SendEvent(this, Events.MEGA_CLILOC, packet);
        }

        private void OnLoginRejection(IRecvPacket packet)
        {
            SendEvent(this, Events.LOGIN_REJECTION, packet);
        }

        private void OnLoginConfirm(IRecvPacket packet)
        {
            SendEvent(this, Events.LOGIN_CONFIRM, packet);
        }

        private void OnLoginComplete(IRecvPacket packet)
        {
            SendEvent(this, Events.LOGIN_COMPLETE, packet);
        }

        private void OnLiftRejection(IRecvPacket packet)
        {
            SendEvent(this, Events.LIFT_REJECTION, packet);
        }

        private void OnInvalidMapEnable(IRecvPacket packet)
        {
            SendEvent(this, Events.INVALID_MAP_ENABLE, packet);
        }

        private void OnHuedEffect(IRecvPacket packet)
        {
            SendEvent(this, Events.HUED_EFFECT, packet);
        }

        private void OnGraphicalEffect1(IRecvPacket packet)
        {
            SendEvent(this, Events.GRAPHICAL_EFFECT1, packet);
        }

        private void OnGlobalQueueCount(IRecvPacket packet)
        {
            SendEvent(this, Events.GLOBAL_QUEUE_COUNT, packet);
        }

        private void OnGeneralInfo(IRecvPacket packet)
        {
            SendEvent(this, Events.GENERAL_INFO, packet);
        }

        private void OnDragEffect(IRecvPacket packet)
        {
            SendEvent(this, Events.DRAG_EFFECT, packet);
        }

        private void OnDisplayMenu(IRecvPacket packet)
        {
            SendEvent(this, Events.DISPLAY_MENU, packet);
        }

        private void OnDisplayGumpFast(IRecvPacket packet)
        {
            SendEvent(this, Events.DISPLAY_GUMP_FAST, packet);
        }

        private void OnDeathAnimation(IRecvPacket packet)
        {
            SendEvent(this, Events.DEATH_ANIMATION, packet);
        }

        private void OnDamage(IRecvPacket packet)
        {
            SendEvent(this, Events.DAMAGE, packet);
        }

        private void OnCorpseClothing(IRecvPacket packet)
        {
            SendEvent(this, Events.CORPSE_CLOTHING, packet);
        }

        private void OnContainerContentUpdate(IRecvPacket packet)
        {
            SendEvent(this, Events.CONTAINER_CONTENT_UPDATE, packet);
        }

        private void OnContainerContent(IRecvPacket packet)
        {
            SendEvent(this, Events.CONTAINER_CONTENT, packet);
        }

        private void OnCompressedGump(IRecvPacket packet)
        {
            SendEvent(this, Events.COMPRESSED_GUMP, packet);
        }

        private void OnCharactersStartingLocations(IRecvPacket packet)
        {
            SendEvent(this, Events.CHARACTERS_STARTING_LOCATIONS, packet);
        }

        private void OnCharacterListUpdate(IRecvPacket packet)
        {
            SendEvent(this, Events.CHARACTER_LIST_UPDATE, packet);
        }

        private void OnChangeCombatant(IRecvPacket packet)
        {
            SendEvent(this, Events.CHANGE_COMBATANT, packet);
        }

        private void OnAsciiMessage(IRecvPacket packet)
        {
            SendEvent(this, Events.ASCII_MESSAGE, packet);
        }
        #endregion

        public void Register(IEventReceiver receiver, short eventId)
        {
            if (!_registeredAllEvents.Contains(receiver))
            {
                if (_registeredEvents.ContainsKey(eventId) &&
                    !_registeredEvents[eventId].Contains(receiver))
                {
                    _log.Debug("Registering Event Id: {0} Type: {1}", eventId, receiver);
                    _registeredEvents[eventId].Add(receiver);
                }
                else
                {
                    _log.Debug("Registering Event Id: {0} Type: {1}", eventId, receiver);
                    _registeredEvents.Add(eventId, new List<IEventReceiver>());
                    _registeredEvents[eventId].Add(receiver);
                }
            }
        }

        public void Unregister(IEventReceiver receiver, short eventId)
        {
            if (_registeredEvents.ContainsKey(eventId) &&
                _registeredEvents[eventId].Contains(receiver))
            {
                _log.Debug("Unregistering Event Id: {0} Type: {1}", eventId, receiver);
                _registeredEvents[eventId].Remove(receiver);
            }
        }

        public void RegisterAll(IEventReceiver receiver)
        {
            if (!_registeredAllEvents.Contains(receiver))
            {
                _log.Debug("Registering All Events Type: {1}",  receiver);
                _registeredAllEvents.Add(receiver);
            }
        }

        public void UnregisterAll(IEventReceiver receiver)
        {
            if (_registeredAllEvents.Contains(receiver))
            {
                _log.Debug("Unregistering All Events Type: {1}",  receiver);
                _registeredAllEvents.Add(receiver);
            }
        }

        public void SendEvent(object sender, short eventId, params object[] args)
        {
            // _log.Debug("ProcessEvent Event Id: {0} Sender: {1}", eventId, sender);

            List<IEventReceiver> receivers = new List<IEventReceiver>(_registeredAllEvents);

            if (_registeredEvents.ContainsKey(eventId))
            {
                receivers.AddRange(_registeredEvents[eventId]);
            }

            for (int i = 0; i < receivers.Count; i++)
            {
                receivers[i].SendEvent(sender, eventId, args);
            }
        }

        public bool IsRegistered(IEventReceiver receiver, short eventId)
        {
            if (_registeredAllEvents.Contains(receiver))
            {
                return true;
            }

            bool registered = false;
            
            if (_registeredEvents.ContainsKey(eventId))
            {
                List<IEventReceiver> receivers = _registeredEvents[eventId];

                for (int i = 0; i < receivers.Count && !registered; i++)
                {
                    registered = receivers[i] == receiver;
                }
            }

            return registered;
        }
    }
}
