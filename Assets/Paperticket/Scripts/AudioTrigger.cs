using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class AudioTrigger : MonoBehaviour {

        [SerializeField] bool debugging;

        public LayerMask layers;

        [SerializeField] bool useTag;
        [SerializeField] string _tag;



        [Header("Controls")]

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


        public void OnTriggerStay( Collider other ) {
            if (activated) return;

            // Check the triggers layer
            if (((1 << other.gameObject.layer) & layers) != 0) {

                if (useTag && other.gameObject.tag != _tag) return;

                ActivateTrigger();
            }
        }
        public void OnTriggerEnter( Collider other ) {
            if (activated) return;

            // Check the triggers layer
            if (((1 << other.gameObject.layer) & layers) != 0) {

                if (useTag && other.gameObject.tag != _tag) return;

                ActivateTrigger();
            }
        }

        void ActivateTrigger() {

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