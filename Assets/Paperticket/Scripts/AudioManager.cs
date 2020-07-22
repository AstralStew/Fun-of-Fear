using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket {

    public enum FootstepType { None, Cave, Stone, Grass }

    public class AudioManager : MonoBehaviour {

        public static AudioManager instance;

        //[SerializeField] AudioSource narrativeSource;
        [SerializeField] AudioSource footstepsSource;
        //[SerializeField] CapsuleCollider bodyMovementCollider;
        [SerializeField] bool debugging;
        [SerializeField] bool frameDebugging;

        [Header("Footsteps Controls")]
        [Space(10)]
        [SerializeField] [Range(0.1f, 100)] float stepDistance = 0.1f;
        [SerializeField] [Range(0.1f, 100)] float checkRate = 0.1f;
        [SerializeField] Vector2 pitchVariance;
        [SerializeField] List<AudioClip> footstepClips = new List<AudioClip>();

               
        
        [Header("Read Only")]
        [Space(10)]
        [SerializeField] FootstepType footstepType;
        public bool footstepsActive { get { return footstepsCo != null; } }

        //[Space(5)]
        //public List<AudioClip> narrativeQueue = new List<AudioClip>();
        //[SerializeField] int narrativeIndex;
        //[SerializeField] bool playingNarrative;

        Coroutine footstepsCo = null;

        // Start is called before the first frame update
        void Awake() {
            
            // Save an instanced version
            instance = instance ?? this;
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Debug.LogError("[AudioManager] ERROR -> Another AudioManager was found! Is this here by mistake?");
                Destroy(gameObject);
            }

            //if (!narrativeSource) {
            //    Debug.LogError("[AudioManager] ERROR -> No Narrative AudioSource found! Disabling");
            //    enabled = false;
            //}
            if (!footstepsSource) {
                Debug.LogError("[AudioManager] ERROR -> No Footsteps AudioSource found! Disabling");
                enabled = false;
            }

        }



        //public void AddNarrationClip( AudioClip clip ) {
        //    if (debugging) Debug.Log("[AudioManager] Adding narration clip("+clip.name+") to queue");

        //    if (!narrativeQueue.Contains(clip)) {
        //        narrativeQueue.Add(clip);
        //    }

        //    // Start playing the clips
        //    if (!playingNarrative) StartCoroutine(PlayingClips());

        //}

        //IEnumerator PlayingClips() {

        //    if (debugging) Debug.Log("[AudioManager] Beginning narration playback");

        //    playingNarrative = true;

        //    while (narrativeIndex < narrativeQueue.Count) {

        //        if (debugging) Debug.Log("[AudioManager] Playing queue index ("+narrativeIndex+"), clip ("+narrativeQueue[narrativeIndex]+")");

        //        narrativeSource.clip = narrativeQueue[narrativeIndex];
        //        narrativeSource.Play();
        //        yield return new WaitForSeconds(0.1f);
        //        yield return new WaitUntil(() => !narrativeSource.isPlaying);
        //        narrativeSource.Stop();

        //        narrativeIndex += 1;
        //    }

        //    if (debugging) Debug.Log("[AudioManager] Finished narration playback");

        //    playingNarrative = false;

        //}




        public void StartFootsteps() {
            if (footstepsCo != null) return;
            footstepsCo = StartCoroutine(Footsteps());
            if (debugging) Debug.Log("[AudioManager] Starting footsteps");
        }

        public void StopFootsteps() {
            if (footstepsCo != null) StopCoroutine(footstepsCo);
            footstepsCo = null;
            if (debugging) Debug.Log("[AudioManager] Stopping footsteps");
        }

        public void SetFootstepType( FootstepType type) {
            footstepType = type;
            if (debugging) Debug.Log("[AudioManager] Footstep type set to: " + type.ToString());
        }


        Coroutine volumeFade;
        public void SetFootstepVolume( float volume ) {
            if (volumeFade != null) StopCoroutine(volumeFade);
            volumeFade = StartCoroutine(PTUtilities.instance.FadeAudioTo(footstepsSource, volume, 1f));
            footstepsSource.volume = volume;
            if (debugging) Debug.Log("[AudioManager] Setting footstep volume to: " + volume);
        }
               

        IEnumerator Footsteps() {

            if (debugging) Debug.Log("[AudioManager] Startomg footstep audio calculation");

            int stepFlip = 0;
            Vector3 lastPlayerPos = PTUtilities.instance.HeadsetPosition();
            //lastPlayerPos.y = 0;
            Vector3 newPlayerPos;

            while (true) {
                yield return new WaitForSeconds(checkRate);

                if (footstepType != FootstepType.None) {
                    
                    newPlayerPos = PTUtilities.instance.HeadsetPosition();
                    //newPlayerPos.y = 0;

                    if (frameDebugging) Debug.Log("[AudioManager] Footstep Data:" + Environment.NewLine + 
                                                    "Footstep type = " + footstepType + Environment.NewLine +
                                                    "Distance = " + (newPlayerPos - lastPlayerPos).magnitude + " / " + stepDistance);

                    if ((newPlayerPos - lastPlayerPos).magnitude > stepDistance) {

                        switch (footstepType) {
                            case FootstepType.Cave:
                                footstepsSource.clip = footstepClips[0 + stepFlip];
                                footstepsSource.pitch = UnityEngine.Random.Range(pitchVariance.x, pitchVariance.y);
                                break;
                            case FootstepType.Stone:
                                footstepsSource.clip = footstepClips[2 + stepFlip];
                                footstepsSource.pitch = UnityEngine.Random.Range(pitchVariance.x, pitchVariance.y);
                                break;
                            case FootstepType.Grass:
                                footstepsSource.clip = footstepClips[4 + stepFlip];
                                footstepsSource.pitch = UnityEngine.Random.Range(pitchVariance.x, pitchVariance.y);
                                break;

                            case FootstepType.None:
                            default:
                                Debug.LogError("[AudioManager] ERROR -> Bad footstep type!");
                                break;
                        }

                        footstepsSource.transform.position = newPlayerPos - (Vector3.up * 1.5f);

                        footstepsSource.Play();
                        lastPlayerPos = newPlayerPos;
                        stepFlip = stepFlip==1?0:1;
                    }
                }
            }            
        }
    }
}
