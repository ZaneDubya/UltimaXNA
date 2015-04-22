using InterXLib.Patterns.MVC;
using System;

namespace UltimaXNA.Ultima
{
    abstract public class AUltimaModel : AModel
    {
        protected AUltimaModel()
        {

        }

        private bool m_IsInitialized;

        public void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

            m_IsInitialized = true;

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
