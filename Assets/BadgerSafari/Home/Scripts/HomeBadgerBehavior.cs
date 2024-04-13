using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles home badger behavior.
/// Should be attached to a badger in the home scene.
/// </summary>
public class HomeBadgerBehavior : MonoBehaviour {
    public void Init(BadgerData badgerData, GameObject infoPanelPrefab) {
        // instantiate info panel under gameobject
        GameObject infoPanel = Instantiate(infoPanelPrefab, transform);

        infoPanel.SetActive(false);
        TextMeshProUGUI[] texts = infoPanel.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = badgerData.name;
        // set date caught to today
        texts[1].text = $"Caught: {System.DateTime.Now: MM/dd/yyyy}";
        
        // Set up awake on hover
        XRBaseInteractable interactable = gameObject.GetOrAddComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(args => OnControllerInteraction(args, _ => {
            infoPanel.SetActive(true);
            // set rotation to face player
            infoPanel.transform.LookAt(Camera.main.transform);
        }));
        interactable.hoverExited.AddListener(args => OnControllerInteraction(args, _ => infoPanel.SetActive(false)));
    }

    private void OnControllerInteraction(BaseInteractionEventArgs args, UnityAction<XRBaseController> action) {
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor) {
            action(controllerInteractor.xrController);
        }
    }
}
