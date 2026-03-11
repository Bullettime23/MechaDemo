using UnityEngine;

namespace Mecha
{
    [ExecuteInEditMode]
    public class FrameLimit : MonoBehaviour
    {
        [SerializeField] private int m_FrameRate = 60;

        void Start()
        {
#if UNITY_EDITOR
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = m_FrameRate;
#endif
        }
    }
}