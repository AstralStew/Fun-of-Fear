using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {

    [RequireComponent(typeof(AudioSource))]
    public class GhostAudio : MonoBehaviour {

        [Header("References")]

        private AudioSource source;
        private GhostPerception ghostPerception;
        
        [SerializeField] AudioClip hoverClip;
        [SerializeField] AudioClip spotClip;
        [SerializeField] AudioClip screamClip;

        [Header("Controls")]
        [Space(20)]
        [SerializeField] [Range(0, 360)] int hoverSpread;
        [SerializeField] [Range(0, 1)] float hoverVolume;
        [Space(10)]
        [SerializeField] [Range(0, 360)] int spotSpread;
        [SerializeField] [Range(0, 1)] float spotVolume;
        [Space(10)]
        [SerializeField] [Range(0, 360)] int screamSpread;
        [SerializeField] [Range(0, 1)] float screamVolume;

        // Start is called before the first frame update
        void Awake() {

            source = GetComponent<AudioSource>();

            // Grab the ghost movement reference
            ghostPerception = ghostPerception ?? GetComponentInParent<GhostPerception>();
            if (!ghostPerception) {
                Debug.LogError("[GhostAudio] ERROR -> No Ghost Perception component found on parent object! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }
            
        }

        void OnEnable() {
            ghostPerception.onSeePlayer += PlaySpotAndFlyAudio;
            ghostPerception.onReachPlayer += PlayScreamAudio;

            PlayHoverAudio();
        }

        void OnDisable() {
            ghostPerception.onSeePlayer -= PlaySpotAndFlyAudio;
            ghostPerception.onReachPlayer -= PlayScreamAudio;
        }



        void PlayHoverAudio() {
            source.loop = true;
            source.spread = hoverSpread;
            source.volume = hoverVolume;
            SetClipAndPlay(hoverClip);

        }

        void PlaySpotAndFlyAudio() {
            source.loop = true;
            source.spread = spotSpread;
            source.volume = spotVolume;
            SetClipAndPlay(spotClip);
        }

        void PlayScreamAudio() {
            source.loop = false;
            source.spread = screamSpread;
            source.volume = screamVolume;
            SetClipAndPlay(screamClip);

        }


        void SetClipAndPlay(AudioClip clip ) {

            source.Stop();
            source.clip = clip;
            source.Play();
        }


    }

}