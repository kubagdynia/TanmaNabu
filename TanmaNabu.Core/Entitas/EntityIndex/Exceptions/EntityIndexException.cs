namespace Entitas
{
    public class EntityIndexException : BaseEntitasException
    {
        public EntityIndexException(string message, string hint)
            : base(message, hint)
        {
        }
    }
}
