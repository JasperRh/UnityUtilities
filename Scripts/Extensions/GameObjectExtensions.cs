using UnityEngine;

namespace Crimsilk.Utilities.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Sets the layer to this gameobject and all its children transforms.
        /// </summary>
        public static void SetLayerRecursive(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }
        
        /// <summary>
        /// Gets a reference to a component of type <see cref="TComponent"/> on the specified GameObject, or any parent of the GameObject.
        /// This method checks the GameObject on which it is called first, then recurses upwards through each parent GameObject, until it finds a matching Component of the type <see cref="TComponent"/> specified.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive parent GameObjects in the search.</param>
        /// <typeparam name="TComponent">The type of component to search for.</typeparam>
        /// <returns></returns>
        public static bool TryGetComponentInParent<TComponent>(this GameObject gameObject, out TComponent component, bool includeInactive = false)
        {
            component = gameObject.GetComponentInParent<TComponent>(includeInactive);
            return component != null;
        }
    }
}