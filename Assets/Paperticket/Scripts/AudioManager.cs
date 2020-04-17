using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket
{
    public class AudioManager : MonoBehaviour {

        public static AudioManager instance;

        public List<AudioClip> AudioClipQueue = new List<AudioClip>();
        private AudioSource audioSource;
        private int clipIndex;

        private bool playingClips;

        // Start is called before the first frame update
        void Awake() {
            
            audioSource = GetComponent<AudioSource>();

            // Save an instanced version
            instance = instance ?? this;
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Debug.LogError("[AudioManager] ERROR -> Another AudioManager was found! Is this here by mistake?");
                Destroy(gameObject);
            }


        }

        public void AddNarrationClip( AudioClip clip ) {

            if (!AudioClipQueue.Contains(clip)) {
                AudioClipQueue.Add(clip);
            }

            // Start playing the clips
            if (!playingClips) StartCoroutine(PlayingClips());

        }

        IEnumerator PlayingClips() {

            playingClips = true;

            while (clipIndex < AudioClipQueue.Count) {

                audioSource.clip = AudioClipQueue[clipIndex];
                audioSource.Play();
                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => !audioSource.isPlaying);
                audioSource.Stop();

                clipIndex += 1;
            }

            playingClips = false;

        }

    }

}
