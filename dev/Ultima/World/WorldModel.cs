using InterXLib.Patterns.MVC;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Login;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.World.Gumps;
using UltimaXNA.Ultima.World.Controllers;
using UltimaXNA.Ultima.World.Maps;

namespace UltimaXNA.Ultima.World
{
    class WorldModel : AUltimaModel
    {
        private INetworkClient m_Network;
        private UserInterfaceService m_UserInterface;
        private UltimaEngine m_Engine;

        public EntityManager Entities
        {
            get;
            private set;
        }

        public EffectsManager Effects
        {
            get;
            private set;
        }

        public WorldClient Client
        {
            get;
            private set;
        }

        public WorldInput Input
        {
            get;
            private set;
        }

        public WorldInteraction Interaction
        {
            get;
            private set;
        }

        private WorldCursor m_Cursor = null;
        public WorldCursor Cursor
        {
            get { return m_Cursor; }
            set
            {
                if (m_Cursor != null)
                {
                    m_Cursor.Dispose();
                }
                m_Cursor = value;
            }
        }

        private Map m_map = null;
        public Map Map
        {
            get { return m_map; }
        }

        public int MapIndex
        {
            get
            {
                if (m_map == null)
                    return -1;
                else
                    return m_map.Index;
            }
            set
            {
                if (value != MapIndex)
                {
                    // clear all entities
                    EntityManager.Reset(false);
                    if (m_map != null)
                    {
                        AEntity player = EntityManager.GetPlayerObject();
                        // save current player position
                        int x = player.X, y = player.Y, z = player.Z;
                        // place the player in null space (allows the map to be reloaded when we return to the same location in a different map).
                        player.SetMap(null);
                        // dispose of map
                        m_map.Dispose();
                        m_map = null;
                        // add new map!
                        m_map = new Map(value);
                        player.SetMap(m_map);
                        // restore previous player position
                        player.Position.Set(x, y, z);
                    }
                    else
                    {
                        AEntity player = EntityManager.GetPlayerObject();
                        m_map = new Map(value);
                        player.SetMap(m_map);
                    }
                }
            }
        }

        public WorldModel()
            : base()
        {
            UltimaServices.Register<WorldModel>(this);

            m_Engine = UltimaServices.GetService<UltimaEngine>();
            m_Network = UltimaServices.GetService<INetworkClient>();
            m_UserInterface = UltimaServices.GetService<UserInterfaceService>();

            Entities = new EntityManager(this);
            EntityManager.Reset(true);

            Effects = new EffectsManager(this);
            Input = new WorldInput(this);
            Interaction = new WorldInteraction(this);
            Client = new WorldClient(this);
        }

        protected override AView CreateView()
        {
            //TODO: this needs to be resolved, not newed up.
            return new WorldView(this);
        }

        protected override void OnInitialize()
        {
            m_Engine.SetupWindowForWorld();

            m_UserInterface.Cursor = Cursor = new WorldCursor(this);
            Client.Initialize();
        }

        public void LoginSequence()
        {
            m_UserInterface.AddControl(new WorldViewGump(), 0, 0); // world gump will always place itself correctly.
            m_UserInterface.AddControl(new TopMenuGump(0), 0, 0);

            Client.SendWorldLoginPackets();
            EngineVars.InWorld = true;
        }

        protected override void OnDispose()
        {
            m_Engine.SaveResolution();

            UltimaServices.Unregister<WorldModel>();

            EntityManager.Reset();
            Entities = null;

            Effects = null;

            Input.Dispose();
            Input = null;

            Interaction = null;

            Client.Dispose();
            Client = null;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (!m_Network.IsConnected)
            {
                if (m_UserInterface.IsModalControlOpen == false)
                {
                    MsgBox g = m_UserInterface.MsgBox("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
                    g.OnClose = onCloseLostConnectionMsgBox;
                }
            }
            else
            {
                Input.Update(frameMS);
                EntityManager.Update(frameMS);
                Effects.Update(frameMS);
                StaticManager.Update(frameMS);
            }
        }

        public void Disconnect()
        {
            m_Network.Disconnect();
            EngineVars.InWorld = false;
            m_Engine.ActiveModel = new LoginModel();
        }

        void onCloseLostConnectionMsgBox()
        {
            Disconnect();
        }

        public int MapCount { get; set; }

        public byte Season { get; set; }
    }
}
