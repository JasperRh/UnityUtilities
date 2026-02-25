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
        
        public static bool TryGetMouseWorldPosition(LayerMask layerMask, out Vector3 worldPosition)
        {
            var cam = Camera.main;
            if (cam == null || Mouse.current == null)
            {
                worldPosition = Vector3.zero;
                return false;
            }

            var mousePos = Mouse.current.position.ReadValue();
            var ray = cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
            if (UnityEngine.Physics.Raycast(ray, out var hit, 200f, layerMask))
            {
                worldPosition = hit.point;
                return true;
            }

            worldPosition = Vector3.zero;
            return false;
        }
    }
}