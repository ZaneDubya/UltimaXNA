using InterXLib.Patterns.MVC;
using UltimaXNA.Ultima.Login.States;
using UltimaXNA.Ultima.UI;

namespace UltimaXNA.Ultima.Login
{
    class LoginModel : AUltimaModel
    {
        private StateManager m_SceneManager;
        private UserInterfaceService m_UserInterface;

        public LoginClient Client
        {
            get;
            private set;
        }

        public LoginModel()
            : base()
        {
            ServiceRegistry.Register<LoginModel>(this);

            Client = new LoginClient();
        }

        protected override AView CreateView()
        {
            return new LoginView(this);
        }

        protected override void OnInitialize()
        {
            ServiceRegistry.GetService<UltimaEngine>().SetupWindowForLogin();

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_UserInterface.Cursor = new UI.UltimaCursor();

            m_SceneManager = new StateManager();
            m_SceneManager.ResetToLoginScreen();
        }

        protected override void OnDispose()
        {
            ServiceRegistry.Unregister<LoginModel>();

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
