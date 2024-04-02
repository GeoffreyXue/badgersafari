using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Animates the hand based on input.
/// From video: https://www.youtube.com/watch?v=pI8l42F6ZVc
/// </summary>
public class HandAnimatorController : MonoBehaviour
{
    [SerializeField]
    private InputActionProperty triggerAction;
    [SerializeField]
    private InputActionProperty gripAction;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        float triggerValue = triggerAction.action.ReadValue<float>();
        float gripValue = gripAction.action.ReadValue<float>();

        anim.SetFloat("Trigger", triggerValue);
        anim.SetFloat("Grip", gripValue);
    }
}
