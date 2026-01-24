using Crimsilk.Utilities.Enums;
using UnityEngine;

namespace Crimsilk.Utilities.Extensions
{
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Mirrors the quaternion along the specified axis.
        /// </summary>
        public static Quaternion Mirror(this Quaternion quaternion, Axis axis)
        {
            if (axis == Axis.None)
                return quaternion;

            var x = quaternion.x;
            var y = quaternion.y;
            var z = quaternion.z;
            var w = quaternion.w;

            if (axis.HasFlag(Axis.X))
            {
                y = -y;
                z = -z;
            }

            if (axis.HasFlag(Axis.Y))
            {
                x = -x;
                z = -z;
            }

            if (axis.HasFlag(Axis.Z))
            {
                x = -x;
                y = -y;
            }

            return new Quaternion(x, y, z, w);
        }
    }
}