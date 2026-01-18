using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    }
}