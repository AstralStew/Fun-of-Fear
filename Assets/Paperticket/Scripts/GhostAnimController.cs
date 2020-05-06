using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimController : MonoBehaviour
{
    
    private Animator animator;
    private GhostPerception ghostPerception;

    [SerializeField] private bool debugging;
         
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
    }

    void OnDisable() {
        ghostPerception.onSeePlayer -= SetAngryAnimation;
        ghostPerception.onForgottenPlayer -= SetCalmAnimation;
    }


    void SetAngryAnimation() {
        SetAnimation(true);
        if (debugging) Debug.Log("[GhostAnimController] Setting animation to: angery! (>w<)");
    }

    void SetCalmAnimation() {
        SetAnimation(false);
        if (debugging) Debug.Log("[GhostAnimController] Setting animation to: calm (uwu)");
    }



    void SetAnimation(bool canSeePlayer) {
        animator.SetBool("canSeePlayer", canSeePlayer);
    }
}
