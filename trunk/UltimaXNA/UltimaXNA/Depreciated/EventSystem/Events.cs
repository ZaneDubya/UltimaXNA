using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.EventSystem
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class LuaExposedEvent : Attribute
    {
        public LuaExposedEvent() 
        {   
        }
    }

    public static class Events
    {
        [LuaExposedEvent]
        public const int ASCII_MESSAGE = 0;
        [LuaExposedEvent]
        public const int CHANGE_COMBATANT = 0;
        public const int CHARACTER_LIST_UPDATE = 0;
        public const int CHARACTERS_STARTING_LOCATIONS = 0;
        public const int COMPRESSED_GUMP = 0;
        [LuaExposedEvent]
        public const int CONTAINER_CONTENT = 0;
        [LuaExposedEvent]
        public const int CONTAINER_CONTENT_UPDATE = 0;
        public const int CORPSE_CLOTHING = 0;
        [LuaExposedEvent]
        public const int DAMAGE = 0;
        [LuaExposedEvent]
        public const int DEATH_ANIMATION = 0;
        public const int DISPLAY_GUMP_FAST = 0;
        public const int DISPLAY_MENU = 0;
        public const int DRAG_EFFECT = 0;
        [LuaExposedEvent]
        public const int GENERAL_INFO = 0;
        public const int GLOBAL_QUEUE_COUNT = 0;
        public const int GRAPHICAL_EFFECT1 = 0;
        public const int HUED_EFFECT = 0;
        public const int INVALID_MAP_ENABLE = 0;
        public const int LIFT_REJECTION = 0;
        public const int LOGIN_COMPLETE = 0;
        public const int LOGIN_CONFIRM = 0;
        public const int LOGIN_REJECTION = 0;
        [LuaExposedEvent]
        public const int MEGA_CLILOC = 0;
        [LuaExposedEvent]
        public const int MESSAGE_LOCALIZED = 0;
        [LuaExposedEvent]
        public const int MESSAGE_LOCALIZED_AFFIX = 0;
        public const int MOBILE_ANIMATION = 0;
        [LuaExposedEvent]
        public const int MOBILE_ATTRIBUTES = 0;
        [LuaExposedEvent]
        public const int MOBILE_INCOMING = 0;
        public const int MOBILE_MOVING = 0;
        [LuaExposedEvent]
        public const int MOBILE_STATUS_COMPACT = 0;
        [LuaExposedEvent]
        public const int MOBILE_UPDATE = 0;
        public const int MOVE_ACKNOWLEDGED = 0;
        public const int MOVEMENT_REJECTED = 0;
        public const int NEW_SUBSERVER = 0;
        public const int OBJECT_HELP_RESPONSE = 0;
        [LuaExposedEvent]
        public const int OPEN_CONTAINER = 0;
        [LuaExposedEvent]
        public const int OPEN_PAPERDOLL = 0;
        [LuaExposedEvent]
        public const int OPEN_WEBBROWSER = 0;
        [LuaExposedEvent]
        public const int OVERALL_LIGHT_LEVEL = 0;
        public const int PARTICL_EEFFECT = 0;
        [LuaExposedEvent]
        public const int PERSONAL_LIGHT_LEVEL = 0;
        public const int PLAYER_MOVE = 0;
        [LuaExposedEvent]
        public const int PLAY_MUSIC = 0;
        [LuaExposedEvent]
        public const int PLAY_SOUND_EFFECT = 0;
        public const int POPUP_MESSAGE = 0;
        [LuaExposedEvent]
        public const int QUEST_ARROW = 0;
        [LuaExposedEvent]
        public const int REMOVE_ENTITY = 0;
        public const int REQUEST_NAME_RESPONSE = 0;
        [LuaExposedEvent]
        public const int RESURRECT_MENU = 0;
        [LuaExposedEvent]
        public const int SEASON_CHANGE = 0;
        public const int SERVER_LIST = 0;
        public const int SERVER_RELAY = 0;
        [LuaExposedEvent]
        public const int SKILLS_LIST = 0;
        [LuaExposedEvent]
        public const int SUPPORTED_FEATURES = 0;
        [LuaExposedEvent]
        public const int SWING = 0;
        [LuaExposedEvent]
        public const int TARGET_CURSOR = 0;
        [LuaExposedEvent]
        public const int TIME = 0;
        public const int TIP_NOTICE_WINDOW = 0;
        public const int TOOLTIP_REVISION = 0;
        [LuaExposedEvent]
        public const int UNICODE_MESSAGE = 0;
        [LuaExposedEvent]
        public const int UPDATE_CURRENT_HEALTH = 0;
        [LuaExposedEvent]
        public const int UPDATE_CURRENT_MANA = 0;
        [LuaExposedEvent]
        public const int UPDATE_CURRENT_STAMINA = 0;
        [LuaExposedEvent]
        public const int VENDOR_BUY_LIST = 0;
        [LuaExposedEvent]
        public const int VENDOR_SELL_LIST = 0;
        public const int VERSION_REQUEST = 0;
        [LuaExposedEvent]
        public const int WAR_MODE = 0;
        [LuaExposedEvent]
        public const int WEATHER = 0;
        public const int WORLD_ITEM = 0;
        public const int WORN_ITEM = 0;
    }
}
