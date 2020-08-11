using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class VolumeTrigger : MonoBehaviour {

        [SerializeField] bool debugging;        

        [Header("Controls")]
        public LayerMask layers;

        [Space(10)]
        [SerializeField] bool oneUse;

        [Space(10)]
        [SerializeField] bool setFogProperties;
        [SerializeField] bool fogActive;
        [SerializeField] Color fogColor;
        [SerializeField] FogMode fogMode;
        [SerializeField] float linearStart;
        [SerializeField] float linearEnd;
        [SerializeField] float exponentialDensity;
                        
        [Space(10)]
        public bool sendEvent;
        public UnityEvent volumeEvent;

        [Header("Read Only")]
        [Space(10)]
        [SerializeField] bool activated;


        public void OnTriggerEnter( Collider other ) {
            if (oneUse && activated) return;
            
            // Check the triggers layer
            if (((1 << other.gameObject.layer) & layers) != 0) {
                
                if (setFogProperties) {
                    RenderSettings.fog = fogActive;
                    if (fogActive) {
                        RenderSettings.fogColor = fogColor;
                        RenderSettings.fogMode = fogMode;
                        if (fogMode == FogMode.Linear) {
                            RenderSettings.fogStartDistance = linearStart;
                            RenderSettings.fogEndDistance = linearEnd;
                        } else {
                            RenderSettings.fogDensity = exponentialDensity;
                        }
                    }
                }
                
                if (sendEvent) {
                    volumeEvent.Invoke();
                }
                
                activated = true;
                if (oneUse) gameObject.SetActive(false);
            }
        }




    }
}