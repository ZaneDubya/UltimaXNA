using InterXLib.Patterns.MVC;

namespace UltimaXNA.UltimaLogin
{
    class LoginModel : AUltimaModel
    {
        private Scenes.SceneManager m_SceneManager;

        public LoginModel()
        {
            UltimaEngine.UserInterface.Cursor = new UltimaGUI.UltimaCursor();
        }

        protected override AView CreateView()
        {
            return new LoginView(this);
        }

        protected override void OnInitialize()
        {
            m_SceneManager = new Scenes.SceneManager(Engine);
            m_SceneManager.ResetToLoginScreen();
        }

        protected override void OnDispose()
        {
            UltimaEngine.UserInterface.Reset();
            m_SceneManager.CurrentScene = null;
            m_SceneManager = null;
        }

        public override void Update(double totalTime, double frameTime)
        {
            m_SceneManager.Update(totalTime, frameTime);
        }
    }
}
