using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Crimsilk.Utilities.Extensions{
	
    public static class Vector3Extensions
    {
        /// <summary>
        /// Clamps the vector between the min and max vector
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
        {
            float x = Mathf.Clamp(vector.x, min.x, max.x);
            float y = Mathf.Clamp(vector.y, min.y, max.y);
            float z = Mathf.Clamp(vector.z, min.z, max.z);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Transforms the vector so that the Up of the vector points in the same direction as the given plane.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="planeUp"></param>
        /// <returns></returns>
        public static Vector3 ToPlane(this Vector3 vector, Vector3 planeUp)
        {
            Vector3 newRight = Vector3.Cross(vector, planeUp).normalized;
            return Vector3.Cross(planeUp, newRight);
        }

        /// <summary>
        /// Returns true if all of the axis of the vector are positive.
        /// </summary>
        /// <returns></returns>
        public static bool IsPositive(this Vector3 vector)
        {
            return vector is {x: >= 0, y: >= 0, z: >= 0};
        }

        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            var enumerable = vectors.ToList();
            var sum = Sum(enumerable);

            return sum / enumerable.Count();
        }
        
        public static Vector3 Sum(this IEnumerable<Vector3> vectors)
        {
            return vectors.Aggregate(Vector3.zero, (current, vector) => current + vector);
        }
    }
}