namespace Entitas
{
    public class CollectorException : BaseEntitasException
    {
        public CollectorException(string message, string hint)
            : base(message, hint)
        {
        }
    }
}
