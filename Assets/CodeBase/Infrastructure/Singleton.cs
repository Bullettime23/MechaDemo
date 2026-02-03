using UnityEngine;

namespace Infrastructure
{
    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Header("Singleton")]
        [SerializeField] private bool m_DoNotDestroyOnLoad;

        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Mono Singleton: object of type already exists, instance will be destroyed = " + typeof(T).Name);
                Destroy(this);
                return;
            }

            Instance = this as T;

            if (m_DoNotDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
    }
}