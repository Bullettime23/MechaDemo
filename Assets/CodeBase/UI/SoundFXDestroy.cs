using System.Collections;
using UnityEngine;


namespace Mecha
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundFXDestroy : MonoBehaviour
    {
        void Start()
        {
            AudioSource aSource = GetComponent<AudioSource>();

            IEnumerator DestroyOnFinish()
            {
                yield return new WaitUntil(() => aSource.isPlaying == false);
                Destroy(gameObject);

            }

            StartCoroutine(DestroyOnFinish());
        }
    }
}