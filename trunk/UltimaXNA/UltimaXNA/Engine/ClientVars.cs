using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Network;
using UltimaXNA.Network.Packets.Server;
using UltimaXNA.Data;
using UltimaXNA.Client;
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.UI;
using UltimaXNA.Input;
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.TileEngine;
using UltimaXNA.UILegacy;

namespace UltimaXNA
{
    class ClientVars
    {
        static ServerListPacket _serverListPacket;
        public static ServerListPacket ServerListPacket { get { return _serverListPacket; } set { _serverListPacket = value; } }

        static bool _charListReloaded = false;
        static CharacterListEntry[] _characters;
        public static bool CharacterList_Reloaded { get { return _charListReloaded; } set { _charListReloaded = value; } }
        public static CharacterListEntry[] CharacterList { get { return _characters; } set { _characters = value; _charListReloaded = true; } }
        static StartingLocation[] _locations;
        public static StartingLocation[] StartingLocations { get { return _locations; } set { _locations = value; } }
        public static int CharacterList_FirstEmptySlot
        {
            get
            {
                for (int i = 0; i < _characters.Length; i++)
                {
                    if (_characters[i].Name == string.Empty)
                        return i;
                }
                return -1;
            }
        }

        static int _map = -1;
        public static int Map { get { return _map; } set { _map = value; } }

        static int _mapCount = -1;
        public static int MapCount { get { return _mapCount; } set { _mapCount = value; } }

        static uint _featureFlags;
        public static uint FeatureFlags { get { return _featureFlags; } set { _featureFlags |= value; } }
        public static bool EnableT2A { get { return ((_featureFlags & 0x1) != 0); } }
        public static bool EnableRen { get { return ((_featureFlags & 0x2) != 0); } }
        public static bool EnableThirdDawn { get { return ((_featureFlags & 0x4) != 0); } }
        public static bool EnableLBR { get { return ((_featureFlags & 0x8) != 0); } }
        public static bool EnableAOS { get { return ((_featureFlags & 0x10) != 0); } }
        public static bool Enable6CharSlots { get { return ((_featureFlags & 0x20) != 0); } }
        public static bool EnableSE { get { return ((_featureFlags & 0x40) != 0); } }
        public static bool EnableML { get { return ((_featureFlags & 0x80) != 0); } }
        public static bool Enable8thSplash { get { return ((_featureFlags & 0x100) != 0); } }
        public static bool Enable9thSplash { get { return ((_featureFlags & 0x200) != 0); } }
        public static bool Enable10thAge { get { return ((_featureFlags & 0x400) != 0); } }
        public static bool EnableMoreStorage { get { return ((_featureFlags & 0x800) != 0); } }
        public static bool Enable7CharSlots { get { return ((_featureFlags & 0x1000) != 0); } }
        public static bool Enable10thAgeFaces { get { return ((_featureFlags & 0x2000) != 0); } }
        public static bool EnableTrialAccount { get { return ((_featureFlags & 0x4000) != 0); } }
        public static bool Enable11thAge { get { return ((_featureFlags & 0x8000) != 0); } }
        public static bool EnableSA { get { return ((_featureFlags & 0x10000) != 0); } }

        static int _season = 0;
        public static int Season { get { return _season; } set { _season = value; } }

        static bool _minimapLarge = false;
        public static bool MiniMap_LargeFormat { get { return _minimapLarge; } set { _minimapLarge = value; } }

        private static Serial _lastTarget;
        public static Serial LastTarget
        {
            get { return _lastTarget; }
            set
            {
                _lastTarget = value;
                UltimaClient.Send(new GetPlayerStatusPacket(0x04, _lastTarget));
            }
        }
        public static bool WarMode
        {
            get { return (EntitiesCollection.GetPlayerObject() != null) ? ((Mobile)EntitiesCollection.GetPlayerObject()).IsWarMode : false; }
            set { ((Mobile)EntitiesCollection.GetPlayerObject()).IsWarMode = value; }
        }
        static GameTime _theTime;
        public static GameTime GameTime
        {
            set { _theTime = value; }
        }
        public static float TheTime
        {
            get { return (float)_theTime.TotalRealTime.TotalSeconds; }
        }

        public static Direction CursorDirection { get; internal set; }
        
        // InWorld allows us to tell when our character object has been loaded in the world.
        public static bool InWorld { get; set; }
        // Set EngineRunning to false to cause the engine to immediately exit.
        public static bool EngineRunning { get; set; }
        public static bool IsMinimized { get; set; }

        public const float SecondsBetweenClickAndPickUp = 0.8f; // this is close to what the legacy client uses.
        public const float SecondsForDoubleClick = 0.5f;
    }
}
