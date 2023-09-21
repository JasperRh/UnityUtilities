using System;

namespace Crimsilk.Utilities.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        /// Returns true if this integer is exactly 1 away from target integer.
        /// </summary>
        /// <returns></returns>
        public static bool IsAdjacent(this int source, int target)
        {
            return Math.Abs(source - target) == 1;
        }
        
        /// <summary>
        /// Returns whether or not the value is in range of the min and max. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min">Minimum value of the range</param>
        /// <param name="max">Maximum value of the range</param>
        /// <param name="inclusive">Inclusive range. If true, min and max are considered in range.</param>
        /// <returns></returns>
        public static bool InRange(this int value, int min, int max, bool inclusive = false)
        {
            return inclusive ? value >= min && value <= max : value > min && value < max;
        }
    }
}