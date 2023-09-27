using System;
using UnityEngine;

namespace Crimsilk.Utilities.Physics
{
    [RequireComponent(typeof(Collider))]
    public class CollisionEnterBroadcaster : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Determines if the Broadcaster will fire events when it is disabled.")]
        private bool eventWhenDisabled = false;

        public Collider Collider { get; private set; }

        public event Action<Collision> CollisionEnter;

        private void Awake()
        {
            Collider = GetComponent<Collider>();
            AssertCollisionCollider();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!eventWhenDisabled && !isActiveAndEnabled)
                return;

            CollisionEnter?.Invoke(collision);
        }

        private void AssertCollisionCollider()
        {
            Debug.Assert(!Collider.isTrigger, $"Collider attached to {nameof(CollisionEnterBroadcaster)} should not be a trigger.", this);
        }
    }
}