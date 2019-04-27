using System.Linq;

namespace Entitas
{
    public class ContextStillHasRetainedEntitiesException : BaseEntitasException
    {
        public ContextStillHasRetainedEntitiesException(IContext context, IEntity[] entities)
            : base($"'{context}' detected retained entities although all entities got destroyed!",
                "Did you release all entities? Try calling systems.ClearReactiveSystems() " +
                "before calling context.DestroyAllEntities() to avoid memory leaks." +
                "Do not forget to activate them back when needed.\n" +
                GetInfo(entities))
        {

        }

        private static string GetInfo(IEntity[] entities)
        {
            return string.Join("\n", entities
                    .Select(e =>
                    {
                        if (e.Aerc is SafeAerc safeAerc)
                        {
                            return e + " - " + string.Join(", ", safeAerc.Owners
                                       .Select(o => o.ToString())
                                       .ToArray());
                        }

                        return e.ToString();
                    })
                    .ToArray());
        } 
    }
}
