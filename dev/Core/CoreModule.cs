#region Usings

using UltimaXNA.Patterns.IoC;

#endregion

namespace UltimaXNA
{
    internal sealed class CoreModule : IModule
    {
        public string Name
        {
            get { return "UltimaXNA Core Module"; }
        }

        public void Load(IContainer container)
        {
        }

        public void Unload(IContainer container)
        {
        }
    }
}