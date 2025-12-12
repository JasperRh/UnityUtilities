using UnityEngine;

namespace Crimsilk.Utilities.Extensions
{
    public static class BoundsExtensions
    {
        /// <summary>
        /// Returns the 8 corner points of the Bounds.
        /// </summary>
        public static Vector3[] GetCorners(this Bounds bounds)
        {
            var min = bounds.min;
            var max = bounds.max;
        
            return new Vector3[]
            {
                new(min.x, min.y, min.z),
                new(max.x, min.y, min.z),
                new(min.x, max.y, min.z),
                new(min.x, min.y, max.z),
            
                new(max.x, max.y, min.z),
                new(max.x, min.y, max.z),
                new(min.x, max.y, max.z),
                new(max.x, max.y, max.z)
            };
        }
    }
}