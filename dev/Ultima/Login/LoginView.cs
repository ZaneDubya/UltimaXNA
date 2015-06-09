using UltimaXNA.Core.Patterns.MVC;

namespace UltimaXNA.Ultima.Login
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
    }
}
