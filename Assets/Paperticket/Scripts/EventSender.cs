using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSender : MonoBehaviour
{

    [SerializeField] float timeBeforeEvent;
    [SerializeField] bool OneTimeUse = true;
    [SerializeField] bool debug;

    bool used;

    [Header("Events")]

    [SerializeField] UnityEvent OnEventTriggered;

    // Start is called before the first frame update
    void OnEnable() {
        StartCountdownToEvent();      
    }

    public void StartCountdownToEvent() {
        if (!(OneTimeUse && used)) {
            StartCoroutine(CountdownToEvent());
        } else {
            if (debug) Debug.Log("[EventSender] This event is set to OneTimeUse and has already been used, disabling self");
            enabled = false;
        }
    }

    IEnumerator CountdownToEvent() {
        if (debug) Debug.Log("[EventSender] Counting down event...");

        used = false;

        yield return new WaitForSeconds(timeBeforeEvent);

        // Trigger the event
        if (OnEventTriggered != null) {
            if (debug) Debug.Log("[EventSender] OnEventTriggered called!");
            OnEventTriggered.Invoke();
        }

        // Destroy this script if this is a one time use, otherwise reset
        if (OneTimeUse) {
            if (debug) Debug.Log("[EventSender] This event is set to OneTimeUse, disabling self");
            enabled = false;
        } 

        used = true;
    }

}
