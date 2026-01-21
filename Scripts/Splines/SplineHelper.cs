using UnityEngine;
using UnityEngine.Splines;

namespace Crimsilk.Utilities.Splines
{
    public static class SplineHelper
    {
        /// <summary>
        /// Returns the closest BezierKnot in the given SplineContainer to the specified world position.
        /// </summary>
        /// <param name="splineContainer"></param>
        /// <param name="pointerWorldPosition"></param>
        /// <returns></returns>
        public static (Spline spline, BezierKnot knot) GetClosestKnotToPosition(SplineContainer splineContainer, Vector3 pointerWorldPosition)
        {
            Spline closestSpline = null;
            BezierKnot closestKnot = default;
            var closestDistanceSqr = float.MaxValue;

            foreach (var spline in splineContainer.Splines)
            {
                foreach (var knot in spline.Knots)
                {
                    var knotWorldPos = splineContainer.transform.TransformPoint(knot.Position);
            
                    var distanceSqr = (knotWorldPos - pointerWorldPosition).sqrMagnitude;

                    if (distanceSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distanceSqr;
                        closestKnot = knot;
                        closestSpline = spline;
                    }
                }
            }

            return (closestSpline, closestKnot);
        }
    }
}