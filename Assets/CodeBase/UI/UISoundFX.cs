using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Mecha {
    public class UISoundFX : MonoBehaviour
    {
        [SerializeField] private AudioResource m_SFX;
        [SerializeField] private Button m_Button;
        public void PlayEffectOnClick()
        {
            AudioPlayer.Instance.PlaySFX(m_SFX);
        }

        private void Start()
        {
            m_Button.onClick.AddListener(PlayEffectOnClick);
        }

        private void OnDestroy()
        {
            m_Button.onClick.RemoveListener(PlayEffectOnClick);
        }
    }
}