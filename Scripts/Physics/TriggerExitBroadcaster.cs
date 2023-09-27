using System;
using UnityEngine;

namespace Crimsilk.Utilities.Physics
{
    [RequireComponent(typeof(Collider))]
    public class TriggerExitBroadcaster : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Determines if the Broadcaster will fire events when it is disabled.")]
        private bool triggerEventWhenDisabled = false;

        public Collider Collider { get; private set; }

        public event Action<Collider> TriggerExit;

        private void Awake()
        {
            Collider = GetComponent<Collider>();
            AssertTriggerCollider();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!triggerEventWhenDisabled && !isActiveAndEnabled)
            {
                return;
            }
            TriggerExit?.Invoke(other);
        }

        private void AssertTriggerCollider()
        {
            Debug.Assert(Collider.isTrigger, $"Collider attached to {nameof(TriggerExitBroadcaster)} is not a trigger.", this);
        }
    }
}