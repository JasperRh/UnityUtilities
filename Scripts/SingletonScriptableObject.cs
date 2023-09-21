using System;
using System.Linq;
using UnityEngine;

namespace Crimsilk.Utilities
{
    public class SingletonScriptableObject<T> : ScriptableObject
        where T : SingletonScriptableObject<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] resources = Resources.LoadAll<T>(string.Empty);
                    if (resources == null || resources.Length < 1)
                    {
                        throw new Exception(
                            $"Could not find any singleton scriptable object instances for type '{typeof(T)}' in the resources folder.");
                    }else if (resources.Length > 1)
                    {
                        Debug.LogWarning($"Found multiple instances for singleton scriptable object for type '{typeof(T)}' in resources folder.");
                    }

                    instance = resources.First();
                }

                return instance;
            }
        }
    }
}