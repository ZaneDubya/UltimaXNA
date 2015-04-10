using InterXLib.Patterns.MVC;
using System;
using UltimaXNA.Core.Patterns.IoC;

namespace UltimaXNA.Ultima
{
    abstract public class AUltimaModel : AModel
    {
        public IEngine Engine { get; private set; }
        protected IContainer Container { get; private set; }

        protected AUltimaModel(IContainer container)
        {
            Container = container;
            Engine = container.Resolve<IEngine>();
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
