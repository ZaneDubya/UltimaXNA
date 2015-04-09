namespace UltimaXNA.Core.Patterns.IoC
{
    public interface IObjectLifetimeProvider
    {
        object GetObject();

        void ReleaseObject();

        void SetObject(object value);
    }
}