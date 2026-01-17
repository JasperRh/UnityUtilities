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
        [Tooltip("How many boxes to use. Higher = smoother curve, but more physics cost.")]
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

        private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modification)
        {
            UpdateColliders();
        }

        [Button]
        public void UpdateColliders()
        {
            if (_colliderRoot == null)
            {
                _colliderRoot = new GameObject("SplineColliders");
                _colliderRoot.transform.SetParent(transform, false);
            }

            var step = 1f / segmentCount;

            for (var i = 0; i < segmentCount; i++)
            {
                var tStart = i * step;
                var tEnd = (i + 1) * step;

                var posStart = _container.EvaluatePosition(tStart);
                var posEnd = _container.EvaluatePosition(tEnd);
            
                var center = (Vector3)(posStart + posEnd) * 0.5f;
                var direction = (Vector3)(posEnd - posStart);
                var distance = direction.magnitude;

                if (distance < 0.001f) 
                    continue;

                var box = GetOrCreateBox(i);
                box.gameObject.SetActive(true);

                box.transform.position = center;
                box.transform.rotation = Quaternion.LookRotation(direction);
            
                box.size = new Vector3(thickness.x, thickness.y, distance);
            }

            for (var i = segmentCount; i < _boxPool.Count; i++)
            {
                _boxPool[i].gameObject.SetActive(false);
            }
        }

        private BoxCollider GetOrCreateBox(int index)
        {
            if (index < _boxPool.Count) 
                return _boxPool[index];

            var go = new GameObject($"Segment_{index}");
            go.transform.SetParent(_colliderRoot.transform, false);
            var box = go.AddComponent<BoxCollider>();
            _boxPool.Add(box);
            return box;
        }
    }
}