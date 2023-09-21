using System;
using System.Collections.Generic;
using System.Linq;
using Crimsilk.Utilities.Extensions;
using UnityEngine;

namespace Crimsilk.Utilities
{
    /// <summary>
    /// Receives the named events from the Animator.
    /// Classes can subscribe to these events.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorEventReceiver : MonoBehaviour
    {
        private record AnimatorEventListener
        {
            /// <summary>
            /// The name of the event in the Animator to listen for.
            /// </summary>
            public string EventName { get; set; }
            
            /// <summary>
            /// A unique registered name to identify the listener by.
            /// </summary>
            public string ListenerName { get; set; }
            
            /// <summary>
            /// The callback that is invoked when this event has been triggered.
            /// </summary>
            public Action Callback { get; set; }
        }
        
        private IList<AnimatorEventListener> eventListeners;

        private void Awake()
        {
            eventListeners = new List<AnimatorEventListener>();
        }

        /// <summary>
        /// Adds a listener for the given animation event.
        /// </summary>
        /// <param name="eventName">The name of the event in the Animator to listen for.</param>
        /// <param name="eventCallback">The callback to add for this event.</param>
        public void AddEventListener(string eventName, Action eventCallback)
        {
            var listener = new AnimatorEventListener()
            {
                EventName = eventName,
                Callback = eventCallback
            };
            eventListeners.Add(listener);
        }
        
        /// <summary>
        /// Adds a listener for the given animation event.
        /// </summary>
        /// <param name="eventName">The name of the event in the Animator to listen for.</param>
        /// <param name="listenerName">A unique registered name to identify the listener by.</param>
        /// <param name="eventCallback">The callback to add for this event.</param>
        public void AddEventListener(string eventName, string listenerName, Action eventCallback)
        {
            var listener = new AnimatorEventListener()
            {
                EventName = eventName,
                ListenerName = listenerName,
                Callback = eventCallback
            };
            
            // Check for uniqueness that there is no listener with the same name for the same event.
            if(eventListeners.Any(x => x.EventName == listener.EventName && x.ListenerName == listener.ListenerName))
            {
                Debug.LogWarning($"Tried to add a duplicate eventListener to the {nameof(AnimatorEventReceiver)}. EventListener with '{nameof(listener.EventName)}: {listener.EventName}' and '{nameof(listener.ListenerName)}: {listener.ListenerName}' already exists.");
                return;
            }
            
            eventListeners.Add(listener);
        }

        /// <summary>
        /// Removes a listener for the given animation event.
        /// </summary>
        /// <param name="eventName">The name of the event in the Animator to listen for.</param>
        /// <param name="listenerCallback">The listenerCallback to remove from this event.</param>
        public void RemoveEventListener(string eventName, Action listenerCallback)
        {
            var listener = eventListeners.SingleOrDefault(listener => listener.Callback == listenerCallback);
            if (listener == null)
            {
                return;
            }

            eventListeners.Remove(listener);
        }
        
        /// <summary>
        /// Removes a listener for the given animation event.
        /// </summary>
        /// <param name="eventName">The name of the event in the Animator to listen for.</param>
        /// <param name="listenerName">The listener name to remove from this event.</param>
        public void RemoveEventListener(string eventName, string listenerName)
        {
            var listener = eventListeners.SingleOrDefault(listener => listener.ListenerName == listenerName);
            if (listener == null)
            {
                return;
            }

            eventListeners.Remove(listener);
        }
        
        private void OnEventInvoked(string eventName)
        {
            IList<Action> listeners = eventListeners.Where(listener => listener.EventName == eventName).Select(listener => listener.Callback).ToList();
            listeners.ForEach(listener => listener.Invoke());
        }
    }
}