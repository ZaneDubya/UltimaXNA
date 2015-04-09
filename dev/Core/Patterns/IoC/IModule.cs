using UltimaXNA.Core.Diagnostics.Tracing;

namespace UltimaXNA.Core.Patterns.IoC
{
    public interface IModule
    {
        string Name
        {
            get;
        }

        void Load(IContainer container);
        void Unload(IContainer container);
    }

    public abstract class Module : IModule
    {
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        public void Load(IContainer container)
        {
            Tracer.Info("Loading Module {0}.", Name);
            OnLoad(container);
        }

        public void Unload(IContainer container)
        {
            Tracer.Info("Unloading Module {0}.", Name);
            OnUnload(container);
        }

        protected abstract void OnLoad(IContainer container);

        protected abstract void OnUnload(IContainer container);
    }
}