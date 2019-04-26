namespace Entitas
{
    public class ContextDoesNotContainEntityException : BaseEntitasException
    {
        public ContextDoesNotContainEntityException(string message, string hint)
            : base($"{message}\nContext does not contain entity!", hint)
        {

        }
    }
}
