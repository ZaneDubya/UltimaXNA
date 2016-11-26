using System;

namespace UltimaXNA.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Model. Maintains the state, core data, and update logic of a model.
    /// </summary>
    public abstract class AModel
    {
        bool m_IsInitialized;
        AView m_View;
        AController m_Controller;

        public AView GetView()
        {
            if (m_View == null)
            {
                m_View = CreateView();
            }
            return m_View;
        }
        
        public AController GetController()
        {
            if (m_Controller == null)
            {
                m_Controller = CreateController();
            }
            return m_Controller;
        }

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

        public abstract void Update(double totalTime, double frameTime);

        protected abstract AView CreateView();
        protected abstract void OnInitialize();
        protected abstract void OnDispose();

        protected virtual AController CreateController()
        {
            throw new NotImplementedException();
        }
    }
}
