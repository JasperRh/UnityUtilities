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
    }
}