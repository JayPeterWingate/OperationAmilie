using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuAnim : MonoBehaviour {

    Animator walkAnim;
    bool walking;

	// Use this for initialization
	void Start () {
        walkAnim = GetComponent<Animator>();
        walking = false;
	}
	
	// Update is called once per frame
	void Update () {
		

        if(!walking && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W)))
        {
            walking = true;
            walkAnim.SetBool("WalkRight", true);
        }
        if (!walking && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A)))
        {
            walkAnim.SetBool("WalkLeft", true);
            walking = true;
        }

    }

    void stopWalking ()
    {
        
        walking = false;
        walkAnim.SetBool("TargetReached", true);
    }
}
