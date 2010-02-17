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
    class ClientVars : GameComponent
    {
        static IInputService Input;
        static IUIManager UserInterface;
        static IWorld World;
        public ClientVars(Game game)
            : base(game)
        {
            Input = Game.Services.GetService<IInputService>();
            UserInterface = Game.Services.GetService<IUIManager>();
            World = game.Services.GetService<IWorld>();
            EngineRunning = true;
            InWorld = false;
        }

        public override void Update(GameTime gameTime)
        {
            BackBufferWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            BackBufferHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            _theTime = gameTime;

            // input for debug variables.
            if (Input.IsKeyDown(Keys.LeftAlt))
            {
                if (Input.IsKeyPress(Keys.D))
                {
                    if (!DEBUG_ShowDataRead)
                        DEBUG_ShowDataRead = true;
                    else
                    {
                        if (!DEBUG_BreakdownDataRead)
                            DEBUG_BreakdownDataRead = true;
                        else
                        {
                            DEBUG_ShowDataRead = false;
                            DEBUG_BreakdownDataRead = false;
                        }
                    }
                }

                if (Input.IsKeyPress(Keys.F))
                    DEBUG_DisplayFPS = Utility.ToggleBoolean(DEBUG_DisplayFPS);
            }

            updateFPS();
            base.Update(gameTime);
        }

        public static int BackBufferWidth = 0, BackBufferHeight = 0;

        public static MouseButtons MouseButton_Interact = MouseButtons.LeftButton;
        public static MouseButtons MouseButton_Move = MouseButtons.RightButton;

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
        public static float TheTime
        {
            get { return (float)_theTime.TotalRealTime.TotalSeconds; }
        }

        public static Direction CursorDirection { get; internal set; }
        
        // InWorld allows us to tell when our character object has been loaded in the world.
        public static bool InWorld { get; set; }

        public static bool EngineRunning { get; set; } // false = engine immediately quits.
        public static bool IsMinimized { get; set; }

        public const float SecondsBetweenClickAndPickUp = 0.8f; // this is close to what the legacy client uses.
        public const float SecondsForDoubleClick = 0.5f;

        // Maintain an accurate count of frames per second.
        static float _FPS = 0, _Frames = 0, _ElapsedSeconds = 0;
        static bool updateFPS()
        {
            _Frames++;
            _ElapsedSeconds += (float)_theTime.ElapsedRealTime.TotalSeconds;
            if (_ElapsedSeconds >= .5f)
            {
                _FPS = _Frames / _ElapsedSeconds;
                _ElapsedSeconds = 0;
                _Frames = 0;
                return true;
            }
            return false;
        }

        public static bool DEBUG_ShowDataRead = false;
        public static bool DEBUG_BreakdownDataRead = false;
        public static bool DEBUG_DisplayFPS = true;
        public static bool DEBUG_HighlightMouseOverObjects = false;
        public static bool DEBUG_DrawWireframe = false;

        // Debug message - I put a lot of crap in here to test values. Feel free to add or remove variables.
        public static string DebugMessage { get { return generateDebugMessage(); } }
        static string generateDebugMessage()
        {
            String debugMessage = string.Empty;

            if (DEBUG_DisplayFPS)
                debugMessage += string.Format("FPS: {0}\n", (int)_FPS);

            if (DEBUG_ShowDataRead)
            {
                if (DEBUG_BreakdownDataRead)
                    debugMessage += Metrics.DataReadBreakdown;
                else
                    debugMessage += string.Format("Data Read: {0}\n", Metrics.TotalDataRead.ToString());
            }

            if (ClientVars.Map != -1 && !UserInterface.IsMouseOverUI)
            {
                debugMessage += string.Format("#Objects: {0}\n", World.ObjectsRendered);
                debugMessage += string.Format("Warmode: {0}\n", ClientVars.WarMode);
                if (World.MouseOverObject != null)
                {
                    debugMessage += string.Format("OBJECT: {0}\n", World.MouseOverObject.ToString());
                    if (World.MouseOverObject is MapObjectStatic)
                    {
                        debugMessage += string.Format("ArtID: {0}\n", ((MapObjectStatic)World.MouseOverObject).ItemID);
                    }
                    else if (World.MouseOverObject is MapObjectMobile)
                    {
                        Mobile iUnit = EntitiesCollection.GetObject<Mobile>(World.MouseOverObject.OwnerSerial, false);
                        if (iUnit != null)
                            debugMessage += string.Format("Name: {0}\n", iUnit.Name);
                        debugMessage += string.Format("AnimID: {0}\nSerial: {1}\nHue: {2}",
                            ((MapObjectMobile)World.MouseOverObject).BodyID, World.MouseOverObject.OwnerSerial, ((MapObjectMobile)World.MouseOverObject).Hue);
                    }
                    else if (World.MouseOverObject is MapObjectItem)
                    {
                        debugMessage +=
                            "ArtID: " + ((MapObjectItem)World.MouseOverObject).ItemID + Environment.NewLine +
                            "Serial: " + World.MouseOverObject.OwnerSerial;
                    }
                    debugMessage += " Z: " + World.MouseOverObject.Z;
                }
                else
                {
                    debugMessage += "OVER: " + "null";
                }
                if (World.MouseOverGround != null)
                {
                    debugMessage += Environment.NewLine + "GROUND: " + World.MouseOverGround.Position.ToString();
                }
                else
                {
                    debugMessage += Environment.NewLine + "GROUND: null";
                }
            }
            return debugMessage;
        }
    }
}
