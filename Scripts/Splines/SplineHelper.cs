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
        
        /// <summary>
        /// Finds the parameter t along the spline segment between tStart and tEnd where the spline reaches the specified local Y height.
        /// </summary>
        /// <param name="spline">The spline to search.</param>
        /// <param name="targetLocalY">The local Y position of the point to find T on.</param>
        /// <param name="tStart">The tStart where to start searching.</param>
        /// <param name="tEnd">The tEnd where to stop searching.</param>
        /// <param name="resultT">The result T at the given height.</param>
        public static bool TryFindTAtHeight(Spline spline, float targetLocalY, float tStart, float tEnd, out float resultT)
        {
            var low = tStart;
            var high = tEnd;
            resultT = low;

            // If the height is outside the range of this spline segment, fail early
            var yStart = ((Vector3)spline.EvaluatePosition(tStart)).y;
            var yEnd = ((Vector3)spline.EvaluatePosition(tEnd)).y;
            if (targetLocalY < Mathf.Min(yStart, yEnd) || targetLocalY > Mathf.Max(yStart, yEnd))
                return false;

            // Perform 10-15 iterations (enough for sub-millimeter precision)
            for (var i = 0; i < 15; i++)
            {
                var mid = (low + high) / 2f;
                var midY = ((Vector3)spline.EvaluatePosition(mid)).y;

                // Determine which half to keep based on whether the spline is rising or falling
                if (yEnd > yStart) // Spline is going UP
                {
                    if (midY < targetLocalY)
                    {
                        low = mid;
                    }
                    else
                    {
                        high = mid;
                    }
                }
                else // Spline is going DOWN
                {
                    if (midY > targetLocalY)
                    {
                        low = mid;
                    }
                    else
                    {
                        high = mid;
                    }
                }
            }

            resultT = (low + high) / 2f;
            return true;
        }
        
        public static void CopySplineExtrudeModValues(SplineExtrudeMod sourcePrefab, SplineExtrudeMod target)
        {
            target.GetComponent<MeshRenderer>().material = sourcePrefab.GetComponent<MeshRenderer>().sharedMaterial;
            target.RebuildOnSplineChange = sourcePrefab.RebuildOnSplineChange;
            target.Width = sourcePrefab.Width;
            target.Height = sourcePrefab.Height;
            target.Rotation = sourcePrefab.Rotation;
            target.Range = sourcePrefab.Range;
            target.Sides = sourcePrefab.Sides;
            target.SegmentsPerUnit = sourcePrefab.SegmentsPerUnit;
            target.Capped = sourcePrefab.Capped;
        }
    }
}