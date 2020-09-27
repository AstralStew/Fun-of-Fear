using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GhostAnimController : MonoBehaviour
{
    
    private Animator animator;
    private GhostPerception ghostPerception;
    
    [SerializeField] private bool debugging;

    public UnityEvent startedScareAnimation;

    public UnityEvent startedEscapeAnimation;

    public UnityEvent finishedEscapeAnimation;

    [Min(0)] public float finishDelayTime; 

    void Awake() {

        // Grab the animator reference
        animator = animator ?? GetComponent<Animator>();
        if (!animator) {
            Debug.LogError("[GhostAnimController] ERROR -> No Animator component found on this object! Please add one. Disabling object.");
            gameObject.SetActive(false);
        }
        
        // Grab the ghost movement reference
        ghostPerception = ghostPerception ?? GetComponentInParent<GhostPerception>();
        if (!ghostPerception) {
            Debug.LogError("[GhostAnimController] ERROR -> No Ghost Perception component found on parent object! Please add one. Disabling object.");
            gameObject.SetActive(false);
        }
                
    }

    void OnEnable() {
        ghostPerception.onSeePlayer += SetAngryAnimation;
        ghostPerception.onForgottenPlayer += SetCalmAnimation;
        ghostPerception.onReachPlayer += SetScreamAnimation;
    }

    void OnDisable() {
        ghostPerception.onSeePlayer -= SetAngryAnimation;
        ghostPerception.onForgottenPlayer -= SetCalmAnimation;
        ghostPerception.onReachPlayer -= SetScreamAnimation;
    }


    void SetAngryAnimation() {
        SetCanSeeAnimationBool(true);
        if (debugging) Debug.Log("[GhostAnimController] Setting animation to: angery! (>w<)");
    }

    void SetCalmAnimation() {
        SetCanSeeAnimationBool(false);
        if (debugging) Debug.Log("[GhostAnimController] Setting animation to: calm (uwu)");
    }


    void SetScreamAnimation() {
        SetHasReachedPlayerAnimationTrigger();
        if (debugging) Debug.Log("[GhostAnimController] Setting animation to: scream!! (@m@)");

        StartCoroutine(WaitForFinalAnimation());
    }





    void SetCanSeeAnimationBool(bool canSeePlayer) {
        animator.SetBool("canSeePlayer", canSeePlayer);
    }

    void SetHasReachedPlayerAnimationTrigger() {
        animator.SetTrigger("hasReachedPlayer");
    }



    IEnumerator WaitForFinalAnimation() {
               

        // Wait until the scare animation has started
        yield return new WaitUntil(() => animator.GetBool("startedScare"));

        if (debugging) Debug.Log("[GhostAnimController] startedScare set to true");
        startedScareAnimation.Invoke();

        // Wait until the escape animation has started
        yield return new WaitUntil(() => animator.GetBool("startedEscape"));

        if (debugging) Debug.Log("[GhostAnimController] startedEscape set to true");
        startedEscapeAnimation.Invoke();

        // Wait until the escape animation has finished
        yield return new WaitUntil(() => animator.GetBool("finishedEscape"));
                
        if (finishDelayTime > 0) {
            // Delay before finishing scape animation
            if (debugging) Debug.Log("[GhostAnimController] Delay before finishedEscape = " + finishDelayTime);
            yield return new WaitForSeconds(finishDelayTime);
        }
        
        if (debugging) Debug.Log("[GhostAnimController] finishedEscape set to true");
        finishedEscapeAnimation.Invoke();
               

    }

}
