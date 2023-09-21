namespace Crimsilk.Utilities.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Returns the float squared.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Sqrd(this float value)
        {
            return value * value;
        }

        /// <summary>
        /// Returns whether or not the value is in range of the min and max. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min">Minimum value of the range</param>
        /// <param name="max">Maximum value of the range</param>
        /// <param name="inclusive">Inclusive range. If true, min and max are considered in range.</param>
        /// <returns></returns>
        public static bool InRange(this float value, float min, float max, bool inclusive = false)
        {
            return inclusive ? value >= min && value <= max : value > min && value < max;
        }
    }
}