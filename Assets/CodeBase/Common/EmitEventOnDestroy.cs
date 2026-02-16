using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    public class EmitEventOnDestroy : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_DestroyEvent;
        public UnityEvent DestroyEvent => m_DestroyEvent;

        private void OnDestroy()
        {
            m_DestroyEvent.Invoke();
        }
    }
}