using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Splines;

namespace Crimsilk.Utilities.Physics.Colliders.SplineSegmentCollider
{
    [RequireComponent(typeof(SplineContainer))]
    [ExecuteAlways]
    public class SplineSegmentCollider : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("How many boxes to use between each pair of knots.")]
        public int segmentCount = 10;

        [Tooltip("Width and Height of the collider chain.")]
        public Vector2 thickness = new(1f, 1f);

        private SplineContainer _container;
        private GameObject _colliderRoot;
        private readonly List<BoxCollider> _boxPool = new();

        private void Awake()
        {
            _container = GetComponent<SplineContainer>();
            Spline.Changed += OnSplineChanged;
            UpdateColliders();
        }

        private void OnDestroy()
        {
            Spline.Changed -= OnSplineChanged;
        }

        private void OnEnable()
        {
            UpdateColliders();
        }

        private void OnDisable()
        {
            _boxPool.ForEach(box => box.gameObject.SetActive(false));
        }

        private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modification)
        {
            if (!enabled || _container == null) 
                return;

            var belongsToContainer = false;
            foreach (var s in _container.Splines)
            {
                if (s == spline)
                {
                    belongsToContainer = true;
                    break;
                }
            }

            if (belongsToContainer)
                UpdateColliders();
        }

        [Button]
        public void UpdateColliders()
        {
            if (_colliderRoot == null)
            {
                var existing = transform.Find("SplineColliders");
                if (existing != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(existing.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(existing.gameObject);
                    }
                }

                _colliderRoot = new GameObject("SplineColliders");
                _colliderRoot.transform.SetParent(transform, false);
                _boxPool.Clear();
            }

            if (_container.Splines.Count == 0)
            {
                foreach (var b in _boxPool)
                {
                    if (b != null) 
                        b.gameObject.SetActive(false);
                }
                return;
            }

            var boxIndex = 0;

            foreach (var spline in _container.Splines)
            {
                if (spline == null || spline.Count < 2) continue;

                var curveCount = spline.Closed ? spline.Count : spline.Count - 1;

                for (var i = 0; i < curveCount; i++)
                {
                    for (var j = 0; j < segmentCount; j++)
                    {
                        var tStartLocal = (float)j / segmentCount;
                        var tEndLocal = (float)(j + 1) / segmentCount;

                        var tStartGlobal = (i + tStartLocal) / curveCount;
                        var tEndGlobal = (i + tEndLocal) / curveCount;

                        var posStart = (Vector3)spline.EvaluatePosition(tStartGlobal);
                        var posEnd = (Vector3)spline.EvaluatePosition(tEndGlobal);

                        var center = (posStart + posEnd) * 0.5f;
                        var direction = posEnd - posStart;
                        var distance = direction.magnitude;

                        if (distance < 0.001f)
                            continue;

                        var box = GetOrCreateBox(boxIndex++);
                        box.gameObject.SetActive(true);

                        box.transform.localPosition = center;

                        var tCenterGlobal = (i + (tStartLocal + tEndLocal) * 0.5f) / curveCount;
                        var upVector = (Vector3)spline.EvaluateUpVector(tCenterGlobal);
                        box.transform.localRotation = Quaternion.LookRotation(direction, upVector);

                        box.size = new Vector3(thickness.x, thickness.y, distance);
                    }
                }
            }

            for (var i = boxIndex; i < _boxPool.Count; i++)
            {
                if (_boxPool[i] != null) 
                    _boxPool[i].gameObject.SetActive(false);
            }
        }

        private BoxCollider GetOrCreateBox(int index)
        {
            if (index < _boxPool.Count && _boxPool[index] != null)
                return _boxPool[index];
            
            var go = new GameObject($"Segment_{index}");
            go.transform.SetParent(_colliderRoot.transform, false);
            var box = go.AddComponent<BoxCollider>();
            
            if (index < _boxPool.Count)
                _boxPool[index] = box;
            else
                _boxPool.Add(box);
                
            return box;
        }
    }
}