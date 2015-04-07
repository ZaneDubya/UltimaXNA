namespace UltimaXNA.Patterns.IoC
{
    public interface IObjectLifetimeProvider
    {
        object GetObject();

        void ReleaseObject();

        void SetObject(object value);
    }
}