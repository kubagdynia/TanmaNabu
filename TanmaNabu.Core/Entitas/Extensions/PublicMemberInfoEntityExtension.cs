namespace Entitas
{
    public static class PublicMemberInfoEntityExtension
    {
        /// Adds copies of all specified components to the target entity.
        /// If replaceExisting is true it will replace existing components.
        public static void CopyTo(this IEntity entity, IEntity target, bool replaceExisting = false, params int[] indices)
        {
            int[] componentIndices = indices.Length == 0 ? entity.GetComponentIndices() : indices;
            foreach (int index in componentIndices)
            {
                IComponent component = entity.GetComponent(index);
                IComponent clonedComponent = target.CreateComponent(index, component.GetType());
                component.CopyPublicMemberValues(clonedComponent);

                if (replaceExisting)
                {
                    target.ReplaceComponent(index, clonedComponent);
                }
                else
                {
                    target.AddComponent(index, clonedComponent);
                }
            }
        }
    }
}
