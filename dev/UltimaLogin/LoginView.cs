using InterXLib.Display;
using InterXLib.Patterns.MVC;

namespace UltimaXNA.UltimaLogin
{
    class LoginView : AView
    {
        protected new LoginModel Model
        {
            get { return (LoginModel)base.Model; }
        }

        public LoginView(LoginModel model)
            : base(model)
        {

        }

        public override void Draw(YSpriteBatch spritebatch, double frameTime)
        {
            UltimaEngine.UserInterface.Draw(frameTime);
        }
    }
}
