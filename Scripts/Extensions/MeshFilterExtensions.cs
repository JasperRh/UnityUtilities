using System.Collections.Generic;
using Crimsilk.Utilities.Models;
using UnityEngine;

namespace Crimsilk.Utilities.Extensions
{
    public static class MeshFilterExtensions
    {
        public static OrientedBoundingBox GenerateOrientedBoundingBoxForMesh(this MeshFilter meshFilter)
        {
            var mesh = meshFilter.sharedMesh;
            var vertices = mesh.vertices;

            if (vertices.Length == 0)
            {
                Debug.LogWarning("Mesh has no vertices.");
                return new OrientedBoundingBox();
            }

            // Transform vertices to world space
            Vector3[] worldVertices = new Vector3[vertices.Length];
            for (var i = 0; i < vertices.Length; i++)
            {
                worldVertices[i] = meshFilter.transform.TransformPoint(vertices[i]);
            }

            var obbMinLocal = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            var obbMaxLocal = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            // Get the corner points of the current mesh's bounding box in its own local space
            var corners = mesh.bounds.GetCorners();
 
            // Transform the 8 corners of the part into the root's local space
            foreach (var corner in corners)
            {
                // Transform from mesh local to world, then world to root local
                var worldCorner = meshFilter.transform.TransformPoint(corner);
                var cornerInRootLocal = meshFilter.transform.InverseTransformPoint(corner);

                obbMinLocal = Vector3.Min(obbMinLocal, corner);
                obbMaxLocal = Vector3.Max(obbMaxLocal, corner);
            }

            return new OrientedBoundingBox
            {
                Center = obbMinLocal + (obbMaxLocal - obbMinLocal) / 2f,
                Size = obbMaxLocal - obbMinLocal,
                Rotation = meshFilter.transform.rotation
            };
        }

        /// <summary>
        /// Generates an oriented bounding box that encompasses all provided mesh filters.
        /// </summary>
        public static OrientedBoundingBox GenerateOrientedBoundingBoxForMeshes(this IEnumerable<MeshFilter> meshFilters, Transform root)
         {
             var obbMinLocal = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
             var obbMaxLocal = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
 
             foreach (var meshFilter in meshFilters)
             {
                 var mesh = meshFilter.sharedMesh;
                 if (mesh == null)
                 {
                     continue;
                 }
 
                 // To find the OBB relative to the root's transform, we must find the 8 corner points of the part's AABB
                 // and transform them into the root's local space.
 
                 // Get the corner points of the current mesh's bounding box in its own local space
                 var corners = mesh.bounds.GetCorners();
 
                 // Transform the 8 corners of the part into the root's local space
                 foreach (var corner in corners)
                 {
                     // Transform from mesh local to world, then world to root local
                     var worldCorner = meshFilter.transform.TransformPoint(corner);
                     var cornerInRootLocal = root.InverseTransformPoint(worldCorner);

                     obbMinLocal = Vector3.Min(obbMinLocal, cornerInRootLocal);
                     obbMaxLocal = Vector3.Max(obbMaxLocal, cornerInRootLocal);
                 }
             }
 
             return new OrientedBoundingBox
             {
                 Center = obbMinLocal + (obbMaxLocal - obbMinLocal) / 2f,
                 Size = obbMaxLocal - obbMinLocal,
                 Rotation = root.rotation
             };
         }
    }
}