using InterXLib.Patterns.MVC;
using System;

namespace UltimaXNA.UltimaLogin
{
    class LoginModel : AModel
    {
        private Scenes.SceneManager m_SceneManager;

        public LoginModel()
        {
            m_SceneManager = new Scenes.SceneManager();
            m_SceneManager.Reset();
        }

        protected override AController CreateController()
        {
 	        throw new NotImplementedException();
        }

        protected override AView CreateView()
        {
 	        throw new NotImplementedException();
        }

        public override void Update(double totalTime, double frameTime)
        {
            m_SceneManager.Update(totalTime, frameTime);
        }
    }
}
