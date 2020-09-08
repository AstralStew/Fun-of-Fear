using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Light))]
public class HandSpotlight : MonoBehaviour
{
    [SerializeField] [Range(0, 0.99f)] float indexTriggerDeadzone = 0.5f;

    Light spotlight;
    
    bool indexReset;

    // Start is called before the first frame update
    void Awake()
    {
        spotlight = GetComponent<Light>();
        spotlight.type = UnityEngine.LightType.Spot;
    }

    // Update is called once per frame
    void Update() {
        // Toggle the light when the index trigger is first pressed
        if (CrossPlatformInputManager.GetAxis("RightIndexTrigger") > indexTriggerDeadzone) {
            if (!indexReset) {
                ToggleSpotlight();
                indexReset = true;
            }
        } else if (indexReset) indexReset = false;
    }


    public void ToggleSpotlight() {
        ToggleSpotlight(!spotlight.enabled);
    }
    public void ToggleSpotlight (bool toggle ) {

        spotlight.enabled = toggle;

    }
}
