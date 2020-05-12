using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonInteractable : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;
    private SpriteRenderer sprite;

    [SerializeField] bool oneUse;
    bool used;
    [SerializeField] private float fadeTime;
    [SerializeField] private bool debugging;

    Coroutine fadingCoroutine;

    void Awake() {

        simpleInteractable = simpleInteractable ?? GetComponent<XRSimpleInteractable>() ?? GetComponentInChildren<XRSimpleInteractable>(true);
        if (!simpleInteractable) {
            if (debugging) Debug.LogError("[ButtonInteractable] ERROR -> No XRSimpleInteractable found on or beneath this button! Please add one. Disabling object.");
            gameObject.SetActive(false);
        }

        sprite = sprite ?? GetComponentInChildren<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);
        if (!sprite) {
            if (debugging) Debug.LogError("[ButtonInteractable] ERROR -> No sprite found on or beneath this button! Please add one. Disabling object.");
            gameObject.SetActive(false);
        }

    }


    // Start is called before the first frame update
    void OnEnable() {
        //simpleInteractable.onHoverEnter.AddListener(HoverOn);
        //simpleInteractable.onHoverExit.AddListener(HoverOff);
        //simpleInteractable.onSelectEnter.AddListener(Select);         //Start of selection (trigger on)
        //simpleInteractable.onSelectExit.AddListener(Deselect);        //End of selection (trigger off)
        //simpleInteractable.onActivate.AddListener(Select);            //Activation while selected (grip on)
        //simpleInteractable.onDeactivate.AddListener(Deselect);        //Deactivation while selected (grip off)

        if (oneUse && used) {
            if (fadingCoroutine != null) {
                StopCoroutine(fadingCoroutine);
            }
            fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, Color.blue, fadeTime));
        
        } else { 
            Invoke("HoverOff", 0.5f);
        }
        
    }

    void OnDisable() {
        //simpleInteractable.onHoverEnter.RemoveListener(HoverOn);
        //simpleInteractable.onHoverExit.RemoveListener(HoverOff);
        //simpleInteractable.onSelectEnter.RemoveListener(Select);
        //simpleInteractable.onSelectExit.RemoveListener(Deselect);
        //simpleInteractable.onActivate.RemoveListener(Select);
        //simpleInteractable.onDeactivate.RemoveListener(Deselect);
    }

    public void HoverOn() { HoverOn(null); }
    public void HoverOn ( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) {
            StopCoroutine(fadingCoroutine);
        }

        fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 1f, fadeTime));

        if (debugging) Debug.Log("[ButtonInteractable] Hovering on!");
    }

    public void HoverOff() { HoverOff(null); }
    public void HoverOff( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) {
            StopCoroutine(fadingCoroutine);
        }

        fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 0.5f, fadeTime));

        if (debugging) Debug.Log("[ButtonInteractable] Hovering off!");
    }

    public void Select() { Select(null); }
    public void Select( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) {
            StopCoroutine(fadingCoroutine);
        }

        fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, Color.blue, fadeTime));
        
        used = true;

        if (debugging) Debug.Log("[ButtonInteractable] Selected!");
    }

    public void Deselect() { Deselect(null); }
    public void Deselect( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) {
            StopCoroutine(fadingCoroutine);
        }

        fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, Color.white, fadeTime));

        if (debugging) Debug.Log("[ButtonInteractable] Deselected!");
    }


}
