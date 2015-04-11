using InterXLib.Patterns.MVC;
using UltimaXNA.Ultima.Login.States;
using UltimaXNA.Ultima.UI;

namespace UltimaXNA.Ultima.Login
{
    class LoginModel : AUltimaModel
    {
        private StateManager m_SceneManager;
        private GUIManager m_UserInterface;

        public LoginClient Client
        {
            get;
            private set;
        }

        public LoginModel()
            : base()
        {
            UltimaServices.Register<LoginModel>(this);

            Client = new LoginClient();
        }

        protected override AView CreateView()
        {
            return new LoginView(this);
        }

        protected override void OnInitialize()
        {
            m_UserInterface = UltimaServices.GetService<GUIManager>();
            m_UserInterface.Cursor = new UI.UltimaCursor();

            m_SceneManager = new StateManager();
            m_SceneManager.ResetToLoginScreen();
        }

        protected override void OnDispose()
        {
            UltimaServices.Unregister<LoginModel>(this);

            Client.Dispose();
            Client = null;

            m_UserInterface.Reset();
            m_SceneManager.CurrentState = null;
            m_SceneManager = null;
        }

        public override void Update(double totalTime, double frameTime)
        {
            m_SceneManager.Update(totalTime, frameTime);
        }
    }
}
