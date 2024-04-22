using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AlexLevel2AnimLoop : StateMachineBehaviour
{
	bool JustKneeled = true;
	int IdleHash = Animator.StringToHash("Idle");
	int TurnAroundHash = Animator.StringToHash("TurnAround00");
	
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.shortNameHash == IdleHash)
		{
			JustKneeled = !JustKneeled;
			if(JustKneeled)
			{
				animator.SetBool("walking", true);
				AlexAnimation aa = animator.gameObject.GetComponent<AlexAnimation>();
				if(aa != null)
				{
					aa.Walk(aa.WalkPointNW.transform, 3f);
				}
			}
			else
			{
				animator.SetBool("walking", false);
				animator.SetTrigger("kneel00");
			}
		}
		else if(stateInfo.shortNameHash == TurnAroundHash)
		{
			
		}
    }
	
	
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
