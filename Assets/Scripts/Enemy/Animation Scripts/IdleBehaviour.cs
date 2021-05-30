﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    public Transform player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameManagerSingleton.instance.player.transform;
    }

    // Also determines what and if to play animations based on bosses variables
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<Enemy>().state == Enemy.State.Attacking)
        {
            if (player.position.y <= animator.transform.position.y)
            {
                //Debug.Log("Player below");
                animator.SetBool("attackDown", true);
            }
            else if (player.position.y > animator.transform.position.y)
            {
                //Debug.Log("Player Above");
                animator.SetBool("attackUp", true);
            }

            if (player.position.x >= animator.transform.position.x)
            {
                // Debug.Log("player to right");
                animator.SetBool("attackRight", true);
            }
            else if (player.position.x < animator.transform.position.x)
            {
                //Debug.Log("Player to left");
                animator.SetBool("attackLeft", true);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
