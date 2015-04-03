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
            // don't need to do anything above and beyond having the GUI drawn, which is handled by UltimaEngine.
        }
    }
}
