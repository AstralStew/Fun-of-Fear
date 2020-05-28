using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCallbackSMB : StateMachineBehaviour
{

    public string onStateEnterName;         // The name of the Bool animation parametre to set to true
    //public string onStateUpdateName;
    public string onStateExitName;
    //public string onStateMoveName;
    //public string onStateIKName;
    [SerializeField] bool debugging;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex ) {
        
        if (onStateEnterName != "") {
            if (debugging) Debug.Log("[AnimCallbackSMB] Setting bool: " + onStateEnterName);
            animator.SetBool(onStateEnterName, true);
        }

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex ) {

    //    if (onStateUpdateName != "") {
    //        animator.SetBool(onStateUpdateName, true);
    //    }
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex ) {

        if (onStateExitName != "") {
            if (debugging) Debug.Log("[AnimCallbackSMB] Setting bool: " + onStateExitName);
            animator.SetBool(onStateExitName, true);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
