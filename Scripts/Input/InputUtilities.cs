using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

namespace Crimsilk.Utilities.Input
{
    public static class InputUtilities
    {
        public static bool IsPointerOverUI()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                return false;

            // Get the object currently under the pointer according to the EventSystem
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = UnityEngine.Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // If the first thing hit (the top-most object) is on the UI layer, it's UI.
            return results.Count > 0 && results[0].gameObject.layer == LayerMask.NameToLayer("UI"); 
        }
        
        public static bool TryGetMouseWorldPosition(LayerMask layerMask, out Vector3 worldPosition, out RaycastHit hit, float maxDistance = 1000f, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            var camera = Camera.main;
            if (!camera || Mouse.current == null)
            {
                worldPosition = Vector3.zero;
                hit = default;
                return false;
            }

            var mousePos = Mouse.current.position.ReadValue();
            var ray = camera.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
            if (UnityEngine.Physics.Raycast(ray, out hit, maxDistance, layerMask, triggerInteraction))
            {
                worldPosition = hit.point;
                return true;
            }

            worldPosition = Vector3.zero;
            hit = default;
            return false;
        }
    }
}