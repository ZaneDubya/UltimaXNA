using InterXLib.Patterns.MVC;
using System;

namespace UltimaXNA.Core
{
    abstract internal class AUltimaModel : AModel
    {
        public UltimaClient Client { get; private set; }

        public void Initialize(UltimaClient client)
        {
            Client = client;
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
