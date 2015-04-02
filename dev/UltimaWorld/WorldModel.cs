using InterXLib.Input.Windows;
using InterXLib.Patterns.MVC;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.WorldGumps;
using UltimaXNA.UltimaWorld.Controller;
using UltimaXNA.UltimaWorld.Model;
using Microsoft.Xna.Framework;

namespace UltimaXNA.UltimaWorld
{
    class WorldModel : Core.AUltimaModel
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

        private Model.WorldCursor m_Cursor = null;
        public Model.WorldCursor Cursor
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
                        // clear all entities and re-add the player 
                        UltimaEntities.AEntity player = EntityManager.GetPlayerObject();
                        Point3D tile = new Point3D(player.X, player.Y, player.Z);
                        player.SetMap(null);
                        EntityManager.Reset();
                        EntityManager.AddEntity(player);
                        // dispose of map
                        m_map.Dispose();
                        m_map = null;
                        m_map = new Map(value);
                        player.SetMap(m_map);
                        player.Position.Set(tile.X, tile.Y, tile.Z);
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

            UltimaEngine.UserInterface.Cursor = Cursor = new Model.WorldCursor(this);
        }

        protected override AView CreateView()
        {
            return new WorldView(this);
        }

        protected override void OnInitialize()
        {
            m_WorldClient.Initialize();
            m_WorldClient.AfterLoginSequence();

            UltimaEngine.UserInterface.AddControl(new TopMenu(0), 0, 0);
            UltimaEngine.UserInterface.AddControl(new ChatWindow(), 0, 0);

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
            if (!Client.IsConnected)
            {
                if (UltimaEngine.UserInterface.IsModalControlOpen == false)
                {
                    MsgBox g = UltimaInteraction.MsgBox("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
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
            Client.Disconnect();
            UltimaVars.EngineVars.InWorld = false;
            UltimaEngine.ActiveModel = new UltimaXNA.UltimaLogin.LoginModel();
        }

        void onCloseLostConnectionMsgBox()
        {
            Disconnect();
        }
    }
}
