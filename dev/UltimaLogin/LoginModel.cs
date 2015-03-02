using InterXLib.Patterns.MVC;
using System;

namespace UltimaXNA.UltimaLogin
{
    class LoginModel : AUltimaModel
    {
        private Scenes.SceneManager m_SceneManager;

        public LoginModel()
        {

        }

        protected override void OnInitialize()
        {
            m_SceneManager = new Scenes.SceneManager(Client);
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
