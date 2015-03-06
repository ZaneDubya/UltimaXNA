using InterXLib.Input.Windows;
using InterXLib.Patterns.MVC;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.Gumps;
using UltimaXNA.UltimaWorld.Controller;
using UltimaXNA.UltimaWorld.View;

namespace UltimaXNA.UltimaWorld
{
    class WorldModel : Core.AUltimaModel
    {
        private EntityManager m_Entities;
        public EntityManager Entities
        {
            get { return m_Entities; }
        }

        private WorldInput m_WorldInput;
        public WorldInput Input
        {
            get { return m_WorldInput; }
        }

        private WorldClient m_WorldClient;

        private UltimaCursor m_Cursor = null;
        public UltimaCursor Cursor
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

        public WorldModel()
        {
            m_Entities = new EntityManager();
            m_WorldInput = new WorldInput(this);
            m_WorldClient = new WorldClient(this);

            UltimaEngine.UserInterface.Cursor = new Model.WorldCursor(this);
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

            IsometricRenderer.LightDirection = -0.6f;
            UltimaVars.EngineVars.InWorld = true;
        }

        protected override void OnDispose()
        {
            m_WorldClient.Dispose();
            m_WorldClient = null;
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
                IsometricRenderer.CenterPosition = EntityManager.GetPlayerObject().Position;
                IsometricRenderer.Update(totalMS, frameMS);


                // Toggle for logout
                if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.Q, false, false, true))
                    Disconnect();
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
