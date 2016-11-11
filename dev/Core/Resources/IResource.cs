namespace UltimaXNA.Core.Resources
{
    public interface IResource<T>
    {
        T GetResource(int resourceIndex);
    }
}
