using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Crimsilk.Utilities.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Destroys all required components for this MonoBehaviour
        /// </summary>
        public static void DestroyRequiredComponents(this Component component)
        {
            MemberInfo memberInfo = component.GetType();
            RequireComponent[] requiredComponentAttributes =
                Attribute.GetCustomAttributes(memberInfo, typeof(RequireComponent), true) as RequireComponent[];
            foreach (RequireComponent requiredComponent in requiredComponentAttributes)
            {
                if (requiredComponent != null && component.GetComponent(requiredComponent.m_Type0) != null)
                {
                    Object.Destroy(component.GetComponent(requiredComponent.m_Type0));
                    continue;
                }

                if (requiredComponent != null && component.GetComponent(requiredComponent.m_Type1) != null)
                {
                    Object.Destroy(component.GetComponent(requiredComponent.m_Type1));
                    continue;
                }

                if (requiredComponent != null && component.GetComponent(requiredComponent.m_Type2) != null)
                {
                    Object.Destroy(component.GetComponent(requiredComponent.m_Type2));
                    continue;
                }
            }
        }

        /// <summary>
        /// Loops through all components on the current GameObject, checks if any of the components requires this component and returns a list of the components this component is required by.
        /// Returns an empty list if not required by any component.
        /// </summary>
        public static IEnumerable<Component> IsRequiredBy(this Component thisComponent)
        {
            Type thisComponentType = thisComponent.GetType();
            IList<Component> dependingComponents = new List<Component>();
            //iterate all gameObject's components to see if
            //one of them requires the componentType
            foreach (var component in thisComponent.GetComponents<Component>())
            {
                //iterate all component's attributes, look for
                //the RequireComponent attributes
                RequireComponent[] requiredComponentAttributes =
                    Attribute.GetCustomAttributes(component.GetType(), typeof(RequireComponent), true) as
                        RequireComponent[];
                foreach (RequireComponent requiredComponent in requiredComponentAttributes)
                {
                    //check all three of the required types to see if
                    //componentType is required (for some reason, you
                    //can require up to 3 component types per attribute).
                    if ((requiredComponent.m_Type0?.IsAssignableFrom(thisComponentType) ?? false) ||
                        (requiredComponent.m_Type1?.IsAssignableFrom(thisComponentType) ?? false) ||
                        (requiredComponent.m_Type2?.IsAssignableFrom(thisComponentType) ?? false))
                    {

                        dependingComponents.Add(component);
                    }
                }
            }

            return dependingComponents;
        }

        /// <summary>
        /// Destroys all components that depend on this component using the <see cref="RequireComponent"/> attribute
        /// </summary>
        /// <param name="destroySelf">Destroys itself when all depending components are destroyed.</param>
        public static void DestroyDependingComponents(this Component thisComponent, bool destroySelf = false)
        {
            IEnumerable<Component> dependingComponents = thisComponent.IsRequiredBy();

            // Destroy depending components recursively.
            foreach (Component component in dependingComponents)
            {
                component.DestroyDependingComponents();
                Object.Destroy(component);
            }

            if (destroySelf)
            {
                Object.Destroy(thisComponent);
            }
        }

		/// <summary>
        /// Creates a copy of the given component.
        /// </summary>
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch
                    {
                    } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }

            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }

            return comp as T;
        }

		public static T GetComponentInParent<T>(this Component component, bool includeSelf = true) where T : Component
        {
            // Get the component in parent objects, including the current object
            var parentComponent = component.transform.parent == null || includeSelf ? component.GetComponentInParent<T>() : component.transform.parent.GetComponentInParent<T>();
    
            // If the component is on the current GameObject, return null
            if (parentComponent == null || parentComponent.gameObject == component.gameObject)
            {
                return null;
            }

            return parentComponent;
        }
    }
}