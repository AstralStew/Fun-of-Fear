using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimController : MonoBehaviour
{
    
    private Animator animator;
    private GhostMovement ghostMovement;
         
    void Awake() {

        // Grab the animator reference
        animator = animator ?? GetComponent<Animator>();
        if (!animator) {
            Debug.LogError("[GhostAnimController] ERROR -> No Animator component found on this object! Please add one. Disabling object.");
            gameObject.SetActive(false);
        }
        
        // Grab the ghost movement reference
        ghostMovement = ghostMovement ?? GetComponentInParent<GhostMovement>();
        if (!ghostMovement) {
            Debug.LogError("[GhostAnimController] ERROR -> No Ghost Movement component found on parent object! Please add one. Disabling object.");
            gameObject.SetActive(false);
        }
                
    }

    void OnEnable() {
        ghostMovement.onSeePlayer += SetAngryAnimation;
        ghostMovement.onLosePlayer += SetCalmAnimation;
    }

    void OnDisable() {
        ghostMovement.onSeePlayer -= SetAngryAnimation;
        ghostMovement.onLosePlayer -= SetCalmAnimation;
    }


    void SetAngryAnimation() {
        SetAnimation(true);
    }

    void SetCalmAnimation() {
        SetAnimation(false);
    }



    void SetAnimation(bool canSeePlayer) {
        animator.SetBool("canSeePlayer", canSeePlayer);
    }
}
