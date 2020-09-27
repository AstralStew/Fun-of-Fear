using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Paperticket
{

    [RequireComponent(typeof(Volume))]
    public class PostVolumeAdjust : MonoBehaviour {

        private Volume volume;

        [SerializeField] [Min(0)] float adjustSpeed;

        [SerializeField] bool reverseAdjustment;

        // Start is called before the first frame update
        void Awake() {

            volume = GetComponent<Volume>();

        }

        // Update is called once per frame
        void Update() {

            if (Input.GetKey(KeyCode.LeftArrow)) {

                if (reverseAdjustment && volume.weight < 1) {
                    SetVolumeWeight(volume.weight + (adjustSpeed * Time.deltaTime));

                } else if (volume.weight > 0) {
                    SetVolumeWeight(volume.weight - (adjustSpeed * Time.deltaTime));

                }                
                
            } else if (Input.GetKey(KeyCode.RightArrow)) {

                if (reverseAdjustment && volume.weight > 0) {
                    SetVolumeWeight(volume.weight - (adjustSpeed * Time.deltaTime));

                } else if (volume.weight < 1) {
                    SetVolumeWeight(volume.weight + (adjustSpeed * Time.deltaTime));

                }

            }

        }

        


        public void SetVolumeWeight (float weight ) {

            volume.weight = Mathf.Clamp01(weight);

        }

    }

}