using InterXLib.Patterns.MVC;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.WorldGumps;
using UltimaXNA.UltimaWorld.Controllers;
using UltimaXNA.UltimaWorld.Maps;

namespace UltimaXNA.UltimaWorld
{
    class WorldModel : AUltimaModel
    {
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

        private WorldClient m_WorldClient;

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
                    // clear all entities except for player.
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
        {
            Entities = new EntityManager(this);
            EntityManager.Reset(true);
            Effects = new EffectsManager(this);
            Input = new WorldInput(this);
            Interaction = new WorldInteraction(this);
            m_WorldClient = new WorldClient(this);
        }

        protected override AView CreateView()
        {
            return new WorldView(this);
        }

        protected override void OnInitialize()
        {
            Engine.UserInterface.Cursor = Cursor = new WorldCursor(this);
            m_WorldClient.Initialize();
        }

        public void LoginSequence()
        {
            Engine.UserInterface.AddControl(new TopMenu(0), 0, 0);
            Engine.UserInterface.AddControl(new ChatWindow(), 0, 0);
            m_WorldClient.SendWorldLoginPackets();
            UltimaVars.EngineVars.InWorld = true;
        }

        protected override void OnDispose()
        {
            m_WorldClient.Dispose();
            m_WorldClient = null;

            Input.Dispose();
            Input = null;

            EntityManager.Reset();
            Entities = null;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (UltimaVars.EngineVars.InWorld)
            {
                if (!Engine.Client.IsConnected)
                {
                    if (Engine.UserInterface.IsModalControlOpen == false)
                    {
                        MsgBox g = Engine.UserInterface.MsgBox("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
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
            else
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            Engine.Client.Disconnect();
            UltimaVars.EngineVars.InWorld = false;
            Engine.ActiveModel = new UltimaXNA.UltimaLogin.LoginModel();
        }

        void onCloseLostConnectionMsgBox()
        {
            Disconnect();
        }
    }
}
