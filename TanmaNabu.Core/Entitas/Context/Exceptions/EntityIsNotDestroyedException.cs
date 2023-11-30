namespace Entitas
{
    public class EntityIsNotDestroyedException : BaseEntitasException
    {
        public EntityIsNotDestroyedException(string message)
            : base($"{message}\nEntity is not destroyed yet!",
                "Did you manually call entity.Release(context) yourself? " +
                "If so, please don't :)")
        {

        }
    }
}
