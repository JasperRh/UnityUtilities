using System;
using UnityEngine;

namespace Crimsilk.Utilities.Physics
{
    [RequireComponent(typeof(Collider))]
    public class TriggerEnterBroadcaster : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Determines if the Broadcaster will fire events when it is disabled.")]
        private bool triggerEventWhenDisabled = false;
        
        public Collider Collider { get; private set; }
        
        public event Action<Collider> TriggerEnter;

        private void Awake()
        {
            Collider = GetComponent<Collider>();
            Debug.Assert(Collider.isTrigger, $"Collider attached to {nameof(TriggerEnterBroadcaster)} is no trigger.",
                this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!triggerEventWhenDisabled && !isActiveAndEnabled)
            {
                return;
            }
            TriggerEnter?.Invoke(other);
        }
    }
}