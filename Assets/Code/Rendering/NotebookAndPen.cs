using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookAndPen : StateMachineBehaviour
{
	bool IsWriting = false;
	
	int WritingHash = Animator.StringToHash("Writing In Book");
	
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		if(stateInfo.shortNameHash == WritingHash)
		{
			IsWriting = true;
		}
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(IsWriting)
		{
			//Debug.Log(stateInfo.normalizedTime);
			
			if(stateInfo.normalizedTime > 0.08f && stateInfo.normalizedTime < 0.12f)
			{
				AlexAnimation aa = animator.gameObject.GetComponent<AlexAnimation>();
				
				if(aa != null)
				{
					if(!aa.Pen.activeSelf)
					{
						aa.Pen.SetActive(true);
					}
									
					if(!aa.Notebook.activeSelf)
					{
						aa.Notebook.SetActive(true);
					}
				}
			}
			else if(stateInfo.normalizedTime > 0.9f && stateInfo.normalizedTime < 0.94f)
			{
				AlexAnimation aa = animator.gameObject.GetComponent<AlexAnimation>();
				
				if(aa != null)
				{
					if(aa.Pen.activeSelf)
					{
						aa.Pen.SetActive(false);
					}
									
					if(aa.Notebook.activeSelf)
					{
						aa.Notebook.SetActive(false);
					}
				}
			}
		}
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.shortNameHash == WritingHash)
		{
			IsWriting = false;
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
