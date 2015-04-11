#region Usings
using UltimaXNA.Core.Patterns.IoC;
#endregion

namespace UltimaXNA
{
    internal sealed class CoreModule : IModule
    {
        public string Name
        {
            get { return "UltimaXNA Core Module"; }
        }

        UltimaEngine m_Engine;

        public void Load()
        {
            m_Engine = UltimaServices.Register<UltimaEngine>(new UltimaEngine());
        }

        public void Unload()
        {
            UltimaServices.Unregister<UltimaEngine>(m_Engine);
        }
    }
}