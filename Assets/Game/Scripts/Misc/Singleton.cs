using UnityEngine;

namespace Cinetica.Utility
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        public virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}