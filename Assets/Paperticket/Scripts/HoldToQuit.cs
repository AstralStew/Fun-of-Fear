using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;

public class HoldToQuit : MonoBehaviour
{
    [SerializeField] TextMeshPro displayObject;

    [SerializeField] [Min(0.1f)] float delay = 0.1f;
    [SerializeField] [Min(0.1f)] float holdDuration = 0.1f;

    [SerializeField] bool debugging;

    Coroutine holdingDown;

    // Start is called before the first frame update
    void Awake() {

        displayObject.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update() {

        if (CrossPlatformInputManager.GetButton("RightBButton")) {

            if (holdingDown == null) holdingDown = StartCoroutine(HoldingDown());

        } else if (holdingDown != null) {
            StopCoroutine(holdingDown);
            holdingDown = null;
            displayObject.gameObject.SetActive(false);
            if (debugging) Debug.Log("[HoldToQuit] No longer holding down, quit timer cancelled.");
        }
        
    }

    IEnumerator HoldingDown() {
        
        // Wait a little while before starting timer
        float time = 0;
        while (time <= delay) {
            yield return null;
            time += Time.deltaTime;
        }
        
        Debug.LogWarning("[HoldToQuit] Starting timer! Application will quit in (" + holdDuration + ") seconds unless cancelled!");

        // Display graphic
        displayObject.text = "Quitting in "+Mathf.Ceil(holdDuration)+"...";
        displayObject.gameObject.SetActive(true);

        // Start the timer to quit
        time = 0;
        while (time <= holdDuration) {
            yield return null;
            time += Time.deltaTime;
            displayObject.text = "Quitting in " + Mathf.Ceil(holdDuration - time) + "...";
        }

        // wait a little extra just in case for some reason
        if (debugging) Debug.Log("[HoldToQuit] Finished holding down, quiting momentarily!");
        yield return new WaitForSeconds(0.1f);


        // Quit if build, stop playing if editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }



}
