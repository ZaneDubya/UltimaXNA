using UltimaXNA.Core.Diagnostics.Tracing;

namespace UltimaXNA.Core.Patterns.IoC
{
    public interface IModule
    {
        string Name
        {
            get;
        }

        void Load();
        void Unload();
    }

    public abstract class Module : IModule
    {
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        public void Load()
        {
            Tracer.Info("Loading Module {0}.", Name);
            OnLoad();
        }

        public void Unload()
        {
            Tracer.Info("Unloading Module {0}.", Name);
            OnUnload();
        }

        protected abstract void OnLoad();

        protected abstract void OnUnload();
    }
}