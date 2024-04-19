using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles ball holder behavior.
/// </summary>
public class BallHolderBehavior : MonoBehaviour {
    [SerializeField]

    private GameObject catchBallPrefab;

    // on awake, grab all socket interactables
    void Awake() {
        XRSocketInteractor[] socketInteractors = GetComponentsInChildren<XRSocketInteractor>();
        // instantiate and add a catch ball prefab to each socket interactor
        foreach (XRSocketInteractor socketInteractor in socketInteractors) {
            GameObject catchBall = Instantiate(catchBallPrefab, socketInteractor.transform.position, Quaternion.identity);
            catchBall.transform.localPosition = Vector3.zero;
            catchBall.transform.localRotation = Quaternion.identity;
            socketInteractor.startingSelectedInteractable = catchBall.GetComponent<XRBaseInteractable>();
        }
    }
}
