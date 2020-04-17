using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket
{
    public class AudioTrigger : MonoBehaviour {

        public AudioClip audioClip;
        public LayerMask layers;

        private bool activated;


        public void OnTriggerEnter( Collider other ) {

            if (!activated && ((1 << other.gameObject.layer) & layers) != 0) {
                AudioManager.instance.AddNarrationClip(audioClip);
                activated = true;
                gameObject.SetActive(false);
            }

        }



    }

}