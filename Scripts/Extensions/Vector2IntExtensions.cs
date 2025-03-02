using UnityEngine;

namespace Crimsilk.Utilities.Extensions
{
    public static class Vector2IntExtensions
    {
        /// <summary>
        /// Clamps the vector between the min and max vector
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector2Int Clamp(this Vector2Int vector, Vector2Int min, Vector2Int max)
        {
            var x = Mathf.Clamp(vector.x, min.x, max.x);
            var y = Mathf.Clamp(vector.y, min.y, max.y);

            return new Vector2Int(x, y);
        }
    }
}

