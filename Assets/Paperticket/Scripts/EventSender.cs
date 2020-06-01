using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSender : MonoBehaviour
{
    [SerializeField] bool debugging;

    [Header("Trigger Controls")]
    [SerializeField] bool requireTriggerEnter;
    [SerializeField] LayerMask triggerLayers;

    [Header("Time Controls")]
    [SerializeField] float timeBeforeEvent;
    [SerializeField] bool OneTimeUse = true;


    bool used;

    [Header("Events")]

    [SerializeField] UnityEvent OnEventTriggered;

    

    // Start is called before the first frame update
    void OnEnable() {
        if (!requireTriggerEnter) {
            StartCountdownToEvent();
        }
    }

    public void OnTriggerEnter( Collider other ) {
        if (((1 << other.gameObject.layer) & triggerLayers) != 0) {
            StartCountdownToEvent();
        }
    }

    public void StartCountdownToEvent() {
        if (!(OneTimeUse && used)) {
            StartCoroutine(CountdownToEvent());
        } else {
            if (debugging) Debug.Log("[EventSender] This event is set to OneTimeUse and has already been used, disabling self");
            enabled = false;
        }
    }

    IEnumerator CountdownToEvent() {
        if (debugging) Debug.Log("[EventSender] Counting down event...");

        used = false;

        yield return new WaitForSeconds(timeBeforeEvent);

        // Trigger the event
        if (OnEventTriggered != null) {
            if (debugging) Debug.Log("[EventSender] OnEventTriggered called!");
            OnEventTriggered.Invoke();
        }

        // Destroy this script if this is a one time use, otherwise reset
        if (OneTimeUse) {
            if (debugging) Debug.Log("[EventSender] This event is set to OneTimeUse, disabling self");
            enabled = false;
        } 

        used = true;
    }

}
