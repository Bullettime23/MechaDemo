using UnityEngine;
using UnityEngine.Audio;
using Infrastructure;

namespace Common
{
    public class SoundFXManager : Singleton<SoundFXManager>
    {
        [SerializeField] private AudioSource m_AudioSource;


        public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
        {
            AudioSource audioSource = Instantiate(m_AudioSource, spawnTransform.position, Quaternion.identity);

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();

            float length = audioSource.clip.length;

            Destroy(audioSource.gameObject, length);
        }

        public void PlaySoundFXClip(AudioResource resource, Transform spawnTransform, float volume, float customLength)
        {
            AudioSource audioSource = Instantiate(m_AudioSource, spawnTransform.position, Quaternion.identity);

            audioSource.resource = resource;
            audioSource.volume = volume;
            audioSource.Play();

            Destroy(audioSource.gameObject, customLength);
        }

        public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume, float customLength)
        {
            AudioSource audioSource = Instantiate(m_AudioSource, spawnTransform.position, Quaternion.identity);

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();

            Destroy(audioSource.gameObject, customLength);
        }
    }
}