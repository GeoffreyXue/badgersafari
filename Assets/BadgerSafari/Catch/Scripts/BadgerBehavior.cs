using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgerBehavior : MonoBehaviour
{
    Animator animator; 

    [SerializeField]
    [Tooltip("Will be found if not set")]
    private CatchSceneManager sceneManager;

    void Start()
    {
        sceneManager = sceneManager == null ? FindObjectOfType<CatchSceneManager>() : sceneManager;
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(sceneManager.isBadgerCaught){
            Debug.Log("Should go to catch animation");
            animator.SetBool("Caught", true);
        }
    }
}
