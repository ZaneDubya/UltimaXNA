using InterXLib.Patterns.MVC;
using System;

namespace UltimaXNA.Ultima
{
    abstract internal class AUltimaModel : AModel
    {
        public UltimaEngine Engine { get; private set; }

        public void Initialize(UltimaEngine engine)
        {
            Engine = engine;
            OnInitialize();
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected abstract void OnInitialize();
        protected abstract void OnDispose();

        protected override AController CreateController()
        {
            throw new NotImplementedException();
        }
    }
}
