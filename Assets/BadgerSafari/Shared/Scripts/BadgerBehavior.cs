using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgerBehavior : MonoBehaviour
{
    Animator animator; 

    // Game state
    internal bool isBadgerCaught = false;

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isBadgerCaught){
            animator.SetBool("Caught", true);
        }
    }
}
