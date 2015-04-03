using InterXLib.Patterns.MVC;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.WorldGumps;
using UltimaXNA.UltimaWorld.Maps;

namespace UltimaXNA.UltimaWorld
{
    class WorldModel : AUltimaModel
    {
        private EntityManager m_Entities;
        public EntityManager Entities
        {
            get { return m_Entities; }
        }

        private EffectsManager m_Effects;
        public EffectsManager Effects
        {
            get { return m_Effects; }
        }

        private WorldInput m_WorldInput;
        public WorldInput Input
        {
            get { return m_WorldInput; }
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
                    if (m_map != null)
                    {
                        // clear all entities
                        EntityManager.Reset(false);
                        UltimaEntities.AEntity player = EntityManager.GetPlayerObject();
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
                        m_map = new Map(value);
                    }
                }
            }
        }

        public WorldModel()
        {
            m_Entities = new EntityManager(this);
            m_Effects = new EffectsManager(this);
            m_WorldInput = new WorldInput(this);
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
            m_WorldClient.AfterLoginSequence();

            Engine.UserInterface.AddControl(new TopMenu(0), 0, 0);
            Engine.UserInterface.AddControl(new ChatWindow(), 0, 0);

            UltimaVars.EngineVars.InWorld = true;
        }

        protected override void OnDispose()
        {
            m_WorldClient.Dispose();
            m_WorldClient = null;

            m_WorldInput.Dispose();
            m_WorldInput = null;

            EntityManager.Reset();
            m_Entities = null;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (!Engine.Client.IsConnected)
            {
                if (Engine.UserInterface.IsModalControlOpen == false)
                {
                    MsgBox g = WorldInteraction.MsgBox("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
                    g.OnClose = onCloseLostConnectionMsgBox;
                }
            }
            else
            {
                m_WorldInput.Update(frameMS);
                EntityManager.Update(frameMS);
                m_Effects.Update(frameMS);
                StaticManager.Update(frameMS);
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
