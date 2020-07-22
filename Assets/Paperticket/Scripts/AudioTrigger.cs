using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class AudioTrigger : MonoBehaviour {

        [SerializeField] bool debugging;



        [Header("Controls")]
        public LayerMask layers;


        [Space(10)]
        public bool AddNarrationClip;
        //public AudioClip audioClip;
        public NarrativeClip narrativeClip;

        [Space(10)]
        public bool setFootsteps;
        public FootstepType footstepType;
        public float footstepVolume;
        
        [Space(10)]
        public bool sendEvent;
        public UnityEvent audioEvent;

        [Header("Read Only")]
        [Space(10)]
        [SerializeField] bool activated;


        public void OnTriggerEnter( Collider other ) {
            if (activated) return;
            
            // Check the triggers layer
            if (((1 << other.gameObject.layer) & layers) != 0) {

                // Check if this is a narration clip
                if (AddNarrationClip) {
                    //TapeRecorder.instance.AddNarrationClip(audioClip);
                    TapeRecorder.instance.AddNarrationClip(narrativeClip);
                } 
                
                if (sendEvent) {
                    audioEvent.Invoke();
                }

                if (setFootsteps) {
                    AudioManager.instance.SetFootstepType(footstepType);
                    AudioManager.instance.SetFootstepVolume(footstepVolume);
                    AudioManager.instance.StartFootsteps();
                }

                activated = true;
                gameObject.SetActive(false);
            }
        }




    }
}