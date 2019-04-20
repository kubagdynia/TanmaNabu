namespace Entitas
{
    public interface IAerc
    {
        int RetainCount { get; }
        void Retain(object owner);
        void Release(object owner);
    }
}