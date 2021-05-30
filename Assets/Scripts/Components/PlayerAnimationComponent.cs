﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationComponent : MonoBehaviour
{
    private InputComponent input;
    private Animator anim;
    private Camera cam;

    private string lastMovementDirection;

    private void Awake()
    {
        cam = Camera.main;
        GetComponent<MovementComponent>().AnimateMovement += Animate; // subscribe to MovementComponents AnimateMovement delegate
        input = GetComponent<InputComponent>(); // used to get input from the player, and not taking input in this class
        anim = GetComponent<Animator>();
        lastMovementDirection = "Down";
    }

    /*
     * Method that controls the AnimationController of the player.
     */ 
    void Animate()
    {
        if (input == null || anim == null || GameManagerSingleton.instance.isPaused) return;

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Magnitude", movement.magnitude);

        if (input.Attack)
            anim.SetBool("Attack", true);

        if (input.Vertical > 0)
        {
            lastMovementDirection = "Up";
        }

        if (input.Horizontal < 0)
        {
            lastMovementDirection = "Left";
        }

        if (input.Vertical < 0)
        {
            lastMovementDirection = "Down";
        }

        if (input.Horizontal > 0)
        {
            lastMovementDirection = "Right";
        }

        if (lastMovementDirection.Equals("Up"))
            anim.SetInteger("Direction", 1);
        else if (lastMovementDirection.Equals("Down"))
            anim.SetInteger("Direction", 3);
        else if (lastMovementDirection.Equals("Right"))
            anim.SetInteger("Direction", 4);
        else if (lastMovementDirection.Equals("Left"))
            anim.SetInteger("Direction", 2);
        
    }

    /*
    public void MouseDirectionAttack()
    {
        
        Vector2 dir = cam.ScreenToViewportPoint((Input.mousePosition));
        //Debug.Log(dir);
        if (dir.y >= 0.65f)
        //if(lastMovementDirection.Equals("Up"))
        {
            //Debug.Log("Top");
            //facingDirection = 1;
            lastMovementDirection = "Up";
        }
        else if (dir.x <= 0.5f && (dir.y > 0.35f && dir.y < 0.65f))
        //else if(lastMovementDirection.Equals("Left"))
        {
            //Debug.Log("Left");
            //facingDirection = 2;
            lastMovementDirection = "Left";
        }
        else if (dir.y <= 0.35f)
        //else if(lastMovementDirection.Equals("Down"))
        {
            //Debug.Log("Down");
            //facingDirection = 3;
            lastMovementDirection = "Down";

        }
        else if (dir.x >= 0.5f && (dir.y > 0.35f && dir.y < 0.65f))
        //else if(lastMovementDirection.Equals("Right"))
        {
            //Debug.Log("Right");
            //facingDirection = 4;
            lastMovementDirection = "Right";
        }
        //Debug.Log(dir);
        
    }
    */
    
    
}
