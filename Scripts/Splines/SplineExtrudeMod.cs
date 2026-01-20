using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Crimsilk.Utilities.Splines
{
   /// <summary>
   /// This is a copy of Unity's Spline Extrude version 2.6.1 that allows to instantiate multiple extrusions without sharing the same mesh.
   /// </summary>
   [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
   [AddComponentMenu("Splines/Spline Extrude Mod")]
   public class SplineExtrudeMod : MonoBehaviour
   {
       [SerializeField, Tooltip("The Spline to extrude.")]
       SplineContainer m_Container;

       [SerializeField, Tooltip("Enable to regenerate the extruded mesh when the target Spline is modified. Disable " +
                                "this option if the Spline will not be modified at runtime.")]
       bool m_RebuildOnSplineChange;

       [SerializeField, Tooltip("The maximum number of times per-second that the mesh will be rebuilt.")]
       int m_RebuildFrequency = 30;

       [SerializeField,
        Tooltip("Automatically update any Mesh, Box, or Sphere collider components when the mesh is extruded.")]
#pragma warning disable 414
       bool m_UpdateColliders = true;
#pragma warning restore 414

       [SerializeField, Tooltip("The number of sides that comprise the radius of the mesh.")]
       int m_Sides = 8;

       [SerializeField, Tooltip("The number of edge loops that comprise the length of one unit of the mesh. The " +
                                "total number of sections is equal to \"Spline.GetLength() * segmentsPerUnit\".")]
       float m_SegmentsPerUnit = 4;

       [SerializeField,
        Tooltip(
            "Indicates if the start and end of the mesh are filled. When the Spline is closed this setting is ignored.")]
       bool m_Capped = true;

       [SerializeField, Tooltip("The radius of the extruded mesh.")]
       [HideInInspector]
       float m_Radius = .25f;

       [SerializeField, Tooltip("The width of the extruded mesh.")]
       float m_Width = 0.5f;

       [SerializeField, Tooltip("The height of the extruded mesh.")]
       float m_Height = 0.5f;

       [SerializeField, Tooltip("The section of the Spline to extrude.")]
       Vector2 m_Range = new Vector2(0f, 1f);

       [SerializeField, Tooltip("Rotation of the extruded mesh cross-section in degrees.")]
       float m_Rotation = 0f;

       Mesh m_Mesh;
       bool m_RebuildRequested;
       float m_NextScheduledRebuild;

       /// <summary>The SplineContainer of the <see cref="Spline"/> to extrude.</summary>
       [Obsolete("Use Container instead.", false)]
       public SplineContainer container => Container;

       /// <summary>The SplineContainer of the <see cref="Spline"/> to extrude.</summary>
       public SplineContainer Container
       {
           get => m_Container;
           set => m_Container = value;
       }

       /// <summary>
       /// Enable to regenerate the extruded mesh when the target Spline is modified. Disable this option if the Spline
       /// will not be modified at runtime.
       /// </summary>
       [Obsolete("Use RebuildOnSplineChange instead.", false)]
       public bool rebuildOnSplineChange => RebuildOnSplineChange;

       /// <summary>
       /// Enable to regenerate the extruded mesh when the target Spline is modified. Disable this option if the Spline
       /// will not be modified at runtime.
       /// </summary>
       public bool RebuildOnSplineChange
       {
           get => m_RebuildOnSplineChange;
           set => m_RebuildOnSplineChange = value;
       }

       /// <summary>The maximum number of times per-second that the mesh will be rebuilt.</summary>
       [Obsolete("Use RebuildFrequency instead.", false)]
       public int rebuildFrequency => RebuildFrequency;

       /// <summary>The maximum number of times per-second that the mesh will be rebuilt.</summary>
       public int RebuildFrequency
       {
           get => m_RebuildFrequency;
           set => m_RebuildFrequency = Mathf.Max(value, 1);
       }

       /// <summary>How many sides make up the radius of the mesh.</summary>
       [Obsolete("Use Sides instead.", false)]
       public int sides => Sides;

       /// <summary>How many sides make up the radius of the mesh.</summary>
       public int Sides
       {
           get => m_Sides;
           set => m_Sides = Mathf.Max(value, 3);
       }

       /// <summary>How many edge loops comprise the one unit length of the mesh.</summary>
       [Obsolete("Use SegmentsPerUnit instead.", false)]
       public float segmentsPerUnit => SegmentsPerUnit;

       /// <summary>How many edge loops comprise the one unit length of the mesh.</summary>
       public float SegmentsPerUnit
       {
           get => m_SegmentsPerUnit;
           set => m_SegmentsPerUnit = Mathf.Max(value, .0001f);
       }

       /// <summary>Whether the start and end of the mesh is filled. This setting is ignored when spline is closed.</summary>
       [Obsolete("Use Capped instead.", false)]
       public bool capped => Capped;

       /// <summary>Whether the start and end of the mesh is filled. This setting is ignored when spline is closed.</summary>
       public bool Capped
       {
           get => m_Capped;
           set => m_Capped = value;
       }

       /// <summary>The radius of the extruded mesh.</summary>
       [Obsolete("Use Radius instead.", false)]
       public float radius => Radius;

       /// <summary>The radius of the extruded mesh.</summary>
       [Obsolete("Radius is deprecated, use Width and Height instead.")]
       public float Radius
       {
           get => m_Radius;
           set
           {
               m_Radius = Mathf.Max(value, .00001f);
               m_Width = m_Radius * 2f;
               m_Height = m_Radius * 2f;
           }
       }
       
       /// <summary>The width of the extruded mesh.</summary>
       public float Width
       {
           get => m_Width;
           set => m_Width = Mathf.Max(value, .00001f);
       }

       /// <summary>The height of the extruded mesh.</summary>
       public float Height
       {
           get => m_Height;
           set => m_Height = Mathf.Max(value, .00001f);
       }

       /// <summary>
       /// The section of the Spline to extrude.
       /// </summary>
       [Obsolete("Use Range instead.", false)]
       public Vector2 range => Range;

       /// <summary>
       /// The section of the Spline to extrude.
       /// </summary>
       public Vector2 Range
       {
           get => m_Range;
           set => m_Range = new Vector2(Mathf.Min(value.x, value.y), Mathf.Max(value.x, value.y));
       }

       /// <summary>
       /// Rotation of the extruded mesh cross-section in degrees.
       /// </summary>
       public float Rotation
       {
           get => m_Rotation;
           set => m_Rotation = value;
       }

       /// <summary>The main Spline to extrude.</summary>
       [Obsolete("Use Spline instead.", false)]
       public Spline spline => Spline;

       /// <summary>The main Spline to extrude.</summary>
       public Spline Spline
       {
           get => m_Container?.Spline;
       }

       /// <summary>The Splines to extrude.</summary>
       public IReadOnlyList<Spline> Splines
       {
           get => m_Container?.Splines;
       }

       private void Awake()
       {
           if (TryGetComponent<MeshFilter>(out var filter))
               filter.sharedMesh = m_Mesh = CreateMeshAsset();
       }

       internal void Reset()
       {
           TryGetComponent(out m_Container);

           if (TryGetComponent<MeshFilter>(out var filter))
               filter.sharedMesh = m_Mesh = CreateMeshAsset();

           if (TryGetComponent<MeshRenderer>(out var renderer) && renderer.sharedMaterial == null)
           {
               // todo Make Material.GetDefaultMaterial() public
               var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
               var mat = cube.GetComponent<MeshRenderer>().sharedMaterial;
               DestroyImmediate(cube);
               renderer.sharedMaterial = mat;
           }

           Rebuild();
       }

       void Start()
       {
#if UNITY_EDITOR
           if (EditorApplication.isPlaying)
#endif
           {
               if (m_Container == null || m_Container.Spline == null || m_Container.Splines.Count == 0)
                   return;
               if ((m_Mesh = GetComponent<MeshFilter>().sharedMesh) == null)
                   return;
           }

           Rebuild();
       }

       internal static readonly string k_EmptyContainerError =
           "Spline Extrude does not have a valid SplineContainer set.";

       bool IsNullOrEmptyContainer()
       {
           var isNull = m_Container == null || m_Container.Spline == null || m_Container.Splines.Count == 0;
           if (isNull)
           {
               if (Application.isPlaying)
                   Debug.LogError(k_EmptyContainerError, this);
           }

           return isNull;
       }

       internal static readonly string k_EmptyMeshFilterError = "SplineExtrude.createMeshInstance is disabled," +
                                                                " but there is no valid mesh assigned. " +
                                                                "Please create or assign a writable mesh asset.";

       bool IsNullOrEmptyMeshFilter()
       {
           var isNull = (m_Mesh = GetComponent<MeshFilter>().sharedMesh) == null;
           if (isNull)
               Debug.LogError(k_EmptyMeshFilterError, this);
           return isNull;
       }

       void OnEnable()
       {
           Spline.Changed += OnSplineChanged;
       }

       void OnDisable()
       {
           Spline.Changed -= OnSplineChanged;
       }

       void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
       {
           if (m_Container != null && Splines.Contains(spline) && m_RebuildOnSplineChange)
               m_RebuildRequested = true;
       }

       void Update()
       {
           if (m_RebuildRequested && Time.time >= m_NextScheduledRebuild)
               Rebuild();
       }

       /// <summary>
       /// Triggers the rebuild of a Spline's extrusion mesh and collider.
       /// </summary>
       public void Rebuild()
       {
           if (IsNullOrEmptyContainer() || IsNullOrEmptyMeshFilter())
               return;

           m_Mesh.Clear();
           var splines = Splines;
           if (splines == null || splines.Count == 0) return;

           var combines = new List<CombineInstance>(splines.Count);
           var settings = new ExtrudeSettings<RotatedCircle>(m_Sides, m_Capped, m_Range, 1f, new RotatedCircle(m_Sides, m_Rotation, m_Width, m_Height));

           foreach (var s in splines)
           {
               if (s.Count < 2) continue;

               var tempMesh = new Mesh();
               var span = Mathf.Abs(settings.Range.y - settings.Range.x);
               settings.SegmentCount = Mathf.Max((int)Mathf.Ceil(s.GetLength() * span * m_SegmentsPerUnit), 1);

               SplineMesh.Extrude(s, tempMesh, settings);
               combines.Add(new CombineInstance() { mesh = tempMesh, transform = Matrix4x4.identity });
           }

           m_Mesh.CombineMeshes(combines.ToArray(), true, false);

           foreach (var c in combines)
           {
               if (Application.isPlaying) Destroy(c.mesh);
               else DestroyImmediate(c.mesh);
           }

           m_NextScheduledRebuild = Time.time + 1f / m_RebuildFrequency;

#if UNITY_PHYSICS_MODULE
           if (m_UpdateColliders)
           {
               if (TryGetComponent<MeshCollider>(out var meshCollider))
                   meshCollider.sharedMesh = m_Mesh;

               if (TryGetComponent<BoxCollider>(out var boxCollider))
               {
                   boxCollider.center = m_Mesh.bounds.center;
                   boxCollider.size = m_Mesh.bounds.size;
               }

               if (TryGetComponent<SphereCollider>(out var sphereCollider))
               {
                   sphereCollider.center = m_Mesh.bounds.center;
                   var ext = m_Mesh.bounds.extents;
                   sphereCollider.radius = Mathf.Max(ext.x, ext.y, ext.z);
               }
           }
#endif
       }

#if UNITY_EDITOR
       void OnValidate()
       {
           

           if (EditorApplication.isPlaying)
               return;

           // Use delayCall to escape the 'OnValidate' restricted scope
           EditorApplication.delayCall += () => {
               if (this == null) 
                   return; // Ensure object wasn't destroyed in the meantime
               
               // Check for MeshFilter and ensure m_Mesh is initialized
               if (TryGetComponent<MeshFilter>(out var filter) && !filter.sharedMesh)
               {
                   filter.sharedMesh = m_Mesh = CreateMeshAsset();
               }
               Rebuild();
           };
       }
#endif

       internal Mesh CreateMeshAsset()
       {
           var mesh = new Mesh();
           mesh.name = name;
/*
#if UNITY_EDITOR
           var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
           var sceneDataDir = "Assets";

           if (!string.IsNullOrEmpty(scene.path))
           {
               var dir = Path.GetDirectoryName(scene.path);
               sceneDataDir = $"{dir}/{Path.GetFileNameWithoutExtension(scene.path)}";
               if (!Directory.Exists(sceneDataDir))
                   Directory.CreateDirectory(sceneDataDir);
           }

           var path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"{sceneDataDir}/SplineExtrude_{mesh.name}.asset");
           UnityEditor.AssetDatabase.CreateAsset(mesh, path);
           UnityEditor.EditorGUIUtility.PingObject(mesh);
#endif*/
           return mesh;
       }
       
       [Serializable]
       struct RotatedCircle : IExtrudeShape
       {
           public int Sides;
           public float Rotation;
           public float Width;
           public float Height;

           public RotatedCircle(int sides, float rotation, float width, float height)
           {
               Sides = sides;
               Rotation = rotation;
               Width = width;
               Height = height;
           }

           public int SideCount => Sides;

           public void Setup(ISpline spline, int segmentCount) { }

           public float2 GetPosition(float t, int index)
           {
               var radiusScale = 1f / math.cos(math.PI / Sides);
               var phaseShift = 90f - (180f / Sides);
               float angle = math.radians(Rotation + phaseShift + (index * 360f / Sides));
               return new float2(math.cos(angle) * Width * 0.5f * radiusScale, math.sin(angle) * Height * 0.5f * radiusScale);
           }
       }
   }
}

