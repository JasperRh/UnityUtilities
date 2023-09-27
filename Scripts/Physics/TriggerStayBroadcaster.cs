using System;
using UnityEngine;

namespace Crimsilk.Utilities.Physics
{
    [RequireComponent(typeof(Collider))]
    public class TriggerStayBroadcaster : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Determines if the Broadcaster will fire events when it is disabled.")]
        private bool triggerEventWhenDisabled = false;

        public Collider Collider { get; private set; }

        public event Action<Collider> TriggerStay;

        private void Awake()
        {
            Collider = GetComponent<Collider>();
            AssertTriggerCollider();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!triggerEventWhenDisabled && !isActiveAndEnabled)
            {
                return;
            }
            TriggerStay?.Invoke(other);
        }

        private void AssertTriggerCollider()
        {
            Debug.Assert(Collider.isTrigger, $"Collider attached to {nameof(TriggerStayBroadcaster)} is not a trigger.", this);
        }
    }
}