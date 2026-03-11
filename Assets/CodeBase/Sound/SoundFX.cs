using UnityEngine;
using UnityEngine.Audio;

namespace Mecha {
    public class SoundFX : MonoBehaviour
    {
        [SerializeField] private AudioSource m_SoundFXPrefab;

        #region Public API 
        public void PlaySoundEffect(AudioResource effect)
        {
            AudioSource audio = Instantiate(m_SoundFXPrefab);

            audio.resource = effect;
            audio.Play();
        }
        #endregion
    }
}