using UnityEngine;

namespace Crimsilk.Utilities
{
    public class SingletonBehaviour<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        public static T Instance { get; private set; } 
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Debug.LogWarning($"Trying to instantiate a SingletonBehaviour for type '{typeof(T)}' while an instance already exists. \n" +
                                 $"Instantiation is ignored.");
            }
        }
    }
}