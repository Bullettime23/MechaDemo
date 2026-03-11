using Infrastructure;
using UnityEngine.Audio;

namespace Mecha {
    public class AudioPlayer : Singleton<AudioPlayer>
    {
        private SoundFX soundFXPlayer;
        
        void Start()
        {
            soundFXPlayer = GetComponentInChildren<SoundFX>();
        }

        #region Public API
        public void PlaySFX(AudioResource audio)
        {
            soundFXPlayer.PlaySoundEffect(audio);
        }

        #endregion
    }
}