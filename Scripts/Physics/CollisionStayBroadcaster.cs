﻿using System;
using UnityEngine;

namespace Crimsilk.Utilities.Physics
{
    [RequireComponent(typeof(Collider))]
    public class CollisionStayBroadcaster : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Determines if the Broadcaster will fire events when it is disabled.")]
        private bool eventWhenDisabled = false;

        public Collider Collider { get; private set; }

        public event Action<Collision> CollisionStay;

        private void Awake()
        {
            Collider = GetComponent<Collider>();
            AssertCollisionCollider();
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!eventWhenDisabled && !isActiveAndEnabled)
                return;

            CollisionStay?.Invoke(collision);
        }

        private void AssertCollisionCollider()
        {
            Debug.Assert(!Collider.isTrigger, $"Collider attached to {nameof(CollisionStayBroadcaster)} should not be a trigger.", this);
        }
    }
}