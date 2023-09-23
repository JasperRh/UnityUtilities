using System;
using UnityEngine;

namespace Crimsilk.Utilities.Physics
{
    [RequireComponent(typeof(Collider))]
    public class CollisionEventBroadcaster : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Determines if the EventCollider will fire events when it is disabled.")]
        private bool triggerEventWhenDisabled = false;
        
        public Collider Collider { get; private set; }
        
        public event Action<Collision> CollisionEnter;
        public event Action<Collision> CollisionStay;
        public event Action<Collision> CollisionExit;

        public event Action<Collider> TriggerEnter;
        public event Action<Collider> TriggerStay;
        public event Action<Collider> TriggerExit;

        private void Awake()
        {
            Collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!triggerEventWhenDisabled && !isActiveAndEnabled)
            {
                return;
            }
            CollisionEnter?.Invoke(other);
        }

        private void OnCollisionStay(Collision other)
        {
            if (!triggerEventWhenDisabled && !isActiveAndEnabled)
            {
                return;
            }
            CollisionStay?.Invoke(other);
        }

        private void OnCollisionExit(Collision other)
        {
            if (!triggerEventWhenDisabled && !isActiveAndEnabled)
            {
                return;
            }
            CollisionExit?.Invoke(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!triggerEventWhenDisabled && !isActiveAndEnabled)
            {
                return;
            }
            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!triggerEventWhenDisabled && !isActiveAndEnabled)
            {
                return;
            }
            TriggerStay?.Invoke(other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!triggerEventWhenDisabled && !isActiveAndEnabled)
            {
                return;
            }
            TriggerExit?.Invoke(other);
        }
    }
}