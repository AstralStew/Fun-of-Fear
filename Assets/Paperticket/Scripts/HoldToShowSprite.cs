using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;

public class HoldToShowSprite : MonoBehaviour {


    [SerializeField] GameObject displayObject;

    //[SerializeField] [Min(0.1f)] float delay = 0.1f;
    //[SerializeField] [Min(0.1f)] float holdDuration = 0.1f;

    //[SerializeField] bool lockPosition;

    [SerializeField] bool debugging;

    bool holdingDown;

    // Start is called before the first frame update
    void Awake() {

        displayObject.SetActive(false);

    }

    // Update is called once per frame
    void Update() {

        if (CrossPlatformInputManager.GetButton("RightAButton")) {

            if (!holdingDown) {
                holdingDown = true;
                displayObject.SetActive(true);
            }

        } else if (holdingDown) {

            holdingDown = false;
            displayObject.SetActive(false);

        }

    }
   



}